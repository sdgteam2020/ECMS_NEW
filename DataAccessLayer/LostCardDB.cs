using Dapper;
using DataAccessLayer.BaseInterfaces;
using DataAccessLayer.Logger;
using DataTransferObject.Constants;
using DataTransferObject.Domain.Model;
using DataTransferObject.Requests;
using DataTransferObject.Response;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Data;
using System.Reflection.Metadata;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace DataAccessLayer
{
    /// <summary>
    /// Repository class for handling Lost Card related operations.
    /// Inherits from <see cref="GenericRepositoryDL{TrnLostCard}"/> to perform CRUD operations on Lost Cards.
    /// </summary>
    public class LostCardDB : GenericRepositoryDL<TrnLostCard> , ILostCardDB
    {
        protected new readonly ApplicationDbContext _context;
        private readonly DapperContext _contextDP;
        private readonly IDataProtector protector;
        private readonly ILogger<LostCardDB> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="LostCardDB"/> class.
        /// </summary>
        /// <param name="context">The <see cref="ApplicationDbContext"/> to interact with the database.</param>
        /// <param name="contextDP">The <see cref="DapperContext"/> for Dapper-based queries.</param>
        /// <param name="dataProtectionProvider">The <see cref="IDataProtectionProvider"/> to create data protectors.</param>
        /// <param name="logger">The <see cref="ILogger{LostCardDB}"/> for logging.</param>
        /// <param name="dataProtectionPurposeStrings">The purpose strings for data protection.</param>
        public LostCardDB(ApplicationDbContext context, DapperContext contextDP, IDataProtectionProvider dataProtectionProvider, ILogger<LostCardDB> logger, DataProtectionPurposeStrings dataProtectionPurposeStrings) : base(context)
        {
            _context = context;
            _contextDP = contextDP;
            _logger = logger;
            // Pass the purpose string as a parameter
            this.protector = dataProtectionProvider.CreateProtector(
                dataProtectionPurposeStrings.AFSACIdRouteValue);
        }

        /// <summary>
        /// Checks if a Lost Card exists for a specific RequestId.
        /// </summary>
        /// <param name="RequestId">The RequestId to check.</param>
        /// <returns>Returns <c>true</c> if a Lost Card with the given RequestId exists, otherwise <c>false</c>.</returns>
        public async Task<bool> FindAnyRequestId(int RequestId)
        {
            try
            {
                return await _context.TrnLostCards
                                .AnyAsync(f => f.RequestId == RequestId);
            }
            catch (Exception ex)
            {
                _logger.LogError(1001, ex, "LostCardDB->FindAnyRequestId");
                return false;
            }
        }

        
        /// <summary>
        /// Checks if a specific ServiceNo is already associated with a lost card request.
        /// </summary>
        /// <param name="ServiceNo">The ServiceNo to check.</param>
        /// <returns>Returns <c>true</c> if the ServiceNo exists in the Lost Card records, otherwise <c>false</c>.</returns>
        public async Task<bool> CheckServiceNoRequestInLost(string ServiceNo)
        {
            try
            {
                const string query = @"
                                        IF EXISTS
                                        (
                                            SELECT 1
                                            FROM TrnLostCards lc
                                            INNER JOIN TrnICardRequest tir ON tir.RequestId = lc.RequestId
                                            LEFT JOIN BasicDetails bd ON bd.BasicDetailId = tir.BasicDetailId
                                            LEFT JOIN AFSAC2.dbo.BasicDetails bd2 ON bd2.BasicDetailId = tir.BasicDetailId
                                            WHERE tir.BasicDetailId =
                                            (
                                                SELECT MAX(BasicDetailId)
                                                FROM
                                                (
                                                    SELECT BasicDetailId
                                                    FROM BasicDetails
                                                    WHERE ServiceNo = @ServiceNo

                                                    UNION ALL

                                                    SELECT BasicDetailId
                                                    FROM AFSAC2.dbo.BasicDetails
                                                    WHERE ServiceNo = @ServiceNo
                                                ) x
                                            )
                                        )
                                            SELECT 1;
                                        ELSE
                                            SELECT 0;";
                using (var connection = _contextDP.CreateConnection())
                {
                    int result = await connection.QuerySingleAsync<int>(query, new { ServiceNo });
                    return result == 1;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(1001, ex, "LostCardDB->CheckServiceNoRequestInLost");
                return false;
            }
        }


        /// <summary>
        /// Retrieves all Lost Card records based on the given DataTables request.
        /// Supports filtering, sorting, and pagination.
        /// </summary>
        /// <param name="dTO">The data transfer object containing the search, sorting, and pagination parameters.</param>
        /// <returns>A <see cref="DTODataTablesResponse{DTOLostCardGetResponse}"/> containing the paginated Lost Card records.</returns>
        public async Task<DTODataTablesWithSelectedIdsResponse<DTOLostCardGetResponse>> GetAllLost(DTODataTablesRequestForCommanCheckAll dTO)
        {
            List<DTOLostCardGetResponse> dTOLostCardGetResponses = new List<DTOLostCardGetResponse>();
            var responseData = new DTODataTablesWithSelectedIdsResponse<DTOLostCardGetResponse>
            {
                draw = dTO.Draw,
                recordsTotal = 0,
                recordsFiltered = 0,
                selectedIds = null,
                data = dTOLostCardGetResponses
            };
            try
            {
                // Map allowed sort columns to DB fields
                var allowedSortColumns = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
                {
                    ["ServiceNo"] = "ServiceNo",
                    ["UpdatedOn"] = "lost.UpdatedOn",
                    ["LostOn"] = "lost.LostOn",
                    ["Remark"] = "lost.Remark"
                };

                var sortColumn = allowedSortColumns.ContainsKey(dTO.sortColumn ?? "") ? allowedSortColumns[dTO.sortColumn!] : "lost.UpdatedOn";
                
                var sortOrder = dTO.sortDirection == "desc" ? "DESC" : "ASC";


                string selectFields = @"appl.Name ApplyFor,
                                        req.RequestId,lost.LostCardId,
                                        basic_2.ServiceNo,ranks.RankAbbreviation RankName,basic_2.FName,basic_2.LName,
                                        Muni.Abbreviation UnitAbbreviation,lost.UpdatedOn,lost.Remark,lost.LostOn,lost.IsFIRLogged,lost.SupportDocName,
                                        (select STRING_AGG(Remarks,'#') from MRemarks where RemarksId in (select value from string_split(lost.RemarksIds,','))) RemarksNameList";
                string fromJoinClause = @"from TrnLostCards lost
                                        inner join TrnICardRequest req on req.RequestId = lost.RequestId
                                        inner join TrnDomainMapping tdm on tdm.Id=req.TrnDomainMappingId
                                        inner join AFSAC2.dbo.BasicDetails basic_2 on basic_2.BasicDetailId = req.BasicDetailId
                                        inner join MRank ranks on ranks.RankId = basic_2.RankId
                                        inner join MapUnit uni on uni.UnitMapId = basic_2.UnitId
                                        inner join MUnit Muni on Muni.UnitId = uni.UnitId
                                        inner join MApplyFor appl on appl.ApplyForId = basic_2.ApplyForId";
                string whereClause = @"Where @SearchTerm IS NULL OR basic_2.ServiceNo LIKE @SearchTerm";

                var multiQuery = $@"
                        WITH RecordCTE AS (
                            select  Count(*) OVER () as TotalFilteredRecords,ROW_NUMBER() OVER (ORDER BY {sortColumn} {sortOrder}) AS RowNum, {selectFields} {fromJoinClause} {whereClause}
                        )
                        SELECT * FROM RecordCTE WHERE RowNum BETWEEN @Offset AND @Limit;";
                string queryRequestIds = $@"SELECT req.RequestId {fromJoinClause} {whereClause}";

                using (var connection = _contextDP.CreateConnection())
                {
                    // Parameters for SQL query
                    var searchTerm = string.IsNullOrEmpty(dTO.searchValue) ? null : $"%{dTO.searchValue.Trim()}%";

                    var parameters = new DynamicParameters();
                    parameters.Add("@Offset", dTO.Start + 1, DbType.Int32, ParameterDirection.Input);
                    parameters.Add("@Limit", (dTO.Start + dTO.Length), DbType.Int32, ParameterDirection.Input);
                    parameters.Add("@SearchTerm", searchTerm, DbType.String, ParameterDirection.Input);

                    var ret = await connection.QueryMultipleAsync(multiQuery, parameters);
                    var records = (await ret.ReadAsync<DTOLostCardGetResponse>()).ToList();
                    var totalFilteredRecords = records?.FirstOrDefault()?.TotalFilteredRecords;

                    List<int>? selectedIds = new List<int>();

                    if (dTO.AllChecked == true && (string.IsNullOrEmpty(dTO.searchValue) || dTO.SearchTextChanged == true))
                    {
                        var result = await connection.QueryMultipleAsync(queryRequestIds, parameters);
                        selectedIds = (await result.ReadAsync<int>()).ToList();
                    }
                    else
                    {
                        selectedIds = null;
                    }

                    responseData = new DTODataTablesWithSelectedIdsResponse<DTOLostCardGetResponse>
                    {
                        draw = dTO.Draw,
                        recordsTotal = totalFilteredRecords.GetValueOrDefault(),
                        recordsFiltered = totalFilteredRecords.GetValueOrDefault(),
                        selectedIds = selectedIds,
                        data = records,
                    };
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(1001, ex, "LostCardDB->GetAllLost");
            }
            return responseData;
        }


        /// <summary>
        /// Retrieves lost card details based on a list of Request IDs.
        /// </summary>
        /// <param name="Data">DTO containing the list of Request IDs for which lost card details need to be fetched.</param>
        /// <returns>A list of <see cref="DTOLostCardExportResponse"/> containing the lost card details.</returns>
        /// <exception cref="Exception">Throws an exception if an error occurs during the query execution.</exception>
        public async Task<List<DTOLostCardExportResponse>> GetDetailsByRequestIds(DTOHotlistCardsExportRequest Data)
        {
            var records = new List<DTOLostCardExportResponse>();
            try
            {
                string query = @"select req.RequestId,lost.LostCardId,basic_2.ServiceNo AS ArmyNo,
	                                ranks.RankAbbreviation,basic_2.FName,basic_2.LName,Muni.Abbreviation Unit,
	                                lost.UpdatedOn as DateAndTime,lost.Remark,lost.IsActive as IsActiveBool,
	                                req.CardSerialNo,req.ChipNo,lost.LostOn
	                                from TrnLostCards lost
	                                inner join TrnICardRequest req on req.RequestId = lost.RequestId
                                    inner join AFSAC2.dbo.BasicDetails basic_2 on basic_2.BasicDetailId = req.BasicDetailId
	                                inner join MRank ranks on ranks.RankId = basic_2.RankId
	                                inner join MapUnit uni on uni.UnitMapId = basic_2.UnitId
	                                inner join MUnit Muni on Muni.UnitId = uni.UnitId
                                  Where req.RequestId in @Ids";

                // Parameters for SQL query, adding the list of Request IDs
                var parameters = new DynamicParameters();
                parameters.Add("@Ids", Data.Ids);

                // Open the connection, execute the query asynchronously, and map results to the DTO
                using (var connection = _contextDP.CreateConnection())
                {
                    // Execute query asynchronously and convert result to list of DTOLostCardExportResponse
                    var ret = await connection.QueryAsync<DTOLostCardExportResponse>(query, parameters);

                    // Convert the results to a list
                    records = ret.ToList();

                }
            }
            catch (Exception ex)
            {
                _logger.LogError(1001, ex, "LostCardDB->GetBesicdetailsByRequestId");
            }
            // Return the list of records (could be empty if no matching records found)
            return records;
        }
        public async Task<DTOCheckBeforeLostReportResponse> CheckBeforeLostReport(int requestId, int TDMId)
        {
            DTOCheckBeforeLostReportResponse response = new DTOCheckBeforeLostReportResponse();
            try
            {
                string query = @"SELECT currentReq.StatusId,currentReq.BasicDetailId,apt.AppointmentName,hot.HotlistCardId,
                                       CASE
                                            WHEN lost.RequestId = @RequestId THEN 0
                                            WHEN dest.RequestId = @RequestId THEN 0
                                            WHEN currentReq.StatusId = 3 THEN 0
                                            WHEN stepcount.StepId NOT IN (6, 11, 12 ,13, 14, 15) THEN 0
                                            ELSE 1
                                        END AS Result,
		                                case
                                            WHEN lost.RequestId = @RequestId THEN 'This card has already been reported as lost.'
                                            WHEN dest.RequestId = @RequestId THEN 'This card has already been destroyed.'
                                            WHEN currentReq.StatusId = 3 THEN 'The application is no longer active.'
                                            WHEN stepcount.StepId NOT IN (6, 11, 12 ,13, 14, 15) THEN 'The application is currently being processed.'
		                                    ELSE 'Valid'
		                                END as Message
                                FROM TrnICardRequest currentReq
                                INNER JOIN TrnStepCounter stepcount on currentReq.RequestId=stepcount.RequestId 
                                LEFT JOIN TrnLostCards lost on lost.RequestId = currentReq.RequestId
                                LEFT JOIN TrnDestructionCards dest on dest.RequestId = currentReq.RequestId
                                LEFT JOIN TrnDomainMapping tdm on tdm.Id = @TDMId
                                LEFT JOIN MAppointment apt on apt.ApptId = tdm.ApptId
                                LEFT JOIN TrnHotlistCards hot on hot.RequestId=currentReq.RequestId
                                WHERE currentReq.RequestId = @RequestId;";

                using (var connection = _contextDP.CreateConnection())
                {
                    var parameters = new DynamicParameters();
                    parameters.Add("@RequestId", requestId, DbType.Int32, ParameterDirection.Input);
                    parameters.Add("@TDMId", TDMId, DbType.Int32, ParameterDirection.Input);
                    var result = await connection.QueryFirstOrDefaultAsync<DTOCheckBeforeLostReportResponse>(query, parameters);
                    return result ?? new DTOCheckBeforeLostReportResponse
                    {
                        Result = false,
                        Message = "Request not found",
                    };
                }
            }
            catch (Exception ee)
            {
                _logger.LogError(1001, ee, "LostCardDB->CheckBeforeLostReport");
                return new DTOCheckBeforeLostReportResponse
                {
                    Result = false,
                    Message = "Something went wrong",
                };
            }
        }
        public async Task<DTOGenericResponse<DTOCommonResponse?>> SaveLostCardRequest(DTOLostCardAddRequest Data, DTOCardMovementHistoryResponse LostReportBy)
        {
            var dTOResponse = new DTOGenericResponse<DTOCommonResponse?>();
            string LostRemarksId = "65";
            byte StatusId = 3;
            byte ReasonId = 8;
            string remarkIds = Data.RemarksIds != null && Data.RemarksIds.Any() ? string.Join(",", Data.RemarksIds) : string.Empty;
            // Initialize transaction for multiple database operations
            var (db, transaction) = _contextDP.CreateConnectionWithTransaction();

            try
            {
                var insertLostCard = @$" INSERT INTO TrnLostCards (RequestId, Remark, LostOn, IsActive, Updatedby, UpdatedOn, IsFIRLogged, SignedXML, SupportDocName, UpdatedbyUserId,RemarksIds)
                                         OUTPUT INSERTED.LostCardId 
                                         VALUES (@RequestId, @Remark, @LostOn, @IsActive, @Updatedby, @UpdatedOn, @IsFIRLogged, @SignedXML, @SupportDocName, @UpdatedbyUserId,@RemarksIds);";
                var parameters = new DynamicParameters();
                parameters.Add("@RequestId", Data.RequestId, DbType.Int32, ParameterDirection.Input);
                parameters.Add("@Remark", Data.Remark, DbType.String, ParameterDirection.Input, 100);
                parameters.Add("@LostOn", Data.LostOn, DbType.DateTime, ParameterDirection.Input);
                parameters.Add("@IsActive", Data.IsActive, DbType.Boolean, ParameterDirection.Input);
                parameters.Add("@Updatedby", Data.Updatedby, DbType.Int32, ParameterDirection.Input);
                parameters.Add("@UpdatedOn", Data.UpdatedOn, DbType.DateTime, ParameterDirection.Input);
                parameters.Add("@IsFIRLogged", Data.IsFIRLogged, DbType.Boolean, ParameterDirection.Input);
                parameters.Add("@SignedXML", Data.SignedXML, DbType.String, ParameterDirection.Input);
                parameters.Add("@SupportDocName", Data.SupportDocName, DbType.String, ParameterDirection.Input, 100);
                parameters.Add("@UpdatedbyUserId", Data.UpdatedbyUserId, DbType.Int32, ParameterDirection.Input);
                parameters.Add("@RemarksIds", remarkIds, DbType.String, ParameterDirection.Input, 100);

                // Insert the new posting record and get its ID
                var LostCardId = await db.QuerySingleAsync<int>(insertLostCard, parameters, transaction: transaction);

                if (Data.HotlistCardId == null)
                {
                    var insertHotlistCard = @$" INSERT INTO TrnHotlistCards (RequestId, RemarksIds, Remark, IsActive, Updatedby, UpdatedOn, UpdatedbyUserId)
                                         VALUES (@RequestId, @RemarksIds, @Remark, @IsActive, @Updatedby, @UpdatedOn, @UpdatedbyUserId);";

                    var parameters2 = new DynamicParameters();
                    parameters2.Add("@RequestId", Data.RequestId, DbType.Int32, ParameterDirection.Input);
                    parameters2.Add("@RemarksIds", LostRemarksId, DbType.String, ParameterDirection.Input, 100);
                    parameters2.Add("@Remark", Data.Remark, DbType.String, ParameterDirection.Input, 100);
                    parameters2.Add("@IsActive", Data.IsActive, DbType.Boolean, ParameterDirection.Input);
                    parameters2.Add("@Updatedby", Data.Updatedby, DbType.Int32, ParameterDirection.Input);
                    parameters2.Add("@UpdatedOn", Data.UpdatedOn, DbType.DateTime, ParameterDirection.Input);
                    parameters2.Add("@UpdatedbyUserId", Data.UpdatedbyUserId, DbType.Int32, ParameterDirection.Input);

                    await db.ExecuteAsync(insertHotlistCard, parameters2, transaction: transaction);
                }

                if (Data.StatusId == (byte)RequestStatusEnum.Running)
                {
                    string query = "UPDATE TrnICardRequest SET StatusId = @StatusId WHERE RequestId = @RequestId";

                    var parameters4 = new DynamicParameters();
                    parameters4.Add("@RequestId", Data.RequestId, DbType.Int32, ParameterDirection.Input);
                    parameters4.Add("@StatusId", StatusId, DbType.Int32, ParameterDirection.Input);

                    await db.ExecuteAsync(query, parameters4, transaction: transaction);


                    var insertApplClose = @$" INSERT INTO TrnApplClose (ReasonId, Authority, Remarks, RequestId, IsActive, Updatedby, UpdatedOn, UserId)
                                         VALUES (@ReasonId, @Authority, @Remarks, @RequestId, @IsActive, @Updatedby, @UpdatedOn, @UserId);";

                    var parameters3 = new DynamicParameters();
                    parameters3.Add("@ReasonId", ReasonId, DbType.Byte, ParameterDirection.Input);
                    parameters3.Add("@Authority", Data.AppointmentName, DbType.String, ParameterDirection.Input, 50);
                    parameters3.Add("@Remarks", Data.Remark, DbType.String, ParameterDirection.Input, 100);
                    parameters3.Add("@RequestId", Data.RequestId, DbType.Int32, ParameterDirection.Input);
                    parameters3.Add("@IsActive", Data.IsActive, DbType.Boolean, ParameterDirection.Input);
                    parameters3.Add("@Updatedby", Data.Updatedby, DbType.Int32, ParameterDirection.Input);
                    parameters3.Add("@UpdatedOn", Data.UpdatedOn, DbType.DateTime, ParameterDirection.Input);
                    parameters3.Add("@UserId", Data.UpdatedbyUserId, DbType.Int32, ParameterDirection.Input);

                    await db.ExecuteAsync(insertApplClose, parameters3, transaction: transaction);
                }

                if (Data.StatusId == (byte)RequestStatusEnum.Complete)
                {
                    
                    string SelectCompleteHistory = @"select CompleteReq.CompletedId,CompleteReq.CardRequestHistoryJson from CompletedICardRequests CompleteReq
	                                                Where CompleteReq.RequestId = @RequestId";

                    // Parameters for SQL query, adding the list of Request IDs
                    var parameters5 = new DynamicParameters();
                    parameters5.Add("@RequestId", Data.RequestId, DbType.Int32, ParameterDirection.Input);
                    CompletedICardRequest completedICard = await db.QuerySingleAsync<CompletedICardRequest>(SelectCompleteHistory, parameters5, transaction: transaction);

                    string? historyJson = completedICard?.CardRequestHistoryJson;

                    if (!string.IsNullOrWhiteSpace(historyJson))
                    {
                        ICardHistoryResponseAll cardHistoryResponseAll = new ICardHistoryResponseAll();
                        cardHistoryResponseAll = JsonConvert.DeserializeObject<ICardHistoryResponseAll>(historyJson) ?? new ICardHistoryResponseAll();
                        cardHistoryResponseAll.CardMovement.Add(LostReportBy);

                        var cardRequestHistoryJson = JsonConvert.SerializeObject(cardHistoryResponseAll);

                        string UpdateCompleteHistory = @"Update CompletedICardRequests Set CardRequestHistoryJson = @CardRequestHistoryJson Where CompletedId = @CompletedId";
                        var parameters6 = new DynamicParameters();
                        parameters6.Add("@CardRequestHistoryJson", cardRequestHistoryJson, DbType.AnsiString, ParameterDirection.Input, size: -1);
                        parameters6.Add("@CompletedId", completedICard?.CompletedId, DbType.Int32, ParameterDirection.Input);

                        await db.ExecuteAsync(UpdateCompleteHistory, parameters6, transaction: transaction);
                    }
                }


                // Commit the transaction if all operations succeed
                transaction.Commit();
                dTOResponse.Result = true;
                dTOResponse.Message = "Lost card request saved successfully.";
                dTOResponse.Value = new DTOCommonResponse
                {
                    Id = LostCardId.ToString(),
                    CurrentTime = Data.UpdatedOn.GetValueOrDefault()
                };
                return dTOResponse;
            }
            catch (Exception ex)
            {
                // Rollback the transaction if any operation fails
                transaction.Rollback();
                _logger.LogError(1001, ex, "LostCardDB->SaveLostCardRequest");
                dTOResponse.Result = false;
                dTOResponse.Message = "Internal Server Error";
                dTOResponse.Value = new DTOCommonResponse();
                return dTOResponse;
            }
            finally
            {
                // Dispose of the connection
                db.Dispose();
            }
        }
    }
}
