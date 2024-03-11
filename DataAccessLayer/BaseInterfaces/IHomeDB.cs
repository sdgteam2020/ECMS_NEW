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
        public Task<DTODashboardCountResponse> GetDashBoardCount(int UserId);
        public Task<DTORequestDashboardCountResponse> GetRequestDashboardCount(int UserId);
    }
}
