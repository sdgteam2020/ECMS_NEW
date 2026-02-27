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
    /// Class for handling operations related to Hotlist Cards in the database.
    /// </summary>
    public class HotlistCardDB : GenericRepositoryDL<TrnHotlistCard>, IHotlistCardDB
    {
        protected new readonly ApplicationDbContext _context;
        private readonly DapperContext _contextDP;
        private readonly IDataProtector protector;
        private readonly ILogger<HotlistCardDB> _logger;

        /// <summary>
        /// Initializes the HotlistCardDB class with the provided context and services.
        /// </summary>
        /// <param name="context">ApplicationDbContext instance for interacting with the database.</param>
        /// <param name="contextDP">DapperContext instance for executing raw SQL queries.</param>
        /// <param name="dataProtectionProvider">Provider for creating data protectors.</param>
        /// <param name="logger">Logger instance for logging errors and information.</param>
        /// <param name="dataProtectionPurposeStrings">Strings for defining the purpose of data protection.</param>
        public HotlistCardDB(ApplicationDbContext context, DapperContext contextDP, IDataProtectionProvider dataProtectionProvider, ILogger<HotlistCardDB> logger, DataProtectionPurposeStrings dataProtectionPurposeStrings) : base(context)
        {
            _context = context;
            _contextDP = contextDP;
            _logger = logger;
            // Pass the purpose string as a parameter
            this.protector = dataProtectionProvider.CreateProtector(
                dataProtectionPurposeStrings.AFSACIdRouteValue);
        }

        /// <summary>
        /// Checks if any request with the specified RequestId exists in the database.
        /// </summary>
        /// <param name="RequestId">The RequestId to check for.</param>
        /// <returns>True if the request exists, otherwise false.</returns>
        public async Task<bool> FindAnyRequestId(int RequestId)
        {
            try
            {
                return await _context.TrnHotlistCards
                                .AnyAsync(f => f.RequestId == RequestId);
            }
            catch (Exception ex)
            {
                _logger.LogError(1001, ex, "HotlistCardDB->FindAnyRequestId");
                return false;
            }
        }


        /// <summary>
        /// Retrieves a paginated list of hotlist cards based on the provided data request parameters.
        /// </summary>
        /// <param name="dTO">The data request parameters including sorting and filtering options.</param>
        /// <returns>A DTODataTablesResponse object containing the paginated list of hotlist cards.</returns>
        public async Task<DTODataTablesResponse<DTOHotlistCardGetResponse>> GetAllHotlist(DTODataTablesRequest dTO)
        {
            List<DTOHotlistCardGetResponse> dTOHotlistCardGetResponses = new List<DTOHotlistCardGetResponse>();
            var responseData = new DTODataTablesResponse<DTOHotlistCardGetResponse>
            {
                draw = 0,
                recordsTotal = 0,
                recordsFiltered = 0,
                data = dTOHotlistCardGetResponses
            };
            try
            {
                // Map allowed sort columns to DB fields
                var allowedSortColumns = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
                {
                    ["ServiceNo"] = "bas.ServiceNo",
                    ["UpdatedOn"] = "hotlist.UpdatedOn",
                    ["RequestId"] = "req.RequestId",
                    ["Remark"] = "hotlist.Remark"
                };

                // Determine the column to sort by, default to "UpdatedOn"
                var sortColumn = allowedSortColumns.ContainsKey(dTO.sortColumn ?? "")
                    ? allowedSortColumns[dTO.sortColumn!]
                    : "hotlist.UpdatedOn";

                var sortOrder = dTO.sortDirection == "desc" ? "DESC" : "ASC";

                // Define the select fields for the query
                string selectFields = @"appl.Name ApplyFor,
                                        req.RequestId,hotlist.HotlistCardId,
                                        bas.ServiceNo,ranks.RankAbbreviation RankName,
                                        bas.FName,bas.LName,
                                        Muni.Abbreviation UnitAbbreviation,
                                        hotlist.UpdatedOn,hotlist.Remark,
                                        (select STRING_AGG(Remarks,'#') from MRemarks where RemarksId in (select value from string_split(hotlist.RemarksIds,','))) RemarksNameList";

                // Join tables for the query
                string fromJoinClause = @"from TrnHotlistCards hotlist
                                        inner join TrnICardRequest req on req.RequestId = hotlist.RequestId
                                        inner join TrnDomainMapping tdm on tdm.Id=req.TrnDomainMappingId
                                        inner join BasicDetails bas on bas.BasicDetailId=req.BasicDetailId
                                        inner join MRank ranks on ranks.RankId=bas.RankId
                                        inner join MapUnit uni on uni.UnitMapId=bas.UnitId
                                        inner join MUnit Muni on Muni.UnitId=uni.UnitId
                                        inner join MApplyFor appl on appl.ApplyForId=bas.ApplyForId";

                // Filter clause for the search term
                string whereClause = @"Where bas.ServiceNo like '%' + @SearchTerm + '%' ";

                // SQL query to retrieve paginated results using a Common Table Expression (CTE)
                var multiQuery = $@"
                        WITH RecordCTE AS (
                            select  Count(*) OVER () as TotalFilteredRecords,ROW_NUMBER() OVER (ORDER BY {sortColumn} {sortOrder}) AS RowNum, {selectFields} {fromJoinClause} {whereClause}
                        )
                        SELECT * FROM RecordCTE WHERE RowNum BETWEEN @Offset AND @Limit;";

                using (var connection = _contextDP.CreateConnection())
                {
                    // Trim search value to avoid unnecessary spaces
                    dTO.searchValue = string.IsNullOrEmpty(dTO.searchValue) ? string.Empty : dTO.searchValue.Trim();

                    // Prepare query parameters
                    var parameters = new DynamicParameters();
                    parameters.Add("@Offset", dTO.Start + 1, DbType.Int32, ParameterDirection.Input);
                    parameters.Add("@Limit", (dTO.Start + dTO.Length), DbType.Int32, ParameterDirection.Input);
                    parameters.Add("@SearchTerm", dTO.searchValue, DbType.String, ParameterDirection.Input);

                    // Execute the query and get the result
                    var ret = await connection.QueryMultipleAsync(multiQuery, parameters);

                    // Read the data
                    var records = (await ret.ReadAsync<DTOHotlistCardGetResponse>()).ToList();
                    var totalFilteredRecords = records?.FirstOrDefault()?.TotalFilteredRecords;

                    // Prepare the response object with filtered data
                    responseData = new DTODataTablesResponse<DTOHotlistCardGetResponse>
                    {
                        draw = dTO.Draw,
                        recordsTotal = totalFilteredRecords.GetValueOrDefault(),
                        recordsFiltered = totalFilteredRecords.GetValueOrDefault(),
                        data = records,
                    };
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(1001, ex, "HotlistCardDB->GetAllHotlist");
            }
            return responseData;
        }


        /// <summary>
        /// Retrieves the detailed hotlist card data for the specified RequestIds.
        /// </summary>
        /// <param name="Data">The request data containing the list of RequestIds to retrieve details for.</param>
        /// <returns>A list of <see cref="DTOHotlistCardExportResponse"/> containing the detailed information for each hotlist card.</returns>
        public async Task<List<DTOHotlistCardExportResponse>> GetDetailsByRequestIds(DTOHotlistCardsExportRequest Data)
        {
            var records = new List<DTOHotlistCardExportResponse>();
            try
            {
                // Define the SQL query to fetch the detailed hotlist card data for the provided RequestIds
                string query = @"select req.RequestId,hotlist.HotlistCardId,bas.ServiceNo as ArmyNo,
	                                ranks.RankAbbreviation,bas.FName,bas.LName,Muni.Abbreviation Unit,
	                                hotlist.UpdatedOn as DateAndTime,hotlist.Remark,hotlist.IsActive as IsActiveBool,
	                                (select STRING_AGG(Remarks,' | ') from MRemarks where RemarksId in (select value from string_split(hotlist.RemarksIds,','))) Reasons,
	                                req.CardSerialNo,req.ChipNo
	                                from TrnHotlistCards hotlist
	                                inner join TrnICardRequest req on req.RequestId = hotlist.RequestId
	                                inner join BasicDetails bas on bas.BasicDetailId=req.BasicDetailId
	                                inner join MRank ranks on ranks.RankId=bas.RankId
	                                inner join MapUnit uni on uni.UnitMapId=bas.UnitId
	                                inner join MUnit Muni on Muni.UnitId=uni.UnitId
                                  Where req.RequestId in @Ids";

                // Prepare parameters to pass into the SQL query
                var parameters = new DynamicParameters();
                parameters.Add("@Ids", Data.Ids);

                // Open a database connection and execute the query
                using (var connection = _contextDP.CreateConnection())
                {
                    var ret = await connection.QueryAsync<DTOHotlistCardExportResponse>(query, parameters);

                    // Convert the result to a list
                    records = ret.ToList();
                }
            }
            catch (Exception ex)
            {
                // Log any exceptions that occur during the query execution
                _logger.LogError(1001, ex, "HotlistCardDB->GetBesicdetailsByRequestId");
            }
            return records;
        }
        public async Task<DTOGenericResponse<string>> CheckBeforeHotListCardReport(int RequestId)
        {
            DTOGenericResponse<string> response = new DTOGenericResponse<string>();
            try
            {
                string query = @"SELECT CASE
                                            WHEN hot.RequestId = @RequestId THEN 0
                                            WHEN currentReq.StatusId IN (1,3)  THEN 0
                                            WHEN stepcount.StepId != 15 THEN 0
                                            ELSE 1
                                        END AS Result,
		                                case
                                            WHEN hot.RequestId = @RequestId THEN 'This card has already been reported as hot list.'
                                            WHEN currentReq.StatusId IN (1,3) THEN 'The application is no longer active.'
                                            WHEN stepcount.StepId != 15 THEN 'The application is currently being processed.'
		                                    ELSE 'Valid'
		                                END as Message
                                FROM TrnICardRequest currentReq
                                INNER JOIN TrnStepCounter stepcount on currentReq.RequestId=stepcount.RequestId 
                                LEFT JOIN TrnHotlistCards hot on hot.RequestId = currentReq.RequestId
                                WHERE currentReq.RequestId = @RequestId;";

                using (var connection = _contextDP.CreateConnection())
                {
                    var parameters = new DynamicParameters();
                    parameters.Add("@RequestId", RequestId, DbType.Int32, ParameterDirection.Input);
                    var result = await connection.QueryFirstOrDefaultAsync<DTOGenericResponse<string>>(query, parameters);
                    return result ?? new DTOGenericResponse<string>
                    {
                        Result = false,
                        Message = "Request not found",
                    };
                }
            }
            catch (Exception ee)
            {
                _logger.LogError(1001, ee, "HotlistCardDB->CheckBeforeLostReport");
                response.Result = false;
                response.Message = "Something went wrong";
                response.Value = string.Empty;  
                return response;
            }
        }
    }
}
