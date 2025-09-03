using Dapper;
using DataAccessLayer.BaseInterfaces;
using DataAccessLayer.Logger;
using DataTransferObject.Domain.Master;
using DataTransferObject.Response;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace DataAccessLayer
{
    /// <summary>
    /// Data Access Layer for ArmedType entity, providing database operations.
    /// and implements the IArmedDB interface.
    /// for basic CRUD operations.
    /// </summary>
    public class ArmedDB : GenericRepositoryDL<MArmedType>, IArmedDB
    {
        protected new readonly ApplicationDbContext _context;// For Entity Framework operations
        private readonly DapperContext _contextDP;// For Dapper operations
        private readonly ILogger<ArmedDB> _logger;// For logging
        public ArmedDB(ApplicationDbContext context, ILogger<ArmedDB> logger, DapperContext contextDP) : base(context)
        {
            _context = context;
            _contextDP = contextDP;
            _logger = logger;
        }

        /// <summary>
        /// Asynchronously checks if any armed type exists with the same ArmedName or Abbreviation but a different ArmedId.
        /// This ensures that there are no duplicates in either ArmedName or Abbreviation, while excluding the current armed type's own ArmedId.
        /// </summary>
        /// <param name="DTo">The MArmedType object containing the ArmedName, Abbreviation, and ArmedId to check.</param>
        /// <returns>
        /// Returns true if a record with the same ArmedName or Abbreviation but a different ArmedId exists, otherwise false.
        /// </returns>
        public async Task<bool> GetByName(MArmedType DTo)
        {
            // LINQ query using AnyAsync() to check if there is any record in the MArmedType table
            // where the ArmedName or Abbreviation matches the provided DTo values (case-insensitive),
            // and the ArmedId is different from the current armed type's ArmedId (i.e., excluding the current record).
            var ret = await _context.MArmedType
                                    .AnyAsync(x => (x.ArmedName.ToUpper() == DTo.ArmedName.ToUpper() ||
                                                     x.Abbreviation.ToUpper() == DTo.Abbreviation.ToUpper()) &&
                                                     x.ArmedId != DTo.ArmedId);

            // Return true if a matching armed type is found, otherwise false.
            return ret;
        }

        /// <summary>
        /// Retrieves all armed types and their associated category information, mapping the data to a list of DTOArmedResponse objects.
        /// The method performs a join between the MArmedType and MArmedCats tables to fetch the related category details.
        /// </summary>
        /// <returns>
        /// A list of DTOArmedResponse objects containing the details of all armed types and their corresponding categories, ordered by ArmedId in descending order.
        /// </returns>
        public Task<List<DTOArmedResponse>> GetALLArmed()
        {
            // LINQ query to fetch armed types along with their associated category details by joining MArmedType and MArmedCats.
            // The results are mapped to DTOArmedResponse objects, which include ArmedId, ArmedName, Abbreviation, FlagInf, and category details.
            var GetALL = (from A in _context.MArmedType
                          join F in _context.MArmedCats
                          on A.ArmedCatId equals F.ArmedCatId  // Join MArmedType and MArmedCats on ArmedCatId
                          select new DTOArmedResponse
                          {
                              ArmedId = A.ArmedId,  // Selects the ArmedId from MArmedType
                              ArmedName = A.ArmedName,  // Selects the ArmedName from MArmedType
                              Abbreviation = A.Abbreviation,  // Selects the Abbreviation from MArmedType
                              FlagInf = A.FlagInf,  // Selects the FlagInf (Flag information) from MArmedType
                              Inf = A.FlagInf == true ? "Yes" : "No",  // Maps FlagInf boolean to "Yes" or "No"
                              ArmedCatId = F.ArmedCatId,  // Selects the ArmedCatId from MArmedCats
                              Name = F.Name,  // Selects the Name from MArmedCats (Armed Category name)
                          })
                          .OrderByDescending(x => x.ArmedId)  // Orders the results by ArmedId in descending order
                          .ToList();  // Executes the query and converts the results to a list

            // Returns the result as a Task (simulating asynchronous behavior).
            return Task.FromResult(GetALL);
        }

        /// <summary>
        /// Asynchronously checks if the given ArmedId exists in the foreign key relationships in the BasicDetails and MRecordOffice tables.
        /// The method performs a SQL query to count the distinct occurrences of BasicDetailId and RecordOfficeId for the given ArmedId.
        /// </summary>
        /// <param name="ArmedId">The ArmedId to check for in the BasicDetails and MRecordOffice tables.</param>
        /// <returns>
        /// A DTOArmedIdCheckInFKTableResponse object containing the counts of distinct BasicDetailId and RecordOfficeId, or null if an error occurs.
        /// </returns>
        public async Task<DTOArmedIdCheckInFKTableResponse?> ArmedIdCheckInFKTable(byte ArmedId)
        {
            try
            {
                // SQL query to count the distinct BasicDetailId and RecordOfficeId for the given ArmedId.
                string query = "Select count(distinct bd.BasicDetailId) as TotalBD, count(mrec.RecordOfficeId) as TotalRO from MArmedType marm " +
                               "left join BasicDetails bd on bd.ArmedId = marm.ArmedId " +
                               "left join MRecordOffice mrec on mrec.ArmedId = marm.ArmedId " +
                               "where marm.ArmedId = @ArmedId";

                // Using a database connection to execute the query asynchronously.
                using (var connection = _contextDP.CreateConnection())
                {
                    // Execute the query and retrieve the result as a DTOArmedIdCheckInFKTableResponse object.
                    var ret = await connection.QueryAsync<DTOArmedIdCheckInFKTableResponse>(query, new { ArmedId });

                    // Return the first (and only) result, or null if no records are found.
                    return ret.FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                // Logs the exception in case of an error.
                _logger.LogError(1001, ex, "ArmedDB->ArmedIdCheckInFKTable");
                return null;  // Return null in case of an exception.
            }
        }

    }
}