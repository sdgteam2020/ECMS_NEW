using Dapper;
using DataAccessLayer.BaseInterfaces;
using DataAccessLayer.Logger;
using DataTransferObject.Domain.Master;
using DataTransferObject.Requests;
using DataTransferObject.Response;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Data;

namespace DataAccessLayer
{
    public class CorpsDB : GenericRepositoryDL<MCorps>, ICorpsDB
    {
        protected new readonly ApplicationDbContext _context;
        protected readonly DapperContext _contextDP;
        private readonly ILogger<CorpsDB> _logger;
        public CorpsDB(ApplicationDbContext context, DapperContext contextDP, ILogger<CorpsDB> logger) : base(context)
        {
            _logger = logger;
            _context = context;
            _contextDP = contextDP;
        }

        /// <summary>
        /// Checks if a given Corps name already exists in the database, excluding the current Corps record based on its ID.
        /// </summary>
        /// <param name="Data">The `MCorps` object that contains the `CorpsName` to be checked.</param>
        /// <returns>
        /// Returns `true` if a Corps with the same name (excluding the current one) exists in the database, otherwise `false`.
        /// </returns>
        /// <remarks>
        /// This method is used to prevent duplicate Corps names from being saved. It compares the `CorpsName` in a case-insensitive manner and ensures that the current record (identified by `CorpsId`) is excluded from the check.
        /// </remarks>
        public async Task<bool> GetByName(MCorps Data)
        {
            //var ret = _context.MCorps.Where(p=> p.ComdId != Data.ComdId).Select(p => p.CorpsName.ToUpper() == Data.CorpsName.ToUpper()).FirstOrDefault();
            
            // Use LINQ to check if any other Corps with the same name (excluding the current one) exists in the database
            var ret = await _context.MCorps.AnyAsync(p => p.CorpsName.ToUpper() == Data.CorpsName.ToUpper() && p.CorpsId != Data.CorpsId);

            // Return the result: true if a matching record is found, otherwise false
            return ret;
        }

        /// <summary>
        /// Retrieves all Corps records from the database, excluding the Corps with `CorpsId = 1 (No Core)`.
        /// The method also joins with the `MComd` table to get the corresponding `ComdName` for each Corps record.
        /// </summary>
        /// <returns>
        /// A list of <see cref="DTOCorpsResponse"/> containing:
        /// - CorpsId: The identifier of the Corps.
        /// - CorpsName: The name of the Corps.
        /// - ComdName: The name of the Command (joined from the MComd table).
        /// - ComdId: The identifier of the Command.
        /// </returns>
        /// <remarks>
        /// This method queries the `MCorps` table and performs a join with the `MComd` table on the `ComdId` field.
        /// It filters out the Corps with `CorpsId = 1` and returns the data as a list of <see cref="DTOCorpsResponse"/> objects.
        /// The query is executed asynchronously using Entity Framework and `ToListAsync()`.
        /// </remarks>
        public async Task<List<DTOCorpsResponse>> GetALLCorps()
        {
            var Corps = await (from c in _context.MCorps
                                 join d in _context.MComd
                                 on c.ComdId equals d.ComdId
                                 where c.CorpsId!=1
                                 select new DTOCorpsResponse
                                 {
                                     CorpsId = c.CorpsId,
                                     CorpsName = c.CorpsName,
                                     ComdName = d.ComdName,
                                     ComdId=d.ComdId,
                                 }).ToListAsync();
            return Corps;  
        }


        /// <summary>
        /// Retrieves a list of Corps that belong to a specific Command (Comd) based on the provided Command ID.
        /// </summary>
        /// <param name="ComdId">The ID of the Command (Comd) for which the associated Corps should be retrieved.</param>
        /// <returns>
        /// A list of `DTOCorpsResponse` containing the `CorpsId` and `CorpsName` of all Corps associated with the specified Command.
        /// </returns>
        /// <remarks>
        /// This method performs a join between the `MCorps` table and the `MComd` table to fetch all Corps associated with the given Command ID (`ComdId`).
        /// The result is a list of Corps that belong to the specified Command.
        /// </remarks>
        public async Task<List<DTOCorpsResponse>> GetByComdId(int ComdId)
        {
            // Perform an asynchronous LINQ query to retrieve Corps associated with the specified Command ID (ComdId)
            var Corps = await (from c in _context.MCorps
                                 join d in _context.MComd
                                 on c.ComdId equals d.ComdId where c.ComdId == ComdId   
                                 select new DTOCorpsResponse
                                 {
                                     CorpsId = c.CorpsId,
                                     CorpsName = c.CorpsName,
                                 }).ToListAsync(); // Convert the result to a List asynchronously

            // Return the list of Corps
            return Corps;
        }

        /// <summary>
        /// Checks if the provided CorpsId exists in foreign key relationships within the database.
        /// </summary>
        /// <param name="CorpsId">The unique identifier for the Corps.</param>
        /// <returns>
        /// A `DTOCorpsIdCheckInFKTableResponse` object containing counts of related entities (e.g., Bde, Div, MapUnit) if any exist.
        /// Returns `null` if no data is found or an error occurs.
        /// </returns>
        /// <remarks>
        /// This method checks for the existence of related entities like Bde, Div, and MapUnit that are associated with a specific CorpsId.
        /// The query counts the number of distinct related records in the respective tables. 
        /// If the CorpsId is referenced in any of the foreign key relationships, those counts are returned as part of the response.
        /// </remarks>
        public async Task<DTOCorpsIdCheckInFKTableResponse?> CorpsIdCheckInFKTable(byte CorpsId)
        {
            try
            {
                // SQL query to check if the CorpsId is referenced in foreign key relationships
                string query = @"Select  count(distinct mbd.BdeId) as TotalBde ,count(distinct mdiv.DivId) as TotalDiv,count(distinct mapunit.UnitMapId) as TotalMapUnit from MCorps mcor
                                left join MBde mbd on mbd.CorpsId = mcor.CorpsId 
                                left join MDiv mdiv on mdiv.CorpsId = mcor.CorpsId 
                                left join MapUnit mapunit on mapunit.CorpsId = mcor.CorpsId 
                                where mcor.CorpsId = @CorpsId";

                using (var connection = _contextDP.CreateConnection())
                {
                    // Execute the query asynchronously and return the first record if it exists
                    var ret = await connection.QueryAsync<DTOCorpsIdCheckInFKTableResponse>(query, new { CorpsId });
                    return ret.FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                // Log the error if something goes wrong during the database operation
                _logger.LogError(1001, ex, "CorpsDB->CorpsIdCheckInFKTable");
                return null; // Return null in case of an error
            }
        }
        public async Task<DTODataTablesResponse<DTOCorpsResponse>> GetAllCorps_Pagination(DTODataTablesRequest dTO)
        {
            string selectFields = "";
            string fromJoinClause = "";
            string whereClause = "";
            // Map allowed sort columns to DB fields
            Dictionary<string, string> allowedSortColumns = new Dictionary<string, string>();

            var sortOrder = dTO.sortDirection == "desc" ? "DESC" : "ASC";

            allowedSortColumns = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                ["ComdName"] = "com.ComdName",
                ["CorpsName"] = "cor.CorpsName",
            };
            selectFields = @"cor.CorpsId,cor.CorpsName,com.ComdId,com.ComdName";
            fromJoinClause = @"from MComd com
                                INNER JOIN MCorps cor on cor.ComdId=com.ComdId";
            whereClause = @"WHERE
                                cor.CorpsId <> 1
                                AND (
                                    cor.CorpsName LIKE '%' + @SearchTerm + '%' OR
                                    com.ComdName LIKE '%' + @SearchTerm + '%'
                                )";
            try
            {
                var sortColumn = allowedSortColumns.ContainsKey(dTO.sortColumn ?? "")
                ? allowedSortColumns[dTO.sortColumn!]
                : "cor.CorpsId";
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
                    var records = (await ret.ReadAsync<DTOCorpsResponse>()).ToList();
                    var totalFilteredRecords = records?.FirstOrDefault()?.TotalFilteredRecords;

                    var responseData = new DTODataTablesResponse<DTOCorpsResponse>
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
                _logger.LogError(1001, ex, "CorpsDB->GetAllCorps_Pagination");
                List<DTOCorpsResponse> dTOAllCommands = new List<DTOCorpsResponse>();
                var responseData = new DTODataTablesResponse<DTOCorpsResponse>
                {
                    draw = dTO.Draw,
                    recordsTotal = 0,
                    recordsFiltered = 0,
                    data = dTOAllCommands
                };
                return responseData;
            }
        }

    }
}