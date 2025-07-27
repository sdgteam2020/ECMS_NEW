using DataTransferObject.Requests;
using DataTransferObject.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.BaseInterfaces
{
    public interface IHomeDB
    {
        public Task<DTOTaskCountResponse> GetTaskBoardCount(int MapUnitId, byte Claim, int TDM_Id);
        public Task<DTODashboardCountResponse> GetDashBoardCount(int UserId, DTOApplFwdConditionRequest dTOApplFwdCondition, short ArmedIdForORO);
        public Task<DTORequestDashboardCountResponse> GetRequestDashboardCount(int UserId, string Type, int UnitMapId);
        public Task<DTORequestSubDashboardCountResponse> GetSubDashboardCount(int UserId, int UnitMapId);
        public Task<List<DTORegisterUserResponse>> GetAllRegisterUser(int UnitId);
        public Task<DTORequestDashboardUserMgtCountResponse> GetDashboardUserMgtCount(int UnitId, int UserId);
    }
}
