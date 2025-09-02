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


        public async Task<List<DTOAppointmentResponse>> GetALLAppt()
        {
            var GetALL = await (from A in _context.MAppointment
                                where A.Approved == 1
                                //join F in _context.MFormation
                                //on A.FormationId equals F.FormationId

                                select new DTOAppointmentResponse
                         {
                             ApptId=A.ApptId,
                             AppointmentName=A.AppointmentName,
                             AppointmentAbbreviation=A.AppointmentAbbreviation,
                             //FormationId=F.FormationId,
                             //FormationName=F.FormationName, 
                         }).OrderByDescending(x=>x.ApptId).ToListAsync();


            return GetALL;
        }
        public async Task<List<DTOAppointmentResponse>> GetALLByAppointmentName(string AppointmentName)
        {
            try
            {
                var GetALL = (from A in _context.MAppointment
                              where A.AppointmentName.Contains(AppointmentName)
                              && A.Approved == 1
                              select new DTOAppointmentResponse
                              {
                                  ApptId = A.ApptId,
                                  AppointmentName = A.AppointmentName,
                              }).Take(5).ToList();
                return await Task.FromResult(GetALL);
            }
            catch(Exception ex)
            {
                _logger.LogError(1001, ex, "ApptDB->GetALLByAppointmentName");
                return new List<DTOAppointmentResponse>();
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