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
    /// <summary>
    /// Data Access Layer for MAppointment entity, providing database operations.
    /// and implements the IApptDB interface.
    /// and inherits from GenericRepositoryDL for basic CRUD operations.
    /// </summary>
    public class ApptDB : GenericRepositoryDL<MAppointment>, IApptDB
    {
        protected new readonly ApplicationDbContext _context;// For Entity Framework operations
        private readonly DapperContext _contextDP;// For Dapper operations
        private readonly ILogger<MAppointment> _logger;// For logging

        //constructor to initialize the ApptDB with necessary contexts and logger.and calls the base class constructor for basic CRUD operations.
        public ApptDB(ApplicationDbContext context, ILogger<MAppointment> logger, DapperContext contextDP) : base(context)
        {
            _context = context;
            _contextDP = contextDP;
            _logger = logger;
        }

        /// <summary>
        /// Asynchronously checks if any appointment exists with the same AppointmentName but a different ApptId.
        /// This ensures that there are no duplicate appointment names, while excluding the current appointment's own ApptId.
        /// </summary>
        /// <param name="Data">The MAppointment object containing the AppointmentName to check and the ApptId to exclude from the check.</param>
        /// <returns>
        /// Returns true if an appointment with the same AppointmentName but a different ApptId exists, otherwise false.
        /// </returns>
        public async Task<bool> GetByName(MAppointment Data)
        {
            // LINQ query using AnyAsync() to check if there is any record in the MAppointment table
            // where the AppointmentName matches the provided Data.AppointmentName (case-insensitive),
            // and the ApptId is different from the current appointment's ApptId (i.e., excluding the current record).
            var ret = await _context.MAppointment
                                    .AnyAsync(p => p.AppointmentName.ToUpper() == Data.AppointmentName.ToUpper() && p.ApptId != Data.ApptId);

            // Return true if a matching appointment is found, otherwise false.
            return ret;
        }


        /// <summary>
        /// Asynchronously retrieves all approved appointments from the MAppointment table, 
        /// and maps the data to a list of DTOAppointmentResponse objects.
        /// </summary>
        /// <returns>
        /// A list of DTOAppointmentResponse objects containing the details of all approved appointments.
        /// </returns>
        public async Task<List<DTOAppointmentResponse>> GetALLAppt()
        {
            var GetALL = await (from A in _context.MAppointment
                                select new DTOAppointmentResponse
                                {
                                    ApptId = A.ApptId,  
                                    AppointmentName = A.AppointmentName,  
                                    AppointmentAbbreviation = A.AppointmentAbbreviation,  
                                    Approved = A.Approved
                                })
                                .OrderByDescending(x => x.ApptId)  
                                .ToListAsync();  

            return GetALL;  // Return the list of appointment responses
        }

        /// <summary>
        /// Asynchronously retrieves a list of appointments whose names contain the provided AppointmentName, 
        /// and are approved. Limits the result to 5 appointments.
        /// </summary>
        /// <param name="AppointmentName">The name (or part of the name) of the appointment to filter by.</param>
        /// <returns>
        /// A list of up to 5 DTOAppointmentResponse objects containing the details of appointments whose names
        /// contain the specified AppointmentName and are approved.
        /// </returns>
        public async Task<List<DTOAppointmentResponse>> GetALLByAppointmentName(string AppointmentName)
        {
            try
            {
                // LINQ query to fetch appointments where the AppointmentName contains the provided AppointmentName
                // and where the appointment is approved (Approved = true).
                // It limits the result to the top 5 records.
                var GetALL = await (from A in _context.MAppointment
                              where A.AppointmentName.Contains(AppointmentName)  // Filters appointments by AppointmentName
                              && A.Approved == true  // Ensures the appointment is approved
                              select new DTOAppointmentResponse
                              {
                                  ApptId = A.ApptId,  // Selects the appointment ID
                                  AppointmentName = A.AppointmentName,  // Selects the appointment name
                              }).Take(5)  // Limits the results to 5 records
                              .ToListAsync();  // Executes the query and materializes the results into a list

                // Wrap the result in Task.FromResult to simulate async behavior
                return await Task.FromResult(GetALL);
            }
            catch (Exception ex)
            {
                // Logs the exception if an error occurs during the operation
                _logger.LogError(1001, ex, "ApptDB->GetALLByAppointmentName");
                return new List<DTOAppointmentResponse>();  // Returns an empty list in case of an error
            }
        }


        /// <summary>
        /// Retrieves a list of appointments based on the given FormationId. The method uses a LINQ query 
        /// to filter appointments by the FormationId and map the results to a list of DTOAppointmentResponse objects.
        /// </summary>
        /// <param name="FormationId">The FormationId used to filter appointments.</param>
        /// <returns>
        /// A Task that returns a list of DTOAppointmentResponse objects containing the details of appointments.
        /// </returns>
        public Task<List<DTOAppointmentResponse>> GetByFormationId(int FormationId)
        {
            // LINQ query to fetch appointments and map them to DTOAppointmentResponse.
            // The query is currently not using the join with MFormation (commented out), and FormationId is not used for filtering.
            var GetALL = (from A in _context.MAppointment
                          select new DTOAppointmentResponse
                          {
                              ApptId = A.ApptId,  // Selects the appointment ID
                              AppointmentName = A.AppointmentName,  // Selects the appointment name
                          }).ToList();  // Executes the query and materializes the results into a list

            // Return the result wrapped in a Task.
            return Task.FromResult(GetALL);
        }

        /// <summary>
        /// Asynchronously retrieves an appointment by its ApptId and maps the data to a DTOAppointmentResponse object.
        /// </summary>
        /// <param name="ApptId">The ApptId of the appointment to retrieve.</param>
        /// <returns>
        /// A DTOAppointmentResponse object containing the details of the appointment if found, otherwise null.
        /// </returns>
        public async Task<DTOAppointmentResponse?> GetByApptId(short ApptId)
        {
            try
            {
                // LINQ query to find the first appointment matching the provided ApptId.
                // Filters the MAppointment table to find the record with the specified ApptId.
                var GetAppt = await (from app in _context.MAppointment.Where(x => x.ApptId == ApptId)
                                     select new DTOAppointmentResponse
                                     {
                                         ApptId = app.ApptId,  // Selects the appointment ID
                                         AppointmentName = app.AppointmentName,  // Selects the appointment name
                                     }).FirstOrDefaultAsync();  // Retrieves the first matching record asynchronously

                // Return the result (either the found appointment or null if no match is found).
                return GetAppt;
            }
            catch (Exception ex)
            {
                // Logs the exception in case of an error.
                _logger.LogError(1001, ex, "ApptDB->GetByApptId");
                return null;  // Return null in case of an exception.
            }
        }
       
        /// <summary>
        /// Asynchronously checks if a given ApptId exists in the foreign key relationship in the TrnDomainMapping table.
        /// The method performs a SQL query to count the distinct occurrences of the ApptId in the TrnDomainMapping table,
        /// and returns the result as a DTOApptIdCheckInFKTableResponse object.
        /// </summary>
        /// <param name="ApptId">The ApptId to check for in the TrnDomainMapping table.</param>
        /// <returns>
        /// A DTOApptIdCheckInFKTableResponse object containing the count of distinct ApptId found in the TrnDomainMapping table.
        /// Returns null if an error occurs or no record is found.
        /// </returns>
        public async Task<DTOApptIdCheckInFKTableResponse?> ApptIdCheckInFKTable(short ApptId)
        {
            try
            {
                // SQL query to check if the given ApptId exists in the TrnDomainMapping table and counts the distinct ApptId values.
                string query = "Select count(distinct tdm.ApptId) as TotalTDM from MAppointment mapp " +
                               "left join TrnDomainMapping tdm on tdm.ApptId = mapp.ApptId " +
                               "where mapp.ApptId = @ApptId";

                // Using a database connection to execute the query asynchronously.
                using (var connection = _contextDP.CreateConnection())
                {
                    // Execute the query and retrieve the result as a DTOApptIdCheckInFKTableResponse object.
                    var ret = await connection.QueryAsync<DTOApptIdCheckInFKTableResponse>(query, new { ApptId });
                    // Return the first (and only) result or null if no records were found.
                    return ret.FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                // Logs the exception in case of an error.
                _logger.LogError(1001, ex, "ApptDB->ApptIdCheckInFKTable");
                // Return null in case of an exception.
                return null;
            }
        }
        public async Task<DTODataTablesResponse<DTOAppointmentResponse>> GetAllAppointment_Pagination(DTODataTablesRequest dTO)
        {
            string selectFields = "";
            string fromJoinClause = "";
            string whereClause = "";
            // Map allowed sort columns to DB fields
            Dictionary<string, string> allowedSortColumns = new Dictionary<string, string>();

            var sortOrder = dTO.sortDirection == "desc" ? "DESC" : "ASC";

            allowedSortColumns = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                ["AppointmentName"] = "app.AppointmentName",
                ["AppointmentAbbreviation"] = "app.AppointmentAbbreviation",
                ["Approved"] = "app.Approved",
                ["ApptId"] = "app.ApptId",
            };
            selectFields = @"app.ApptId,app.AppointmentName,app.AppointmentAbbreviation,app.Approved";
            fromJoinClause = @"from MAppointment app";
            whereClause = @"WHERE
                                (
                                    app.AppointmentName LIKE '%' + @SearchTerm + '%' OR
                                    app.AppointmentAbbreviation LIKE '%' + @SearchTerm + '%'
                                )";
            try
            {
                var sortColumn = allowedSortColumns.ContainsKey(dTO.sortColumn ?? "")
                ? allowedSortColumns[dTO.sortColumn!]
                : "app.ApptId";
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
                    var records = (await ret.ReadAsync<DTOAppointmentResponse>()).ToList();
                    var totalFilteredRecords = records?.FirstOrDefault()?.TotalFilteredRecords;

                    var responseData = new DTODataTablesResponse<DTOAppointmentResponse>
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
                _logger.LogError(1001, ex, "ApptDB->GetAllAppointment_Pagination");
                List<DTOAppointmentResponse> dTOAppointments = new List<DTOAppointmentResponse>();
                var responseData = new DTODataTablesResponse<DTOAppointmentResponse>
                {
                    draw = 0,
                    recordsTotal = 0,
                    recordsFiltered = 0,
                    data = dTOAppointments
                };
                return responseData;
            }
        }

    }
}