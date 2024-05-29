using DataTransferObject.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.BaseInterfaces
{
    public interface IReportReturnDB
    {
        public Task<List<DTOReportReturnCount>> GetMstepCount(int UserId);
        public Task<List<DTOReportReturnCount>> GetRecordOffOffers();
        public Task<List<DTOReportReturnCount>> GetRecordOffOffersCount(int UserId);

    }
}
