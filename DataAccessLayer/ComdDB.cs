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
    public class ComdDB : GenericRepositoryDL<MComd>, IComdDB
    {
        protected new readonly ApplicationDbContext _context;
        protected readonly DapperContext _contextDP;
        private readonly ILogger<ComdDB> _logger;
        public ComdDB(ApplicationDbContext context, DapperContext contextDP, ILogger<ComdDB> logger) : base(context)
        {
            _logger = logger;
            _context = context;
            _contextDP = contextDP;
        }

        
        /// <summary>
        /// Checks if a command (MComd) with the given name or abbreviation already exists in the database, excluding the current command by its ID.
        /// </summary>
        /// <param name="DTo">The command data transfer object containing the command name and abbreviation to check against the database.</param>
        /// <returns>
        /// Returns <c>true</c> if a command with the same name or abbreviation exists, excluding the current command (based on <paramref name="DTo.ComdId"/>).
        /// Returns <c>false</c> if no matching command is found.
        /// </returns>
        /// <remarks>
        /// This method performs a case-insensitive check for both the command name and abbreviation. It excludes the current command from the check by comparing the command ID.
        /// </remarks>
        public async Task<bool> GetByName(MComd DTo)
        {
            // Querying the database to check if any command with the same name or abbreviation exists, excluding the current command by its ID
            var ret = await _context.MComd.AnyAsync(p =>( p.ComdName.ToUpper() == DTo.ComdName.ToUpper() || p.ComdAbbreviation.ToUpper() == DTo.ComdAbbreviation.ToUpper()) && p.ComdId != DTo.ComdId);
            return ret; // Returns true if a duplicate is found, otherwise false
        }


        /// <summary>
        /// Retrieves the maximum 'Orderby' value from the MComd table and increments it by 1.
        /// </summary>
        /// <returns>
        /// The next available 'Orderby' value, which is the maximum 'Orderby' value plus one.
        /// </returns>
        /// <remarks>
        /// This method calculates the next available order value for use when creating or updating records in the MComd table.
        /// It helps in maintaining sequential order values for the 'Orderby' column.
        /// </remarks>
        public async Task<int> GetByMaxOrder()
        {
            // Fetch the maximum 'Orderby' value from the MComd table
            int ret = await _context.MComd.MaxAsync(P => P.Orderby);
            // Return the next order value, incremented by 1
            return ret + 1;
        }


        /// <summary>
        /// Retrieves the command ID (<see cref="MComd.ComdId"/>) for a given order number (<paramref name="OrderBy"/>).
        /// </summary>
        /// <param name="OrderBy">The order number to search for in the <see cref="MComd"/> table.</param>
        /// <returns>
        /// Returns the command ID (<see cref="MComd.ComdId"/>) if the order number exists, or zero if no matching record is found.
        /// </returns>
        /// <remarks>
        /// This method performs a query to the database to find the <see cref="MComd.ComdId"/> for the provided order number (<paramref name="OrderBy"/>).
        /// It selects the command ID based on the specified order number and returns the first result found. 
        /// If no matching command is found, it returns zero.
        /// </remarks>
        public async Task<byte> GetComdIdbyOrderby(int OrderBy)
        {
            // Query the database to find the command ID for the provided order number
            var ret = await _context.MComd.Where(P => P.Orderby == OrderBy).Select(c=>c.ComdId).FirstOrDefaultAsync(); 
           
            return ret;// Return the command ID, or zero if not found
        }


        /// <summary>
        /// Retrieves all commands (MComd) from the database, ordered by their specified 'Orderby' field.
        /// </summary>
        /// <returns>
        /// Returns an IEnumerable collection of MComd objects sorted by the 'Orderby' field in ascending order.
        /// </returns>
        /// <remarks>
        /// This method performs an asynchronous query to fetch all commands from the MComd table, sorted by their 'Orderby' field.
        /// It uses Entity Framework's `OrderBy` method to ensure that the commands are returned in the correct order based on the 'Orderby' value.
        /// </remarks>
        public async Task<IEnumerable<MComd>> GetAllByorder()
        {
            var ret = await _context.MComd.OrderBy(x => x.Orderby).ToListAsync();
            return ret;
        }


        /// <summary>
        /// Retrieves the binary tree structure related to a command, including its associated corps, divisions, brigades, and units.
        /// </summary>
        /// <param name="Id">The ID of the command for which the binary tree structure is being retrieved. This ID corresponds to the <see cref="MComd.ComdId"/>.</param>
        /// <returns>
        /// A <see cref="DTOTreeViewUnitResponse"/> object containing the binary tree structure, including the command details, corps, divisions, brigades, and units.
        /// If an error occurs during the process, it returns <c>null</c>.
        /// </returns>
        /// <remarks>
        /// This method executes multiple SQL queries to fetch related data for the given <paramref name="Id">command ID</paramref>. The method retrieves:
        /// - Command details from <c>MComd</c>
        /// - Corps details from <c>MCorps</c>
        /// - Division details from <c>MDiv</c>
        /// - Brigade details from <c>MBde</c>
        /// - Unit details from <c>MapUnit</c> and <c>MUnit</c>
        /// The method returns a <see cref="DTOTreeViewUnitResponse"/> object containing the retrieved data in separate lists.
        /// </remarks>
        public async Task<DTOTreeViewUnitResponse> GetBinaryTree(int Id)
        {
            try
            {

                string query = "Select ComdId,ComdName from MComd  where ComdId=@Id";
                string MCorps = " Select ComdId,CorpsId,CorpsName from MCorps  where ComdId=@Id";
                string MDiv = " Select CorpsId,DivId,DivName from MDiv  where ComdId=@Id";
                string MBde = " Select CorpsId,DivId,BdeId,BdeName from MBde  where ComdId=@Id";
                string MapUnit = " Select UnitMapId UnitId,ComdId,CorpsId,DivId,BdeId,UnitName from MapUnit inner join MUnit on MapUnit.UnitId=MUnit.UnitId  where ComdId=@Id";

                // Using Dapper to execute the queries and fetch the data asynchronously
                using (var connection = _contextDP.CreateConnection())
                {
                    // Execute queries and store results in respective variables
                    var ret = await connection.QueryAsync<MComd>(query, new { Id });
                    var ret1 = await connection.QueryAsync<MCorps>(MCorps, new { Id });
                    var ret2 = await connection.QueryAsync<MDiv>(MDiv, new { Id });
                    var ret3 = await connection.QueryAsync<MBde>(MBde, new { Id });
                    var ret4 = await connection.QueryAsync<DTOMapUnitResponse>(MapUnit, new { Id });
                    

                    DTOTreeViewUnitResponse dTOTreeViewUnitResponse = new DTOTreeViewUnitResponse();

                    dTOTreeViewUnitResponse.MComd  = (List<MComd>)ret;
                    dTOTreeViewUnitResponse.MCorps = (List<MCorps>)ret1;
                    dTOTreeViewUnitResponse.MDiv   = (List<MDiv>)ret2;
                    dTOTreeViewUnitResponse.MBde   = (List<MBde>)ret3;
                    dTOTreeViewUnitResponse.Unit   = (List<DTOMapUnitResponse>)ret4;

                    // Return the populated response object
                    return dTOTreeViewUnitResponse;

                }
            }
            catch (Exception ex)
            {
                _logger.LogError(1001, ex, "ComdDB->GetBinaryTree");
                return null;
            }
        }


        /// <summary>
        /// Checks if the given command (MComd) is referenced in any foreign key tables (Corps, Bde, Div, MapUnit).
        /// </summary>
        /// <param name="ComdId">The ID of the command (MComd) to check for references in foreign key tables.</param>
        /// <returns>
        /// Returns a <see cref="DTOComdIdCheckInFKTableResponse"/> containing counts of references in the related tables (Corps, Bde, Div, MapUnit).
        /// If the command is not referenced in any of these tables, the counts will be zero.
        /// Returns <c>null</c> if an error occurs during the query execution.
        /// </returns>
        /// <remarks>
        /// This method performs a SQL query that joins the `MComd` table with the `MCorps`, `MBde`, `MDiv`, and `MapUnit` tables,
        /// counting the number of distinct references in each of these tables where the command ID is present.
        /// The result is returned as a <see cref="DTOComdIdCheckInFKTableResponse"/> object.
        /// If the command is referenced in any of these tables, the corresponding count values will be greater than zero.
        /// </remarks>
        public async Task<DTOComdIdCheckInFKTableResponse?> ComdIdCheckInFKTable(byte ComdId)
        {
            try
            {
                string query = "Select  count(distinct mcor.CorpsId) as TotalCorps,count(distinct mbd.BdeId) as TotalBde ,count(distinct mdiv.DivId) as TotalDiv,count(distinct mapunit.UnitMapId) as TotalMapUnit from MComd mcom"+
                                " left join MCorps mcor on mcor.ComdId = mcom.ComdId " +
                                " left join MBde mbd on mbd.ComdId = mcom.ComdId " +
                                " left join MDiv mdiv on mdiv.ComdId = mcom.ComdId " +
                                " left join MapUnit mapunit on mapunit.ComdId = mcom.ComdId " +
                                " where mcom.ComdId = @ComdId";

                using (var connection = _contextDP.CreateConnection())
                {
                    var ret = await connection.QueryAsync<DTOComdIdCheckInFKTableResponse>(query, new { ComdId });
                    return ret.FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(1001, ex, "ComdDB->ComdIdCheckInFKTable");
                return null;
            }
        }
        public async Task<DTODataTablesResponse<DTOAllCommand_PaginationResponse>> GetAllCommand_Pagination(DTODataTablesRequest dTO)
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
                ["ComdAbbreviation"] = "com.ComdAbbreviation",
                ["Orderby"] = "com.Orderby",
            };
            selectFields = @"com.ComdId,com.ComdName,com.ComdAbbreviation,com.Orderby,com.IsActive";
            fromJoinClause = @"from MComd com";
            whereClause = @"WHERE
                                com.ComdId <> 1
                                AND (
                                    com.ComdName LIKE '%' + @SearchTerm + '%' OR
                                    com.ComdAbbreviation LIKE '%' + @SearchTerm + '%'
                                )";
            try
            {
                var sortColumn = allowedSortColumns.ContainsKey(dTO.sortColumn ?? "")
                ? allowedSortColumns[dTO.sortColumn!]
                : "com.ComdId";
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
                    var records = (await ret.ReadAsync<DTOAllCommand_PaginationResponse>()).ToList();
                    var totalFilteredRecords = records?.FirstOrDefault()?.TotalFilteredRecords;

                    var responseData = new DTODataTablesResponse<DTOAllCommand_PaginationResponse>
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
                _logger.LogError(1001, ex, "BasicDetailDB->GetAllDispatchCard");
                List<DTOAllCommand_PaginationResponse> dTOAllCommands = new List<DTOAllCommand_PaginationResponse>();
                var responseData = new DTODataTablesResponse<DTOAllCommand_PaginationResponse>
                {
                    draw = 0,
                    recordsTotal = 0,                                               
                    recordsFiltered = 0,
                    data = dTOAllCommands
                };
                return responseData;
            }
        }
    }
}