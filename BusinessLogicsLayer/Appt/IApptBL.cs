using DataAccessLayer;
using DataTransferObject.Domain.Master;
using DataTransferObject.Requests;
using DataTransferObject.Response;

namespace BusinessLogicsLayer.Appt
{
    public interface IApptBL : IGenericRepositoryDL<MAppointment>
    {
        public Task<bool> GetByName(MAppointment Data);
        public Task<List<DTOAppointmentResponse>> GetALLAppt();
        public Task<List<DTOAppointmentResponse>> GetByFormationId(int FormationId);
        public Task<List<DTOAppointmentResponse>> GetALLByAppointmentName(string AppointmentName);
        public Task<DTOAppointmentResponse?> GetByApptId(short ApptId);
        public Task<DTOApptIdCheckInFKTableResponse?> ApptIdCheckInFKTable(short ApptId);
        public Task<DTODataTablesResponse<DTOAppointmentResponse>> GetAllAppointment_Pagination(DTODataTablesRequest dTO);
    }
}
