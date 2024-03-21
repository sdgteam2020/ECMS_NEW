using DataAccessLayer;
using DataAccessLayer.BaseInterfaces;
using DataTransferObject.Response;
using System;
using System.Collections.Generic;
using System.Linq;
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
        public async Task<DTODashboardCountResponse> GetDashBoardCount(int UserId)
        {
          return  await _iHomeDB.GetDashBoardCount(UserId);
        }
        public async Task<DTORequestDashboardCountResponse> GetRequestDashboardCount(int UserId, string Type)
        {
            return await _iHomeDB.GetRequestDashboardCount(UserId,Type);
        }
        public async Task<DTORequestSubDashboardCountResponse> GetSubDashboardCount(int UserId)
        {
            return await _iHomeDB.GetSubDashboardCount(UserId);
        }
    }
}
