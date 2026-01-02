using Dapper;
using DataAccessLayer.BaseInterfaces;
using DataAccessLayer.Logger;
using DataTransferObject.Domain.Model;
using DataTransferObject.Requests;
using DataTransferObject.Response;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Data;

namespace DataAccessLayer
{
    /// <summary>
    /// Data Access Layer for AfsacCellMapping entity, providing database operations.
    /// And implements the IAfsacCellMappingDB interface.
    /// </summary>
    public class AfsacCellMappingDB : GenericRepositoryDL<AfsacCellMapping>, IAfsacCellMappingDB
    {
        protected new readonly ApplicationDbContext _context;// For Entity Framework operations
        private readonly DapperContext _contextDP;// For Dapper operations
        private readonly ILogger<AfsacCellMappingDB> _logger;// For logging

        /// <summary>
        /// Constructor to initialize the AfsacCellMappingDB with necessary contexts and logger.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="contextDP"></param>
        /// <param name="logger"></param>
        public AfsacCellMappingDB(ApplicationDbContext context, DapperContext contextDP, ILogger<AfsacCellMappingDB> logger) : base(context)
        {
            _context = context;
            _contextDP = contextDP;
            _logger = logger;
        }


        /// <summary>
        /// Asynchronously checks if any record in the AfsacCellMapping table exists with a different AfsacCellMappingId 
        /// than the one provided in the Dto parameter.
        /// </summary>
        /// <param name="Dto">The Data Transfer Object containing the AfsacCellMappingId to be checked against.</param>
        /// <returns>Returns true if a record exists with a different AfsacCellMappingId, otherwise false.</returns>
        public async Task<bool> GetByName(AfsacCellMapping Dto)
        {
            // LINQ query using AnyAsync() to check if there are any records in the AfsacCellMapping table 
            // where the AfsacCellMappingId is not equal to the one in the provided Dto.
            // This query returns a boolean indicating whether any such record exists.
            var ret = await _context.AfsacCellMapping
                                    .AnyAsync(x => x.AfsacCellMappingId != Dto.AfsacCellMappingId);

            // Return the result of the query.
            return ret;
        }

        /// <summary>
        /// Asynchronously retrieves all records from the AfsacCellMapping table and its related tables 
        /// (TrnDomainMapping, AspNetUsers, UserProfile, MRank, MapUnit, and MUnit), mapping the result to 
        /// a list of DTOAfsacCellMappingResponse objects.
        /// </summary>
        /// <returns>
        /// A list of DTOAfsacCellMappingResponse objects representing the retrieved records, or null if an error occurs.
        /// </returns>
        public async Task<List<DTOAfsacCellMappingResponse>?> GetAllAfsacCellMapping()
        {
            try
            {
                // SQL query to fetch data from AfsacCellMapping and its related tables.
                // The query retrieves fields from AfsacCellMapping, TrnDomainMapping, AspNetUsers, UserProfile,
                // MRank, MapUnit, and MUnit, joining them using LEFT JOINs.
                // The results are ordered by AfsacCellMappingId in descending order.
                string query = "";
                query = @"Select acmap.AfsacCellMappingId, acmap.TDMId, acmap.UnitId, users.DomainId, usep.ArmyNo, ra.RankAbbreviation, 
                            usep.Name, munit.Sus_no, munit.Suffix, munit.UnitName 
                            from AfsacCellMapping acmap 
                            left join TrnDomainMapping trndomain on trndomain.Id = acmap.TDMId 
                            left join AspNetUsers users on users.Id = trndomain.AspNetUsersId 
                            left join UserProfile usep on usep.UserId = trndomain.UserId 
                            left join MRank ra on ra.RankId = usep.RankId 
                            left join MapUnit mapunit on mapunit.UnitMapId = acmap.UnitId 
                            left join MUnit munit on munit.UnitId = mapunit.UnitId 
                            order by acmap.AfsacCellMappingId desc";

                // Using the database connection to execute the SQL query asynchronously.
                // QueryAsync is used to fetch the result into a collection of DTOAfsacCellMappingResponse.
                // The results are converted to a list and returned.
                using (var connection = _contextDP.CreateConnection())
                {
                    var allrecord = await connection.QueryAsync<DTOAfsacCellMappingResponse>(query);
                    return allrecord.ToList();
                }
            }
            catch (Exception ex)
            {
                // Logging any exceptions that occur during the database operation.
                _logger.LogError(1001, ex, "AfsacCellMappingDB->GetAllAfsacCellMapping");
                return null;  // Returning null in case of an error.
            }
        }
        public async Task<DTODataTablesResponse<DTOAfsacCellMappingResponse>> GetAllAfsacCellMapping_Pagination(DTODataTablesRequest dTO)
        {
            string selectFields = "";
            string fromJoinClause = "";
            string whereClause = "";
            // Map allowed sort columns to DB fields
            Dictionary<string, string> allowedSortColumns = new Dictionary<string, string>();

            var sortOrder = dTO.sortDirection == "desc" ? "DESC" : "ASC";

            allowedSortColumns = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                ["AfsacCellMappingId"] = "acmap.AfsacCellMappingId",
            };
            selectFields = @"acmap.AfsacCellMappingId, acmap.TDMId, acmap.UnitId, users.DomainId, usep.ArmyNo, ra.RankAbbreviation,usep.Name, munit.Sus_no, munit.Suffix, munit.UnitName";
            fromJoinClause = @"from AfsacCellMapping acmap 
                                left join TrnDomainMapping trndomain on trndomain.Id = acmap.TDMId 
                                left join AspNetUsers users on users.Id = trndomain.AspNetUsersId 
                                left join UserProfile usep on usep.UserId = trndomain.UserId 
                                left join MRank ra on ra.RankId = usep.RankId 
                                left join MapUnit mapunit on mapunit.UnitMapId = acmap.UnitId 
                                left join MUnit munit on munit.UnitId = mapunit.UnitId";
            whereClause = @"WHERE
                                (
                                   @SearchTerm = '' OR
                                   users.DomainId LIKE '%' + @SearchTerm + '%'
                                )";
            try
            {
                var sortColumn = allowedSortColumns.ContainsKey(dTO.sortColumn ?? "")
                ? allowedSortColumns[dTO.sortColumn!]
                : "acmap.AfsacCellMappingId";
                var multiQuery = $@"
                        WITH RecordCTE AS (
                            select  Count(*) OVER () as TotalFilteredRecords,ROW_NUMBER() OVER (ORDER BY {sortColumn} {sortOrder}) AS RowNum, {selectFields} {fromJoinClause} {whereClause}
                        )
                        SELECT * FROM RecordCTE WHERE RowNum BETWEEN @Offset AND @Limit;";

                using (var connection = _contextDP.CreateConnection())
                {
                    dTO.searchValue = string.IsNullOrEmpty(dTO.searchValue) ? string.Empty : dTO.searchValue.Trim();
                    var parameters = new DynamicParameters();
                    parameters.Add("@Offset", dTO.Start + 1, DbType.Int32, ParameterDirection.Input);
                    parameters.Add("@Limit", (dTO.Start + dTO.Length), DbType.Int32, ParameterDirection.Input);
                    parameters.Add("@SearchTerm", dTO.searchValue, DbType.String, ParameterDirection.Input);

                    var ret = await connection.QueryMultipleAsync(multiQuery, parameters);
                    var records = (await ret.ReadAsync<DTOAfsacCellMappingResponse>()).ToList();
                    var totalFilteredRecords = records?.FirstOrDefault()?.TotalFilteredRecords;

                    var responseData = new DTODataTablesResponse<DTOAfsacCellMappingResponse>
                    {
                        draw = dTO.Draw,
                        recordsTotal = totalFilteredRecords.GetValueOrDefault(),
                        recordsFiltered = totalFilteredRecords.GetValueOrDefault(),
                        data = records,
                    };
                    return responseData;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(1001, ex, "AfsacCellMappingDB->GetAllAfsacCellMapping_Pagination");
                List<DTOAfsacCellMappingResponse> dTOAfsacs = new List<DTOAfsacCellMappingResponse>();
                var responseData = new DTODataTablesResponse<DTOAfsacCellMappingResponse>
                {
                    draw = 0,
                    recordsTotal = 0,
                    recordsFiltered = 0,
                    data = dTOAfsacs
                };
                return responseData;
            }
        }

    }
}