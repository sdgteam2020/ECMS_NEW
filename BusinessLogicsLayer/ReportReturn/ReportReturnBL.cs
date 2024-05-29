using DataAccessLayer.BaseInterfaces;
using DataTransferObject.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicsLayer.ReportReturn
{
    public class ReportReturnBL : IReportReturnBL
    {
        private readonly IReportReturnDB _IReportReturnDB;
        public ReportReturnBL(IReportReturnDB reportReturnDB)
        {
            _IReportReturnDB = reportReturnDB;

        }
        public async Task<DTOReportReturnCountlst> GetMstepCount(int UserId)
        {
            DTOReportReturnCountlst dTOReportReturnCountlst = new DTOReportReturnCountlst();

            dTOReportReturnCountlst.dTOReportReturnCount = await _IReportReturnDB.GetMstepCount(UserId);
            dTOReportReturnCountlst.RecordOff = await _IReportReturnDB.GetRecordOffOffers();
            dTOReportReturnCountlst.RecordoffCount = await _IReportReturnDB.GetRecordOffOffersCount(UserId);

            return dTOReportReturnCountlst;
        }
    }
}
