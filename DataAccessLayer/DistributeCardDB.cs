using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using DataAccessLayer.BaseInterfaces;
using DataAccessLayer.Logger;
using DataTransferObject.Domain.Model;
using DataTransferObject.Requests;
using DataTransferObject.Response;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.Internal;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using static System.Runtime.InteropServices.JavaScript.JSType;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace DataAccessLayer
{
    public class DistributeCardDB : GenericRepositoryDL<TrnDistributeCard>, IDistributeCardDB
    {
        protected new readonly ApplicationDbContext _context;
        private readonly DapperContext _contextDP;
        private readonly IDataProtector protector;
        private readonly ILogger<DistributeCardDB> _logger;

        public DistributeCardDB(ApplicationDbContext context, DapperContext contextDP, IDataProtectionProvider dataProtectionProvider, ILogger<DistributeCardDB> logger, DataProtectionPurposeStrings dataProtectionPurposeStrings) : base(context)
        {
            _context = context;
            _contextDP = contextDP;
            _logger = logger;
            // Pass the purpose string as a parameter
            this.protector = dataProtectionProvider.CreateProtector(
                dataProtectionPurposeStrings.AFSACIdRouteValue);
        }

        public async Task<bool> FindAnyRequestId(int RequestId)
        {
            try
            {
                return _context.TrnDistributeCards
                                .Any(f => f.RequestId == RequestId);
            }
            catch (Exception ex)
            {
                _logger.LogError(1001, ex, "DistributeCardDB->FindAnyRequestId");
                return false;
            }
        }

        public async Task<DTODataTablesResponse<DTODistributeCardGetResponse>> GetAllDistribute(DTODataTablesRequest dTO)
        {
            List<DTODistributeCardGetResponse> dTODistributeCardGetResponses = new List<DTODistributeCardGetResponse>();
            var responseData = new DTODataTablesResponse<DTODistributeCardGetResponse>
            {
                draw = 0,
                recordsTotal = 0,
                recordsFiltered = 0,
                data = dTODistributeCardGetResponses
            };
            try
            {
                // Map allowed sort columns to DB fields
                var allowedSortColumns = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
                {
                    ["ModifiedServiceNo"] = "bas.ServiceNo",
                    ["UpdatedOn"] = "tdc.UpdatedOn",
                    ["RequestId"] = "req.RequestId",
                    ["Remark"] = "tdc.Remark"
                };

                var sortColumn = allowedSortColumns.ContainsKey(dTO.sortColumn ?? "")
                    ? allowedSortColumns[dTO.sortColumn!]
                    : "tdc.UpdatedOn";

                var sortOrder = dTO.sortDirection;

                string query = "";
                    query = @"appl.Name ApplyFor,
                                req.RequestId,tdc.DistributeCardId,
                                bas.ServiceNo,ranks.RankAbbreviation RankName,
                                bas.FName,bas.LName,
                                Muni.UnitName,Muni.Abbreviation UnitAbbreviation,
                                tdc.UpdatedOn,tdc.Remark,tdc.IsActive,
                                bas.NameAsPerRecord,
                                regi.Abbreviation RegimentalName,
                                CASE
                                WHEN LEFT(bas.ServiceNo, 2) LIKE '[A-Za-z][A-Za-z]' THEN
                                CONCAT(SUBSTRING(bas.ServiceNo, 1, 2), ' ', SUBSTRING(bas.ServiceNo, 3, LEN(bas.ServiceNo) - 2))
                                ELSE
                                bas.ServiceNo
                                END AS ModifiedServiceNo,tdc.DistributedOn
                                from TrnDistributeCards tdc
                                inner join TrnICardRequest req on req.RequestId = tdc.RequestId
                                inner join TrnDomainMapping tdm on tdm.Id=req.TrnDomainMappingId
                                inner join BasicDetails bas on bas.BasicDetailId=req.BasicDetailId
                                inner join MRank ranks on ranks.RankId=bas.RankId
                                inner join MapUnit uni on uni.UnitMapId=bas.UnitId
                                inner join MUnit Muni on Muni.UnitId=uni.UnitId
                                inner join MApplyFor appl on appl.ApplyForId=bas.ApplyForId
                                left join MRegimental regi on regi.RegId=bas.RegimentalId
                                Where bas.ServiceNo like '%' + @SearchTerm + '%' ";

                var multiQuery = query = $@"
                            WITH RecordCTE AS (
                                select ROW_NUMBER() OVER (ORDER BY {sortColumn} {sortOrder}) AS RowNum, {query}
                            )
                            SELECT * FROM RecordCTE
                            WHERE RowNum BETWEEN @Offset AND @Limit;

                            Select Count(*) from TrnDistributeCards;
                        ";

                using (var connection = _contextDP.CreateConnection())
                {
                    var ret = await connection.QueryMultipleAsync(query, new { Offset = dTO.Start, Limit = dTO.Length, SearchTerm = string.IsNullOrWhiteSpace(dTO.searchValue) ? "" : dTO.searchValue });
                    var records = (await ret.ReadAsync<DTODistributeCardGetResponse>()).ToList();
                    var totalRecords = (await ret.ReadAsync<int>()).Single();
                    responseData = new DTODataTablesResponse<DTODistributeCardGetResponse>
                    {
                        draw = dTO.Draw,
                        recordsTotal = totalRecords, // Total records without filtering
                        recordsFiltered = records.Count(), // Total records after filtering
                        data = (from e in records
                                select new DTODistributeCardGetResponse()
                                {
                                    EncryptedId = protector.Protect(e.DistributeCardId.ToString()),
                                    NameAsPerRecord = e.NameAsPerRecord,
                                    FName = e.FName,
                                    LName = e.LName,
                                    ServiceNo = e.ServiceNo,
                                    ModifiedServiceNo = e.ModifiedServiceNo,
                                    UnitName = e.UnitName,
                                    UnitAbbreviation = e.UnitAbbreviation,
                                    RankName = e.RankName,
                                    ArmedName = e.ArmedName,
                                    RequestId = e.RequestId,
                                    UpdatedOn = e.UpdatedOn,
                                    ApplyFor = e.ApplyFor,
                                    DistributeCardId = e.DistributeCardId,
                                    Remark = e.Remark,
                                    IsActive = e.IsActive,
                                    DistributedOn = e.DistributedOn
                                }).ToList()
                    };
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(1001, ex, "DistributeCardDB->GetAllDistribute");
            }
            return responseData;
        }

        public async Task<List<DTODistributeCardExportResponse>> GetDetailsByRequestIds(DTOHotlistCardsExportRequest Data)
        {
            var records = new List<DTODistributeCardExportResponse>();
            try
            {
                string query = @"select req.RequestId,tdc.DistributeCardId,bas.ServiceNo as ArmyNo,
	                                ranks.RankAbbreviation,bas.FName,bas.LName,Muni.Abbreviation Unit,
	                                tdc.UpdatedOn as DateAndTime,tdc.Remark,tdc.IsActive as IsActiveBool,
	                                req.CardSerialNo,req.ChipNo,tdc.DistributedOn
	                                from TrnDistributeCards tdc
	                                inner join TrnICardRequest req on req.RequestId = tdc.RequestId
	                                inner join BasicDetails bas on bas.BasicDetailId=req.BasicDetailId
	                                inner join MRank ranks on ranks.RankId=bas.RankId
	                                inner join MapUnit uni on uni.UnitMapId=bas.UnitId
	                                inner join MUnit Muni on Muni.UnitId=uni.UnitId
                                  Where req.RequestId in @Ids";
                var parameters = new DynamicParameters();
                parameters.Add("@Ids", Data.Ids);
                using (var connection = _contextDP.CreateConnection())
                {
                    var ret = await connection.QueryAsync<DTODistributeCardExportResponse>(query, parameters);
                    records = ret.ToList();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(1001, ex, "DistributeCardDB->GetDetailsByRequestIds");
            }
            return records;
        }

        public async Task<DTOCommonSaveResponse> SaveDistributeCard(TrnDistributeCard model, ICardHistoryResponseAll cardRequestHistory)
        {
            var (db, transaction) = _contextDP.CreateConnectionWithTransaction();
            DTOCommonSaveResponse dtoResponse = new DTOCommonSaveResponse();
            try
            {
                var cardRequestHistoryJson = JsonConvert.SerializeObject(cardRequestHistory);
                var insertQuery = @$"Insert into TrnDistributeCards(RequestId,DistributedOn,Remark,UpdatedbyUserId,IsActive,Updatedby,UpdatedOn) 
                                                             Values(@RequestId,@DistributedOn,@Remark,@UpdatedbyUserId,@IsActive,@Updatedby,@UpdatedOn);

                                     DECLARE @DistributeCardId INT = SCOPE_IDENTITY();
                                     
                                     update TrnICardRequest set StatusId = 3,UpdatedOn = @UpdatedOn,Updatedby = @Updatedby where RequestId = @RequestId;
                                     
                                     update TrnStepCounter set StepId = 18,UpdatedOn = @UpdatedOn,Updatedby = @Updatedby where RequestId = @RequestId;
                                     {(cardRequestHistory?.FaultyCard?.Count > 0 ? "update TrnFaultyCard set TrnFwdId = null where RequestId = @RequestId;" : "")}
                                     {(cardRequestHistory?.PostingOut?.Count > 0 ? "update TrnPostingOut set TrnFwdId = null where RequestId = @RequestId;" : "")}
                                     
                                     Delete from TrnFwds where RequestId = @RequestId;
                                     Insert into CompletedICardRequests(RequestId,CardRequestHistoryJson,UpdatedbyUserId,IsActive,Updatedby,UpdatedOn)
                                     values(@RequestId,@CardRequestHistoryJson,@UpdatedbyUserId,@IsActive,@Updatedby,@UpdatedOn);
                                     
                                     Select @DistributeCardId;
                                    ";
                var parameters = new DynamicParameters();
                parameters.Add("@RequestId", model.RequestId, DbType.Int32, ParameterDirection.Input);
                parameters.Add("@DistributedOn", model.DistributedOn, DbType.DateTime, ParameterDirection.Input);
                parameters.Add("@Remark", model.Remark, DbType.String, ParameterDirection.Input);
                parameters.Add("@UpdatedbyUserId", model.UpdatedbyUserId, DbType.String, ParameterDirection.Input);
                parameters.Add("@IsActive", model.IsActive, DbType.Boolean, ParameterDirection.Input);
                parameters.Add("@Updatedby", model.Updatedby, DbType.Int32, ParameterDirection.Input);
                parameters.Add("@UpdatedOn", model.UpdatedOn, DbType.DateTime, ParameterDirection.Input);
                parameters.Add("@CardRequestHistoryJson", cardRequestHistoryJson, DbType.String, ParameterDirection.Input);

                model.DistributeCardId = await db.ExecuteScalarAsync<int>(insertQuery, parameters, transaction: transaction);
                transaction.Commit();
                dtoResponse.Result = true;
                dtoResponse.Message = "Record Created!";
                dtoResponse.Id = model.DistributeCardId.ToString();
                dtoResponse.CurrentTime = model.UpdatedOn.GetValueOrDefault();
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                _logger.LogError(1001, ex, "DistributeCardDB->SaveDistributeCard");
                dtoResponse.Result = false;
                dtoResponse.Message = "Internal Server Error!";
            }

            return dtoResponse;
        }
    }
}
