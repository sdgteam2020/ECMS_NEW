using DataAccessLayer.BaseInterfaces;
using DataTransferObject.Requests;
using DataTransferObject.Response;

namespace BusinessLogicsLayer.ReportReturn
{
    public class ReportReturnBL : IReportReturnBL
    {
        private readonly IReportReturnDB _IReportReturnDB;
        //private readonly IMapUnitDB _IMapUnitDB;
        public ReportReturnBL(IReportReturnDB reportReturnDB) //IMapUnitDB iMapUnitDB
        {
            _IReportReturnDB = reportReturnDB;
            //_IMapUnitDB = iMapUnitDB;   
        } 
        public async Task<DTOReportReturnCountlst> GetMstepCount(DTOMHierarchyRequest Data,short ArmedIdForORO)
        {
            DTOReportReturnCountlst dTOReportReturnCountlst = new DTOReportReturnCountlst();
           // var listunit=await _IMapUnitDB.GetUnitByHierarchyForIcardRequest(Data);
            dTOReportReturnCountlst.dTOReportReturnCountOffs = await _IReportReturnDB.GetMstepCount(Data, 1);
            dTOReportReturnCountlst.dTOReportReturnCountJco = await _IReportReturnDB.GetMstepCount(Data, 2);

            //dTOReportReturnCountlst.dToCountApprovedRejectOffs = await _IReportReturnDB.GetMstepCountApprovedReject(Data, 1);
            //dTOReportReturnCountlst.dToCountApprovedRejectJco = await _IReportReturnDB.GetMstepCountApprovedReject(Data, 2);


            dTOReportReturnCountlst.RecordOff = await _IReportReturnDB.GetRecordOffOffers(ArmedIdForORO);
            dTOReportReturnCountlst.RecordoffCount = await _IReportReturnDB.GetRecordOffOffersCount(Data);

            dTOReportReturnCountlst.RecordJco = await _IReportReturnDB.GetRecordJco(ArmedIdForORO);
            dTOReportReturnCountlst.RecordJcoPending = await _IReportReturnDB.GetRecordJcoCount(Data,0, ArmedIdForORO);
            //dTOReportReturnCountlst.RecordJcoCountApproved = await _IReportReturnDB.GetRecordJcoCount(Data,1);
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
