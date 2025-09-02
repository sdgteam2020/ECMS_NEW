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
            // LINQ query to fetch all appointments from the MAppointment table where Approved = 1.
            // The query selects specific fields and maps them to a DTOAppointmentResponse object.
            var GetALL = await (from A in _context.MAppointment
                                where A.Approved == 1  // Filters appointments that are approved
                                                       // Join with MFormation table (commented out for now)
                                                       // on A.FormationId equals F.FormationId
                                select new DTOAppointmentResponse
                                {
                                    ApptId = A.ApptId,  // Selects the appointment ID
                                    AppointmentName = A.AppointmentName,  // Selects the appointment name
                                    AppointmentAbbreviation = A.AppointmentAbbreviation,  // Selects the appointment abbreviation
                                                                                          // FormationId = F.FormationId,  // Formation ID from MFormation table (currently commented out)
                                                                                          // FormationName = F.FormationName,  // Formation Name from MFormation table (currently commented out)
                                })
                                .OrderByDescending(x => x.ApptId)  // Orders the appointments by ApptId in descending order
                                .ToListAsync();  // Executes the query asynchronously and returns the result as a list

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
                // and where the appointment is approved (Approved = 1).
                // It limits the result to the top 5 records.
                var GetALL = (from A in _context.MAppointment
                              where A.AppointmentName.Contains(AppointmentName)  // Filters appointments by AppointmentName
                              && A.Approved == 1  // Ensures the appointment is approved
                              select new DTOAppointmentResponse
                              {
                                  ApptId = A.ApptId,  // Selects the appointment ID
                                  AppointmentName = A.AppointmentName,  // Selects the appointment name
                              }).Take(5)  // Limits the results to 5 records
                              .ToList();  // Executes the query and materializes the results into a list

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


        public Task<List<DTOAppointmentResponse>> GetByFormationId(int FormationId)
        {
            var GetALL = (from A in _context.MAppointment
                          //join F in _context.MFormation
                          //on A.FormationId equals F.FormationId
                         // where F.FormationId == FormationId
                          select new DTOAppointmentResponse
                          {
                              ApptId = A.ApptId,
                              AppointmentName = A.AppointmentName,
                             // FormationId = F.FormationId,
                             // FormationName = F.FormationName,


                          }).ToList();

            return Task.FromResult(GetALL);
        }
        public async Task<DTOAppointmentResponse?> GetByApptId(short ApptId)
        {
            try
            {
                var GetAppt = await (from app in _context.MAppointment.Where(x => x.ApptId == ApptId)
                               select new DTOAppointmentResponse
                               {
                                   ApptId = app.ApptId,
                                   AppointmentName = app.AppointmentName,
                               }).FirstOrDefaultAsync();

                return GetAppt;
            }
            catch (Exception ex)
            {
                _logger.LogError(1001, ex, "ApptDB->GetByApptId");
                return null;
            }
        }

        //public async Task<bool> GetByName(MCorps Data)
        //{
        //    var ret = _context.MCorps.Where(p=> p.ComdId == Data.ComdId).Select(p => p.CorpsName.ToUpper() == Data.CorpsName.ToUpper()).FirstOrDefault();
        //    return ret;
        //}

        //public Task<List<DTOCorpsResponse>> GetALLCorps()
        //{
        //    var Corps = (from c in _context.MCorps
        //                 join d in _context.MComd
        //                 on c.ComdId equals d.ComdId
        //                 where c.CorpsId!=1
        //                 select new DTOCorpsResponse
        //                 {

        //                     CorpsId = c.CorpsId,
        //                     CorpsName = c.CorpsName,
        //                     comdName = d.ComdName,
        //                     ComdId=d.ComdId,

        //                 }).ToList();


        //    return Task.FromResult(Corps);  
        //}

        //public async Task<List<DTOCorpsResponse>> GetByComdId(int ComdId)
        //{
        //    var Corps = (from c in _context.MCorps
        //                 join d in _context.MComd
        //                 on c.ComdId equals d.ComdId where c.ComdId == ComdId   
        //                 select new DTOCorpsResponse
        //                 {

        //                     CorpsId = c.CorpsId,
        //                     CorpsName = c.CorpsName,



        //                 }).ToList();


        //    return await Task.FromResult(Corps);
        //}



        //public UserDB(IConfiguration configuration)
        //{
        //    this.configuration = configuration;
        //}
        public async Task<DTOApptIdCheckInFKTableResponse?> ApptIdCheckInFKTable(short ApptId)
        {
            try
            {
                string query = "Select count(distinct tdm.ApptId)as TotalTDM from MAppointment mapp" +
                                " left join TrnDomainMapping tdm on tdm.ApptId = mapp.ApptId " +
                                " where mapp.ApptId =@ApptId";

                using (var connection = _contextDP.CreateConnection())
                {
                    var ret = await connection.QueryAsync<DTOApptIdCheckInFKTableResponse>(query, new { ApptId });
                    return ret.FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(1001, ex, "ApptDB->ApptIdCheckInFKTable");
                return null;
            }
        }
    }
}