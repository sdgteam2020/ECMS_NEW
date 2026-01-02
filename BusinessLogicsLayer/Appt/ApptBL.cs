using DataAccessLayer;
using DataAccessLayer.BaseInterfaces;
using DataTransferObject.Domain.Master;
using DataTransferObject.Requests;
using DataTransferObject.Response;


namespace BusinessLogicsLayer.Appt
{
    internal class ApptBL : GenericRepositoryDL<MAppointment>, IApptBL
    {
        private readonly IApptDB _apptDB;

        public ApptBL(ApplicationDbContext context, IApptDB apptDB) : base(context)
        {
            _apptDB = apptDB;
        }

        public async Task<List<DTOAppointmentResponse>> GetALLAppt()
        {
            return await _apptDB.GetALLAppt();
        }
        public Task<List<DTOAppointmentResponse>> GetByFormationId(int FormationId)
        {
            return _apptDB.GetByFormationId(FormationId);
        }

        public Task<bool> GetByName(MAppointment Data)
        {
            return _apptDB.GetByName(Data);
        }
        public async Task<List<DTOAppointmentResponse>> GetALLByAppointmentName(string AppointmentName)
        {
            return await _apptDB.GetALLByAppointmentName(AppointmentName);
        }
        public async Task<DTOAppointmentResponse?> GetByApptId(short ApptId)
        {
            return await _apptDB.GetByApptId(ApptId);
        }
        public async Task<DTOApptIdCheckInFKTableResponse?> ApptIdCheckInFKTable(short ApptId)
        {
            return await _apptDB.ApptIdCheckInFKTable(ApptId);
        }
        public async Task<DTODataTablesResponse<DTOAppointmentResponse>> GetAllAppointment_Pagination(DTODataTablesRequest dTO)
        {
            return await _apptDB.GetAllAppointment_Pagination(dTO);  
        }
    }
}
