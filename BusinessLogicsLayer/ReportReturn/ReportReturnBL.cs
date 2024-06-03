using DataAccessLayer.BaseInterfaces;
using DataTransferObject.Requests;
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
        public async Task<DTOReportReturnCountlst> GetMstepCount(DTOMHierarchyRequest Data)
        {
            DTOReportReturnCountlst dTOReportReturnCountlst = new DTOReportReturnCountlst();

            dTOReportReturnCountlst.dTOReportReturnCountOffs = await _IReportReturnDB.GetMstepCount(Data, 1);
            dTOReportReturnCountlst.dTOReportReturnCountJco = await _IReportReturnDB.GetMstepCount(Data, 2);

            //dTOReportReturnCountlst.dToCountApprovedRejectOffs = await _IReportReturnDB.GetMstepCountApprovedReject(Data, 1);
            //dTOReportReturnCountlst.dToCountApprovedRejectJco = await _IReportReturnDB.GetMstepCountApprovedReject(Data, 2);


            dTOReportReturnCountlst.RecordOff = await _IReportReturnDB.GetRecordOffOffers();
            dTOReportReturnCountlst.RecordoffCount = await _IReportReturnDB.GetRecordOffOffersCount(Data);

            dTOReportReturnCountlst.RecordJco = await _IReportReturnDB.GetRecordJco();
            dTOReportReturnCountlst.RecordJcoPending = await _IReportReturnDB.GetRecordJcoCount(Data,0);
            //dTOReportReturnCountlst.RecordJcoCountApproved = await _IReportReturnDB.GetRecordJcoCount(Data,1);
            return dTOReportReturnCountlst;
        }

        public Task<List<DTOReportReturnListResponse>> GetRecordHistory(DTOMHierarchyRequest Data, int ApplyForId ,int StepId, int IsApproveId)
        {
            return _IReportReturnDB.GetRecordHistory(Data, ApplyForId, StepId, IsApproveId);
        }
    }
}
