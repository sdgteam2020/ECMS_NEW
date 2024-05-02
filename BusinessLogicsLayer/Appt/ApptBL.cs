using BusinessLogicsLayer.Corps;
using DataAccessLayer.BaseInterfaces;
using DataAccessLayer;
using DataTransferObject.Domain.Master;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataTransferObject.Response;
using BusinessLogicsLayer.Formation;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace BusinessLogicsLayer.Appt
{
    internal class ApptBL : GenericRepositoryDL<MAppointment>, IApptBL
    {
        private readonly IApptDB _apptDB;

        public ApptBL(ApplicationDbContext context, IApptDB apptDB) : base(context)
        {
            _apptDB = apptDB;
        }

        public Task<List<DTOAppointmentResponse>> GetALLAppt()
        {
            return _apptDB.GetALLAppt();
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
    }
}
