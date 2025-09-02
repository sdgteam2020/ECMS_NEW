using DataAccessLayer.BaseInterfaces;
using DataTransferObject.Domain.Master;
using DataTransferObject.Requests;
using DataTransferObject.Response;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using DataAccessLayer.Logger;
using Dapper;

namespace DataAccessLayer
{
    public class UnitDB : GenericRepositoryDL<MUnit>, IUnitDB
    {
        protected new readonly ApplicationDbContext _context;
        private readonly DapperContext _contextDP;
        private readonly ILogger<UnitDB> _logger;
        public UnitDB(ApplicationDbContext context, ILogger<UnitDB> logger, DapperContext contextDP) : base(context)
        {
            _context = context;
            _logger = logger;
            _contextDP = contextDP;
        }

        /// <summary>
        /// Checks if a unit with the specified name already exists in the database, excluding the current unit (based on UnitId).
        /// </summary>
        /// <param name="Data">The MUnit object containing the unit name to check.</param>
        /// <returns>
        /// A boolean value indicating whether a unit with the specified name already exists (excluding the current unit).
        /// Returns <c>true</c> if the unit name exists in the database for another unit; otherwise, <c>false</c>.
        /// </returns>
        /// <remarks>
        /// This method performs a case-insensitive comparison of the unit name in the database and checks if any unit with the same name
        /// already exists, excluding the current unit's ID. It is useful for ensuring that unit names are unique when adding or updating records.
        /// </remarks>
        public async Task<bool> GetByName(MUnit Data)
        {
            // Retrieve all units from the database without tracking them in memory (for performance reasons).
            List<MUnit> mUnits = await _context.MUnit.AsNoTracking().ToListAsync();

            // Check if any unit already exists with the same name (case-insensitive) and a different UnitId.
            var ret = mUnits.Any(p => p.UnitName.ToUpper() == Data.UnitName.ToUpper() && p.UnitId != Data.UnitId);

            // Return the result of the check (true if a duplicate unit name exists, false otherwise).
            return ret;
        }

        /// <summary>
        /// Checks if a unit with the specified SUS number (concatenating the Sus_no and Suffix) exists in the database.
        /// </summary>
        /// <param name="Sus_no">The SUS number (combination of Sus_no and Suffix) to check for in the database.</param>
        /// <returns>
        /// A boolean value indicating whether a unit with the given SUS number exists in the database.
        /// Returns <c>true</c> if the SUS number exists; otherwise, <c>false</c>.
        /// </returns>
        /// <remarks>
        /// This method concatenates the `Sus_no` and `Suffix` properties from the `MUnit` table and performs a case-insensitive comparison
        /// with the provided SUS number. It ensures that the SUS number, which is a unique identifier for units, is checked for existence
        /// in the database.
        /// </remarks>
        public async Task<bool> FindSusNo(string Sus_no)
        {
            // Perform a case-insensitive check to see if any unit's Sus_no concatenated with its Suffix matches the provided SUS number.
            var ret = _context.MUnit.Any(x => (x.Sus_no.ToUpper() + x.Suffix.ToUpper()) == Sus_no.ToUpper());

            // Return true if the SUS number exists, otherwise false.
            return ret;
        }


        /// <summary>
        /// Retrieves a unit from the database based on the provided SUS number, which is a combination of Sus_no and Suffix.
        /// </summary>
        /// <param name="Sus_no">The SUS number (combination of Sus_no and Suffix) to look up in the database.</param>
        /// <returns>
        /// A unit (`MUnit`) object that matches the given SUS number, or <c>null</c> if no matching unit is found.
        /// </returns>
        /// <remarks>
        /// This method concatenates the `Sus_no` and `Suffix` properties from the `MUnit` table and performs a case-insensitive comparison
        /// to find the matching unit. It returns the first matching unit or <c>null</c> if no match is found.
        /// </remarks>
        public async Task<MUnit?> GetBySusNo(string Sus_no)
        {
            try
            {
                // Retrieve the first unit that matches the concatenated Sus_no and Suffix (case-insensitive).
                return await _context.MUnit
                                     .Where(x => (x.Sus_no.ToUpper() + x.Suffix.ToUpper()) == Sus_no)
                                     .FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                // Log any exceptions that occur during the database operation
                _logger.LogError(1001, ex, "UnitDB->GetBySusNo");

                // Return null if an exception occurs
                return null;
            }
        }

        /// <summary>
        /// Checks if a unit exists with the specified SUS number (combination of Sus_no and Suffix) 
        /// that does not have the given UnitId.
        /// </summary>
        /// <param name="Sus_no">The SUS number (combination of Sus_no and Suffix) to check in the database.</param>
        /// <param name="UnitId">The UnitId to exclude from the search results.</param>
        /// <returns>
        /// A nullable boolean indicating whether a unit with the specified SUS number exists, but has a different UnitId.
        /// Returns <c>null</c> if an exception occurs.
        /// </returns>
        /// <remarks>
        /// This method performs a case-insensitive search for a unit with the provided SUS number 
        /// and checks if the UnitId is different from the provided value. 
        /// It returns <c>true</c> if such a unit exists, otherwise returns <c>false</c>.
        /// </remarks>
        public async Task<bool?> GetBySusNoWithUnitId(string Sus_no, int UnitId)
        {
            try
            {
                // Check if there exists any unit with the given SUS number and a different UnitId
                return await _context.MUnit
                                     .AnyAsync(x => (x.Sus_no.ToUpper() + x.Suffix.ToUpper()) == Sus_no && x.UnitId != UnitId);
            }
            catch (Exception ex)
            {
                // Log any exceptions that occur during the database operation
                _logger.LogError(1001, ex, "UnitDB->GetBySusNoWithUnitId");

                // Return null if an exception occurs
                return null;
            }
        }

        /// <summary>
        /// Retrieves all units from the database, applying filtering, sorting, and pagination based on the provided request.
        /// </summary>
        /// <param name="request">The data table request containing the search value, sorting column, and pagination information.</param>
        /// <returns>
        /// A <see cref="DTODataTablesResponse{MUnit}"/> containing the filtered and paginated list of units, along with total records and filtered records counts.
        /// </returns>
        /// <remarks>
        /// This method is used to retrieve unit data for display in a DataTable, supporting search, sorting, and pagination. 
        /// The data is fetched from the <c>MUnit</c> table, with the results filtered based on the search value 
        /// and sorted by the specified column in either ascending or descending order. Pagination is applied to limit the number of results returned.
        /// </remarks>
        public async Task<DTODataTablesResponse<MUnit>> GetAllUnit(DTODataTablesRequest request)
        {
            try
            {
                var queryableData = (from u in _context.MUnit.OrderByDescending(x => x.UnitId)
                                     select new MUnit()
                                     {
                                         UnitId = u.UnitId,
                                         Sus_no = u.Sus_no,
                                         Suffix = u.Suffix,
                                         UnitName = u.UnitName,
                                         Abbreviation = u.Abbreviation,
                                         IsVerify = u.IsVerify,
                                     }).AsQueryable();
                // Total records without filtering
                var totalRecords = queryableData.Count();


                // Apply filtering
                if (!string.IsNullOrEmpty(request.searchValue))
                {
                    string searchValue = request.searchValue.ToLower();
                    queryableData = queryableData.Where(x => x.Sus_no.ToLower().Contains(searchValue));
                }

                // Apply sorting

                if (!string.IsNullOrEmpty(request.sortColumn) && !string.IsNullOrEmpty(request.sortDirection))
                {
                    if (request.sortColumn == "UnitName" || request.sortColumn == "Abbreviation")
                    {

                    }
                    else 
                    {
                        //queryableData = queryableData.OrderBy(request.SortColumn + " " + request.SortColumnDirection);
                        queryableData = request.sortDirection.ToLower() == "asc"
                        ? queryableData.OrderBy(item => EF.Property<object>(item, request.sortColumn))
                        : queryableData.OrderByDescending(item => EF.Property<object>(item, request.sortColumn));
                    }

                }

                // Total records after filtering
                var filteredRecords = queryableData.Count();

                // Paginate the result
                var paginatedData = await queryableData.Skip(request.Start).Take(request.Length).ToListAsync();

                var responseData = new DTODataTablesResponse<MUnit>
                {
                    draw = request.Draw,
                    recordsTotal = totalRecords, // Total records without filtering
                    recordsFiltered = filteredRecords, // Total records after filtering
                    data = paginatedData
                };
                return responseData;
            }
            catch (Exception ex)
            {
                _logger.LogError(1001, ex, "UnitDB->GetAllUnit_");
                List<MUnit> dTOUserRegnResponses = new List<MUnit>();
                var responseData = new DTODataTablesResponse<MUnit>
                {
                    draw = 0,
                    recordsTotal = 0,
                    recordsFiltered = 0,
                    data = dTOUserRegnResponses
                };
                return responseData;
            }
        }


        /// <summary>
        /// Retrieves the top unit information that matches the provided SUSNo (Service Unit Serial Number).
        /// </summary>
        /// <param name="SUSNo">The SUSNo (Service Unit Serial Number) to search for in the unit database.</param>
        /// <returns>
        /// A list of DTOUnitResponse objects containing unit details that match the SUSNo.
        /// Returns null if an error occurs during the query execution.
        /// </returns>
        /// <remarks>
        /// This method queries the database for units where the combined SUSNo and Suffix match the provided SUSNo.
        /// The result is limited to the top 5 matching units. The method also handles exceptions and logs errors.
        /// </remarks>
        public async Task<List<DTOUnitResponse>?> GetTopBySUSNo(string SUSNo)
        {
            try
            {
                // Query to find units where the combined SUSNo and Suffix match the input SUSNo
                var Unit = await (from unit in _context.MUnit.Where(x => (x.Sus_no + x.Suffix).Contains(SUSNo))
                                  select new DTOUnitResponse
                                  {
                                      UnitId = unit.UnitId,
                                      Sus_no = unit.Sus_no + unit.Suffix,
                                      UnitName = unit.UnitName,
                                      Abbreviation = unit.Abbreviation,
                                      IsVerify = unit.IsVerify,
                                  }).Take(5).ToListAsync();

                // Return the list of matching units
                return Unit;
            }
            catch (Exception ex)
            {
                // Log the error in case of an exception
                _logger.LogError(1001, ex, "UnitDB->GetTopBySUSNo");

                // Return null in case of an error
                return null;
            }
        }


        /// <summary>
        /// Checks if the provided UnitId exists in foreign key references within the MapUnit table.
        /// </summary>
        /// <param name="UnitId">The UnitId to check for foreign key references in the MapUnit table.</param>
        /// <returns>
        /// A DTOUnitIdCheckInFKTableResponse object containing the count of records in the MapUnit table 
        /// that are associated with the provided UnitId.
        /// Returns null if an error occurs during the query execution.
        /// </returns>
        /// <remarks>
        /// This method checks the `MapUnit` table for records linked to the provided `UnitId` by counting 
        /// the number of `UnitMapId` associated with the `UnitId` in the `MUnit` table. 
        /// It helps determine if there are any existing foreign key relationships involving the unit.
        /// </remarks>
        public async Task<DTOUnitIdCheckInFKTableResponse?> UnitIdCheckInFKTable(int UnitId)
        {
            try
            {
                // SQL query to check the foreign key references in the MapUnit table
                string query = @"Select  count(mapunit.UnitMapId) as TotalMapUnit from MUnit munit
                                left join MapUnit mapunit on mapunit.UnitId = munit.UnitId 
                                where munit.UnitId = @UnitId";

                // Create a connection and execute the query
                using (var connection = _contextDP.CreateConnection())
                {
                    // Execute the query and get the result
                    var ret = await connection.QueryAsync<DTOUnitIdCheckInFKTableResponse>(query, new { UnitId });

                    // Return the first result (or null if no matching records found)
                    return ret.FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                // Log any errors that occur during the execution of the query
                _logger.LogError(1001, ex, "UnitDB->UnitIdCheckInFKTable");

                // Return null if an error occurs
                return null;
            }
        }
    }
}