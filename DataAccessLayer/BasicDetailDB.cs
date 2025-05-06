using Azure;
using Dapper;
using DataAccessLayer.BaseInterfaces;
using DataAccessLayer.Healpers;
using DataAccessLayer.Logger;
using DataTransferObject.Domain.Master;
using DataTransferObject.Domain.Model;
using DataTransferObject.Requests;
using DataTransferObject.Response;
using DataTransferObject.ViewModels;
using EntityFramework.Exceptions.Common;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Collections.Immutable;
using System.Data;
using Azure.Core;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
using Microsoft.Data.SqlClient;
using System.Linq.Expressions;
using static System.Runtime.InteropServices.JavaScript.JSType;
using DataTransferObject.Constants;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace DataAccessLayer
{
    public class BasicDetailDB : GenericRepositoryDL<BasicDetail>, IBasicDetailDB
    {
        protected new readonly ApplicationDbContext _context;
        private readonly DapperContext _contextDP;
        private readonly IDataProtector protector;
        private readonly ILogger<BasicDetailDB> _logger;
        public BasicDetailDB(ApplicationDbContext context, DapperContext contextDP, IDataProtectionProvider dataProtectionProvider, ILogger<BasicDetailDB> logger, DataProtectionPurposeStrings dataProtectionPurposeStrings) : base(context)
        {
            _context = context;
            _contextDP=contextDP;
            _logger = logger;
            // Pass the purpose string as a parameter
            this.protector = dataProtectionProvider.CreateProtector(
                dataProtectionPurposeStrings.AFSACIdRouteValue);
        }
        public async Task<DTOUploadChipAndSerialResponse> UploadChipAndSerial(List<DTOUploadChipAndSerialRequest> Data)
        {
            int i = 0;
            var (db, transaction) = _contextDP.CreateConnectionWithTransaction();
            DTOUploadChipAndSerialResponse response = new DTOUploadChipAndSerialResponse();
            try
            {
                foreach (var item in Data)
                {
                    if(item.IsValid == true)
                    {
                        string query = " UPDATE TrnICardRequest set CardSerialNo=@CardSerialNo, ChipNo=@ChipNo where RequestId=@RequestId ";

                        var parameters = new DynamicParameters();
                        parameters.Add("@RequestId", item.RequestId, DbType.Int32, ParameterDirection.Input);
                        parameters.Add("@CardSerialNo", item.CardSerialNo, DbType.String, ParameterDirection.Input, 30);
                        parameters.Add("@ChipNo", item.ChipNo, DbType.String, ParameterDirection.Input, 30);

                        await db.ExecuteAsync(query, parameters, transaction: transaction);
                    }
                }
                // Commit the transaction if all operations succeed
                transaction.Commit();
                response.Result = true;
                response.Message = "Data processed successfully!";
                return response;
            }
            catch (Exception ex)
            {
                // Rollback the transaction if any operation fails
                transaction.Rollback();
                _logger.LogError(1001, ex, "BasicDetailDB->UploadChipAndSerial");
                response.Result = false;
                response.Message = ex.Message;
                return response;
            }
            finally
            {
                // Dispose of the connection
                db.Dispose();
            }
        }
        public async Task<List<DTOTopArmyNoFromICardRequestResponse>?> GetTopArmyNoFromICardRequest(string ArmyNo)
        {
            try
            {
                var ret = await (from bd in _context.BasicDetails
                                 join irequest in _context.TrnICardRequest on bd.BasicDetailId equals irequest.BasicDetailId
                                 where bd.ServiceNo.Contains(ArmyNo) && irequest.StatusId == 1
                                 select new DTOTopArmyNoFromICardRequestResponse
                                 {
                                     RequestId = irequest.RequestId,
                                     ServiceNo = bd.ServiceNo,
                                 }
                                ).Take(5).ToListAsync();
                return ret;
            }
            catch (Exception ex)
            {
                _logger.LogError(1001, ex, "BasicDetailDB->GetTopArmyNoFromICardRequest");
                return null;
            }

        }
        public async Task<DTOBDetailByRequestIdResponse?> GetBDetailByRequestId(int RequestId)
        {
            try
            {
                var ret = await (from irequest in _context.TrnICardRequest
                                 join bd in _context.BasicDetails on irequest.BasicDetailId equals bd.BasicDetailId
                                 join rk in _context.MRank on bd.RankId equals rk.RankId
                                 join umap in _context.MapUnit on bd.UnitId equals umap.UnitMapId
                                 join munit in _context.MUnit on umap.UnitId equals munit.UnitId
                                 where irequest.RequestId == RequestId
                                 select new DTOBDetailByRequestIdResponse
                                 {
                                     RankName= rk.RankAbbreviation,
                                     FName =  bd.FName,
                                     LName = bd.LName,
                                     UnitName = munit.UnitName,
                                 }
                                ).FirstOrDefaultAsync();
                return ret;
            }
            catch (Exception ex)
            {
                _logger.LogError(1001, ex, "BasicDetailDB->GetBasicDetailByRequestId");
                return null;
            }
        }
        public async Task<List<DTOICardRequestHoldResponse>?> GetAllICardRequestHold()
        {
            string query = "";
            query = " SELECT munit.UnitName,B.FName,B.LName,B.ServiceNo,trnicrd.RequestId,Afor.Name ApplyFor,ran.RankAbbreviation RankName,thold.ICardHoldId,thold.HoldReason,thold.UnHoldReason,thold.IsHold,u.DomainId,u.UpdatedOn " +
                    " FROM MTrnICardHold thold " +
                    " inner join AspNetUsers u on u.Id = thold.Updatedby " +
                    " inner join TrnICardRequest trnicrd on trnicrd.RequestId = thold.RequestId " +
                    " inner join BasicDetails B on B.BasicDetailId = trnicrd.BasicDetailId " +
                    " inner join MRank ran on ran.RankId=B.RankId " +
                    " inner join MapUnit mapunit on mapunit.UnitMapId=B.UnitId " +
                    " inner join MUnit munit on munit.UnitId=mapunit.UnitId " +
                    " inner join MApplyFor Afor on Afor.ApplyForId = B.ApplyForId ";
            try
            {
                using (var connection = _contextDP.CreateConnection())
                {
                    var allrecord = await connection.QueryAsync<DTOICardRequestHoldResponse>(query);
                    return await Task.FromResult(allrecord.ToList());
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(1001, ex, "BasicDetailDB->GetAllICardRequestHold");
                return null;
            }
        }
        public async Task<DTOBasicDetailsSaveResponse> SaveBasicDetailsWithAll(BasicDetail Data, MTrnAddress address, MTrnUpload trnUpload, MTrnIdentityInfo mTrnIdentityInfo, MTrnICardRequest mTrnICardRequest, MStepCounter mStepCounter)
        {
            bool EFCoreOrDapper = true; // true mean EFCore
            using var transaction_ = _context.Database.BeginTransaction();
            var (db, transaction) = _contextDP.CreateConnectionWithTransaction();
            DTOBasicDetailsSaveResponse dTOBasicDetailsSaveResponse = new DTOBasicDetailsSaveResponse();
            if (EFCoreOrDapper)
            {
                try
                {
                    if (Data.BasicDetailId == 0)
                    {
                        _context.BasicDetails.Add(Data);
                        await _context.SaveChangesAsync();
                        int BasicDetailId = Data.BasicDetailId;
                        address.BasicDetailId = BasicDetailId;
                        _context.TrnAddress.Add(address);
                        await _context.SaveChangesAsync();
                        trnUpload.BasicDetailId = BasicDetailId;
                        _context.TrnUpload.Add(trnUpload);
                        await _context.SaveChangesAsync();
                        mTrnIdentityInfo.BasicDetailId = BasicDetailId;
                        _context.TrnIdentityInfo.Add(mTrnIdentityInfo);
                        await _context.SaveChangesAsync();
                        mTrnICardRequest.BasicDetailId = BasicDetailId;
                        _context.TrnICardRequest.Add(mTrnICardRequest);
                        await _context.SaveChangesAsync();
                        mStepCounter.RequestId = mTrnICardRequest.RequestId;
                        _context.TrnStepCounter.Add(mStepCounter);

                        await _context.SaveChangesAsync();

                        transaction_.Commit();
                        dTOBasicDetailsSaveResponse.Result = true;
                        dTOBasicDetailsSaveResponse.Message = "Save";
                        return dTOBasicDetailsSaveResponse;
                    }
                    else
                    {

                        address.BasicDetailId = Data.BasicDetailId;
                        trnUpload.BasicDetailId = Data.BasicDetailId;
                        mTrnIdentityInfo.BasicDetailId = Data.BasicDetailId;

                        _context.Update(address);
                        await _context.SaveChangesAsync();
                        _context.Update(trnUpload);
                        await _context.SaveChangesAsync();
                        _context.Update(mTrnIdentityInfo);
                        await _context.SaveChangesAsync();

                        _context.Entry(Data).State = EntityState.Modified;
                        _context.Update(Data);
                        await _context.SaveChangesAsync();

                        transaction_.Commit();
                        dTOBasicDetailsSaveResponse.Result = true;
                        dTOBasicDetailsSaveResponse.Message = "Updae";
                        return dTOBasicDetailsSaveResponse;
                    }
                    //do other things, then commit or rollback


                }
                catch (ReferenceConstraintException ex)
                {
                    transaction_.Rollback();
                    _logger.LogError(1001, ex, "ReferenceConstraintException");
                    dTOBasicDetailsSaveResponse.Result = false;
                    dTOBasicDetailsSaveResponse.Message = ex.Message;
                    return dTOBasicDetailsSaveResponse;

                }
                catch (UniqueConstraintException ex)
                {
                    transaction_.Rollback();
                    _logger.LogError(1002, ex, "UniqueConstraintException");
                    if (ex.InnerException != null)
                    {
                        if (ex.InnerException.Message.Contains("IX_AadhaarNo"))
                        {
                            dTOBasicDetailsSaveResponse.Result = false;
                            dTOBasicDetailsSaveResponse.Message = "The provided Aadhaar number already exists. Please check and try again.";
                            return dTOBasicDetailsSaveResponse;
                        }
                        else if (ex.InnerException.Message.Contains("IX_PaperIcardNo"))
                        {
                            dTOBasicDetailsSaveResponse.Result = false;
                            dTOBasicDetailsSaveResponse.Message = "The provided PaperIcardNo number already exists. Please check and try again.";
                            return dTOBasicDetailsSaveResponse;
                        }
                        else
                        {
                            dTOBasicDetailsSaveResponse.Result = false;
                            dTOBasicDetailsSaveResponse.Message = ex.Message;
                            return dTOBasicDetailsSaveResponse;
                        }
                    }
                    else
                    {
                        dTOBasicDetailsSaveResponse.Result = false;
                        dTOBasicDetailsSaveResponse.Message = ex.Message;
                        return dTOBasicDetailsSaveResponse;

                    }


                }
                catch (MaxLengthExceededException ex)
                {
                    transaction_.Rollback();
                    _logger.LogError(1003, ex, "MaxLengthExceededException");
                    dTOBasicDetailsSaveResponse.Result = false;
                    dTOBasicDetailsSaveResponse.Message = ex.Message;
                    return dTOBasicDetailsSaveResponse;
                }
                catch (CannotInsertNullException ex)
                {
                    transaction_.Rollback();
                    _logger.LogError(1004, ex, "CannotInsertNullException");
                    dTOBasicDetailsSaveResponse.Result = false;
                    dTOBasicDetailsSaveResponse.Message = ex.Message;
                    return dTOBasicDetailsSaveResponse;
                }
                catch (NumericOverflowException ex)
                {
                    transaction_.Rollback();
                    _logger.LogError(1005, ex, "NumericOverflowException");
                    dTOBasicDetailsSaveResponse.Result = false;
                    dTOBasicDetailsSaveResponse.Message = ex.Message;
                    return dTOBasicDetailsSaveResponse;
                }
                catch (Exception ex)
                {
                    transaction_.Rollback();
                    _logger.LogError(1006, ex, "Exception");
                    dTOBasicDetailsSaveResponse.Result = false;
                    dTOBasicDetailsSaveResponse.Message = ex.Message;
                    return dTOBasicDetailsSaveResponse;
                }
            }
            else
            {
                try
                {
                    if (Data.BasicDetailId == 0)
                    {
                        var insertBasicDetail = " INSERT INTO BasicDetails (ArmedId, RankId, ServiceNo, DOB, PlaceOfIssue, DateOfIssue, DateOfCommissioning, ApplyForId, UnitId, PaperIcardNo,IsActive, Updatedby, UpdatedOn, IssuingAuthorityId, NameAsPerRecord, RegimentalId, FName, LName)" +
                                                " OUTPUT INSERTED.BasicDetailId " +
                                                " VALUES (@ArmedId, @RankId, @ServiceNo, @DOB, @PlaceOfIssue, @DateOfIssue, @DateOfCommissioning, @ApplyForId, @UnitId, @PaperIcardNo, @IsActive, @Updatedby, @UpdatedOn, @IssuingAuthorityId, @NameAsPerRecord, @RegimentalId, @FName, @LName );";
                        var parametersBD = new DynamicParameters();
                        //parametersBD.Add("@BasicDetailId", Data.BasicDetailId, DbType.Int32, ParameterDirection.Output);
                        parametersBD.Add("@ArmedId", Data.ArmedId, DbType.Byte, ParameterDirection.Input);
                        parametersBD.Add("@RankId", Data.RankId, DbType.Int16, ParameterDirection.Input);
                        parametersBD.Add("@ServiceNo", Data.ServiceNo, DbType.String, ParameterDirection.Input, 10);
                        parametersBD.Add("@DOB", Data.DOB, DbType.DateTime, ParameterDirection.Input);
                        parametersBD.Add("@PlaceOfIssue", Data.PlaceOfIssue, DbType.String, ParameterDirection.Input, 50);
                        parametersBD.Add("@DateOfIssue", Data.DateOfIssue, DbType.DateTime, ParameterDirection.Input);
                        parametersBD.Add("@DateOfCommissioning", Data.DateOfCommissioning, DbType.DateTime, ParameterDirection.Input);
                        parametersBD.Add("@ApplyForId", Data.ApplyForId, DbType.Byte, ParameterDirection.Input);
                        parametersBD.Add("@UnitId", Data.UnitId, DbType.Int32, ParameterDirection.Input);
                        parametersBD.Add("@PaperIcardNo", Data.PaperIcardNo, DbType.String, ParameterDirection.Input, 12);
                        parametersBD.Add("@IsActive", Data.IsActive, DbType.Boolean, ParameterDirection.Input);
                        parametersBD.Add("@Updatedby", Data.Updatedby, DbType.Int32, ParameterDirection.Input);
                        parametersBD.Add("@UpdatedOn", Data.UpdatedOn, DbType.DateTime, ParameterDirection.Input);
                        parametersBD.Add("@IssuingAuthorityId", Data.IssuingAuthorityId, DbType.Byte, ParameterDirection.Input);
                        parametersBD.Add("@NameAsPerRecord", Data.NameAsPerRecord, DbType.AnsiString, ParameterDirection.Input, 36);
                        parametersBD.Add("@RegimentalId", Data.RegimentalId, DbType.Byte, ParameterDirection.Input);
                        parametersBD.Add("@FName", Data.FName, DbType.AnsiString, ParameterDirection.Input, 18);
                        parametersBD.Add("@LName", Data.LName, DbType.AnsiString, ParameterDirection.Input, 18);
                        int BasicDetailId = await db.QuerySingleAsync<int>(insertBasicDetail, parametersBD, transaction: transaction);

                        address.BasicDetailId = BasicDetailId;

                        var insertAddress = " INSERT INTO TrnAddress (BasicDetailId, State, District, PS, PO, Tehsil, Village, PinCode)" +
                                            " VALUES (@BasicDetailId, @State, @District, @PS, @PO, @Tehsil, @Village, @PinCode);";
                        var parametersAddr = new DynamicParameters();
                        //parametersAddr.Add("@AddressId", address.AddressId, DbType.Int32, ParameterDirection.Input);
                        parametersAddr.Add("@BasicDetailId", address.BasicDetailId, DbType.Int32, ParameterDirection.Input);
                        parametersAddr.Add("@State", address.State, DbType.AnsiString, ParameterDirection.Input, 50);
                        parametersAddr.Add("@District", address.District, DbType.AnsiString, ParameterDirection.Input, 50);
                        parametersAddr.Add("@PS", address.PS, DbType.AnsiString, ParameterDirection.Input, 50);
                        parametersAddr.Add("@PO", address.PO, DbType.AnsiString, ParameterDirection.Input, 50);
                        parametersAddr.Add("@Tehsil", address.Tehsil, DbType.AnsiString, ParameterDirection.Input, 50);
                        parametersAddr.Add("@Village", address.Village, DbType.AnsiString, ParameterDirection.Input, 50);
                        parametersAddr.Add("@PinCode", address.PinCode, DbType.Int32, ParameterDirection.Input);
                        await db.ExecuteAsync(insertAddress, parametersAddr, transaction: transaction);

                        trnUpload.BasicDetailId = BasicDetailId;

                        var insertTrnUpload = " INSERT INTO TrnUpload (BasicDetailId, SignatureImagePath, PhotoImagePath)" +
                                              " VALUES (@BasicDetailId, @SignatureImagePath, @PhotoImagePath);";
                        var parametersUpload = new DynamicParameters();
                        //parametersUpload.Add("@UploadId", address.AddressId, DbType.Int32, ParameterDirection.Input);
                        parametersUpload.Add("@BasicDetailId", address.BasicDetailId, DbType.Int32, ParameterDirection.Input);
                        parametersUpload.Add("@SignatureImagePath", trnUpload.SignatureImagePath, DbType.String, ParameterDirection.Input, 100);
                        parametersUpload.Add("@PhotoImagePath", trnUpload.PhotoImagePath, DbType.String, ParameterDirection.Input, 100);
                        await db.ExecuteAsync(insertTrnUpload, parametersUpload, transaction: transaction);

                        mTrnIdentityInfo.BasicDetailId = BasicDetailId;

                        var insertIdentityInfo = " INSERT INTO TrnIdentityInfo (BasicDetailId, IdenMark1, IdenMark2, AadhaarNo, Height, BloodGroupId)" +
                                                 " VALUES (@BasicDetailId, @IdenMark1, @IdenMark2, @AadhaarNo, @Height, @BloodGroupId);";
                        var parametersIdentityInfo = new DynamicParameters();
                        //parametersIdentityInfo.Add("@InfoId", mTrnIdentityInfo.InfoId, DbType.Int32, ParameterDirection.Input);
                        parametersIdentityInfo.Add("@BasicDetailId", mTrnIdentityInfo.BasicDetailId, DbType.Int32, ParameterDirection.Input);
                        parametersIdentityInfo.Add("@IdenMark1", mTrnIdentityInfo.IdenMark1, DbType.String, ParameterDirection.Input, 200);
                        parametersIdentityInfo.Add("@IdenMark2", mTrnIdentityInfo.IdenMark2, DbType.String, ParameterDirection.Input, 200);
                        parametersIdentityInfo.Add("@AadhaarNo", mTrnIdentityInfo.AadhaarNo, DbType.Int64, ParameterDirection.Input);
                        parametersIdentityInfo.Add("@Height", mTrnIdentityInfo.Height, DbType.Single, ParameterDirection.Input);
                        parametersIdentityInfo.Add("@BloodGroupId", mTrnIdentityInfo.BloodGroupId, DbType.Byte, ParameterDirection.Input);
                        await db.ExecuteAsync(insertIdentityInfo, parametersIdentityInfo, transaction: transaction);

                        mTrnICardRequest.BasicDetailId = BasicDetailId;

                        var insertTrnICardRequest = " INSERT INTO TrnICardRequest (BasicDetailId, TypeId, RegistrationId, TrnDomainMappingId, TrackingId, IsActive, Updatedby, UpdatedOn, StatusId, CardSerialNo, ChipNo)" +
                                                    " OUTPUT INSERTED.RequestId " +
                                                    " VALUES (@BasicDetailId, @TypeId, @RegistrationId, @TrnDomainMappingId, @TrackingId, @IsActive, @Updatedby, @UpdatedOn, @StatusId, @CardSerialNo, @ChipNo);";
                        var parametersTrnICardRequest = new DynamicParameters();
                        //parametersTrnICardRequest.Add("@RequestId", mTrnICardRequest.RequestId, DbType.Int32, ParameterDirection.Output);
                        parametersTrnICardRequest.Add("@BasicDetailId", mTrnICardRequest.BasicDetailId, DbType.Int32, ParameterDirection.Input);
                        parametersTrnICardRequest.Add("@TypeId", mTrnICardRequest.TypeId, DbType.Byte, ParameterDirection.Input);
                        parametersTrnICardRequest.Add("@RegistrationId", mTrnICardRequest.RegistrationId, DbType.Byte, ParameterDirection.Input);
                        parametersTrnICardRequest.Add("@TrnDomainMappingId", mTrnICardRequest.TrnDomainMappingId, DbType.Int32, ParameterDirection.Input);
                        parametersTrnICardRequest.Add("@TrackingId", mTrnICardRequest.TrackingId, DbType.Int64, ParameterDirection.Input);
                        parametersTrnICardRequest.Add("@IsActive", mTrnICardRequest.IsActive, DbType.Boolean, ParameterDirection.Input);
                        parametersTrnICardRequest.Add("@Updatedby", mTrnICardRequest.Updatedby, DbType.Int32, ParameterDirection.Input);
                        parametersTrnICardRequest.Add("@UpdatedOn", mTrnICardRequest.UpdatedOn, DbType.DateTime, ParameterDirection.Input);
                        parametersTrnICardRequest.Add("@StatusId", mTrnICardRequest.StatusId, DbType.Byte, ParameterDirection.Input);
                        parametersTrnICardRequest.Add("@CardSerialNo", mTrnICardRequest.CardSerialNo, DbType.String, ParameterDirection.Input, 30);
                        parametersTrnICardRequest.Add("@ChipNo", mTrnICardRequest.ChipNo, DbType.String, ParameterDirection.Input, 30);
                        int RequestId = await db.QuerySingleAsync<int>(insertTrnICardRequest, parametersTrnICardRequest, transaction: transaction);
                        mStepCounter.RequestId = RequestId;

                        var insertTrnStepCounter = " INSERT INTO TrnStepCounter (RequestId, StepId, IsActive, Updatedby, UpdatedOn, ApplyForId)" +
                                                   " VALUES (@RequestId, @StepId, @IsActive, @Updatedby, @UpdatedOn, @ApplyForId);";
                        var parametersTrnStepCounter = new DynamicParameters();
                        //parametersTrnStepCounter.Add("@Id", mStepCounter.Id, DbType.Int32, ParameterDirection.Input);
                        parametersTrnStepCounter.Add("@RequestId", mStepCounter.RequestId, DbType.Int32, ParameterDirection.Input);
                        parametersTrnStepCounter.Add("@StepId", mStepCounter.StepId, DbType.Byte, ParameterDirection.Input);
                        parametersTrnStepCounter.Add("@IsActive", mStepCounter.IsActive, DbType.Boolean, ParameterDirection.Input);
                        parametersTrnStepCounter.Add("@Updatedby", mStepCounter.Updatedby, DbType.Int32, ParameterDirection.Input);
                        parametersTrnStepCounter.Add("@UpdatedOn", mStepCounter.UpdatedOn, DbType.DateTime, ParameterDirection.Input);
                        parametersTrnStepCounter.Add("@ApplyForId", mStepCounter.ApplyForId, DbType.Byte, ParameterDirection.Input);
                        await db.ExecuteAsync(insertTrnStepCounter, parametersTrnStepCounter, transaction: transaction);

                        transaction.Commit();
                        dTOBasicDetailsSaveResponse.Result = true;
                        dTOBasicDetailsSaveResponse.Message = "Save";
                        return dTOBasicDetailsSaveResponse;
                    }
                    else
                    {
                        address.BasicDetailId = Data.BasicDetailId;
                        trnUpload.BasicDetailId = Data.BasicDetailId;
                        mTrnIdentityInfo.BasicDetailId = Data.BasicDetailId;

                        var updateBasicDetail = " UPDATE BasicDetails SET ArmedId=@ArmedId, RankId=@RankId, ServiceNo=@ServiceNo, DOB=@DOB, PlaceOfIssue=@PlaceOfIssue, DateOfIssue=@DateOfIssue, DateOfCommissioning=@DateOfCommissioning, ApplyForId=@ApplyForId, UnitId=@UnitId, PaperIcardNo=@PaperIcardNo,IsActive=@IsActive, Updatedby=@Updatedby, UpdatedOn=@UpdatedOn, IssuingAuthorityId=@IssuingAuthorityId, NameAsPerRecord=@NameAsPerRecord, RegimentalId=@RegimentalId, FName=@FName, LName=@LName WHERE BasicDetailId=@BasicDetailId ";
                        var parametersBD = new DynamicParameters();
                        parametersBD.Add("@BasicDetailId", Data.BasicDetailId, DbType.Int32, ParameterDirection.Input);
                        parametersBD.Add("@ArmedId", Data.ArmedId, DbType.Byte, ParameterDirection.Input);
                        parametersBD.Add("@RankId", Data.RankId, DbType.Int16, ParameterDirection.Input);
                        parametersBD.Add("@ServiceNo", Data.ServiceNo, DbType.String, ParameterDirection.Input, 10);
                        parametersBD.Add("@DOB", Data.DOB, DbType.DateTime, ParameterDirection.Input);
                        parametersBD.Add("@PlaceOfIssue", Data.PlaceOfIssue, DbType.String, ParameterDirection.Input, 50);
                        parametersBD.Add("@DateOfIssue", Data.DateOfIssue, DbType.DateTime, ParameterDirection.Input);
                        parametersBD.Add("@DateOfCommissioning", Data.DateOfCommissioning, DbType.DateTime, ParameterDirection.Input);
                        parametersBD.Add("@ApplyForId", Data.ApplyForId, DbType.Byte, ParameterDirection.Input);
                        parametersBD.Add("@UnitId", Data.UnitId, DbType.Int32, ParameterDirection.Input);
                        parametersBD.Add("@PaperIcardNo", Data.PaperIcardNo, DbType.String, ParameterDirection.Input, 12);
                        parametersBD.Add("@IsActive", Data.IsActive, DbType.Boolean, ParameterDirection.Input);
                        parametersBD.Add("@Updatedby", Data.Updatedby, DbType.Int32, ParameterDirection.Input);
                        parametersBD.Add("@UpdatedOn", Data.UpdatedOn, DbType.DateTime, ParameterDirection.Input);
                        parametersBD.Add("@IssuingAuthorityId", Data.IssuingAuthorityId, DbType.Byte, ParameterDirection.Input);
                        parametersBD.Add("@NameAsPerRecord", Data.NameAsPerRecord, DbType.AnsiString, ParameterDirection.Input, 36);
                        parametersBD.Add("@RegimentalId", Data.RegimentalId, DbType.Byte, ParameterDirection.Input);
                        parametersBD.Add("@FName", Data.FName, DbType.AnsiString, ParameterDirection.Input, 18);
                        parametersBD.Add("@LName", Data.LName, DbType.AnsiString, ParameterDirection.Input, 18);
                        await db.ExecuteAsync(updateBasicDetail, parametersBD, transaction: transaction);

                        var updateAddress = " UPDATE TrnAddress SET BasicDetailId=@BasicDetailId, State=@State, District=@District, PS=@PS, PO=@PO, Tehsil=@Tehsil, Village=@Village, PinCode=@PinCode WHERE AddressId=@AddressId";
                        var parametersAddr = new DynamicParameters();
                        parametersAddr.Add("@AddressId", address.AddressId, DbType.Int32, ParameterDirection.Input);
                        parametersAddr.Add("@BasicDetailId", address.BasicDetailId, DbType.Int32, ParameterDirection.Input);
                        parametersAddr.Add("@State", address.State, DbType.AnsiString, ParameterDirection.Input, 50);
                        parametersAddr.Add("@District", address.District, DbType.AnsiString, ParameterDirection.Input, 50);
                        parametersAddr.Add("@PS", address.PS, DbType.AnsiString, ParameterDirection.Input, 50);
                        parametersAddr.Add("@PO", address.PO, DbType.AnsiString, ParameterDirection.Input, 50);
                        parametersAddr.Add("@Tehsil", address.Tehsil, DbType.AnsiString, ParameterDirection.Input, 50);
                        parametersAddr.Add("@Village", address.Village, DbType.AnsiString, ParameterDirection.Input, 50);
                        parametersAddr.Add("@PinCode", address.PinCode, DbType.Int32, ParameterDirection.Input);
                        await db.ExecuteAsync(updateAddress, parametersAddr, transaction: transaction);

                        var updateTrnUpload = " UPDATE TrnUpload SET BasicDetailId=@BasicDetailId, SignatureImagePath=@SignatureImagePath, PhotoImagePath=@PhotoImagePath WHERE UploadId=@UploadId";
                        var parametersUpload = new DynamicParameters();
                        parametersUpload.Add("@UploadId", address.AddressId, DbType.Int32, ParameterDirection.Input);
                        parametersUpload.Add("@BasicDetailId", address.BasicDetailId, DbType.Int32, ParameterDirection.Input);
                        parametersUpload.Add("@SignatureImagePath", trnUpload.SignatureImagePath, DbType.String, ParameterDirection.Input, 100);
                        parametersUpload.Add("@PhotoImagePath", trnUpload.PhotoImagePath, DbType.String, ParameterDirection.Input, 100);
                        await db.ExecuteAsync(updateTrnUpload, parametersUpload, transaction: transaction);

                        var updateIdentityInfo = " UPDATE TrnIdentityInfo SET BasicDetailId=@BasicDetailId, IdenMark1=@IdenMark1, IdenMark2=@IdenMark2, AadhaarNo=@AadhaarNo, Height=@Height, BloodGroupId=@BloodGroupId WHERE InfoId=@InfoId";
                        var parametersIdentityInfo = new DynamicParameters();
                        parametersIdentityInfo.Add("@InfoId", mTrnIdentityInfo.InfoId, DbType.Int32, ParameterDirection.Input);
                        parametersIdentityInfo.Add("@BasicDetailId", mTrnIdentityInfo.BasicDetailId, DbType.Int32, ParameterDirection.Input);
                        parametersIdentityInfo.Add("@IdenMark1", mTrnIdentityInfo.IdenMark1, DbType.String, ParameterDirection.Input, 200);
                        parametersIdentityInfo.Add("@IdenMark2", mTrnIdentityInfo.IdenMark2, DbType.String, ParameterDirection.Input, 200);
                        parametersIdentityInfo.Add("@AadhaarNo", mTrnIdentityInfo.AadhaarNo, DbType.Int64, ParameterDirection.Input);
                        parametersIdentityInfo.Add("@Height", mTrnIdentityInfo.Height, DbType.Single, ParameterDirection.Input);
                        parametersIdentityInfo.Add("@BloodGroupId", mTrnIdentityInfo.BloodGroupId, DbType.Byte, ParameterDirection.Input);
                        await db.ExecuteAsync(updateIdentityInfo, parametersIdentityInfo, transaction: transaction);

                        transaction.Commit();
                        dTOBasicDetailsSaveResponse.Result = true;
                        dTOBasicDetailsSaveResponse.Message = "Updae";
                        return dTOBasicDetailsSaveResponse;
                    }
                }
                catch (SqlException ex) // Unique constraint violation error number
                {
                    transaction.Rollback();  // Rollback the transaction
                    _logger.LogError(1006, ex, "BasicDetailDB->SaveBasicDetailsWithAll");
                    if (ex.Number == 2601 || ex.Number == 2627)
                    {
                        if (ex.Message.Contains("IX_AadhaarNo"))
                        {
                            dTOBasicDetailsSaveResponse.Result = false;
                            dTOBasicDetailsSaveResponse.Message = "The provided Aadhaar number already exists. Please check and try again.";
                            return dTOBasicDetailsSaveResponse;
                        }
                        else if (ex.Message.Contains("IX_PaperIcardNo"))
                        {
                            dTOBasicDetailsSaveResponse.Result = false;
                            dTOBasicDetailsSaveResponse.Message = "The provided PaperIcardNo number already exists. Please check and try again.";
                            return dTOBasicDetailsSaveResponse;
                        }
                        else
                        {
                            dTOBasicDetailsSaveResponse.Result = false;
                            dTOBasicDetailsSaveResponse.Message = ex.Message;
                            return dTOBasicDetailsSaveResponse;
                        }
                    }
                    else
                    {
                        dTOBasicDetailsSaveResponse.Result = false;
                        dTOBasicDetailsSaveResponse.Message = ex.Message;
                        return dTOBasicDetailsSaveResponse;
                    }


                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    //_logger.LogError(1006, ex, "BasicDetailDB->SaveBasicDetailsWithAll");
                    dTOBasicDetailsSaveResponse.Result = false;
                    dTOBasicDetailsSaveResponse.Message = ex.Message;
                    return dTOBasicDetailsSaveResponse;
                }
                finally
                {
                    // Dispose of the connection
                    db.Dispose();
                }
            }
        }
        public async Task<BasicDetail?> FindServiceNo(string ServiceNo)
        {
            string query = "Select * from BasicDetails where ServiceNo = @ServiceNo ";
            try
            {
                using (var connection = _contextDP.CreateConnection())
                {
                    BasicDetail basicDetail = await connection.QuerySingleOrDefaultAsync<BasicDetail>(query, new { ServiceNo });
                    if (basicDetail != null)
                    {
                        return basicDetail;
                    }
                    else
                    {
                        return null;
                    }
                }
            }
            catch (Exception ex) 
            {
                _logger.LogError(1001, ex, "BasicDetailDB->FindServiceNo");
                return null;
            }


        }
        public async Task<List<DTOSmartSearch>?> SearchAllServiceNo(DTOSearchArmyNoRequest dto)
        {
            string unitQuery = dto.Claim ? "" : "and tdm.UnitId=@MapUnitId";
            string query = "";
            if (dto.TypeId == KeyConstants.ApplicantPostingOut || dto.TypeId == KeyConstants.ApplicantClose)
            {
                query = @"Select Distinct TOP 5 basi.BasicDetailId,FName,LName,ServiceNo,PhotoImagePath Image,req.RequestId
                            from BasicDetails basi
                            inner join TrnICardRequest req on req.BasicDetailId=basi.BasicDetailId and req.StatusId=1
                            inner join TrnDomainMapping map on map.Id = req.TrnDomainMappingId and map.UnitId=@MapUnitId
                            inner join TrnUpload trnu on basi.BasicDetailId=trnu.BasicDetailId 
                            where ServiceNo like @ServiceNo ";
            }
            else if (dto.TypeId == KeyConstants.FaultyCardRequest)
            {
                query = @$"Select Distinct TOP 5 basi.BasicDetailId,FName,LName,ServiceNo,PhotoImagePath Image,req.RequestId,COALESCE(MAX(fwd.TrnFwdId), NULL) AS MaxTrnFwdId  
                                from BasicDetails basi
                                inner join TrnUpload trnu on basi.BasicDetailId=trnu.BasicDetailId 
                                inner join TrnICardRequest req on req.BasicDetailId=basi.BasicDetailId and req.StatusId=1
                                inner join TrnStepCounter stepcount on req.RequestId=stepcount.RequestId and stepcount.StepId=6
                                inner join TrnDomainMapping tdm on tdm.Id=req.TrnDomainMappingId {unitQuery}
                                LEFT JOIN TrnFwds fwd ON fwd.RequestId = req.RequestId
                                where ServiceNo like @ServiceNo";
            }
            else if (dto.TypeId == KeyConstants.HoltlistCardRequest)
            {
                query = @$"Select Distinct TOP 5 basi.BasicDetailId,FName,LName,ServiceNo,PhotoImagePath Image,req.RequestId,COALESCE(MAX(fwd.TrnFwdId), NULL) AS MaxTrnFwdId  
                                from BasicDetails basi
                                inner join TrnUpload trnu on basi.BasicDetailId=trnu.BasicDetailId 
                                inner join TrnICardRequest req on req.BasicDetailId=basi.BasicDetailId and req.StatusId=1
                                inner join TrnStepCounter stepcount on req.RequestId=stepcount.RequestId and stepcount.StepId=6
                                inner join TrnDomainMapping tdm on tdm.Id=req.TrnDomainMappingId and tdm.UnitId=@MapUnitId
                                LEFT JOIN TrnFwds fwd ON fwd.RequestId = req.RequestId
                                Left join TrnHotlistCards thc on req.RequestId = thc.RequestId
                                where thc.RequestId is null and ServiceNo like @ServiceNo";
            }
            else if (dto.TypeId == KeyConstants.LostCardRequest)
            {
                query = @$"Select Distinct TOP 5 basi.BasicDetailId,FName,LName,ServiceNo,PhotoImagePath Image,req.RequestId,COALESCE(MAX(fwd.TrnFwdId), NULL) AS MaxTrnFwdId  
                                from BasicDetails basi
                                inner join TrnUpload trnu on basi.BasicDetailId=trnu.BasicDetailId 
                                inner join TrnICardRequest req on req.BasicDetailId=basi.BasicDetailId and req.StatusId=1
                                inner join TrnStepCounter stepcount on req.RequestId=stepcount.RequestId and stepcount.StepId=6
                                inner join TrnDomainMapping tdm on tdm.Id=req.TrnDomainMappingId
                                LEFT JOIN TrnFwds fwd ON fwd.RequestId = req.RequestId
                                Left join TrnLostCards tlc on req.RequestId = tlc.RequestId
                                where tlc.RequestId is null and ServiceNo like @ServiceNo";
            }

            try
            {
                //ServiceNo = "%" + ServiceNo.Replace("[", "[[]").Replace("%", "[%]") + "%";
                using (var connection = _contextDP.CreateConnection())
                {
                    var parameters = new DynamicParameters();
                    parameters.Add("@AspNetUsersId", dto.AspNetUsersId, DbType.Int32, ParameterDirection.Input);
                    parameters.Add("@MapUnitId", dto.MapUnitId, DbType.Int32, ParameterDirection.Input);
                    parameters.Add("@ServiceNo", $"%{dto.ArmyNo}%", DbType.String, ParameterDirection.Input);

                    var basicDetail = await connection.QueryAsync<DTOSmartSearch>(query, parameters);
                    if (basicDetail != null)
                    {
                        return basicDetail.ToList();
                    }
                    else
                    {
                        return null;
                    }
                }
            }
            catch(Exception ex)
            {
                _logger.LogError(1001, ex, "BasicDetailDB->SearchAllServiceNo");
                return null;
            }
        }
        public async Task<DTOBasicDetailForParitalViewResponse> GetBasicDetailForParitalViewByRequestId(int RequestId)
        {
            try
            {

                string query = @"SELECT bas.PaperIcardNo,bas.NameAsPerRecord,bas.FName,bas.LName,bas.ServiceNo,bas.DOB,bas.DateOfIssue,bas.DateOfCommissioning,bas.PlaceOfIssue,
                                issaut.Name IssuingAuthorityName,trnadd.State,trnadd.District,trnadd.PS,trnadd.PO,trnadd.Tehsil,trnadd.Village,trnadd.PinCode,
                                IdenMark1,AadhaarNo,Height,bld.BloodGroup,regi.Abbreviation RegimentalName,Muni.UnitName,
                                ranks.RankAbbreviation RankName,arm.Abbreviation ArmedName,
                                icardreq.RequestId,icardreq.UpdatedOn RequestDate,appl.Name ApplyFor,uplod.PhotoImagePath,uplod.SignatureImagePath,
                                CASE
                                WHEN LEFT(bas.ServiceNo, 2) LIKE '[A-Za-z][A-Za-z]' THEN
                                CONCAT(SUBSTRING(bas.ServiceNo, 1, 2), ' ', SUBSTRING(bas.ServiceNo, 3, LEN(bas.ServiceNo) - 2))
                                ELSE
                                bas.ServiceNo
                                END AS ModifiedServiceNo
                                from BasicDetails bas
                                inner join MIssuingAuthority issaut on issaut.IssuingAuthorityId=bas.IssuingAuthorityId
                                inner join TrnAddress trnadd on trnadd.BasicDetailId=bas.BasicDetailId
                                inner join TrnUpload uplod on uplod.BasicDetailId=bas.BasicDetailId
                                inner join TrnIdentityInfo trninfo on trninfo.BasicDetailId=bas.BasicDetailId
                                inner join MBloodGroup bld on bld.BloodGroupId=trninfo.BloodGroupId
                                inner join MRank ranks on ranks.RankId=bas.RankId
                                inner join MArmedType arm on arm.ArmedId=bas.ArmedId
                                inner join MapUnit uni on uni.UnitMapId=bas.UnitId
                                inner join MUnit Muni on Muni.UnitId=uni.UnitId
                                inner join MApplyFor appl on appl.ApplyForId=bas.ApplyForId
                                left join MRegimental regi on regi.RegId=bas.RegimentalId
                                inner join TrnICardRequest icardreq on icardreq.BasicDetailId=bas.BasicDetailId
                                inner join TrnStepCounter stepcount on icardreq.RequestId=stepcount.RequestId
                                where icardreq.RequestId=@RequestId";
                using (var connection = _contextDP.CreateConnection())
                {
                    var ret = await connection.QueryAsync<DTOBasicDetailForParitalViewResponse>(query, new { RequestId });

                    return ret.FirstOrDefault() ?? new DTOBasicDetailForParitalViewResponse();

                }
            }
            catch (Exception ex)
            {
                _logger.LogError(1001, ex, "BasicDetailDB->GetBasicDetailForParitalViewByRequestId");
                return new DTOBasicDetailForParitalViewResponse();
            }
        }
        public async Task<List<DTOICardTypeRequest>> GetAllICardType()
        {
            string query = "Select * from MICardType";
            try
            {
                using (var connection = _contextDP.CreateConnection())
                {
                    var ICardTypeList = await connection.QueryAsync<DTOICardTypeRequest>(query);
                    var allrecord = (from e in ICardTypeList
                                     select new DTOICardTypeRequest()
                                     {
                                         TypeId = e.TypeId,
                                         EncryptedId = protector.Protect(e.TypeId.ToString()),
                                         Name = e.Name,
                                     }).ToList();
                    return await Task.FromResult(allrecord);
                }
            }
            catch(Exception ex)
            {
                _logger.LogError(1001, ex, "BasicDetailDB->GetAllICardType");
                return new List<DTOICardTypeRequest>();
            }

        }

        public async Task<List<BasicDetailVM>> GetALLForIcardSttaus(int UserId, int stepcount, int TypeId, int apply)
        {
            int? applyfor = 0;
            if (apply == 0) applyfor = null; else applyfor = apply;

            string query = "";

            if (stepcount == 0)//////For all record
            {
                query = " SELECT " +
                        " CASE " +
                        " WHEN LEFT(B.ServiceNo, 2) LIKE '[A-Za-z][A-Za-z]' THEN " +
                        " CONCAT(SUBSTRING(B.ServiceNo, 1, 2), ' ', SUBSTRING(B.ServiceNo, 3, LEN(B.ServiceNo) - 2)) " +
                        " ELSE " +
                        " B.ServiceNo " +
                        " END AS ModifiedServiceNo," +
                        " trnicrd.RegistrationId RegistrationApplyFor,munit.UnitName,B.UnitId,B.BasicDetailId,B.FName,B.LName,B.ServiceNo,B.DOB,B.DateOfCommissioning,C.StepId StepCounter,C.Id StepId,ty.Name ICardType,trnicrd.RequestId,ISNULL(fwd.TrnFwdId,0) IsTrnFwdId,fwd.Remark,ISNULL(fwd.FwdStatusId,0) IsFwdStatusId,Afor.Name ApplyFor,Afor.ApplyForId ,trnicrd.TrackingId,ran.RankAbbreviation RankName,ISNULL(Postout.Id,0) IsPosting FROM BasicDetails B " +
                        " inner join MRank ran on ran.RankId=B.RankId "  +
                        " inner join MapUnit mapunit on mapunit.UnitMapId=B.UnitId " +
                        " inner join MUnit munit on munit.UnitId=mapunit.UnitId " +
                        " inner join TrnICardRequest trnicrd on trnicrd.BasicDetailId = B.BasicDetailId " +
                        " inner join MApplyFor Afor on Afor.ApplyForId = B.ApplyForId " +
                        " inner join TrnStepCounter C on trnicrd.RequestId = C.RequestId " +
                        " inner join MICardType ty on ty.TypeId = trnicrd.TypeId " +
                        " inner join TrnDomainMapping map on map.Id= trnicrd.TrnDomainMappingId " +
                        " inner join UserProfile pr on pr.UserId = map.UserId " +
                        " left join TrnFwds fwd on fwd.FromAspNetUsersId= map.AspNetUsersId and fwd.IsComplete=0 and fwd.RequestId=trnicrd.RequestId " +
                        " left join MTrnFwdStatus mtrnfwdstatus on mtrnfwdstatus.FwdStatusId = fwd.FwdStatusId " +
                        " left join TrnPostingOut Postout on Postout.RequestId=trnicrd.RequestId and trnicrd.StatusId=1 " +
                        " WHERE map.AspNetUsersId = @UserId and Afor.ApplyForId=ISNULL(@applyfor,Afor.ApplyForId) ORDER BY B.UpdatedOn DESC";

            }
            else if (stepcount == 1)//////For Draft
            {
                query = " SELECT " +
                        " CASE " +
                        " WHEN LEFT(B.ServiceNo, 2) LIKE '[A-Za-z][A-Za-z]' THEN " +
                        " CONCAT(SUBSTRING(B.ServiceNo, 1, 2), ' ', SUBSTRING(B.ServiceNo, 3, LEN(B.ServiceNo) - 2)) " +
                        " ELSE " +
                        " B.ServiceNo " +
                        " END AS ModifiedServiceNo," +
                        " trnicrd.RegistrationId RegistrationApplyFor,munit.UnitName,B.UnitId,B.BasicDetailId,B.FName,B.LName,B.ServiceNo,B.DOB,B.DateOfCommissioning,C.StepId StepCounter,C.Id StepId,ty.Name ICardType,trnicrd.RequestId,ISNULL(fwd.TrnFwdId,0) IsTrnFwdId,fwd.Remark,ISNULL(fwd.FwdStatusId,0) IsFwdStatusId,Afor.Name ApplyFor,Afor.ApplyForId ,trnicrd.TrackingId,ran.RankAbbreviation RankName,ISNULL(Postout.Id,0) IsPosting FROM BasicDetails B " +
                        " inner join MRank ran on ran.RankId=B.RankId " +
                        " inner join MapUnit mapunit on mapunit.UnitMapId=B.UnitId " +
                        " inner join MUnit munit on munit.UnitId=mapunit.UnitId " +
                        " inner join TrnICardRequest trnicrd on trnicrd.BasicDetailId = B.BasicDetailId " +
                        " inner join MApplyFor Afor on Afor.ApplyForId = B.ApplyForId " +
                        " inner join TrnStepCounter C on trnicrd.RequestId = C.RequestId " +
                        " inner join MICardType ty on ty.TypeId = trnicrd.TypeId " +
                        " inner join TrnDomainMapping map on map.Id= trnicrd.TrnDomainMappingId " +
                        " inner join UserProfile pr on pr.UserId = map.UserId " +
                        " left join TrnFwds fwd on fwd.FromAspNetUsersId= map.AspNetUsersId and fwd.IsComplete=0 and fwd.RequestId=trnicrd.RequestId " +
                        " left join MTrnFwdStatus mtrnfwdstatus on mtrnfwdstatus.FwdStatusId = fwd.FwdStatusId " +
                        " left join TrnPostingOut Postout on Postout.RequestId=trnicrd.RequestId and trnicrd.StatusId=1 " +
                        " WHERE map.AspNetUsersId = @UserId and Afor.ApplyForId=ISNULL(@applyfor,Afor.ApplyForId) and trnicrd.StatusId=1  and C.StepId = @stepcount  ORDER BY B.UpdatedOn DESC";

            }
            else if (stepcount == 777)//////For Completed   
            {
                query = " SELECT distinct " +
                        " CASE " +
                        " WHEN LEFT(B.ServiceNo, 2) LIKE '[A-Za-z][A-Za-z]' THEN " +
                        " CONCAT(SUBSTRING(B.ServiceNo, 1, 2), ' ', SUBSTRING(B.ServiceNo, 3, LEN(B.ServiceNo) - 2)) " +
                        " ELSE " +
                        " B.ServiceNo " +
                        " END AS ModifiedServiceNo," +
                        " trnicrd.RegistrationId RegistrationApplyFor,trnicrd.RegistrationId RegistrationApplyFor,munit.UnitName,B.UnitId,B.BasicDetailId,B.FName,B.LName,B.ServiceNo,B.DOB,B.DateOfCommissioning,C.StepId StepCounter,C.Id StepId,ty.Name ICardType,trnicrd.RequestId,fwd.Remark,ISNULL(fwd.FwdStatusId,0) IsFwdStatusId,Afor.Name ApplyFor,Afor.ApplyForId ,trnicrd.TrackingId,ran.RankAbbreviation RankName,ISNULL(Postout.Id,0) IsPosting FROM BasicDetails B " +
                        " inner join MRank ran on ran.RankId=B.RankId " +
                        " inner join MapUnit mapunit on mapunit.UnitMapId=B.UnitId " +
                        " inner join MUnit munit on munit.UnitId=mapunit.UnitId " +
                        " inner join TrnICardRequest trnicrd on trnicrd.BasicDetailId = B.BasicDetailId " +
                        " inner join MApplyFor Afor on Afor.ApplyForId = B.ApplyForId " +
                        " inner join TrnStepCounter C on trnicrd.RequestId = C.RequestId " +
                        " inner join MICardType ty on ty.TypeId = trnicrd.TypeId " +
                        " inner join TrnDomainMapping map on map.Id= trnicrd.TrnDomainMappingId " +
                        " inner join UserProfile pr on pr.UserId = map.UserId " +
                        " inner join TrnFwds fwd on fwd.FromAspNetUsersId= map.AspNetUsersId and fwd.IsComplete=1 and fwd.RequestId=trnicrd.RequestId " +
                        " inner join MTrnFwdStatus mtrnfwdstatus on mtrnfwdstatus.FwdStatusId = fwd.FwdStatusId " +
                        " left join TrnPostingOut Postout on Postout.RequestId=trnicrd.RequestId and trnicrd.StatusId=1 " +
                        " WHERE map.AspNetUsersId = @UserId and Afor.ApplyForId=ISNULL(@applyfor,Afor.ApplyForId) and trnicrd.StatusId = 2 ";

            }
            else if (stepcount == 888)//////For Submitted
            {
                query = " SELECT distinct " +
                        " CASE " +
                        " WHEN LEFT(B.ServiceNo, 2) LIKE '[A-Za-z][A-Za-z]' THEN " +
                        " CONCAT(SUBSTRING(B.ServiceNo, 1, 2), ' ', SUBSTRING(B.ServiceNo, 3, LEN(B.ServiceNo) - 2)) " +
                        " ELSE " +
                        " B.ServiceNo " +
                        " END AS ModifiedServiceNo," +
                        " trnicrd.RegistrationId RegistrationApplyFor,munit.UnitName,B.UnitId,B.BasicDetailId,B.FName,B.LName,B.ServiceNo,B.DOB,B.DateOfCommissioning,C.StepId StepCounter,C.Id StepId,ty.Name ICardType,trnicrd.RequestId,Afor.Name ApplyFor,Afor.ApplyForId ,trnicrd.TrackingId,ran.RankAbbreviation RankName,ISNULL(Postout.Id,0) IsPosting FROM BasicDetails B " +
                        " inner join MRank ran on ran.RankId=B.RankId " +
                        " inner join MapUnit mapunit on mapunit.UnitMapId=B.UnitId " +
                        " inner join MUnit munit on munit.UnitId=mapunit.UnitId " +
                        " inner join TrnICardRequest trnicrd on trnicrd.BasicDetailId = B.BasicDetailId " +
                        " inner join MApplyFor Afor on Afor.ApplyForId = B.ApplyForId " +
                        " inner join TrnStepCounter C on trnicrd.RequestId = C.RequestId " +
                        " inner join MICardType ty on ty.TypeId = trnicrd.TypeId " +
                        " inner join TrnDomainMapping map on map.Id= trnicrd.TrnDomainMappingId " +
                        " inner join UserProfile pr on pr.UserId = map.UserId " +
                        " left join TrnPostingOut Postout on Postout.RequestId=trnicrd.RequestId  " +
                        " WHERE map.AspNetUsersId = @UserId and Afor.ApplyForId=ISNULL(@applyfor,Afor.ApplyForId) and C.StepId > 1 ";

            }
            else if (stepcount == 5)
            {
                query = " SELECT distinct " +
                        " CASE " +
                        " WHEN LEFT(B.ServiceNo, 2) LIKE '[A-Za-z][A-Za-z]' THEN " +
                        " CONCAT(SUBSTRING(B.ServiceNo, 1, 2), ' ', SUBSTRING(B.ServiceNo, 3, LEN(B.ServiceNo) - 2)) " +
                        " ELSE " +
                        " B.ServiceNo " +
                        " END AS ModifiedServiceNo," +
                        " trnicrd.RegistrationId RegistrationApplyFor,munit.UnitName,B.UnitId,B.BasicDetailId,B.FName,B.LName,B.ServiceNo,B.DOB,B.DateOfCommissioning,C.StepId StepCounter,C.Id StepId,ty.TypeId,ty.name ICardType,trnicrd.RequestId,ISNULL(fwd.TrnFwdId,0) IsTrnFwdId, ISNULL(fwd.FwdStatusId,0) IsFwdStatusId, Afor.Name ApplyFor,Afor.ApplyForId ,trnicrd.TrackingId,ran.RankAbbreviation RankName,ISNULL(Postout.Id,0) IsPosting" +
                        " FROM BasicDetails B" +
                        " inner join MRank ran on ran.RankId=B.RankId" +
                        " inner join MapUnit mapunit on mapunit.UnitMapId=B.UnitId " +
                        " inner join MUnit munit on munit.UnitId=mapunit.UnitId " +
                        " inner join TrnICardRequest trnicrd on trnicrd.BasicDetailId = B.BasicDetailId" +
                        " inner join MApplyFor Afor on Afor.ApplyForId = B.ApplyForId " +
                        " inner join TrnStepCounter C on trnicrd.RequestId = C.RequestId" +
                        " inner join MICardType ty on ty.TypeId = trnicrd.TypeId" +
                        " left join TrnPostingOut Postout on Postout.RequestId=trnicrd.RequestId and trnicrd.StatusId=2 " +
                        " inner join TrnFwds fwd on fwd.RequestId = trnicrd.RequestId and fwd.FromAspNetUsersId = @UserId and Afor.ApplyForId=ISNULL(@applyfor,Afor.ApplyForId) and C.StepId = @stepcount " +
                        " inner join MTrnFwdStatus mtrnfwdstatus on mtrnfwdstatus.FwdStatusId = fwd.FwdStatusId " +
                        " where trnicrd.StatusId=2 ";
            }
            else if (stepcount == 2 || stepcount == 3 || stepcount == 4 || stepcount == 6)//IO
            {
               //if(TypeId==2)
                {
                    query = " SELECT distinct " +
                            " CASE" +
                            " WHEN LEFT(B.ServiceNo, 2) LIKE '[A-Za-z][A-Za-z]' THEN " +
                            " CONCAT(SUBSTRING(B.ServiceNo, 1, 2), ' ', SUBSTRING(B.ServiceNo, 3, LEN(B.ServiceNo) - 2)) " +
                            " ELSE " +
                            " B.ServiceNo " +
                            " END AS ModifiedServiceNo," +
                            " trnicrd.RegistrationId RegistrationApplyFor,munit.UnitName,B.UnitId,B.BasicDetailId,B.FName,B.LName,B.ServiceNo,B.DOB,B.DateOfCommissioning,C.StepId StepCounter,C.Id StepId,ty.TypeId,ty.name ICardType,trnicrd.RequestId,ISNULL(fwd.TrnFwdId,0) IsTrnFwdId, ISNULL(fwd.FwdStatusId,0) IsFwdStatusId ,Afor.Name ApplyFor,Afor.ApplyForId ,trnicrd.TrackingId,ran.RankAbbreviation RankName,ISNULL(Postout.Id,0) IsPosting" +
                            " FROM BasicDetails B" +
                            " inner join MRank ran on ran.RankId=B.RankId" +
                            " inner join MapUnit mapunit on mapunit.UnitMapId=B.UnitId " +
                            " inner join MUnit munit on munit.UnitId=mapunit.UnitId " +
                            " inner join TrnICardRequest trnicrd on trnicrd.BasicDetailId = B.BasicDetailId" +
                            " inner join MApplyFor Afor on Afor.ApplyForId = B.ApplyForId " +
                            " inner join TrnStepCounter C on trnicrd.RequestId = C.RequestId" +
                            " inner join MICardType ty on ty.TypeId = trnicrd.TypeId" +
                            " left join TrnPostingOut Postout on Postout.RequestId=trnicrd.RequestId and trnicrd.StatusId=1 " +
                            " inner join TrnFwds fwd on fwd.RequestId = trnicrd.RequestId and fwd.FromAspNetUsersId = @UserId and Afor.ApplyForId=ISNULL(@applyfor,Afor.ApplyForId) and fwd.IsComplete=0 and C.StepId = @stepcount " +
                            " inner join MTrnFwdStatus mtrnfwdstatus on mtrnfwdstatus.FwdStatusId = fwd.FwdStatusId " +
                            " where trnicrd.StatusId=1 ";
                }
            //    else if (TypeId == 3)
            //    {
            //        query = " SELECT distinct B.BasicDetailId,B.Name,B.ServiceNo,B.DOB,B.DateOfCommissioning,C.StepId StepCounter,C.Id StepId,ty.TypeId,ty.name ICardType,trnicrd.RequestId ,Afor.Name ApplyFor,Afor.ApplyForId ,trnicrd.TrackingId,ran.RankAbbreviation RankName,ISNULL(Postout.Id,0) IsPosting" +
            //" FROM BasicDetails B" +
            //" inner join MRank ran on ran.RankId=B.RankId" +
            //" inner join TrnICardRequest trnicrd on trnicrd.BasicDetailId = B.BasicDetailId" +
            //" inner join MApplyFor Afor on Afor.ApplyForId = B.ApplyForId " +
            //" inner join TrnStepCounter C on trnicrd.RequestId = C.RequestId" +
            //" inner join MICardType ty on ty.TypeId = trnicrd.TypeId" +
            //" left join TrnPostingOut Postout on Postout.RequestId=trnicrd.RequestId and trnicrd.StatusId=1 " +
            //" inner join TrnFwds fwd on fwd.RequestId = trnicrd.RequestId and fwd.FromAspNetUsersId = @UserId and Afor.ApplyForId=ISNULL(@applyfor,Afor.ApplyForId) and C.StepId = @stepcount where trnicrd.StatusId=1";
            //    }


            }
            else if (stepcount == 7 || stepcount == 8 || stepcount == 9 || stepcount == 10 )//Reject From IO
            {

                query = " SELECT distinct " +
                        " CASE " +
                        " WHEN LEFT(B.ServiceNo, 2) LIKE '[A-Za-z][A-Za-z]' THEN " +
                        " CONCAT(SUBSTRING(B.ServiceNo, 1, 2), ' ', SUBSTRING(B.ServiceNo, 3, LEN(B.ServiceNo) - 2)) " +
                        " ELSE " +
                        " B.ServiceNo " +
                        " END AS ModifiedServiceNo," +
                        " trnicrd.RegistrationId RegistrationApplyFor,munit.UnitName,B.UnitId,B.BasicDetailId,B.FName,B.LName,B.ServiceNo,B.DOB,B.DateOfCommissioning,C.StepId StepCounter,C.Id StepId,ty.TypeId,ty.name ICardType,trnicrd.RequestId, ISNULL(fwd.TrnFwdId,0) IsTrnFwdId,ISNULL(fwd.FwdStatusId,0) IsFwdStatusId, Afor.Name ApplyFor,Afor.ApplyForId,trnicrd.TrackingId,ran.RankAbbreviation RankName,ISNULL(Postout.Id,0) IsPosting" +
                        " FROM BasicDetails B" +
                        " inner join MRank ran on ran.RankId=B.RankId" +
                        " inner join MapUnit mapunit on mapunit.UnitMapId=B.UnitId " +
                        " inner join MUnit munit on munit.UnitId=mapunit.UnitId " +
                        " inner join TrnICardRequest trnicrd on trnicrd.BasicDetailId = B.BasicDetailId" +
                        " inner join MApplyFor Afor on Afor.ApplyForId = B.ApplyForId " +
                        " inner join TrnStepCounter C on trnicrd.RequestId = C.RequestId" +
                        " inner join MICardType ty on ty.TypeId = trnicrd.TypeId" +
                        " left join TrnPostingOut Postout on Postout.RequestId=trnicrd.RequestId and trnicrd.StatusId=1 " +
                        " inner join TrnFwds fwd on fwd.RequestId = trnicrd.RequestId and fwd.ToAspNetUsersId = @UserId and Afor.ApplyForId=ISNULL(@applyfor,Afor.ApplyForId) and fwd.FwdStatusId=3 " +
                        " inner join MTrnFwdStatus mtrnfwdstatus on mtrnfwdstatus.FwdStatusId = fwd.FwdStatusId " +
                        " where trnicrd.StatusId=1 ";
            }
            else if (stepcount == 999)//Reject From IO,MI11 and HQ 54
            {

                query = " SELECT distinct " +
                        " CASE" +
                        " WHEN LEFT(B.ServiceNo, 2) LIKE '[A-Za-z][A-Za-z]' THEN" +
                        " CONCAT(SUBSTRING(B.ServiceNo, 1, 2), ' ', SUBSTRING(B.ServiceNo, 3, LEN(B.ServiceNo) - 2))" +
                        " ELSE" +
                        " B.ServiceNo" +
                        " END AS ModifiedServiceNo," +
                        " trnicrd.RegistrationId RegistrationApplyFor,munit.UnitName,B.UnitId,B.BasicDetailId,B.FName,B.LName,B.ServiceNo,B.DOB,B.DateOfCommissioning,C.StepId StepCounter,C.Id StepId,ty.TypeId,ty.name ICardType,trnicrd.RequestId, ISNULL(fwd.FwdStatusId,0) IsFwdStatusId ,Afor.Name ApplyFor,Afor.ApplyForId,trnicrd.TrackingId,ran.RankAbbreviation RankName,ISNULL(Postout.Id,0) IsPosting" +
                        " FROM BasicDetails B" +
                        " inner join MRank ran on ran.RankId=B.RankId" +
                        " inner join MapUnit mapunit on mapunit.UnitMapId=B.UnitId " +
                        " inner join MUnit munit on munit.UnitId=mapunit.UnitId " +
                        " inner join TrnICardRequest trnicrd on trnicrd.BasicDetailId = B.BasicDetailId" +
                        " inner join MApplyFor Afor on Afor.ApplyForId = B.ApplyForId " +
                        " inner join TrnStepCounter C on trnicrd.RequestId = C.RequestId" +
                        " inner join MICardType ty on ty.TypeId = trnicrd.TypeId" +
                        " left join TrnPostingOut Postout on Postout.RequestId=trnicrd.RequestId and trnicrd.StatusId=1 " +
                        " inner join TrnFwds fwd on fwd.RequestId = trnicrd.RequestId and fwd.ToAspNetUsersId = @UserId and Afor.ApplyForId=ISNULL(@applyfor,Afor.ApplyForId) and fwd.FwdStatusId=3 and C.StepId in (7,8,9,10) " +
                        " inner join MTrnFwdStatus mtrnfwdstatus on mtrnfwdstatus.FwdStatusId = fwd.FwdStatusId "+
                        " where trnicrd.StatusId=1";
            }
            try
            {
                using (var connection = _contextDP.CreateConnection())
                {
                    var BasicDetailList = await connection.QueryAsync<BasicDetailVM>(query, new { UserId, stepcount, TypeId, applyfor });
                        int sno = 1;
                    var allrecord = (from e in BasicDetailList
                                     select new BasicDetailVM()
                                     {
                                         BasicDetailId = e.BasicDetailId,
                                         RegistrationApplyFor = e.RegistrationApplyFor,
                                         EncryptedId = protector.Protect(e.BasicDetailId.ToString()),
                                         EncryptedRequestId = protector.Protect(e.RequestId.ToString()),
                                         Sno = sno++,
                                         FName = e.FName,
                                         LName = e.LName,
                                         ServiceNo = e.ServiceNo,
                                         ModifiedServiceNo = e.ModifiedServiceNo,
                                         DOB = e.DOB,
                                         DateOfCommissioning = e.DateOfCommissioning,
                                         PermanentAddress = e.PermanentAddress,
                                         IsTrnFwdId = e.IsTrnFwdId,
                                         StepCounter = e.StepCounter,
                                         StepId = e.StepId,
                                         ICardType = e.ICardType,
                                         ApplyFor = e.ApplyFor,
                                         ApplyForId = e.ApplyForId,
                                         RequestId = e.RequestId,
                                         IsFwdStatusId = e.IsFwdStatusId,
                                         Remark = e.Remark,
                                         TrackingId = e.TrackingId,
                                         RankName = e.RankName,
                                         IsPosting = e.IsPosting,
                                         UnitName = e.UnitName,
                                         UnitId = e.UnitId
                                     }).ToList();
                    return await Task.FromResult(allrecord);

                }
            }
            catch(Exception ex)
            {
                _logger.LogError(1001, ex, "BasicDetailDB->GetALLForIcardSttaus");
                return new List<BasicDetailVM>();
            }
        }
        public async Task<List<BasicDetailVM>> GetALLBasicDetail(int UserId,int stepcount, int TypeId, int applyForId)
        {
            string query = "";

            if(stepcount == 0 || stepcount == 1)//////For Fwd Record
            {
                query = " SELECT " +
                        " CASE " +
                        " WHEN LEFT(B.ServiceNo, 2) LIKE '[A-Za-z][A-Za-z]' THEN " +
                        " CONCAT(SUBSTRING(B.ServiceNo, 1, 2), ' ', SUBSTRING(B.ServiceNo, 3, LEN(B.ServiceNo) - 2)) " +
                        " ELSE " +
                        " B.ServiceNo " +
                        " END AS ModifiedServiceNo," +
                        " trnicrd.RegistrationId RegistrationApplyFor,munit.UnitName,B.UnitId,B.BasicDetailId,B.FName,B.LName,B.ServiceNo,B.DOB,B.DateOfCommissioning,C.StepId StepCounter,ISNULL(fwd.TrnFwdId,0) IsTrnFwdId,C.Id StepId,ty.Name ICardType,trnicrd.RequestId,fwd.Remark,ISNULL(fwd.FwdStatusId,0) IsFwdStatusId,Afor.Name ApplyFor,Afor.ApplyForId,trnicrd.TrackingId,ran.RankAbbreviation RankName  FROM BasicDetails B " +
                        " inner join MRank ran on ran.RankId=B.RankId " +
                        " inner join MapUnit mapunit on mapunit.UnitMapId=B.UnitId " +
                        " inner join MUnit munit on munit.UnitId=mapunit.UnitId " +
                        " inner join TrnICardRequest trnicrd on trnicrd.BasicDetailId = B.BasicDetailId " +
                        " inner join MApplyFor Afor on Afor.ApplyForId = B.ApplyForId " +
                        " inner join TrnStepCounter C on trnicrd.RequestId = C.RequestId " +
                        " inner join MICardType ty on ty.TypeId = trnicrd.TypeId " +
                        " inner join TrnDomainMapping map on map.Id= trnicrd.TrnDomainMappingId " +
                        " inner join UserProfile pr on pr.UserId = map.UserId " +
                        " left join TrnFwds fwd on fwd.ToAspNetUsersId= map.AspNetUsersId and fwd.IsComplete=0 and Afor.ApplyForId=IsNULL(@applyForId,Afor.ApplyForId) and fwd.RequestId=trnicrd.RequestId " +
                        " left join MTrnFwdStatus mtrnfwdstatus on mtrnfwdstatus.FwdStatusId = fwd.FwdStatusId " +
                        " WHERE map.AspNetUsersId = @UserId and trnicrd.StatusId=1 ORDER BY B.UpdatedOn DESC";
                
            }
            else if (stepcount == 2 || stepcount == 3 || stepcount == 4 || stepcount == 5 || stepcount == 6)//IO
            {
                if(TypeId==1)///For Icard Submit
                {
                    query = " SELECT distinct " +
                            " CASE " +
                            " WHEN LEFT(B.ServiceNo, 2) LIKE '[A-Za-z][A-Za-z]' THEN " +
                            " CONCAT(SUBSTRING(B.ServiceNo, 1, 2), ' ', SUBSTRING(B.ServiceNo, 3, LEN(B.ServiceNo) - 2)) " +
                            " ELSE " +
                            " B.ServiceNo " +
                            " END AS ModifiedServiceNo," +
                            " trnicrd.RegistrationId RegistrationApplyFor,munit.UnitName,B.UnitId,B.BasicDetailId,B.FName,B.LName,B.ServiceNo,B.DOB,B.DateOfCommissioning,ISNULL(fwd.TrnFwdId,0) IsTrnFwdId,C.StepId StepCounter,C.Id StepId,ty.TypeId,ty.name ICardType,trnicrd.RequestId,ISNULL(fwd.FwdStatusId,0) IsFwdStatusId ,Afor.Name ApplyFor,Afor.ApplyForId,trnicrd.TrackingId,ran.RankAbbreviation RankName" +
                            " FROM BasicDetails B" +
                            " inner join MRank ran on ran.RankId=B.RankId " +
                            " inner join MapUnit mapunit on mapunit.UnitMapId=B.UnitId " +
                            " inner join MUnit munit on munit.UnitId=mapunit.UnitId " +
                            " inner join TrnICardRequest trnicrd on trnicrd.BasicDetailId = B.BasicDetailId" +
                            " inner join MApplyFor Afor on Afor.ApplyForId = B.ApplyForId " +
                            " inner join TrnStepCounter C on trnicrd.RequestId = C.RequestId" +
                            " inner join MICardType ty on ty.TypeId = trnicrd.TypeId" +
                            " inner join TrnFwds fwd on fwd.RequestId = trnicrd.RequestId and fwd.FromAspNetUsersId = @UserId and Afor.ApplyForId=IsNULL(@applyForId,Afor.ApplyForId) and fwd.TypeId=@stepcount and C.StepId = @stepcount and trnicrd.StatusId=1" +
                            " inner join MTrnFwdStatus mtrnfwdstatus on mtrnfwdstatus.FwdStatusId = fwd.FwdStatusId ";
                }
                else if (stepcount == 3 && TypeId == 2 && applyForId ==2) //// For For Action
                {
                    query = " SELECT distinct " +
                            " CASE " +
                            " WHEN LEFT(B.ServiceNo, 2) LIKE '[A-Za-z][A-Za-z]' THEN " +
                            " CONCAT(SUBSTRING(B.ServiceNo, 1, 2), ' ', SUBSTRING(B.ServiceNo, 3, LEN(B.ServiceNo) - 2)) " +
                            " ELSE " +
                            " B.ServiceNo " +
                            " END AS ModifiedServiceNo," +
                            " trnicrd.RegistrationId RegistrationApplyFor,munit.UnitName,B.UnitId,B.BasicDetailId,B.FName,B.LName,B.ServiceNo,B.DOB,B.DateOfCommissioning,ISNULL(fwd.TrnFwdId,0) IsTrnFwdId,C.StepId StepCounter,C.Id StepId,ty.TypeId,ty.name ICardType,trnicrd.RequestId,ISNULL(fwd.FwdStatusId,0) IsFwdStatusId ,Afor.Name ApplyFor,Afor.ApplyForId,trnicrd.TrackingId,ran.RankAbbreviation RankName" +
                            " FROM BasicDetails B" +
                            " inner join MRank ran on ran.RankId=B.RankId" +
                            " inner join MapUnit mapunit on mapunit.UnitMapId=B.UnitId " +
                            " inner join MUnit munit on munit.UnitId=mapunit.UnitId " +
                            " inner join TrnICardRequest trnicrd on trnicrd.BasicDetailId = B.BasicDetailId" +
                            " inner join MApplyFor Afor on Afor.ApplyForId = B.ApplyForId " +
                            " inner join TrnStepCounter C on trnicrd.RequestId = C.RequestId" +
                            " inner join MICardType ty on ty.TypeId = trnicrd.TypeId" +
                            " inner join TrnFwds fwd on fwd.RequestId = trnicrd.RequestId and fwd.ToAspNetUsersId = @UserId and fwd.IsComplete=0 and Afor.ApplyForId=IsNULL(@applyForId,Afor.ApplyForId) and fwd.TypeId=@stepcount and C.StepId = @stepcount and trnicrd.StatusId=1" +
                            " inner join MTrnFwdStatus mtrnfwdstatus on mtrnfwdstatus.FwdStatusId = fwd.FwdStatusId ";
                }
                else if (TypeId == 2) //// For For Action
                {
                    query = " SELECT distinct " +
                            " CASE " +
                            " WHEN LEFT(B.ServiceNo, 2) LIKE '[A-Za-z][A-Za-z]' THEN " +
                            " CONCAT(SUBSTRING(B.ServiceNo, 1, 2), ' ', SUBSTRING(B.ServiceNo, 3, LEN(B.ServiceNo) - 2)) " +
                            " ELSE " +
                            " B.ServiceNo " +
                            " END AS ModifiedServiceNo," +
                            " trnicrd.RegistrationId RegistrationApplyFor,munit.UnitName,B.UnitId,B.BasicDetailId,B.FName,B.LName,B.ServiceNo,B.DOB,B.DateOfCommissioning,ISNULL(fwd.TrnFwdId,0) IsTrnFwdId,C.StepId StepCounter,C.Id StepId,ty.TypeId,ty.name ICardType,trnicrd.RequestId ,ISNULL(fwd.FwdStatusId,0) IsFwdStatusId ,Afor.Name ApplyFor,Afor.ApplyForId,trnicrd.TrackingId,ran.RankAbbreviation RankName,mreg.Abbreviation RegimentalName" +
                            " FROM BasicDetails B" +
                            " inner join MRank ran on ran.RankId=B.RankId" +
                            " inner join MapUnit mapunit on mapunit.UnitMapId=B.UnitId " +
                            " inner join MUnit munit on munit.UnitId=mapunit.UnitId " +
                            " inner join TrnICardRequest trnicrd on trnicrd.BasicDetailId = B.BasicDetailId" +
                            " inner join MApplyFor Afor on Afor.ApplyForId = B.ApplyForId " +
                            " inner join TrnStepCounter C on trnicrd.RequestId = C.RequestId" +
                            " inner join MICardType ty on ty.TypeId = trnicrd.TypeId" +
                            " inner join TrnFwds fwd on fwd.RequestId = trnicrd.RequestId and fwd.ToAspNetUsersId = @UserId and Afor.ApplyForId=IsNULL(@applyForId,Afor.ApplyForId) and fwd.TypeId=@stepcount and fwd.IsComplete = 0 and C.StepId = @stepcount and trnicrd.StatusId=1" +
                            " inner join MTrnFwdStatus mtrnfwdstatus on mtrnfwdstatus.FwdStatusId = fwd.FwdStatusId "+
                            " left join MRegimental mreg on mreg.RegId = B.RegimentalId ";
                }
                else if (TypeId == 3 && stepcount == 3) 
                {
                    query = " SELECT distinct " +
                            " CASE " +
                            " WHEN LEFT(B.ServiceNo, 2) LIKE '[A-Za-z][A-Za-z]' THEN " +
                            " CONCAT(SUBSTRING(B.ServiceNo, 1, 2), ' ', SUBSTRING(B.ServiceNo, 3, LEN(B.ServiceNo) - 2)) " +
                            " ELSE " +
                            " B.ServiceNo " +
                            " END AS ModifiedServiceNo," +
                            " trnicrd.RegistrationId RegistrationApplyFor,munit.UnitName,B.UnitId,B.BasicDetailId,B.FName,B.LName,B.ServiceNo,B.DOB,B.DateOfCommissioning,C.StepId StepCounter,C.Id StepId,ty.TypeId,ty.name ICardType,trnicrd.RequestId ,ISNULL(fwd.FwdStatusId,0) IsFwdStatusId ,Afor.Name ApplyFor,Afor.ApplyForId,ran.RankAbbreviation RankName,mreg.Abbreviation RegimentalName" +
                            " FROM BasicDetails B" +
                            " inner join MRank ran on ran.RankId=B.RankId" +
                            " inner join MapUnit mapunit on mapunit.UnitMapId=B.UnitId " +
                            " inner join MUnit munit on munit.UnitId=mapunit.UnitId " +
                            " inner join TrnICardRequest trnicrd on trnicrd.BasicDetailId = B.BasicDetailId" +
                            " inner join MApplyFor Afor on Afor.ApplyForId = B.ApplyForId " +
                            " inner join TrnStepCounter C on trnicrd.RequestId = C.RequestId" +
                            " inner join MICardType ty on ty.TypeId = trnicrd.TypeId" +
                            " inner join TrnFwds fwd on fwd.RequestId = trnicrd.RequestId and fwd.FromAspNetUsersId = @UserId and Afor.ApplyForId=IsNULL(@applyForId,Afor.ApplyForId) and fwd.FwdStatusId=2 and fwd.TypeId=3 " +  //and fwd.TypeId=2 --and fwd.IsComplete=1
                            " inner join MTrnFwdStatus mtrnfwdstatus on mtrnfwdstatus.FwdStatusId = fwd.FwdStatusId "+
                            " left join MRegimental mreg on mreg.RegId = B.RegimentalId ";
                }
                else if (TypeId == 3 && stepcount == 4)
                {
                    query = " SELECT distinct " +
                            " CASE " +
                            " WHEN LEFT(B.ServiceNo, 2) LIKE '[A-Za-z][A-Za-z]' THEN " +
                            " CONCAT(SUBSTRING(B.ServiceNo, 1, 2), ' ', SUBSTRING(B.ServiceNo, 3, LEN(B.ServiceNo) - 2)) " +
                            " ELSE " +
                            " B.ServiceNo " +
                            " END AS ModifiedServiceNo," +
                            " trnicrd.RegistrationId RegistrationApplyFor,munit.UnitName,B.UnitId,B.BasicDetailId,B.FName,B.LName,B.ServiceNo,B.DOB,B.DateOfCommissioning,C.StepId StepCounter,C.Id StepId,ty.TypeId,ty.name ICardType,trnicrd.RequestId ,ISNULL(fwd.FwdStatusId,0) IsFwdStatusId ,Afor.Name ApplyFor,Afor.ApplyForId,ran.RankAbbreviation RankName" +
                            " FROM BasicDetails B" +
                            " inner join MRank ran on ran.RankId=B.RankId" +
                            " inner join MapUnit mapunit on mapunit.UnitMapId=B.UnitId " +
                            " inner join MUnit munit on munit.UnitId=mapunit.UnitId " +
                            " inner join TrnICardRequest trnicrd on trnicrd.BasicDetailId = B.BasicDetailId" +
                            " inner join MApplyFor Afor on Afor.ApplyForId = B.ApplyForId " +
                            " inner join TrnStepCounter C on trnicrd.RequestId = C.RequestId" +
                            " inner join MICardType ty on ty.TypeId = trnicrd.TypeId" +
                            " inner join TrnFwds fwd on fwd.RequestId = trnicrd.RequestId and fwd.FromAspNetUsersId = @UserId and Afor.ApplyForId=IsNULL(@applyForId,Afor.ApplyForId) and fwd.FwdStatusId=2 and fwd.TypeId=4 " +  //and fwd.TypeId=2 --and fwd.IsComplete=1
                            " inner join MTrnFwdStatus mtrnfwdstatus on mtrnfwdstatus.FwdStatusId = fwd.FwdStatusId ";
                }
                else if(stepcount == 5 || stepcount == 6)///for exported data
                {
                    query = " SELECT distinct " +
                            " CASE " +
                            " WHEN LEFT(B.ServiceNo, 2) LIKE '[A-Za-z][A-Za-z]' THEN " +
                            " CONCAT(SUBSTRING(B.ServiceNo, 1, 2), ' ', SUBSTRING(B.ServiceNo, 3, LEN(B.ServiceNo) - 2)) " +
                            " ELSE " +
                            " B.ServiceNo " +
                            " END AS ModifiedServiceNo," +
                            " trnicrd.RegistrationId RegistrationApplyFor,munit.UnitName,B.UnitId,B.BasicDetailId,B.FName,B.LName,B.ServiceNo,B.DOB,B.DateOfCommissioning,ISNULL(fwd.TrnFwdId,0) IsTrnFwdId,C.StepId StepCounter,C.Id StepId,ty.TypeId,ty.name ICardType,trnicrd.RequestId ,ISNULL(fwd.FwdStatusId,0) IsFwdStatusId ,Afor.Name ApplyFor,Afor.ApplyForId,ran.RankAbbreviation RankName,mreg.Abbreviation RegimentalName" +
                            " FROM BasicDetails B" +
                            " inner join MRank ran on ran.RankId=B.RankId" +
                            " inner join MapUnit mapunit on mapunit.UnitMapId=B.UnitId " +
                            " inner join MUnit munit on munit.UnitId=mapunit.UnitId " +
                            " inner join TrnICardRequest trnicrd on trnicrd.BasicDetailId = B.BasicDetailId" +
                            " inner join MApplyFor Afor on Afor.ApplyForId = B.ApplyForId " +
                            " inner join TrnStepCounter C on trnicrd.RequestId = C.RequestId" +
                            " inner join MICardType ty on ty.TypeId = trnicrd.TypeId" +
                            " inner join TrnFwds fwd on fwd.RequestId = trnicrd.RequestId and fwd.ToAspNetUsersId = @UserId and Afor.ApplyForId=IsNULL(@applyForId,Afor.ApplyForId)  and trnicrd.StatusId=1" +
                            " inner join MTrnFwdStatus mtrnfwdstatus on mtrnfwdstatus.FwdStatusId = fwd.FwdStatusId"+
                            " left join MRegimental mreg on mreg.RegId = B.RegimentalId ";

                }
                else //if (TypeId == 3) //// For For Show
                {
                    TypeId = stepcount - 1;
                    //if (TypeId == 3) TypeId = 2;
                    //if (TypeId == 4) TypeId = 3;
                    //if (TypeId == 5) TypeId = 4;
                    query = " SELECT distinct " +
                            " CASE " +
                            " WHEN LEFT(B.ServiceNo, 2) LIKE '[A-Za-z][A-Za-z]' THEN " +
                            " CONCAT(SUBSTRING(B.ServiceNo, 1, 2), ' ', SUBSTRING(B.ServiceNo, 3, LEN(B.ServiceNo) - 2)) " +
                            " ELSE " +
                            " B.ServiceNo " +
                            " END AS ModifiedServiceNo," +
                            " trnicrd.RegistrationId RegistrationApplyFor,munit.UnitName,B.UnitId,B.BasicDetailId,B.FName,B.LName,B.ServiceNo,B.DOB,B.DateOfCommissioning,C.StepId StepCounter,C.Id StepId,ty.TypeId,ty.name ICardType,trnicrd.RequestId ,ISNULL(fwd.FwdStatusId,0) IsFwdStatusId ,Afor.Name ApplyFor,Afor.ApplyForId,ran.RankAbbreviation RankName" +
                            " FROM BasicDetails B" +
                            " inner join MRank ran on ran.RankId=B.RankId" +
                            " inner join MapUnit mapunit on mapunit.UnitMapId=B.UnitId " +
                            " inner join MUnit munit on munit.UnitId=mapunit.UnitId " +
                            " inner join TrnICardRequest trnicrd on trnicrd.BasicDetailId = B.BasicDetailId" +
                            " inner join MApplyFor Afor on Afor.ApplyForId = B.ApplyForId " +
                            " inner join TrnStepCounter C on trnicrd.RequestId = C.RequestId" +
                            " inner join MICardType ty on ty.TypeId = trnicrd.TypeId" +
                            " inner join TrnFwds fwd on fwd.RequestId = trnicrd.RequestId and fwd.FromAspNetUsersId = @UserId and Afor.ApplyForId=IsNULL(@applyForId,Afor.ApplyForId) and fwd.FwdStatusId=2 " +  //and fwd.TypeId=2 --and fwd.IsComplete=1
                            " inner join MTrnFwdStatus mtrnfwdstatus on mtrnfwdstatus.FwdStatusId = fwd.FwdStatusId ";
                }
            }
            else if (stepcount == 7 || stepcount == 8 || stepcount == 9 || stepcount == 10 )//Reject From IO
            {

                query = " SELECT distinct " +
                        " CASE " +
                        " WHEN LEFT(B.ServiceNo, 2) LIKE '[A-Za-z][A-Za-z]' THEN " +
                        " CONCAT(SUBSTRING(B.ServiceNo, 1, 2), ' ', SUBSTRING(B.ServiceNo, 3, LEN(B.ServiceNo) - 2)) " +
                        " ELSE " +
                        " B.ServiceNo " +
                        " END AS ModifiedServiceNo," +
                        " trnicrd.RegistrationId RegistrationApplyFor,munit.UnitName,B.UnitId,B.BasicDetailId,B.FName,B.LName,B.ServiceNo,B.DOB,B.DateOfCommissioning,fwd.StepId StepCounter,C.Id StepId,ty.TypeId,ty.name ICardType,trnicrd.RequestId,ISNULL(fwd.FwdStatusId,0) IsFwdStatusId ,Afor.Name ApplyFor,Afor.ApplyForId,trnicrd.TrackingId,ran.RankAbbreviation RankName,mreg.Abbreviation RegimentalName" +
                        " FROM BasicDetails B" +
                        " inner join MRank ran on ran.RankId=B.RankId" +
                        " inner join MapUnit mapunit on mapunit.UnitMapId=B.UnitId " +
                        " inner join MUnit munit on munit.UnitId=mapunit.UnitId " +
                        " inner join TrnICardRequest trnicrd on trnicrd.BasicDetailId = B.BasicDetailId" +
                        " inner join MApplyFor Afor on Afor.ApplyForId = B.ApplyForId " +
                        " inner join TrnStepCounter C on trnicrd.RequestId = C.RequestId" +
                        " inner join MICardType ty on ty.TypeId = trnicrd.TypeId" +
                        " inner join TrnFwds fwd on fwd.RequestId = trnicrd.RequestId and fwd.FromAspNetUsersId = @UserId and Afor.ApplyForId=IsNULL(@applyForId,Afor.ApplyForId) and fwd.StepId=@stepcount " +
                        " inner join MTrnFwdStatus mtrnfwdstatus on mtrnfwdstatus.FwdStatusId = fwd.FwdStatusId"+
                        " left join MRegimental mreg on mreg.RegId = B.RegimentalId ";
            }
            else if(stepcount == 11)
            {
                query = " SELECT distinct " +
                        " CASE " +
                        " WHEN LEFT(B.ServiceNo, 2) LIKE '[A-Za-z][A-Za-z]' THEN " +
                        " CONCAT(SUBSTRING(B.ServiceNo, 1, 2), ' ', SUBSTRING(B.ServiceNo, 3, LEN(B.ServiceNo) - 2)) " +
                        " ELSE " +
                        " B.ServiceNo " +
                        " END AS ModifiedServiceNo," +
                        " trnicrd.RegistrationId RegistrationApplyFor,munit.UnitName,B.UnitId,B.BasicDetailId,B.FName,B.LName,B.ServiceNo,B.DOB,B.DateOfCommissioning,fwd.StepId StepCounter,C.Id StepId,ty.TypeId,ty.name ICardType,trnicrd.RequestId,ISNULL(fwd.FwdStatusId,0) IsFwdStatusId ,Afor.Name ApplyFor,Afor.ApplyForId,trnicrd.TrackingId,ran.RankAbbreviation RankName" +
                        " FROM BasicDetails B" +
                        " inner join MRank ran on ran.RankId=B.RankId " +
                        " inner join MapUnit mapunit on mapunit.UnitMapId=B.UnitId " +
                        " inner join MUnit munit on munit.UnitId=mapunit.UnitId " +
                        " inner join TrnICardRequest trnicrd on trnicrd.BasicDetailId = B.BasicDetailId" +
                        " inner join MApplyFor Afor on Afor.ApplyForId = B.ApplyForId " +
                        " inner join TrnStepCounter C on trnicrd.RequestId = C.RequestId" +
                        " inner join MICardType ty on ty.TypeId = trnicrd.TypeId" +
                        " inner join TrnFwds fwd on fwd.RequestId = trnicrd.RequestId and fwd.FromAspNetUsersId = @UserId and Afor.ApplyForId=IsNULL(@applyForId,Afor.ApplyForId) and fwd.FwdStatusId=4 and trnicrd.StatusId=1" +
                        " inner join MTrnFwdStatus mtrnfwdstatus on mtrnfwdstatus.FwdStatusId = fwd.FwdStatusId ";
            }
            try
            {
                using (var connection = _contextDP.CreateConnection())
                {
                    var BasicDetailList = await connection.QueryAsync<BasicDetailVM>(query, new { UserId, stepcount, TypeId, applyForId });
                    int sno = 1;
                    var allrecord = (from e in BasicDetailList
                                     select new BasicDetailVM()
                                     {
                                         BasicDetailId = e.BasicDetailId,
                                         RegistrationApplyFor = e.RegistrationApplyFor,
                                         EncryptedId = protector.Protect(e.BasicDetailId.ToString()),
                                         EncryptedRequestId = protector.Protect(e.RequestId.ToString()),
                                         Sno = sno++,
                                         FName = e.FName,
                                         LName = e.LName,
                                         ServiceNo = e.ServiceNo,
                                         ModifiedServiceNo = e.ModifiedServiceNo,
                                         DOB = e.DOB,
                                         DateOfCommissioning = e.DateOfCommissioning,
                                         PermanentAddress = e.PermanentAddress,
                                         IsTrnFwdId = e.IsTrnFwdId,
                                         StepCounter = e.StepCounter,
                                         StepId = e.StepId,
                                         ICardType = e.ICardType,
                                         ApplyFor = e.ApplyFor,
                                         ApplyForId = e.ApplyForId,
                                         RequestId = e.RequestId,
                                         IsFwdStatusId = e.IsFwdStatusId,
                                         TrackingId = e.TrackingId,
                                         RankName = e.RankName,
                                         UnitId = e.UnitId,
                                         UnitName= e.UnitName,
                                         RegimentalName = e.RegimentalName,

                                     }).ToList();
                    return await Task.FromResult(allrecord);

                }
            }
            catch(Exception ex)
            {
                _logger.LogError(1001, ex, "BasicDetailDB->GetALLBasicDetail");
                return new List<BasicDetailVM>();
            }
        }
        public async Task<BasicDetailCrtAndUpdVM?> GetBasicDetailByRequestId(int RequestId)
        {
            string query = "select bas.NameAsPerRecord,bas.FName,bas.LName,bas.ServiceNo,bas.DOB,bas.DateOfIssue,bas.DateOfCommissioning,bas.PlaceOfIssue," +
                            " issaut.Name IssuingAuthorityName,trnadd.State,trnadd.District,trnadd.PS,trnadd.PO,trnadd.Tehsil,trnadd.Village,trnadd.PinCode," +
                            " trnup.SignatureImagePath,trnup.PhotoImagePath,IdenMark1,IdenMark2,AadhaarNo,Height,bld.BloodGroup,bld.BloodGroupId," +
                            " regi.Abbreviation RegimentalName,Muni.UnitName,uni.UnitMapId UnitId,icardreq.TypeId,icardreq.RegistrationId," +
                            " ran.RankId,ran.RankAbbreviation RankName,arm.Abbreviation ArmedName,trnadd.AddressId,trnup.UploadId,trninfo.InfoId," +
                            " CASE "+
                            " WHEN LEFT(bas.ServiceNo, 2) LIKE '[A-Za-z][A-Za-z]' THEN " +
                            " CONCAT(SUBSTRING(bas.ServiceNo, 1, 2), ' ', SUBSTRING(bas.ServiceNo, 3, LEN(bas.ServiceNo) - 2)) " +
                            " ELSE" +
                            " bas.ServiceNo " +
                            " END AS ModifiedServiceNo " +
                            " from BasicDetails bas" +
                            " inner join MIssuingAuthority issaut on issaut.IssuingAuthorityId=bas.IssuingAuthorityId" +
                            " inner join TrnAddress trnadd on trnadd.BasicDetailId=bas.BasicDetailId" +
                            " inner join TrnUpload trnup on trnup.BasicDetailId=bas.BasicDetailId"+
                            " inner join TrnIdentityInfo trninfo on trninfo.BasicDetailId=bas.BasicDetailId"+
                            " inner join MBloodGroup bld on bld.BloodGroupId=trninfo.BloodGroupId" +
                            " inner join MRank ran on ran.RankId=bas.RankId" +
                            " inner join MArmedType arm on arm.ArmedId=bas.ArmedId"+
                            " inner join MapUnit uni on uni.UnitMapId=bas.UnitId"+
                            " inner join MUnit Muni on Muni.UnitId=uni.UnitId"+
                            " inner join TrnICardRequest icardreq on icardreq.BasicDetailId=bas.BasicDetailId and icardreq.StatusId in (1,2,3)" +
                            " left join MRegimental regi on regi.RegId=bas.RegimentalId" +
                            " where icardreq.RequestId=@RequestId";
            try
            {
                using (var connection = _contextDP.CreateConnection())
                {
                    BasicDetailCrtAndUpdVM? BasicDetailList = (await connection.QueryAsync<BasicDetailCrtAndUpdVM>(query, new { RequestId })).FirstOrDefault();
                    return BasicDetailList;
                }
            }

            catch (Exception ex)
            {
                _logger.LogError(1001, ex, "BasicDetailDB->GetBasicDetailByRequestId");
                return null;
            }
        }
        public async Task<BasicDetailCrtAndUpdVM?> GetBasicDetailById(int BasicDetailId)
        {
            string query = "select bas.*," +
                            " issaut.Name IssuingAuthorityName,trnadd.State,trnadd.District,trnadd.PS,trnadd.PO,trnadd.Tehsil,trnadd.Village,trnadd.PinCode," +
                            " trnup.SignatureImagePath,trnup.PhotoImagePath,IdenMark1,IdenMark2,AadhaarNo,Height,bld.BloodGroup,bld.BloodGroupId," +
                            " regi.Abbreviation RegimentalName,Muni.UnitName,uni.UnitMapId UnitId," +
                            " ran.RankId,ran.RankAbbreviation RankName,arm.Abbreviation ArmedName,trnadd.AddressId,trnup.UploadId,trninfo.InfoId from BasicDetails bas" +
                            " inner join MIssuingAuthority issaut on issaut.IssuingAuthorityId=bas.IssuingAuthorityId" +
                            " inner join TrnAddress trnadd on trnadd.BasicDetailId=bas.BasicDetailId" +
                            " inner join TrnUpload trnup on trnup.BasicDetailId=bas.BasicDetailId" +
                            " inner join TrnIdentityInfo trninfo on trninfo.BasicDetailId=bas.BasicDetailId" +
                            " inner join MBloodGroup bld on bld.BloodGroupId=trninfo.BloodGroupId" +
                            " inner join MRank ran on ran.RankId=bas.RankId" +
                            " inner join MArmedType arm on arm.ArmedId=bas.ArmedId" +
                            " inner join MapUnit uni on uni.UnitMapId=bas.UnitId" +
                            " inner join MUnit Muni on Muni.UnitId=uni.UnitId" +
                            " left join MRegimental regi on regi.RegId=bas.RegimentalId" +
                            " where bas.BasicDetailId=@BasicDetailId";
            try
            {
                using (var connection = _contextDP.CreateConnection())
                {
                    var BasicDetailList = await connection.QueryAsync<BasicDetailCrtAndUpdVM>(query, new { BasicDetailId });

                    return BasicDetailList.FirstOrDefault();
                }
            }

            catch (Exception ex)
            {
                _logger.LogError(1001, ex, "BasicDetailDB->GetBasicDetailById");
                return null;
            }
        }
        public async Task<BasicDetailCrtAndUpdVM?> GetBesicDetailForEditById(int BasicDetailId)
        { 
            string query = "select bas.*," +
                            " issaut.Name IssuingAuthorityName,trnadd.State,trnadd.District,trnadd.PS,trnadd.PO,trnadd.Tehsil,trnadd.Village,trnadd.PinCode," +
                            " trnup.SignatureImagePath,trnup.PhotoImagePath,IdenMark1,IdenMark2,AadhaarNo,Height,bld.BloodGroup,bld.BloodGroupId," +
                            " regi.Abbreviation RegimentalName,Muni.UnitName,uni.UnitMapId UnitId,icardreq.TypeId,icardreq.RegistrationId," +
                            " ran.RankId,ran.RankAbbreviation RankName,arm.Abbreviation ArmedName,trnadd.AddressId,trnup.UploadId,trninfo.InfoId from BasicDetails bas" +
                            " inner join MIssuingAuthority issaut on issaut.IssuingAuthorityId=bas.IssuingAuthorityId" +
                            " inner join TrnAddress trnadd on trnadd.BasicDetailId=bas.BasicDetailId" +
                            " inner join TrnUpload trnup on trnup.BasicDetailId=bas.BasicDetailId" +
                            " inner join TrnIdentityInfo trninfo on trninfo.BasicDetailId=bas.BasicDetailId" +
                            " inner join MBloodGroup bld on bld.BloodGroupId=trninfo.BloodGroupId" +
                            " inner join MRank ran on ran.RankId=bas.RankId" +
                            " inner join MArmedType arm on arm.ArmedId=bas.ArmedId" +
                            " inner join MapUnit uni on uni.UnitMapId=bas.UnitId" +
                            " inner join MUnit Muni on Muni.UnitId=uni.UnitId" +
                            " left join TrnICardRequest icardreq on icardreq.BasicDetailId=bas.BasicDetailId and icardreq.StatusId=1 " +
                            " left join MRegimental regi on regi.RegId=bas.RegimentalId" +
                            " where bas.BasicDetailId=@BasicDetailId";
            try
            {
                using (var connection = _contextDP.CreateConnection())
                {
                    var BasicDetailList = await connection.QueryAsync<BasicDetailCrtAndUpdVM>(query, new { BasicDetailId });

                    return BasicDetailList.FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(1001, ex, "BasicDetailDB->GetBesicDetailForEditById");
                return null;
            }

        }
        public async Task<List<DTODataExportsResponse>> GetBesicdetailsByRequestId(DTODataExportRequest Data, DTOApplFwdConditionRequest dTOApplFwdCondition)
        {
            #region Old Code 
            //var (db, transaction) = _contextDP.CreateConnectionWithTransaction();
            //int[] Ids = Data.Ids;
            //string query = "";
            //try
            //{
            //    string query1 = " update TrnFwds set IsComplete=1 where RequestId in @Ids ";
            //    await db.ExecuteAsync(query1, new { Ids }, transaction: transaction);

            //    string query2 = " update TrnStepCounter set StepId=5 where RequestId in @Ids ";
            //    await db.ExecuteAsync(query2, new { Ids }, transaction: transaction);

            //    string query3 = " update TrnICardRequest set StatusId=2 where  RequestId in @Ids ";
            //    await db.ExecuteAsync(query3, new { Ids }, transaction: transaction);

            //    // Commit the transaction if all operations succeed
            //    transaction.Commit();

            //    if (Data.IsJco == 0)
            //    {
            //        query = " select bas.*,issaut.Name IssuingAuth,mapl.Name ApplyFor, " +
            //                " trnadd.State,trnadd.District,trnadd.PS,trnadd.PO,trnadd.Tehsil,trnadd.Village,trnadd.PinCode," +
            //                " trnup.SignatureImagePath,trnup.PhotoImagePath,IdenMark1,IdenMark2,AadhaarNo,Height,bld.BloodGroup,bld.BloodGroupId," +
            //                " regi.Abbreviation RegimentalName,regi.Location RegimentalLocation,Muni.UnitName,uni.UnitMapId UnitId,icardreq.TypeId,icardreq.RegistrationId," +
            //                " ran.RankId,ran.RankAbbreviation RankName,arm.Abbreviation ArmedName,trnadd.AddressId,trnup.UploadId,trninfo.InfoId,MICardType.Name ICardType,reco.RecordOfficeId,reco.Name RecordOffice,icardreq.RequestId from BasicDetails bas" +
            //                " inner join MIssuingAuthority issaut on issaut.IssuingAuthorityId=bas.IssuingAuthorityId" +
            //                " inner join TrnAddress trnadd on trnadd.BasicDetailId=bas.BasicDetailId" +
            //                " inner join TrnUpload trnup on trnup.BasicDetailId=bas.BasicDetailId" +
            //                " inner join TrnIdentityInfo trninfo on trninfo.BasicDetailId=bas.BasicDetailId" +
            //                " inner join MBloodGroup bld on bld.BloodGroupId=trninfo.BloodGroupId" +
            //                " inner join MRank ran on ran.RankId=bas.RankId" +
            //                " inner join MArmedType arm on arm.ArmedId=bas.ArmedId" +
            //                " inner join MapUnit uni on uni.UnitMapId=bas.UnitId" +
            //                " inner join MUnit Muni on Muni.UnitId=uni.UnitId" +
            //                " inner join TrnICardRequest icardreq on icardreq.BasicDetailId=bas.BasicDetailId " + //and icardreq.Status=0 
            //                " inner join TrnStepCounter scounter on scounter.RequestId=icardreq.RequestId " +
            //                " inner join MApplyFor mapl on mapl.ApplyForId=scounter.ApplyForId " +
            //                " inner join MRecordOffice reco on bas.ArmedId=reco.ArmedId" +
            //                " inner join MICardType MICardType on MICardType.TypeId=icardreq.TypeId " +
            //                " left join MRegimental regi on regi.RegId=bas.RegimentalId" +
            //                " where icardreq.RequestId in @Ids";
            //    }
            //    else
            //    {
            //        query = " select bas.*,issaut.Name IssuingAuth,mapl.Name ApplyFor, trnadd.State,trnadd.District,trnadd.PS,trnadd.PO,trnadd.Tehsil,trnadd.Village,trnadd.PinCode, " +
            //                 " trnup.SignatureImagePath,trnup.PhotoImagePath,IdenMark1,IdenMark2,AadhaarNo,Height,bld.BloodGroup,bld.BloodGroupId, " +
            //                 " regi.Abbreviation RegimentalName,Muni.UnitName,uni.UnitMapId UnitId,icardreq.TypeId,icardreq.RegistrationId, " +
            //                 " ran.RankId,ran.RankAbbreviation RankName,arm.Abbreviation ArmedName,trnadd.AddressId,trnup.UploadId,trninfo.InfoId," +
            //                 " MICardType.Name ICardType," +
            //                 " CASE WHEN ran.orderby<=4 THEN '126' ELSE reco.RecordOfficeId END RecordOfficeId," +
            //                 " CASE WHEN ran.orderby<=4 THEN 'MP 6A' ELSE reco.Name END RecordOffice,icardreq.RequestId" +
            //                 " from BasicDetails bas " +
            //                 " inner join MIssuingAuthority issaut on issaut.IssuingAuthorityId=bas.IssuingAuthorityId" +
            //                 " inner join TrnAddress trnadd on trnadd.BasicDetailId=bas.BasicDetailId " +
            //                 " inner join TrnUpload trnup on trnup.BasicDetailId=bas.BasicDetailId " +
            //                 " inner join TrnIdentityInfo trninfo on trninfo.BasicDetailId=bas.BasicDetailId " +
            //                 " inner join MBloodGroup bld on bld.BloodGroupId=trninfo.BloodGroupId " +
            //                 " inner join MRank ran on ran.RankId=bas.RankId " +
            //                 " inner join MArmedType arm on arm.ArmedId=bas.ArmedId " +
            //                 " inner join MapUnit uni on uni.UnitMapId=bas.UnitId " +
            //                 " inner join MUnit Muni on Muni.UnitId=uni.UnitId " +
            //                 " inner join TrnICardRequest icardreq on icardreq.BasicDetailId=bas.BasicDetailId  " +
            //                 " inner join TrnStepCounter scounter on scounter.RequestId=icardreq.RequestId " +
            //                 " inner join MApplyFor mapl on mapl.ApplyForId=scounter.ApplyForId " +
            //                 " inner join MICardType MICardType on MICardType.TypeId=icardreq.TypeId  " +
            //                 " inner join MRecordOffice reco on reco.ArmedId=56" +
            //                 " inner join OROMapping OROMap on reco.RecordOfficeId=OROMap.RecordOfficeId" +
            //                 " left join MRegimental regi on regi.RegId=bas.RegimentalId where icardreq.RequestId in @Ids" +
            //                 " and bas.ArmedId in (select value from string_split(oromap.ArmedIdList,',')) " +
            //        " order by reco.RecordOfficeId";
            //    }

            //    var BasicDetailList = await db.QueryAsync<DTODataExportsResponse>(query, new { Ids });

            //    return BasicDetailList.ToList();

            //}
            //catch (Exception ex)
            //{
            //    // Rollback the transaction if any operation fails
            //    transaction.Rollback();
            //    _logger.LogError(1001, ex, "BasicDetailDB->GetBesicdetailsByRequestId");
            //    return new List<DTODataExportsResponse>();
            //}
            //finally
            //{
            //    // Dispose of the connection
            //    db.Dispose();
            //}
            #endregion Old Code 

            var (db, transaction) = _contextDP.CreateConnectionWithTransaction();
            int[] Ids = Data.Ids;
            string query = "";
            try
            {
                string query1 = " update TrnFwds set IsComplete=1 where RequestId in @Ids ";
                await db.ExecuteAsync(query1, new { Ids }, transaction: transaction);

                string query2 = " update TrnStepCounter set StepId=5 where RequestId in @Ids ";
                await db.ExecuteAsync(query2, new { Ids }, transaction: transaction);

                //string query3 = " update TrnICardRequest set StatusId=2 where  RequestId in @Ids ";
                //await db.ExecuteAsync(query3, new { Ids }, transaction: transaction);

                // Commit the transaction if all operations succeed
                transaction.Commit();

                if (Data.IsJco == 0) 
                {
                    query = " select bas.*,issaut.Name IssuingAuth,mapl.Name ApplyFor, " +
                            " trnadd.State,trnadd.District,trnadd.PS,trnadd.PO,trnadd.Tehsil,trnadd.Village,trnadd.PinCode," +
                            " trnup.SignatureImagePath,trnup.PhotoImagePath,IdenMark1,IdenMark2,AadhaarNo,Height,bld.BloodGroup,bld.BloodGroupId," +
                            " regi.Abbreviation RegimentalName,regi.Location RegimentalLocation,Muni.UnitName,uni.UnitMapId UnitId,icardreq.TypeId,icardreq.RegistrationId," +
                            " ran.RankId,ran.RankAbbreviation RankName,arm.Abbreviation ArmedName,trnadd.AddressId,trnup.UploadId,trninfo.InfoId,MICardType.Name ICardType,reco.RecordOfficeId,reco.Name RecordOffice,icardreq.RequestId from BasicDetails bas" +
                            " inner join MIssuingAuthority issaut on issaut.IssuingAuthorityId=bas.IssuingAuthorityId" +
                            " inner join TrnAddress trnadd on trnadd.BasicDetailId=bas.BasicDetailId" +
                            " inner join TrnUpload trnup on trnup.BasicDetailId=bas.BasicDetailId" +
                            " inner join TrnIdentityInfo trninfo on trninfo.BasicDetailId=bas.BasicDetailId" +
                            " inner join MBloodGroup bld on bld.BloodGroupId=trninfo.BloodGroupId" +
                            " inner join MRank ran on ran.RankId=bas.RankId" +
                            " inner join MArmedType arm on arm.ArmedId=bas.ArmedId" +
                            " inner join MapUnit uni on uni.UnitMapId=bas.UnitId" +
                            " inner join MUnit Muni on Muni.UnitId=uni.UnitId" +
                            " inner join TrnICardRequest icardreq on icardreq.BasicDetailId=bas.BasicDetailId " + //and icardreq.Status=0 
                            " inner join TrnStepCounter scounter on scounter.RequestId=icardreq.RequestId " +
                            " inner join MApplyFor mapl on mapl.ApplyForId=scounter.ApplyForId " +
                            " inner join MRecordOffice reco on bas.ArmedId=reco.ArmedId" +
                            " inner join MICardType MICardType on MICardType.TypeId=icardreq.TypeId " +
                            " left join MRegimental regi on regi.RegId=bas.RegimentalId" +
                            " where icardreq.RequestId in @Ids";
                }
                else
                {
                    query = @"select bas.*,issaut.Name IssuingAuth,mapl.Name ApplyFor, trnadd.State,trnadd.District,trnadd.PS,trnadd.PO,trnadd.Tehsil,trnadd.Village,trnadd.PinCode,
                                trnup.SignatureImagePath,trnup.PhotoImagePath,IdenMark1,IdenMark2,AadhaarNo,Height,bld.BloodGroup,bld.BloodGroupId,
                                regi.Abbreviation RegimentalName,Muni.UnitName,uni.UnitMapId UnitId,icardreq.TypeId,icardreq.RegistrationId,
                                ran.RankId,ran.RankAbbreviation RankName,arm.Abbreviation ArmedName,trnadd.AddressId,trnup.UploadId,trninfo.InfoId,
                                MICardType.Name ICardType,
                                CASE
	                                WHEN arm.Abbreviation in @MPRSO_ArmedAbbreviation THEN @MPRSO_RecordOfficeId
	                                WHEN UPPER(LEFT(bas.ServiceNo,2)) = @MP6F_ArmyNoPrefix THEN @MP6F_RecordOfficeId
	                                WHEN ran.Orderby<=@MP6A_RankOrderby THEN @MP6A_RecordOfficeId 
	                                ELSE reco.RecordOfficeId 
	                                END AS RecordOfficeId,
                                CASE 
	                                WHEN arm.Abbreviation in @MPRSO_ArmedAbbreviation THEN @MPRSO_Name
	                                WHEN UPPER(LEFT(bas.ServiceNo,2)) = @MP6F_ArmyNoPrefix THEN @MP6F_Name
	                                WHEN ran.Orderby<=@MP6A_RankOrderby THEN @MP6A_Name 
	                                ELSE reco.Name 
	                                END AS RecordOffice
                                ,icardreq.RequestId from BasicDetails bas
                                inner join MIssuingAuthority issaut on issaut.IssuingAuthorityId=bas.IssuingAuthorityId
                                inner join TrnAddress trnadd on trnadd.BasicDetailId=bas.BasicDetailId
                                inner join TrnUpload trnup on trnup.BasicDetailId=bas.BasicDetailId
                                inner join TrnIdentityInfo trninfo on trninfo.BasicDetailId=bas.BasicDetailId
                                inner join MBloodGroup bld on bld.BloodGroupId=trninfo.BloodGroupId
                                inner join MRank ran on ran.RankId=bas.RankId
                                inner join MArmedType arm on arm.ArmedId=bas.ArmedId
                                inner join MapUnit uni on uni.UnitMapId=bas.UnitId
                                inner join MUnit Muni on Muni.UnitId=uni.UnitId
                                inner join TrnICardRequest icardreq on icardreq.BasicDetailId=bas.BasicDetailId
                                inner join TrnStepCounter scounter on scounter.RequestId=icardreq.RequestId
                                inner join MApplyFor mapl on mapl.ApplyForId=scounter.ApplyForId
                                inner join MICardType MICardType on MICardType.TypeId=icardreq.TypeId
                                inner join MRecordOffice reco on reco.ArmedId=56
                                inner join OROMapping OROMap on reco.RecordOfficeId=OROMap.RecordOfficeId
                                left join MRegimental regi on regi.RegId=bas.RegimentalId where icardreq.RequestId in @Ids
                                and bas.ArmedId in (select value from string_split(oromap.ArmedIdList,','))
                                order by RecordOfficeId
                                ";
                }
                var parameters = new DynamicParameters();
                parameters.Add("@Ids", Ids);
                parameters.Add("@MPRSO_RecordOfficeId", dTOApplFwdCondition.MPRSO.RecordOfficeId);
                parameters.Add("@MPRSO_ArmedAbbreviation", dTOApplFwdCondition.MPRSO.ArmedAbbreviation);
                parameters.Add("@MPRSO_Name", dTOApplFwdCondition.MPRSO.Name);

                parameters.Add("@MP6F_RecordOfficeId", dTOApplFwdCondition.MP6F.RecordOfficeId);
                parameters.Add("@MP6F_ArmyNoPrefix", dTOApplFwdCondition.MP6F.ArmyNoPrefix);
                parameters.Add("@MP6F_Name", dTOApplFwdCondition.MP6F.Name);

                parameters.Add("@MP6A_RecordOfficeId", dTOApplFwdCondition.MP6A.RecordOfficeId);
                parameters.Add("@MP6A_RankOrderby", dTOApplFwdCondition.MP6A.RankOrderby);
                parameters.Add("@MP6A_Name", dTOApplFwdCondition.MP6A.Name);

                var BasicDetailList = await db.QueryAsync<DTODataExportsResponse>(query, parameters);

                return BasicDetailList.ToList();

            }
            catch (Exception ex)
            {
                // Rollback the transaction if any operation fails
                transaction.Rollback();
                _logger.LogError(1001, ex, "BasicDetailDB->GetBesicdetailsByRequestId");
                return new List<DTODataExportsResponse>();
            }
            finally
            {
                // Dispose of the connection
                db.Dispose();
            }
        }
        public async Task<DTOXMLDigitalResponse> GetDataDigitalXmlSign(DTODataExportRequest Data)
        {
            DTOXMLDigitalSignResponse dTOXMLDigitalSignResponse = new DTOXMLDigitalSignResponse();
            string query = "select bas.*,issaut.Name IssuingAuth ,trnadd.State,trnadd.District,trnadd.PS,trnadd.PO,trnadd.Tehsil,trnadd.Village,trnadd.PinCode, " +
                           " trnup.SignatureImagePath,trnup.PhotoImagePath,IdenMark1,IdenMark2,AadhaarNo,Height,bld.BloodGroup,bld.BloodGroupId, " +
                           " regi.Abbreviation RegimentalName,Muni.UnitName,uni.UnitMapId UnitId,icardreq.TypeId,icardreq.RegistrationId,"+ 
                           " ran.RankId,ran.RankAbbreviation RankName,arm.Abbreviation ArmedName,trnadd.AddressId,trnup.UploadId,"+
                           " trninfo.InfoId,MICardType.Name ICardType ,GETDATE() XmlCreatedOn," +
                           " App.Name ProApplyFor,reg.Name ProRegistraion,(select Name from MICardType where TypeId=icardreq.TypeId) ProType,users.DomainId ProDomainId,unit.UnitName ProUnitName,unit.Suffix ProSuffix,unit.Sus_no ProSUSNO,pro.Name ProName,ranks.RankAbbreviation ProRankName,pro.ArmyNo ProArmyName"+
                           " from BasicDetails bas "+
                           " inner join MIssuingAuthority issaut on issaut.IssuingAuthorityId=bas.IssuingAuthorityId" +
                           " inner join TrnAddress trnadd on trnadd.BasicDetailId=bas.BasicDetailId " +
                           " inner join TrnUpload trnup on trnup.BasicDetailId=bas.BasicDetailId "+
                           " inner join TrnIdentityInfo trninfo on trninfo.BasicDetailId=bas.BasicDetailId "+
                           " inner join MBloodGroup bld on bld.BloodGroupId=trninfo.BloodGroupId "+
                           " inner join MRank ran on ran.RankId=bas.RankId "+
                           " inner join MArmedType arm on arm.ArmedId=bas.ArmedId "+
                           " inner join MapUnit uni on uni.UnitMapId=bas.UnitId "+
                           " inner join MUnit Muni on Muni.UnitId=uni.UnitId "+
                           " inner join TrnICardRequest icardreq on icardreq.BasicDetailId=bas.BasicDetailId and icardreq.StatusId=1  " +
                           " inner join MICardType MICardType on MICardType.TypeId=icardreq.TypeId  "+
                           " inner join TrnDomainMapping trn on trn.Id=icardreq.TrnDomainMappingId"+
                           " inner join AspNetUsers users on users.Id = trn.AspNetUsersId "+
                           " inner join MapUnit mapuni on mapuni.UnitMapId = trn.UnitId "+
                           " inner join MUnit unit on unit.UnitId = mapuni.UnitId "+
                           " left join UserProfile pro on pro.UserId = trn.UserId "+
                           " inner join MRank ranks on ranks.RankId = pro.RankId"+
                           " inner join MApplyFor App on App.ApplyForId=bas.ApplyForId"+
                           " inner join MRegistration reg on App.ApplyForId=reg.ApplyForId and App.ApplyForId=bas.ApplyForId and reg.RegistrationId= icardreq.RegistrationId"+
                           " left join MRegimental regi on regi.RegId=bas.RegimentalId where icardreq.RequestId in @Ids";
            int[] Ids = Data.Ids;
            using (var connection = _contextDP.CreateConnection())
            {
                var BasicDetailList = await connection.QueryFirstAsync<dynamic>(query, new { Ids });
                if(BasicDetailList!=null)
                {
                    ApplicationDetails applicationDetails = new ApplicationDetails();
                    string FN = BasicDetailList.FName;
                    string LN = BasicDetailList.LName != null ? BasicDetailList.LName : "";

                    applicationDetails.Name = (FN + " " + LN).Trim();
                    applicationDetails.ServiceNo = BasicDetailList.ServiceNo;
                    applicationDetails.DOB = BasicDetailList.DOB;
                    applicationDetails.PlaceOfIssue = BasicDetailList.PlaceOfIssue;
                    applicationDetails.DateOfIssue = BasicDetailList.DateOfIssue;
                    applicationDetails.IssuingAuth = BasicDetailList.IssuingAuth;
                    applicationDetails.DateOfCommissioning = BasicDetailList.DateOfCommissioning;
                    applicationDetails.PaperIcardNo = BasicDetailList.PaperIcardNo;
                    applicationDetails.State = BasicDetailList.State;
                    applicationDetails.District = BasicDetailList.District;
                    applicationDetails.PS = BasicDetailList.PS;
                    applicationDetails.PO = BasicDetailList.PO;
                    applicationDetails.Tehsil = BasicDetailList.Tehsil;
                    applicationDetails.Village = BasicDetailList.Village;
                    applicationDetails.PinCode = BasicDetailList.PinCode;
                    applicationDetails.SignatureImagePath = BasicDetailList.SignatureImagePath;
                    applicationDetails.PhotoImagePath = BasicDetailList.PhotoImagePath;
                    applicationDetails.IdenMark1 = BasicDetailList.IdenMark1;
                    applicationDetails.IdenMark2 = BasicDetailList.IdenMark2;
                    applicationDetails.AadhaarNo = Convert.ToString(BasicDetailList.AadhaarNo);
                    applicationDetails.Height = Convert.ToString(BasicDetailList.Height);
                    applicationDetails.BloodGroup = BasicDetailList.BloodGroup;
                    applicationDetails.RegimentalName = BasicDetailList.RegimentalName;
                    applicationDetails.UnitName = BasicDetailList.UnitName;
                    applicationDetails.RankName = BasicDetailList.RankName;
                    applicationDetails.ArmedName = BasicDetailList.ArmedName;

                    applicationDetails.ICardType = BasicDetailList.ICardType;
                    applicationDetails.XmlCreatedOn = BasicDetailList.XmlCreatedOn;

                    Profiledtls profiledtls = new Profiledtls();
                    profiledtls.ProApplyFor = BasicDetailList.ProApplyFor;
                    profiledtls.ProRegistraion = BasicDetailList.ProRegistraion;
                    profiledtls.ProType = BasicDetailList.ProType;
                    profiledtls.ProDomainId = BasicDetailList.ProDomainId;
                    profiledtls.ProUnitName = BasicDetailList.ProUnitName;
                    profiledtls.ProSuffix = BasicDetailList.ProSuffix;
                    profiledtls.ProSUSNO = BasicDetailList.ProSUSNO;
                    profiledtls.ProName = BasicDetailList.ProName;
                    profiledtls.ProRankName = BasicDetailList.ProRankName;
                    profiledtls.ProArmyName = BasicDetailList.ProArmyName;

                    dTOXMLDigitalSignResponse.applicationDetails = applicationDetails;
                    dTOXMLDigitalSignResponse.profiledtls = profiledtls;
                }
                
                DTOFwdLastRecForDigitalSign dTOFwdLastRecForDigitalSign = new DTOFwdLastRecForDigitalSign();
                dTOFwdLastRecForDigitalSign = await ICardFwdLastRec(Ids[0]);
                dTOFwdLastRecForDigitalSign.StepId = Data.StepId;
                dTOXMLDigitalSignResponse.RecForDigitalSign = dTOFwdLastRecForDigitalSign;

                DTOXMLDigitalResponse dTOXMLDigitalResponse = new DTOXMLDigitalResponse();
                dTOXMLDigitalResponse.Header = dTOXMLDigitalSignResponse;
                return dTOXMLDigitalResponse;
            }
        }
        public async Task<string?> GetCSVString(DTOCSVExportRequest Data)
        {
            string query = string.Empty;
            if (Data.IdsTypeRequestIdOrTrnFwdId == true)
            {
                //Ids is TrnFwdId.
               query = " Select B.ServiceNo,B.NameAsPerRecord,B.DOB,B.DateOfCommissioning,ran.RankAbbreviation,B.FName,B.LName,munit.UnitName,trnicrd.TrackingId,Afor.Name ApplyFor,ty.name ICardType,trnadd.State,trnadd.District,trnadd.PS,trnadd.PO,trnadd.Tehsil,trnadd.Village,trnadd.PinCode from BasicDetails B " +
                       " inner join TrnAddress trnadd on trnadd.BasicDetailId = B.BasicDetailId " +
                       " inner join MApplyFor Afor on Afor.ApplyForId = B.ApplyForId " +
                       " inner join MRank ran on ran.RankId=B.RankId " +
                       " inner join MapUnit mapunit on mapunit.UnitMapId=B.UnitId " +
                       " inner join MUnit munit on munit.UnitId=mapunit.UnitId " +
                       " inner join TrnICardRequest trnicrd on trnicrd.BasicDetailId = B.BasicDetailId " +
                       " inner join MICardType ty on ty.TypeId = trnicrd.TypeId " +
                       " inner join TrnFwds fwd on fwd.RequestId = trnicrd.RequestId " +
                       " where fwd.TrnFwdId in @Ids";
            }
            else
            {
                //Ids is RequestId.
                query = " Select B.ServiceNo,B.NameAsPerRecord,B.DOB,B.DateOfCommissioning,ran.RankAbbreviation,B.FName,B.LName,munit.UnitName,trnicrd.TrackingId,Afor.Name ApplyFor,ty.name ICardType,trnadd.State,trnadd.District,trnadd.PS,trnadd.PO,trnadd.Tehsil,trnadd.Village,trnadd.PinCode from BasicDetails B " +
                        " inner join TrnAddress trnadd on trnadd.BasicDetailId = B.BasicDetailId " +
                        " inner join MApplyFor Afor on Afor.ApplyForId = B.ApplyForId " +
                        " inner join MRank ran on ran.RankId=B.RankId " +
                        " inner join MapUnit mapunit on mapunit.UnitMapId=B.UnitId " +
                        " inner join MUnit munit on munit.UnitId=mapunit.UnitId " +
                        " inner join TrnICardRequest trnicrd on trnicrd.BasicDetailId = B.BasicDetailId " +
                        " inner join MICardType ty on ty.TypeId = trnicrd.TypeId " +
                        " where trnicrd.RequestId in @Ids";
            }

            int[] Ids = Data.Ids;
            try
            {
                using (var connection = _contextDP.CreateConnection())
                {
                    var BasicDetailList = await connection.QueryAsync<DTOCSVExportResponseForSqlQuery>(query, new { Ids });
                    int sno = 1;
                    var allrecord = (from e in BasicDetailList
                                     select new DTOCSVExportResponse()
                                     {
                                         Sno = sno++,
                                         ServiceNo = e.ServiceNo,
                                         NameAsPerRecord = e.NameAsPerRecord,
                                         DOB = DateOnly.FromDateTime(e.DOB),
                                         DateOfCommissioning = DateOnly.FromDateTime(e.DateOfCommissioning),
                                         RankAbbreviation = e.RankAbbreviation,
                                         FName = e.FName,
                                         LName = e.LName,
                                         UnitName = e.UnitName,
                                         TrackingId = e.TrackingId,
                                         ApplyFor = e.ApplyFor,
                                         ICardType = e.ICardType,
                                         State=e.State,
                                         District=e.District,
                                         PS=e.PS,
                                         PO=e.PO,
                                         Tehsil=e.Tehsil,
                                         Village=e.Village,
                                         PinCode = e.PinCode,
                                         PermanentAddress = "Village - " + (e.Village ?? "")+ ", Post Office - " + (e.PO ?? "") + ", Tehsil - "+ (e.Tehsil ?? "") + ", District - "+ (e.District ?? "") + ", State - " + (e.State ?? "") + ", Pin Code - " + e.PinCode,
                                     }).ToImmutableList();
                    CsvService csvService = new CsvService();
                    string csvData = csvService.GenerateCsv(allrecord);

                    return csvData;

                }
            }
            catch (Exception ex)
            {
                _logger.LogError(1001, ex, "BasicDetailDB->GetCSVString");
                return null;
            }
        }

        public async Task<ICardHistoryResponseAll> ICardHistory(int RequestId)
        { 
            #region Old Code
            //string query = @"select usersfrom.UserName FromDomain,profrom.Name FromProfile,ranlfrom.RankAbbreviation FromRank,
            //                usersto.UserName ToDomain,proto.Name ToProfile,ranlto.RankAbbreviation ToRank ,
            //                CASE fwd.FwdStatusId WHEN 1 THEN 'Pending' WHEN 2 THEN 'Approved' WHEN 3 THEN 'Reject' WHEN 4 THEN 'Internal Forward' END Status,
            //                fwd.UpdatedOn,isnull(fwd.Remark,'Nill') Remark,
            //                fwd.IsComplete,(select STRING_AGG(Remarks,'#') from MRemarks where RemarksId in (select value from string_split(fwd.RemarksIds,','))) Remarks2,
            //                reason.Reason,postind.Authority,initres.UnitName
            //                from TrnFwds fwd
            //                inner join TrnStepCounter step on fwd.RequestId=step.RequestId
            //                inner join TrnDomainMapping mapfrom on mapfrom.AspNetUsersId=fwd.FromAspNetUsersId
            //                inner join AspNetUsers usersfrom on usersfrom.Id=mapfrom.AspNetUsersId
            //                inner join TrnDomainMapping mapto on mapto.AspNetUsersId=fwd.ToAspNetUsersId
            //                inner join AspNetUsers usersto on usersto.Id=mapto.AspNetUsersId
            //                left join UserProfile profrom on mapfrom.UserId=profrom.UserId
            //                inner join MRank ranlfrom on ranlfrom.RankId=profrom.RankId
            //                left join UserProfile proto on mapto.UserId=proto.UserId
            //                left join TrnPostingOut postind on postind.Id=fwd.PostingOutId
            //                left join MPostingReason reason on reason.Id=postind.ReasonId
            //                left join MapUnit Munitres on Munitres.UnitMapId=postind.ToUnitID
            //                left join MUnit initres on initres.UnitId=Munitres.UnitId
            //                inner join MRank ranlto on ranlto.RankId=proto.RankId where fwd.RequestId=@RequestId
            //                order by fwd.TrnFwdId asc";
            //try
            //{
            //    using (var connection = _contextDP.CreateConnection())
            //    {
            //        var BasicDetailList = await connection.QueryAsync<ICardHistoryResponse>(query, new { RequestId });

            //        return BasicDetailList.ToList();
            //    }
            //}
            //catch (Exception ex)
            //{
            //    _logger.LogError(1001, ex, "BasicDetailDB->ICardHistory");
            //    return null;
            //}
            #endregion
            string query = @"select fwd.TrnFwdId,usersfrom.UserName FromDomain,profrom.Name FromProfile,ranlfrom.RankAbbreviation FromRank,
                            usersto.UserName ToDomain,proto.Name ToProfile,ranlto.RankAbbreviation ToRank ,
                            CASE fwd.FwdStatusId WHEN 1 THEN 'Pending' WHEN 2 THEN 'Approved' WHEN 3 THEN 'Reject' WHEN 4 THEN 'Internal Forward' END Status,
                            fwd.UpdatedOn,isnull(fwd.Remark,'Nill') Remark,
                            fwd.IsComplete,(select STRING_AGG(Remarks,'#') from MRemarks where RemarksId in (select value from string_split(fwd.RemarksIds,','))) Remarks2
                            from TrnFwds fwd
                            inner join TrnStepCounter step on fwd.RequestId=step.RequestId
                            inner join AspNetUsers usersfrom on usersfrom.Id=fwd.FromAspNetUsersId
                            inner join AspNetUsers usersto on usersto.Id=fwd.ToAspNetUsersId
                            inner join UserProfile profrom on fwd.FromUserId=profrom.UserId
                            inner join MRank ranlfrom on ranlfrom.RankId=profrom.RankId
                            inner join UserProfile proto on fwd.ToUserId=proto.UserId
                            inner join MRank ranlto on ranlto.RankId=proto.RankId
                            where fwd.RequestId=@RequestId
                            order by fwd.TrnFwdId asc

	                        select reason.Reason,postind.Authority,initres.UnitName,initresfrom.UnitName FromUnit,ISNULL(postind.TrnFwdId,0) TrnFwdId from  
                            TrnPostingOut postind 
                            left join MPostingReason reason on reason.Id=postind.ReasonId
                            left join MapUnit Munitres on Munitres.UnitMapId=postind.ToUnitID
                            left join MUnit initres on initres.UnitId=Munitres.UnitId
                              left join MapUnit Munitresfrom on Munitresfrom.UnitMapId=postind.FromUnitID
                              left join MUnit initresfrom on initresfrom.UnitId=Munitresfrom.UnitId
							where postind.RequestId=@RequestId

                            select mcat.Name FaultyStage,mcat.CategoryId,ISNULL(faulty.TrnFwdId,0) 
                            TrnFwdId,(select STRING_AGG(Remarks,'#') from MRemarks where RemarksId in (select value from string_split(faulty.RemarksIds,','))) RemarksNameList
							from TrnFaultyCard faulty
							inner join MCategory mcat on mcat.CategoryId = faulty.CategoryId where faulty.RequestId=@RequestId


                            select trnclose.Authority,trnclose.Remarks,res.Reasons from TrnApplClose trnclose
                            inner join MReasons res on trnclose.ReasonId=res.ReasonId where trnclose.RequestId=@RequestId

";
            try
            {
                ICardHistoryResponseAll cardHistoryResponseAll=new ICardHistoryResponseAll();   
                using (var connection = _contextDP.CreateConnection())
                {
                    using (var multi = await connection.QueryMultipleAsync(query, new { RequestId }))
                    {
                        // var ICardHistory = await multi.ReadFirstOrDefaultAsync<ICardHistoryResponse>();
                        var ICardHistory = (await multi.ReadAsync<ICardHistoryResponse>()).ToList();
                        var PostingOut = (await multi.ReadAsync<ICardHistoryPostingOutResponse>()).ToList();
                        var FaultyCard = (await multi.ReadAsync<ICardHistoryFaultyCardResponse>()).ToList();
                        var CloseCard = await multi.ReadFirstOrDefaultAsync<ICardApplCloseCardResponse>();

                        cardHistoryResponseAll.ICardHistory = ICardHistory;
                        cardHistoryResponseAll.PostingOut = PostingOut;
                        cardHistoryResponseAll.FaultyCard = FaultyCard;
                        cardHistoryResponseAll.CloseCard = CloseCard;
                       
                    }

                   // var BasicDetailList = await connection.QueryAsync<ICardHistoryResponseAll>(query, new { RequestId });

                    // return BasicDetailList.ToList();
                    return cardHistoryResponseAll;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(1001, ex, "BasicDetailDB->ICardHistory");
                return null;
            }

        }
        public async Task<DTOFwdLastRecForDigitalSign> ICardFwdLastRec(int RequestId)
        {
            string query = " if exists (select StepId from TrnStepCounter where RequestId=@RequestId and StepId=2)" +
                           " begin" +
                           " select profrom.ArmyNo FromArmyNo,usersfrom.DomainId FromDomain,profrom.Name FromProfile,ranlfrom.RankAbbreviation FromRank," +
                           " Getdate() FromDate,trnste.StepId from BasicDetails basi" +
                           " inner join TrnDomainMapping mapfrom on mapfrom.AspNetUsersId=basi.Updatedby " +
                           " inner join AspNetUsers usersfrom on usersfrom.Id=mapfrom.AspNetUsersId " +
                           " left join UserProfile profrom on profrom.UserId=mapfrom.UserId " +
                           " inner join MRank ranlfrom on ranlfrom.RankId=profrom.RankId " +
                           " inner join TrnICardRequest req on  req.BasicDetailId=basi.BasicDetailId and req.StatusId=1" +
                           " inner join TrnStepCounter trnste on trnste.RequestId=req.RequestId" +
                           " where trnste.RequestId=@RequestId" +
                           " end" +
                           " else" +
                           " begin" +
                           " select top 1 profrom.ArmyNo FromArmyNo,usersfrom.DomainId FromDomain,profrom.Name FromProfile, " +
                           " ranlfrom.RankAbbreviation FromRank,Getdate() FromDate,step.StepId from TrnFwds fwd  " +
                           " inner join TrnStepCounter step on fwd.RequestId=step.RequestId " +
                           " inner join TrnDomainMapping mapfrom on mapfrom.AspNetUsersId=fwd.FromAspNetUsersId " +
                           " inner join AspNetUsers usersfrom on usersfrom.Id=mapfrom.AspNetUsersId " +
                           " left join UserProfile profrom on mapfrom.UserId=profrom.UserId " +
                           " inner join MRank ranlfrom on ranlfrom.RankId=profrom.RankId " +
                           " where fwd.RequestId=@RequestId order by fwd.TrnFwdId desc" +
                           " end";
            try
            {
                using (var connection = _contextDP.CreateConnection())
                {
                    var BasicDetailList = await connection.QueryAsync<DTOFwdLastRecForDigitalSign>(query, new { RequestId });

                    return BasicDetailList.FirstOrDefault()?? new DTOFwdLastRecForDigitalSign();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(1001, ex, "BasicDetailDB->ICardHistory");
                return new DTOFwdLastRecForDigitalSign();
            }

        }
        public async Task<List<ICardHistoryResponse>?> ICardHistoryByTrackingId(string TrackingId)
        {
            string query =  " select usersfrom.UserName FromDomain,profrom.Name FromProfile,ranlfrom.RankAbbreviation FromRank, " +
                            " usersto.UserName ToDomain,proto.Name ToProfile,ranlto.RankAbbreviation ToRank ," +
                            " CASE fwd.FwdStatusId WHEN 1 THEN 'Pending' WHEN 2 THEN 'Approved' WHEN 3 THEN 'Reject' WHEN 4 THEN 'Internal Forward' END Status," +
                            " fwd.UpdatedOn,isnull(fwd.Remark,'Nill') Remark, " +
                            " fwd.IsComplete,(select STRING_AGG(Remarks,'#') from MRemarks where RemarksId in (select value from string_split(fwd.RemarksIds,','))) Remarks2, " +
                            " reason.Reason,postind.Authority,initres.UnitName,req.RequestId " +
                            " from TrnFwds fwd " +
                            " inner join TrnICardRequest req on req.RequestId=fwd.RequestId " +
                            " inner join TrnStepCounter step" +
                            " on fwd.RequestId=step.RequestId" +
                            " inner join TrnDomainMapping mapfrom on mapfrom.AspNetUsersId=fwd.FromAspNetUsersId" +
                            " inner join AspNetUsers usersfrom on usersfrom.Id=mapfrom.AspNetUsersId" +
                            " inner join TrnDomainMapping mapto on mapto.AspNetUsersId=fwd.ToAspNetUsersId" +
                            " inner join AspNetUsers usersto on usersto.Id=mapto.AspNetUsersId" +
                            " left join UserProfile profrom" +
                            " on mapfrom.UserId=profrom.UserId" +
                            " inner join MRank ranlfrom on ranlfrom.RankId=profrom.RankId" +
                            " left join UserProfile proto" +
                            " on mapto.UserId=proto.UserId" +
                            " left join TrnPostingOut postind on postind.Id=fwd.PostingOutId" +
                            " left join MPostingReason reason on reason.Id=postind.ReasonId" +
                            " left join MapUnit Munitres on Munitres.UnitMapId=postind.ToUnitID" +
                            " left join MUnit initres on initres.UnitId=Munitres.UnitId" +
                            " inner join MRank ranlto on ranlto.RankId=proto.RankId where req.TrackingId=@TrackingId" +
                            " order by fwd.TrnFwdId asc";
            try
            {
                using (var connection = _contextDP.CreateConnection())
                {
                    var BasicDetailList = await connection.QueryAsync<ICardHistoryResponse>(query, new { TrackingId });

                    return BasicDetailList.ToList();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(1001, ex, "BasicDetailDB->ICardHistory");
                return null;
            }

        }
        public async Task<DTOICardTaskCountResponse?> GetTaskCountICardRequest(int UserId,int Type, int applyForId)
        {
            string query = "";
            if (Type==1) // Submitted
            {
                query = "declare @ToDrafted int=0 declare @ToSubmitted int=0 declare @ToCompleted int=0 declare @ToRejected int=0" +
                        " select @ToDrafted=COUNT(distinct req.RequestId) from TrnDomainMapping domain" +
                        " inner join TrnICardRequest req on req.TrnDomainMappingId=domain.Id " +
                        " inner join TrnStepCounter trnstepcout on trnstepcout.RequestId= req.RequestId where domain.AspNetUsersId=@UserId and trnstepcout.StepId=1 and trnstepcout.ApplyForId=@applyForId " +

                        " select @ToSubmitted=COUNT(distinct req.RequestId) from TrnDomainMapping domain" +
                        " inner join TrnICardRequest req on req.TrnDomainMappingId=domain.Id " +
                        " inner join TrnStepCounter trnstepcout on trnstepcout.RequestId= req.RequestId where domain.AspNetUsersId=@UserId and trnstepcout.StepId>1 and trnstepcout.ApplyForId=@applyForId " +

                        " select @ToCompleted=COUNT(distinct req.RequestId) from TrnDomainMapping domain" +
                        " inner join TrnICardRequest req on req.TrnDomainMappingId=domain.Id " +
                        " inner join TrnStepCounter trnstepcout on trnstepcout.RequestId= req.RequestId where domain.AspNetUsersId=@UserId and req.StatusId=2 and trnstepcout.ApplyForId=@applyForId " +

                        " select @ToRejected=COUNT(distinct fwd.RequestId) from TrnDomainMapping domain" +
                        " inner join TrnICardRequest req on req.TrnDomainMappingId=domain.Id " +
                        " inner join TrnStepCounter trnstepcout on trnstepcout.RequestId= req.RequestId" +
                        " inner join TrnFwds fwd on fwd.RequestId= trnstepcout.RequestId where fwd.ToAspNetUsersId=@UserId and req.StatusId=1 and trnstepcout.StepId in(7,8,9,10) and trnstepcout.ApplyForId=@applyForId " +

                        " select @ToDrafted ToDrafted,@ToSubmitted ToSubmitted,@ToCompleted ToCompleted,@ToRejected ToRejected";
            }
            else if (Type == 2) // Pending
            {
                query = " declare @_2ndLevelPending int declare @_2ndLevelApproved int declare @_2ndLevelReject int" +
                        " declare @_3rdLevelPending int declare @_3rdLevelApproved int declare @_3rdLevelReject int" +
                        " declare @_4thLevelPending int declare @_4thLevelApproved int declare @_4thLevelReject int" +
                        " declare @ExportPending int declare @ExportApproved int declare @ExportReject int declare @ToInternalForward int declare @CsvUploadCount int" +


                        " select @_2ndLevelPending=COUNT(distinct fwd.RequestId) from TrnFwds fwd " +
                        " inner join TrnStepCounter cou on fwd.RequestId=cou.RequestId and cou.ApplyForId=@applyForId " +
                        " inner join TrnICardRequest trncard  on trncard.RequestId=cou.RequestId " +
                        " where ToAspNetUsersId=@UserId and IsComplete=0 and fwd.TypeId=2 and  trncard.StatusId=1" +

                        " select @_2ndLevelApproved=COUNT(distinct fwd.RequestId) from TrnFwds fwd " +
                        " inner join TrnStepCounter cou on fwd.RequestId=cou.RequestId and cou.ApplyForId=@applyForId " +
                        " where FromAspNetUsersId=@UserId and fwd.FwdStatusId=2 and TypeId=3" +

                        " select @_2ndLevelReject=COUNT(distinct fwd.RequestId) from TrnFwds fwd " +
                        " inner join TrnStepCounter cou on fwd.RequestId=cou.RequestId and cou.ApplyForId=@applyForId " +
                        " where FromAspNetUsersId=@UserId and fwd.StepId=7 and fwd.TypeId=1" +

                        " select @_3rdLevelPending=COUNT(distinct fwd.RequestId) from TrnFwds fwd " +
                        " inner join TrnStepCounter cou on fwd.RequestId=cou.RequestId and cou.ApplyForId=@applyForId " +
                        " inner join TrnICardRequest trncard  on trncard.RequestId=cou.RequestId " +
                        " where ToAspNetUsersId=@UserId and IsComplete=0 and fwd.TypeId=3 and  trncard.StatusId=1" +

                        " select @_3rdLevelApproved=COUNT(distinct fwd.RequestId) from TrnFwds fwd " +
                        " inner join TrnStepCounter cou on fwd.RequestId=cou.RequestId and cou.ApplyForId=@applyForId " +
                        " where FromAspNetUsersId=@UserId and fwd.FwdStatusId=2 and fwd.TypeId=4" +

                        " select @_3rdLevelReject=COUNT(distinct fwd.RequestId)  from TrnFwds fwd " +
                        " inner join TrnStepCounter cou on fwd.RequestId=cou.RequestId and cou.ApplyForId=@applyForId " +
                        " where FromAspNetUsersId=@UserId and fwd.StepId=8 and fwd.TypeId=1" +

                        " select @_4thLevelPending=COUNT(distinct fwd.RequestId)  from TrnFwds fwd " +
                        " inner join TrnStepCounter cou on fwd.RequestId=cou.RequestId and cou.ApplyForId=@applyForId " +
                        " inner join TrnICardRequest trncard  on trncard.RequestId=cou.RequestId " +
                        " where ToAspNetUsersId=@UserId and IsComplete=0 and cou.StepId=4 and  trncard.StatusId=1" +

                        " select @_4thLevelApproved=COUNT(distinct fwd.RequestId)  from TrnFwds fwd " +
                        " inner join TrnStepCounter cou on fwd.RequestId=cou.RequestId and cou.ApplyForId=@applyForId " +
                        " inner join TrnICardRequest trncard  on trncard.RequestId=cou.RequestId " +
                        " where ToAspNetUsersId=@UserId and  trncard.StatusId=1" +

                        " select @_4thLevelReject=COUNT(distinct fwd.RequestId) from TrnFwds fwd " +
                        " inner join TrnStepCounter cou on fwd.RequestId=cou.RequestId and cou.ApplyForId=@applyForId " +
                        " where FromAspNetUsersId=@UserId and fwd.StepId=9 and fwd.TypeId=1" +

                        " select @ExportPending=COUNT(distinct fwd.RequestId) from TrnFwds fwd " +
                        " inner join TrnStepCounter cou on fwd.RequestId=cou.RequestId and cou.ApplyForId=@applyForId " +
                        " inner join TrnICardRequest trncard  on trncard.RequestId=cou.RequestId " +
                        " where ToAspNetUsersId=@UserId and IsComplete=0 and trncard.StatusId=1" +

                        " select @ExportApproved=COUNT(distinct fwd.RequestId) from TrnFwds fwd " +
                        " inner join TrnStepCounter cou on fwd.RequestId=cou.RequestId and cou.ApplyForId=@applyForId " +
                        " inner join TrnICardRequest trncard  on trncard.RequestId=cou.RequestId " +
                        " where ToAspNetUsersId=@UserId and  trncard.StatusId=1" +

                        " select @ExportReject=COUNT(distinct fwd.RequestId)  from TrnFwds fwd " +
                        " inner join TrnStepCounter cou on fwd.RequestId=cou.RequestId and cou.ApplyForId=@applyForId " +
                        " where FromAspNetUsersId=@UserId and fwd.StepId=10 and fwd.TypeId=1" +

                        " select @ToInternalForward=COUNT(distinct fwd.RequestId)  from TrnFwds fwd " +
                        " inner join TrnStepCounter cou on fwd.RequestId=cou.RequestId and cou.ApplyForId=@applyForId " +
                        " inner join TrnICardRequest trncard  on trncard.RequestId=cou.RequestId " +
                        " where FromAspNetUsersId=@UserId and FwdStatusId=4 and trncard.StatusId=1" +

                        " select @CsvUploadCount=COUNT(Id) from CSVImports" +

                        " select @_2ndLevelPending _2ndLevelPending,@_2ndLevelApproved _2ndLevelApproved,@_2ndLevelReject _2ndLevelReject, " +
                        " @_3rdLevelPending _3rdLevelPending,@_3rdLevelApproved _3rdLevelApproved,@_3rdLevelReject _3rdLevelReject, " +
                        " @_4thLevelPending _4thLevelPending,@_4thLevelApproved _4thLevelApproved,@_4thLevelReject _4thLevelReject, " +
                        " @ExportPending ExportPending,@ExportApproved ExportApproved,@ExportReject ExportReject,@ToInternalForward ToInternalForward,@CsvUploadCount CsvUploadCount";

            } 
          
            using (var connection = _contextDP.CreateConnection())
            {
                try
                {
                    var ret = await connection.QueryAsync<DTOICardTaskCountResponse>(query, new { UserId, applyForId });
                    return ret.FirstOrDefault();
                }
                catch(Exception ex)
                {
                    _logger.LogError(1001, ex, "BasicDetailDB->GetTaskCountICardRequest");
                    return null;
                }

            }
        }
        public async Task<List<DTONotificationResponse>?> GetNotification(int UserId, int Type, int applyForId)
        {
            string query = "select dis.DisplayId,Spanname,Message,ranks.RankAbbreviation,bas.Name,bas.ServiceNo,tre.TrackingId,uplod.PhotoImagePath,dis.Url  from TrnNotification noti" +
                            " inner join TrnNotificationDisplay dis on noti.DisplayId=dis.DisplayId"+
                            " inner join AspNetUsers users on users.Id=noti.SentAspNetUsersId"+
                            " inner join TrnStepCounter stepc on stepc.RequestId=noti.RequestId "+
                            " inner join TrnICardRequest tre on tre.RequestId = noti.RequestId " +
                             " inner join BasicDetails bas on bas.BasicDetailId=tre.BasicDetailId" +
                            " inner join MRank ranks on ranks.RankId=bas.RankId" +
                            " inner join TrnUpload uplod on uplod.BasicDetailId=bas.BasicDetailId" +
                            " where noti.ReciverAspNetUsersId=@UserId and NotificationTypeId=@Type and stepc.applyforId=@applyForId and [Read]=0 and ReciverAspNetUsersId!=SentAspNetUsersId";
            try 
            {
                using (var connection = _contextDP.CreateConnection())
                {
                    var ret = await connection.QueryAsync<DTONotificationResponse>(query, new { UserId, Type, applyForId });
                    return ret.ToList();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(1001, ex, "BasicDetailDB->GetNotification");
                return null;
            }
        }
        public async Task<List<DTONotificationResponse>?> GetNotificationRequestId(int UserId, int Type, int applyForId)
        {
            string query = "select Distinct tre.RequestId, dis.DisplayId,Spanname + 'self' Spanname,Message,ranks.RankAbbreviation,bas.Name,bas.ServiceNo,tre.TrackingId,uplod.PhotoImagePath,CASE WHEN dis.DisplayId in (7,8,9,10,17,18,19,20) THEN dis.Url ELSE '' END AS Url  from TrnNotification noti " +
                            " inner join TrnNotificationDisplay dis on noti.DisplayId = dis.DisplayId" +
                            " inner join AspNetUsers users on users.Id = noti.SentAspNetUsersId" +
                            " inner join TrnICardRequest tre on tre.RequestId = noti.RequestId" +
                            " inner join TrnDomainMapping dmap on dmap.Id = tre.TrnDomainMappingId" +
                            " inner join TrnStepCounter cou on cou.RequestId=tre.RequestId" +
                            " inner join BasicDetails bas on bas.BasicDetailId=tre.BasicDetailId" +
                            " inner join MRank ranks on ranks.RankId=bas.RankId"+
                             " inner join TrnUpload uplod on uplod.BasicDetailId=bas.BasicDetailId" +
                            " where NotificationTypeId = @Type and dmap.AspNetUsersId = @UserId and [Read]=0 and cou.applyforId=@applyForId and ReciverAspNetUsersId=SentAspNetUsersId";
            try
            {
                using (var connection = _contextDP.CreateConnection())
                {
                    var ret = await connection.QueryAsync<DTONotificationResponse>(query, new { UserId, Type, applyForId });
                    return ret.ToList();
                }
            }
            catch(Exception ex) 
            {
                _logger.LogError(1001, ex, "BasicDetailDB->GetNotificationRequestId");
                return null;
            }

        }
        public async Task<List<MRecordOffice>?> GetROListByArmedId(byte ArmedId)
        {
            try
            {
                return await _context.MRecordOffice.Where(x => x.ArmedId == ArmedId).ToListAsync();
            }
            catch(Exception ex) 
            {
                _logger.LogError(1001, ex, "BasicDetailDB->GetROListByArmedId");
                return null;
            }
        }
        public async Task<DTOApplicationTrack?> ApplicationHistory(string TrackingId)
        {
            DTOApplicationTrack lst=new DTOApplicationTrack();
            try
            {
                string query = " select ran.RankAbbreviation RankName,bas.Name,bas.ServiceNo ArmyNo,unit.UnitName,uplod.PhotoImagePath," +
               " ranfrom.RankAbbreviation FromRank,pr.Name FromName,pr.ArmyNo FromArmyNo,users.DomainId" +
               " from BasicDetails bas " +
               " inner join TrnICardRequest req on bas.BasicDetailId=req.BasicDetailId" +
               " inner join TrnUpload uplod on bas.BasicDetailId=uplod.BasicDetailId" +
               " inner join MRank ran on bas.RankId=ran.RankId" +
               " inner join MapUnit muni on bas.UnitId=muni.UnitMapId" +
               " inner join MUnit unit on  muni.UnitId=unit.UnitId" +
               " inner join TrnDomainMapping map on map.Id= req.TrnDomainMappingId" +
               " inner join AspNetUsers users on map.AspNetUsersId=users.Id" +
               " inner join UserProfile pr on pr.UserId = map.UserId" +
               " inner join MRank ranfrom on pr.RankId=ranfrom.RankId" +
               " where req.StatusId=1 and req.TrackingId=@TrackingId";

                //" select fwd.FwdStatusId,fwd.stepId,fwd.UpdatedOn,step.Name,fwd.IsComplete" +
                //" from TrnFwds fwd " +
                //" inner join TrnICardRequest req on fwd.RequestId=req.RequestId" +
                //" inner join MStepCounterStep step on fwd.StepId=step.StepId" +
                //"  where fwd.RequestId=@RequestId" +
                //" order by fwd.TrnFwdId asc";
                using (var connection = _contextDP.CreateConnection())
                {
                    var ret = await connection.QueryAsync<DTOApplicationDetails>(query, new { TrackingId });
                    lst.dTOApplicationDetails = ret.FirstOrDefault() ?? new DTOApplicationDetails();
                }
                query = " select fwd.FwdStatusId,fwd.stepId,fwd.UpdatedOn,step.Name,fwd.IsComplete," +
                        " isnull(fwd.Remark,'') Remark," +
                        " (select STRING_AGG(Remarks,'#') from MRemarks where RemarksId in (select value from string_split(fwd.RemarksIds,','))) Remark2" +
                        " from TrnFwds fwd " +
                        " inner join TrnICardRequest req on fwd.RequestId=req.RequestId" +
                        " inner join MStepCounterStep step on fwd.StepId=step.StepId" +
                        " where req.StatusId=1 and req.TrackingId=@TrackingId" +
                        " order by fwd.TrnFwdId asc";
                using (var connection = _contextDP.CreateConnection())
                {
                    var ret1 = await connection.QueryAsync<DTOTrackHistory>(query, new { TrackingId });
                    lst.dTOTrackHistory = ret1.ToList();
                }
                return lst;
            }
            catch (Exception ex) 
            {
                _logger.LogError(1001, ex, "BasicDetailDB->ApplicationHistory");
                return null;
            }
        }

        public async Task<List<DTOCardPriningRequest>> CardPrintingCSVCheck(List<DTOCardPriningRequest> requests)
        {
            byte StepId = 5;
            var response = new List<DTOCardPriningRequest>();
            foreach (var batchRecords in requests.Chunk(5000))
            {
                using (var connection = _contextDP.CreateConnection())
                {
                   var resultInChunks = (from record in batchRecords
                                join dbrecord in _context.TrnICardRequest on record.RequestId equals dbrecord.RequestId.ToString() into dbRecordJoin
                                from matchRecord in dbRecordJoin.DefaultIfEmpty()
                                join cardNoMatch in _context.TrnICardRequest on record.CardSerialNo equals cardNoMatch.CardSerialNo into cardNoJoin
                                from cardNoExists in cardNoJoin.DefaultIfEmpty()
                                join chipNoMatch in _context.TrnICardRequest on record.ChipNo equals chipNoMatch.ChipNo into chipNoJoin
                                from chipNoExists in chipNoJoin.DefaultIfEmpty()
                                join stepStatus in _context.TrnStepCounter on new { RequestId = (matchRecord == null ? 0 : matchRecord.RequestId), StepId } equals new { stepStatus.RequestId, stepStatus.StepId } into stepStatusJoin
                                from stepStatus in stepStatusJoin.DefaultIfEmpty()
                                join armyNoCheck in _context.BasicDetails on new { BasicDetailId = (matchRecord == null ? 0 : matchRecord.BasicDetailId), ServiceNo = record.ServiceNo } equals new { armyNoCheck.BasicDetailId, armyNoCheck.ServiceNo } into basicDetailJoin
                                from armyNoCheck in basicDetailJoin.DefaultIfEmpty()
                                select new DTOCardPriningRequest
                                {
                                    RequestId = record.RequestId,
                                    ServiceNo = record.ServiceNo,
                                    ChipNo = record.ChipNo,
                                    CardSerialNo = record.CardSerialNo,
                                    IsValid = matchRecord != null && cardNoExists == null && chipNoExists == null && stepStatus != null && armyNoCheck != null,
                                    Status = matchRecord != null && cardNoExists == null && chipNoExists == null && stepStatus != null ? "Valid" : "DbInvalid",
                                    Remarks = (matchRecord == null ? "RequestId not exists; " : "") +
                                                  (cardNoExists != null ? "CardSerialNo already exists; " : "") +
                                                  (chipNoExists != null ? "ChipNo already exists; " : "") +
                                                  (matchRecord != null && stepStatus == null ? "Card application is not available for printing; " : "") +
                                                  (matchRecord != null && armyNoCheck == null ? "Service no. is invalid for this card application; " : "")
                                }
                       ).ToList();

                    response.AddRange(resultInChunks);
                }
            }

            return response;
        }

        public async Task<DTOUploadChipAndSerialResponse> CardPrintingCSVUpload(List<DTOCardPriningRequest> requests)
        {
            DTOUploadChipAndSerialResponse response = new DTOUploadChipAndSerialResponse();
            try
            {
               foreach (var batchRecords in requests.Chunk(5000))
               {
                   using (var connection = _contextDP.CreateConnection())
                   {
                           DataTable cardDistribution = DataTableHelper.ToDataTable(batchRecords, "Remarks", "IsValid","Status");
                           var parameters = new DynamicParameters();
                           parameters.Add("@data", cardDistribution.AsTableValuedParameter("UT_CardPriningCSV"));

                           response = (await connection.QueryAsync<DTOUploadChipAndSerialResponse>("CardPriningCSVImport",
                                                                                                   parameters,
                                                                                                   commandType: CommandType.StoredProcedure
                                      )).FirstOrDefault();
                   }
               }
            }
            catch(Exception ee)
            {
                response.Message = ee.Message;
            }
            return response;
        }

    }
}
