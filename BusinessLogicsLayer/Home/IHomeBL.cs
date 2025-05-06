using DataTransferObject.Requests;
using DataTransferObject.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicsLayer.Home
{
   public interface IHomeBL
    {
        public Task<DTODashboardCountResponse> GetDashBoardCount(int UserId, DTOApplFwdConditionRequest dTOApplFwdCondition, short ArmedIdForORO, int MapUnitId, bool Claim);
        public Task<DTORequestDashboardCountResponse> GetRequestDashboardCount(int UserId, int UnitId, string Type);
        public Task<DTORequestSubDashboardCountResponse> GetSubDashboardCount(int UserId);
        public Task<List<DTORegisterUserResponse>> GetAllRegisterUser(int UnitId);
        public Task<DTORequestDashboardUserMgtCountResponse> GetDashboardUserMgtCount(int UnitId,int UserId);
    }
}
