using DataAccessLayer;
using DataTransferObject.Domain.Master;
using DataTransferObject.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicsLayer.Appt
{
    public interface IApptBL : IGenericRepository<MAppointment>
    {
        public Task<bool> GetByName(MAppointment Data);
        public Task<List<DTOAppointmentResponse>> GetALLAppt();
        public Task<List<DTOAppointmentResponse>> GetByFormationId(int FormationId);
    }
}
