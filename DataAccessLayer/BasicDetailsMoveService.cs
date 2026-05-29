using Dapper;
using DataAccessLayer.Logger;
using DataTransferObject.Domain.Model;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DataAccessLayer
{
    public class BasicDetailsMoveService : BackgroundService
    {
        private readonly DapperContext _contextDP;
        private readonly DapperContextDb2 _contextDP2;
        private readonly ILogger<BasicDetailsMoveService> _logger;
        public BasicDetailsMoveService(DapperContext contextDP, DapperContextDb2 contextDP2, ILogger<BasicDetailsMoveService> logger)
        {
            _contextDP = contextDP;
            _contextDP2 = contextDP2;
            _logger = logger;
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                DateTime now = DateTime.Now;

                DateTime nextRun = DateTime.Today
                    .AddHours(23)
                    .AddMinutes(55); // 11:55 PM

                if (now > nextRun)
                {
                    nextRun = nextRun.AddDays(1);
                }

                TimeSpan delay = nextRun - now;

                _logger.LogInformation("BasicDetailsMoveService next run at: {NextRun}", nextRun);

                await Task.Delay(delay, stoppingToken);

                await MoveDataAsync(stoppingToken);
            }
            //For testing purposes, you can use the below code to run the data move every minute instead of once a day at 11:55 PM. Just remember to comment out the above code block and uncomment the below block.
            //while (!stoppingToken.IsCancellationRequested)
            //{
            //    await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
            //    await MoveDataAsync(stoppingToken);
            //}
        }
        private async Task MoveDataAsync(CancellationToken cancellationToken)
        {
            int batchSize = 50;
            int totalMoved = 0;

            while (!cancellationToken.IsCancellationRequested)
            {
                int movedInThisBatch = await MoveOneBatchAsync(batchSize, cancellationToken);

                if (movedInThisBatch == 0)
                    break;

                totalMoved += movedInThisBatch;
            }

            _logger.LogInformation("Total {TotalMoved} records moved successfully.", totalMoved);
        }
        private async Task<int> MoveOneBatchAsync(int batchSize,CancellationToken cancellationToken)
        {
            // Initialize transaction for multiple database operations
            var (db, transaction) = _contextDP.CreateConnectionWithTransaction();
            var (db2, transaction2) = _contextDP2.CreateConnectionWithTransaction();

            try
            {
                var records = (await db.QueryAsync<BasicDetail>(
                    new CommandDefinition(@"
                    SELECT TOP (@BatchSize)
                        bs.BasicDetailId, bs.ArmedId, bs.RankId, bs.ServiceNo, bs.DOB,
                        bs.PlaceOfIssue, bs.DateOfIssue, bs.DateOfCommissioning,
                        bs.ApplyForId, bs.UnitId, bs.PaperIcardNo, bs.IsActive,
                        bs.Updatedby, bs.UpdatedOn, bs.IssuingAuthorityId,
                        bs.NameAsPerRecord, bs.RegimentalId, bs.FName, bs.LName,
                        bs.PreviousBasicDetailId, bs.IsLock
                    FROM dbo.BasicDetails bs
                    INNER JOIN TrnICardRequest req on req.BasicDetailId = bs.BasicDetailId and req.StatusId =2
                    INNER JOIN TrnDistributeCards dist on dist.RequestId = req.RequestId
                    ORDER BY bs.BasicDetailId;
                ",
                    new { BatchSize = batchSize },
                    transaction,
                    cancellationToken: cancellationToken)
                )).ToList();

                if (!records.Any())
                {
                    transaction.Commit();
                    transaction2.Commit();
                    return 0;
                }
                
                var ids = records.Select(x => x.BasicDetailId).ToList();

                var uploads = (await db.QueryAsync<MTrnUpload>(
                    new CommandDefinition(@"
                                SELECT UploadId, BasicDetailId, SignatureImagePath, PhotoImagePath
                                FROM dbo.TrnUpload
                                WHERE BasicDetailId IN @Ids;
                            ",
                    new { Ids = ids },
                    transaction,
                    cancellationToken: cancellationToken)
                )).ToList();

                var identities = (await db.QueryAsync<MTrnIdentityInfo>(
                    new CommandDefinition(@"
                        SELECT InfoId, BasicDetailId, IdenMark1, IdenMark2,
                               AadhaarNo, Height, BloodGroupId
                        FROM dbo.TrnIdentityInfo
                        WHERE BasicDetailId IN @Ids;
                    ",
                    new { Ids = ids },
                    transaction,
                    cancellationToken: cancellationToken)
                )).ToList();

                var addresses = (await db.QueryAsync<MTrnAddress>(
                    new CommandDefinition(@"
                        SELECT AddressId, BasicDetailId, State, District, PS, PO,
                               Tehsil, Village, PinCode
                        FROM dbo.TrnAddress
                        WHERE BasicDetailId IN @Ids;
                    ",
                    new { Ids = ids },
                    transaction,
                    cancellationToken: cancellationToken)
                )).ToList();

                foreach (var item in records)
                {
                    int alreadyExists = await db2.ExecuteScalarAsync<int>(
                        new CommandDefinition(@"
                        SELECT COUNT(1)
                        FROM dbo.BasicDetails
                        WHERE BasicDetailId = @BasicDetailId;
                    ",
                        new { item.BasicDetailId },
                        transaction2,
                        cancellationToken: cancellationToken)
                    );

                    if (alreadyExists == 0)
                    {
                        var p = new DynamicParameters();

                        p.Add("@BasicDetailId", item.BasicDetailId, DbType.Int32);
                        p.Add("@ArmedId", item.ArmedId, DbType.Byte);
                        p.Add("@RankId", item.RankId, DbType.Int16);
                        p.Add("@ServiceNo", item.ServiceNo, DbType.AnsiString, size: 10);
                        p.Add("@DOB", item.DOB, DbType.DateTime);
                        p.Add("@PlaceOfIssue", item.PlaceOfIssue, DbType.AnsiString, size: 50);
                        p.Add("@DateOfIssue", item.DateOfIssue, DbType.DateTime);
                        p.Add("@DateOfCommissioning", item.DateOfCommissioning, DbType.DateTime);
                        p.Add("@ApplyForId", item.ApplyForId, DbType.Byte);
                        p.Add("@UnitId", item.UnitId, DbType.Int32);
                        p.Add("@PaperIcardNo", item.PaperIcardNo, DbType.AnsiString, size: 12);
                        p.Add("@IsActive", item.IsActive, DbType.Boolean);
                        p.Add("@Updatedby", item.Updatedby, DbType.Int32);
                        p.Add("@UpdatedOn", item.UpdatedOn, DbType.DateTime);
                        p.Add("@IssuingAuthorityId", item.IssuingAuthorityId, DbType.Byte);
                        p.Add("@NameAsPerRecord", item.NameAsPerRecord, DbType.AnsiString, size: 36);
                        p.Add("@RegimentalId", item.RegimentalId, DbType.Byte);
                        p.Add("@FName", item.FName, DbType.AnsiString, size: 18);
                        p.Add("@LName", item.LName, DbType.AnsiString, size: 18);
                        p.Add("@PreviousBasicDetailId", item.PreviousBasicDetailId, DbType.Int32);
                        p.Add("@IsLock", item.IsLock, DbType.Boolean);

                        await db2.ExecuteAsync(
                            new CommandDefinition(@"
                            INSERT INTO dbo.BasicDetails
                            (
                                BasicDetailId, ArmedId, RankId, ServiceNo, DOB,
                                PlaceOfIssue, DateOfIssue, DateOfCommissioning,
                                ApplyForId, UnitId, PaperIcardNo, IsActive,
                                Updatedby, UpdatedOn, IssuingAuthorityId,
                                NameAsPerRecord, RegimentalId, FName, LName,
                                PreviousBasicDetailId, IsLock
                            )
                            VALUES
                            (
                                @BasicDetailId, @ArmedId, @RankId, @ServiceNo, @DOB,
                                @PlaceOfIssue, @DateOfIssue, @DateOfCommissioning,
                                @ApplyForId, @UnitId, @PaperIcardNo, @IsActive,
                                @Updatedby, @UpdatedOn, @IssuingAuthorityId,
                                @NameAsPerRecord, @RegimentalId, @FName, @LName,
                                @PreviousBasicDetailId, @IsLock
                            );
                        ",
                            p,
                            transaction2,
                            cancellationToken: cancellationToken)
                        );
                    }

                    

                    var upload = uploads.FirstOrDefault(x => x.BasicDetailId == item.BasicDetailId);

                    if (upload != null)
                    {
                        int uploadExists = await db2.ExecuteScalarAsync<int>(
                            new CommandDefinition(@"
                                    SELECT COUNT(1)
                                    FROM dbo.TrnUpload
                                    WHERE UploadId = @UploadId;
                                    ",
                            new { upload.UploadId },
                            transaction2,
                            cancellationToken: cancellationToken)
                        );
                        // 1. Insert TrnUpload
                        if (uploadExists == 0)
                        {
                            var pUpload = new DynamicParameters();
                            pUpload.Add("@UploadId", upload.UploadId, DbType.Int32);
                            pUpload.Add("@BasicDetailId", upload.BasicDetailId, DbType.Int32);
                            pUpload.Add("@SignatureImagePath", upload.SignatureImagePath, DbType.AnsiString, size: 100);
                            pUpload.Add("@PhotoImagePath", upload.PhotoImagePath, DbType.AnsiString, size: 100);

                            await db2.ExecuteAsync(
                                new CommandDefinition(@"
                                        INSERT INTO dbo.TrnUpload
                                        (
                                            UploadId, BasicDetailId, SignatureImagePath, PhotoImagePath
                                        )
                                        VALUES
                                        (
                                            @UploadId, @BasicDetailId, @SignatureImagePath, @PhotoImagePath
                                        );
                                    ",
                                pUpload,
                                transaction2,
                                cancellationToken: cancellationToken)
                            );
                        }
                    }

                    var identity = identities.FirstOrDefault(x => x.BasicDetailId == item.BasicDetailId);

                    if (identity != null)
                    {
                        int identityExists = await db2.ExecuteScalarAsync<int>(
                            new CommandDefinition(@"
                                SELECT COUNT(1)
                                FROM dbo.TrnIdentityInfo
                                WHERE InfoId = @InfoId;
                            ",
                            new { identity.InfoId },
                            transaction2,
                            cancellationToken: cancellationToken)
                        );
                        // 2. Insert TrnIdentityInfo
                        if (identityExists == 0)
                        {
                            var pIdentity = new DynamicParameters();
                            pIdentity.Add("@InfoId", identity.InfoId, DbType.Int32);
                            pIdentity.Add("@BasicDetailId", identity.BasicDetailId, DbType.Int32);
                            pIdentity.Add("@IdenMark1", identity.IdenMark1, DbType.AnsiString, size: 200);
                            pIdentity.Add("@IdenMark2", identity.IdenMark2, DbType.AnsiString, size: 200);
                            pIdentity.Add("@AadhaarNo", identity.AadhaarNo, DbType.Int64);
                            pIdentity.Add("@Height", identity.Height, DbType.Single);
                            pIdentity.Add("@BloodGroupId", identity.BloodGroupId, DbType.Byte);

                            await db2.ExecuteAsync(
                                new CommandDefinition(@"
                                    INSERT INTO dbo.TrnIdentityInfo
                                    (
                                        InfoId, BasicDetailId, IdenMark1, IdenMark2,
                                        AadhaarNo, Height, BloodGroupId
                                    )
                                    VALUES
                                    (
                                        @InfoId, @BasicDetailId, @IdenMark1, @IdenMark2,
                                        @AadhaarNo, @Height, @BloodGroupId
                                    );
                                ",
                                pIdentity,
                                transaction2,
                                cancellationToken: cancellationToken)
                            );
                        }
                    }


                    
                    var address = addresses.FirstOrDefault(x => x.BasicDetailId == item.BasicDetailId);


                    if (address != null)
                    {
                        int addressExists = await db2.ExecuteScalarAsync<int>(
                            new CommandDefinition(@"
                                    SELECT COUNT(1)
                                    FROM dbo.TrnAddress
                                    WHERE AddressId = @AddressId;
                                ",
                            new { address.AddressId },
                            transaction2,
                            cancellationToken: cancellationToken)
                        );
                        // 3. Insert TrnAddress
                        if (addressExists == 0)
                        {
                            var pAddress = new DynamicParameters();
                            pAddress.Add("@AddressId", address.AddressId, DbType.Int32);
                            pAddress.Add("@BasicDetailId", address.BasicDetailId, DbType.Int32);
                            pAddress.Add("@State", address.State, DbType.AnsiString, size: 50);
                            pAddress.Add("@District", address.District, DbType.AnsiString, size: 50);
                            pAddress.Add("@PS", address.PS, DbType.AnsiString, size: 50);
                            pAddress.Add("@PO", address.PO, DbType.AnsiString, size: 50);
                            pAddress.Add("@Tehsil", address.Tehsil, DbType.AnsiString, size: 50);
                            pAddress.Add("@Village", address.Village, DbType.AnsiString, size: 50);
                            pAddress.Add("@PinCode", address.PinCode, DbType.Int32);

                            await db2.ExecuteAsync(
                                new CommandDefinition(@"
                                    INSERT INTO dbo.TrnAddress
                                    (
                                        AddressId, BasicDetailId, State, District, PS, PO,
                                        Tehsil, Village, PinCode
                                    )
                                    VALUES
                                    (
                                        @AddressId, @BasicDetailId, @State, @District, @PS, @PO,
                                        @Tehsil, @Village, @PinCode
                                    );
                                ",
                                pAddress,
                                transaction2,
                                cancellationToken: cancellationToken)
                            );
                        }
                    }


                    // 4. Delete child records first from old DB
                    await db.ExecuteAsync(
                        new CommandDefinition(@"
                            DELETE FROM dbo.TrnUpload WHERE BasicDetailId IN @Ids;
                            DELETE FROM dbo.TrnIdentityInfo WHERE BasicDetailId IN @Ids;
                            DELETE FROM dbo.TrnAddress WHERE BasicDetailId IN @Ids;
                            DELETE FROM dbo.BasicDetails WHERE BasicDetailId IN @Ids;
                        ",
                        new { Ids = ids },
                        transaction,
                        cancellationToken: cancellationToken)
                    );
                }

                transaction.Commit();
                transaction2.Commit();
                return records.Count;

            }
            catch
            {
                transaction.Rollback();
                transaction2.Rollback();
                throw;
            }
        }
    }
}
