using Dapper;
using DataAccessLayer.BaseInterfaces;
using DataAccessLayer.Logger;
using DataTransferObject.Domain.Model;
using DataTransferObject.Requests;
using DataTransferObject.Response;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Data;

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
                                        IF EXISTS (
                                            SELECT 1
                                            FROM TrnLostCards lc
                                            JOIN TrnICardRequest tir ON lc.RequestId = tir.RequestId
                                            JOIN BasicDetails bd ON tir.BasicDetailId = bd.BasicDetailId
                                            WHERE bd.BasicDetailId = (
                                                SELECT MAX(BasicDetailId)
                                                FROM BasicDetails
                                                WHERE ServiceNo = @ServiceNo
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
        public async Task<DTODataTablesResponse<DTOLostCardGetResponse>> GetAllLost(DTODataTablesRequest dTO)
        {
            List<DTOLostCardGetResponse> dTOLostCardGetResponses = new List<DTOLostCardGetResponse>();
            var responseData = new DTODataTablesResponse<DTOLostCardGetResponse>
            {
                draw = 0,
                recordsTotal = 0,
                recordsFiltered = 0,
                data = dTOLostCardGetResponses
            };
            try
            {
                // Map allowed sort columns to DB fields
                var allowedSortColumns = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
                {
                    ["ServiceNo"] = "bas.ServiceNo",
                    ["UpdatedOn"] = "lost.UpdatedOn",
                    ["LostOn"] = "lost.LostOn",
                    ["Remark"] = "lost.Remark"
                };

                var sortColumn = allowedSortColumns.ContainsKey(dTO.sortColumn ?? "")
                    ? allowedSortColumns[dTO.sortColumn!]
                    : "lost.UpdatedOn";

                var sortOrder = dTO.sortDirection;


                string selectFields = @"appl.Name ApplyFor,
                                        req.RequestId,lost.LostCardId,
                                        bas.ServiceNo,ranks.RankAbbreviation RankName,
                                        bas.FName,bas.LName,
                                        Muni.UnitName,Muni.Abbreviation UnitAbbreviation,
                                        lost.UpdatedOn,lost.Remark,lost.IsActive,
                                        bas.NameAsPerRecord,lost.LostOn,
                                        regi.Abbreviation RegimentalName,
                                        lost.IsFIRLogged,lost.SupportDocName";
                string fromJoinClause = @"from TrnLostCards lost
                                        inner join TrnICardRequest req on req.RequestId = lost.RequestId
                                        inner join TrnDomainMapping tdm on tdm.Id=req.TrnDomainMappingId
                                        inner join BasicDetails bas on bas.BasicDetailId=req.BasicDetailId
                                        inner join MRank ranks on ranks.RankId=bas.RankId
                                        inner join MapUnit uni on uni.UnitMapId=bas.UnitId
                                        inner join MUnit Muni on Muni.UnitId=uni.UnitId
                                        inner join MApplyFor appl on appl.ApplyForId=bas.ApplyForId
                                        left join MRegimental regi on regi.RegId=bas.RegimentalId";
                string whereClause = @"Where bas.ServiceNo like '%' + @SearchTerm + '%'";

                var multiQuery = $@"
                        WITH RecordCTE AS (
                            select  Count(*) OVER () as TotalFilteredRecords,ROW_NUMBER() OVER (ORDER BY {sortColumn} {sortOrder}) AS RowNum, {selectFields} {fromJoinClause} {whereClause}
                        )
                        SELECT * FROM RecordCTE WHERE RowNum BETWEEN @Offset AND @Limit;";

                using (var connection = _contextDP.CreateConnection())
                {
                    // Parameters for SQL query
                    dTO.searchValue = string.IsNullOrEmpty(dTO.searchValue) ? string.Empty : dTO.searchValue.Trim();
                    var parameters = new DynamicParameters();
                    parameters.Add("@Offset", dTO.Start + 1, DbType.Int32, ParameterDirection.Input);
                    parameters.Add("@Limit", (dTO.Start + dTO.Length), DbType.Int32, ParameterDirection.Input);
                    parameters.Add("@SearchTerm", dTO.searchValue, DbType.String, ParameterDirection.Input);

                    var ret = await connection.QueryMultipleAsync(multiQuery, parameters);
                    var records = (await ret.ReadAsync<DTOLostCardGetResponse>()).ToList();
                    var totalFilteredRecords = records?.FirstOrDefault()?.TotalFilteredRecords;

                    responseData = new DTODataTablesResponse<DTOLostCardGetResponse>
                    {
                        draw = dTO.Draw,
                        recordsTotal = totalFilteredRecords.GetValueOrDefault(),
                        recordsFiltered = totalFilteredRecords.GetValueOrDefault(),
                        data = (from e in records
                                select new DTOLostCardGetResponse()
                                {
                                    EncryptedId = protector.Protect(e.LostCardId.ToString()),
                                    NameAsPerRecord = e.NameAsPerRecord,
                                    FName = e.FName,
                                    LName = e.LName,
                                    ServiceNo = e.ServiceNo,
                                    UnitName = e.UnitName,
                                    UnitAbbreviation = e.UnitAbbreviation,
                                    RankName = e.RankName,
                                    ArmedName = e.ArmedName,
                                    RequestId = e.RequestId,
                                    UpdatedOn = e.UpdatedOn,
                                    ApplyFor = e.ApplyFor,
                                    LostCardId = e.LostCardId,
                                    LostOn = e.LostOn,
                                    Remark = e.Remark,
                                    IsActive = e.IsActive,
                                    SupportDocName = e.SupportDocName,
                                    IsFIRLogged = e.IsFIRLogged
                                }).ToList()
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
                string query = @"select req.RequestId,lost.LostCardId,bas.ServiceNo as ArmyNo,
	                                ranks.RankAbbreviation,bas.FName,bas.LName,Muni.Abbreviation Unit,
	                                lost.UpdatedOn as DateAndTime,lost.Remark,lost.IsActive as IsActiveBool,
	                                req.CardSerialNo,req.ChipNo,lost.LostOn
	                                from TrnLostCards lost
	                                inner join TrnICardRequest req on req.RequestId = lost.RequestId
	                                inner join BasicDetails bas on bas.BasicDetailId=req.BasicDetailId
	                                inner join MRank ranks on ranks.RankId=bas.RankId
	                                inner join MapUnit uni on uni.UnitMapId=bas.UnitId
	                                inner join MUnit Muni on Muni.UnitId=uni.UnitId
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
    }
}
