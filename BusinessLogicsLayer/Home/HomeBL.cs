using DataAccessLayer;
using DataAccessLayer.BaseInterfaces;
using DataTransferObject.Requests;
using DataTransferObject.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicsLayer.Home
{
    public class HomeBL : IHomeBL
    {
        private readonly IHomeDB _iHomeDB;

        public HomeBL(IHomeDB iHomeDB)
        {
            _iHomeDB = iHomeDB;
        }
        public async Task<DTODashboardCountResponse> GetDashBoardCount(int UserId, DTOApplFwdConditionRequest dTOApplFwdCondition, short ArmedIdForORO, int MapUnitId,byte Claim,int TDM_Id)
        {
          return  await _iHomeDB.GetDashBoardCount(UserId, dTOApplFwdCondition, ArmedIdForORO, MapUnitId, Claim, TDM_Id);
        }
        public async Task<DTORequestDashboardCountResponse> GetRequestDashboardCount(int UserId, string Type,int UnitMapId)
        {
            return await _iHomeDB.GetRequestDashboardCount(UserId,Type, UnitMapId);
        }
        public async Task<DTORequestSubDashboardCountResponse> GetSubDashboardCount(int UserId, int UnitMapId)
        {
            return await _iHomeDB.GetSubDashboardCount(UserId, UnitMapId);
        }
        public async Task<List<DTORegisterUserResponse>> GetAllRegisterUser(int UnitId)
        {
            return await _iHomeDB.GetAllRegisterUser(UnitId);
        }
        public async Task<DTORequestDashboardUserMgtCountResponse> GetDashboardUserMgtCount(int UnitId,int UserId)
        {
            return await _iHomeDB.GetDashboardUserMgtCount(UnitId,UserId);
        }
    }
}
