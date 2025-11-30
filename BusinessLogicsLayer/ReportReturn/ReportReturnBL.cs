using DataAccessLayer.BaseInterfaces;
using DataTransferObject.Requests;
using DataTransferObject.Response;

namespace BusinessLogicsLayer.ReportReturn
{
    public class ReportReturnBL : IReportReturnBL
    {
        private readonly IReportReturnDB _IReportReturnDB;
        public ReportReturnBL(IReportReturnDB reportReturnDB)
        {
            _IReportReturnDB = reportReturnDB;
        }
        public async Task<DTOReportCardDashboardCountResponse> GetReportCardDashboardCount(DTOMHierarchyRequest dTO)
        {
            return await _IReportReturnDB.GetReportCardDashboardCount(dTO);
        }
        public async Task<DTODataTablesResponse<DTOReportCardResponse>> GetReportCardData(DTODataTablesRequestForReportCard dTO)
        {
            return await _IReportReturnDB.GetReportCardData(dTO);
        }
        public async Task<DTOReportReturnCountlst> GetMstepCount(DTOMHierarchyRequest Data,short ArmedIdForORO)
        {
            DTOReportReturnCountlst dTOReportReturnCountlst = new DTOReportReturnCountlst();
            dTOReportReturnCountlst.dTOReportReturnCountOffs = await _IReportReturnDB.GetMstepCount(Data, 1);
            dTOReportReturnCountlst.dTOReportReturnCountJco = await _IReportReturnDB.GetMstepCount(Data, 2);


            dTOReportReturnCountlst.RecordOff = await _IReportReturnDB.GetRecordOffOffers(ArmedIdForORO);
            dTOReportReturnCountlst.RecordoffCount = await _IReportReturnDB.GetRecordOffOffersCount(Data);

            dTOReportReturnCountlst.RecordJco = await _IReportReturnDB.GetRecordJco(ArmedIdForORO);
            dTOReportReturnCountlst.RecordJcoPending = await _IReportReturnDB.GetRecordJcoCount(Data,0, ArmedIdForORO);

            return dTOReportReturnCountlst;
        }

        public Task<DTODataTablesResponse<DTOReportReturnListResponse>> GetRecordHistory(DTORecordHistory dTORecord)
        {
            return _IReportReturnDB.GetRecordHistory(dTORecord);
        }

        public Task<List<DTOReportReturnListResponse>> GetReportForm11(DTOMHierarchyRequest Data)
        {
            return _IReportReturnDB.GetReportForm11(Data);
        }
        public async Task<DTODataTablesResponse<DTOReportResponse>> GetReportData(DTODataTablesRequestForReport dTO)
        {
            return await _IReportReturnDB.GetReportData(dTO);
        }
        public async Task<DTOReportDashboardCountResponse> GetReportDashboardCount(DTOMHierarchyRequest dTO) 
        {
            return await _IReportReturnDB.GetReportDashboardCount(dTO);
        }
    }
}
