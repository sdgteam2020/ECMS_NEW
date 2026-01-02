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
    public class DivDB : GenericRepositoryDL<MDiv>, IDivDB
    {
        protected new readonly ApplicationDbContext _context;
        protected readonly DapperContext _contextDP;
        private readonly ILogger<DivDB> _logger;
        public DivDB(ApplicationDbContext context, DapperContext contextDP, ILogger<DivDB> logger) : base(context)
        {
            _logger = logger;
            _context = context;
            _contextDP = contextDP;
        }
        /// <summary>
        /// Checks if a division with the same name exists in the database, excluding the current division.
        /// </summary>
        /// <param name="Data">The division object containing the division name and ID to check for uniqueness.</param>
        /// <returns>
        /// Returns <c>true</c> if a division with the same name (but different ID) exists, otherwise <c>false</c>.
        /// </returns>
        /// <remarks>
        /// This method is useful for validating the uniqueness of a division name when adding or updating division records.
        /// It ensures that no two divisions with the same name exist in the database.
        /// </remarks>
        public async Task<bool> GetByName(MDiv Data)
        {
            // Query to check if any division with the same name exists, excluding the current division by ID
            var ret = await _context.MDiv.AnyAsync(p => p.DivName.ToUpper() == Data.DivName.ToUpper() && p.DivId != Data.DivId);
            
            // Return true if a division with the same name exists, otherwise false
            return ret;
        }


        /// <summary>
        /// Retrieves all DIV (MDiv) data from the database, joining with the Corps (MCorps) and Command (MComd) tables.
        /// Excludes the default DIV with DivId = 1 and returns the data in a list of DTO (Data Transfer Object) format.
        /// </summary>
        /// <returns>
        /// Returns a list of `DTODivResponse` objects containing the DIV data, including the DIV's name, associated corps, and command.
        /// </returns>
        /// <remarks>
        /// The method performs a LINQ query that joins the `MDiv`, `MCorps`, and `MComd` tables, filtering out the default DIV 
        /// where the `DivId` is 1. It then selects specific fields to return in a `DTODivResponse` object, which is a DTO 
        /// designed for transferring DIV data to the client.
        /// </remarks>
        public async Task<List<DTODivResponse>> GetALLDiv()
        {
            //on new { Div.UnitId, a.Years_Months } equals new { c.UnitId, c.Years_Months }
            // Perform a LINQ query to join MDiv, MCorps, and MComd and select relevant data fields
            var Div = await (from div in _context.MDiv
                               join cor in _context.MCorps on div.CorpsId equals cor.CorpsId
                               join Com in _context.MComd on div.ComdId equals Com.ComdId
                               where div.DivId!=1
                               select new DTODivResponse
                               {
                                   DivId = div.DivId,
                                   DivName = div.DivName,   
                                   CorpsId = cor.CorpsId,
                                   CorpsName = cor.CorpsName,
                                   ComdName = Com.ComdName,
                                   ComdId = Com.ComdId,
                               }
                             ).ToListAsync(); // Execute query asynchronously and return results as a list
            return Div; // Return the list of DIV data as DTO
        }


        /// <summary>
        /// Retrieves a list of divisions based on the provided command and corps IDs.
        /// </summary>
        /// <param name="Data">An instance of the <see cref="DTOParentChildIdRequest"/> containing the command ID and corps ID for filtering divisions.</param>
        /// <returns>
        /// A list of <see cref="DTODivResponse"/> containing division IDs and names that match the provided command and corps IDs.
        /// </returns>
        /// <remarks>
        /// This method is used to retrieve divisions within a specific corps and command, excluding the default division (with ID = 1).
        /// </remarks>
        public async Task<List<DTODivResponse>> GetByHId(DTOParentChildIdRequest Data)
        {
            // Query to retrieve divisions filtered by Command ID and Corps ID, excluding the default division with ID = 1
            var Div = await (from div in _context.MDiv
                             join cor in _context.MCorps on div.CorpsId equals cor.CorpsId
                             join d in _context.MComd
                             on cor.ComdId equals d.ComdId
                             where div.CorpsId == Data.CorpsId && div.ComdId==Data.ComdId && div.DivId!=1
                             select new DTODivResponse
                             {
                                 DivId = div.DivId,
                                 DivName = div.DivName,
                             }).ToListAsync();
            return Div;
        }


        /// <summary>
        /// Checks if a division (MDiv) is referenced in the `MBde` or `MapUnit` tables using the division's ID.
        /// This is used to prevent the deletion of a division if it is referenced in other tables.
        /// </summary>
        /// <param name="DivId">The ID of the division to check for foreign key references.</param>
        /// <returns>
        /// Returns a <see cref="DTODivIdCheckInFKTableResponse"/> object containing the counts of referenced records:
        /// - **TotalBde**: The number of references in the `MBde` table.
        /// - **TotalMapUnit**: The number of references in the `MapUnit` table.
        /// Returns `null` if an error occurs.
        /// </returns>
        /// <remarks>
        /// The method performs the following steps:
        /// 1. Executes an SQL query to check the number of records in the `MBde` and `MapUnit` tables that reference the specified division ID.
        /// 2. If the division is referenced in either table, it returns the counts for `TotalBde` and `TotalMapUnit`.
        /// 3. If an exception occurs, it logs the error and returns `null`.
        /// </remarks>
        public async Task<DTODivIdCheckInFKTableResponse?> DivIdCheckInFKTable(byte DivId)
        {
            try
            {
                // SQL query to check for references to the division ID in the MBde and MapUnit tables
                string query = "Select  count(distinct mbd.BdeId) as TotalBde ,count(distinct mapunit.UnitMapId) as TotalMapUnit from MDiv mdiv" +
                                " left join MBde mbd on mbd.DivId = mdiv.DivId " +
                                " left join MapUnit mapunit on mapunit.DivId = mdiv.DivId " +
                                " where mdiv.DivId = @DivId";

                // Execute the query using the database connection and return the result
                using (var connection = _contextDP.CreateConnection())
                {
                    var ret = await connection.QueryAsync<DTODivIdCheckInFKTableResponse>(query, new { DivId });
                    return ret.FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                // Log any errors that occur during the query execution
                _logger.LogError(1001, ex, "DivDB->DivIdCheckInFKTable");
                return null;  // Return null if an error occurs
            }
        }
        public async Task<DTODataTablesResponse<DTODivResponse>> GetAllDiv_Pagination(DTODataTablesRequest dTO)
        {
            string selectFields = "";
            string fromJoinClause = "";
            string whereClause = "";
            // Map allowed sort columns to DB fields
            Dictionary<string, string> allowedSortColumns = new Dictionary<string, string>();

            var sortOrder = dTO.sortDirection == "desc" ? "DESC" : "ASC";

            allowedSortColumns = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                ["DivName"] = "div.DivName",
                ["ComdName"] = "com.ComdName",
                ["CorpsName"] = "cor.CorpsName",
            };
            selectFields = @"div.DivId,div.DivName,cor.CorpsId,cor.CorpsName,com.ComdId,com.ComdName";
            fromJoinClause = @"from MDiv div
                                INNER JOIN MCorps cor on cor.CorpsId=div.CorpsId
                                INNER JOIN MComd com ON com.ComdId =div.ComdId";
            whereClause = @"WHERE
                                div.DivId <> 1
                                AND (
                                    div.DivName LIKE '%' + @SearchTerm + '%' OR
                                    cor.CorpsName LIKE '%' + @SearchTerm + '%' OR
                                    com.ComdName LIKE '%' + @SearchTerm + '%'
                                )";
            try
            {
                var sortColumn = allowedSortColumns.ContainsKey(dTO.sortColumn ?? "")
                ? allowedSortColumns[dTO.sortColumn!]
                : "div.DivId";
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
                    var records = (await ret.ReadAsync<DTODivResponse>()).ToList();
                    var totalFilteredRecords = records?.FirstOrDefault()?.TotalFilteredRecords;

                    var responseData = new DTODataTablesResponse<DTODivResponse>
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
                _logger.LogError(1001, ex, "DivDB->GetAllDiv_Pagination");
                List<DTODivResponse> dTODivs = new List<DTODivResponse>();
                var responseData = new DTODataTablesResponse<DTODivResponse>
                {
                    draw = 0,
                    recordsTotal = 0,
                    recordsFiltered = 0,
                    data = dTODivs
                };
                return responseData;
            }
        }
    }
}
