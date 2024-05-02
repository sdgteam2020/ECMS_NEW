using DataTransferObject.Domain.Master;
using DataTransferObject.Response;
using DataTransferObject.Response.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.BaseInterfaces
{
    public interface IApptDB : IGenericRepositoryDL<MAppointment>
    {
        public Task<bool> GetByName(MAppointment Data);
        public Task<List<DTOAppointmentResponse>> GetALLAppt();
        public Task<List<DTOAppointmentResponse>> GetByFormationId(int ApptId);
        public Task<List<DTOAppointmentResponse>> GetALLByAppointmentName(string AppointmentName);
        public Task<DTOAppointmentResponse?> GetByApptId(short ApptId);
        public Task<DTOApptIdCheckInFKTableResponse?> ApptIdCheckInFKTable(short ApptId);
    }
}
