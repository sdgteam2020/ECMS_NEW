using DataTransferObject.Requests;
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
        public Task<List<DTOReportReturnCount>> GetMstepCount(DTOMHierarchyRequest Data, int ApplyForId);

        public Task<List<DTOReportReturnCount>> GetMstepCountApprovedReject(DTOMHierarchyRequest Data, int ApplyForId);
       
        public Task<List<DTOReportReturnCount>> GetRecordOffOffers(short ArmedIdForORO);
        public Task<List<DTOReportReturnCount>> GetRecordOffOffersCount(DTOMHierarchyRequest Data);
         
        public Task<List<DTOReportReturnCount>> GetRecordJco(short ArmedIdForORO);
        public Task<List<DTOReportReturnCount>> GetRecordJcoCount(DTOMHierarchyRequest Data,int IsComplete, short ArmedIdForORO);
        public Task<DTODataTablesResponse<DTOReportReturnListResponse>> GetRecordHistory(DTORecordHistory dTORecord);
        public Task<List<DTOReportReturnListResponse>> GetReportForm11(DTOMHierarchyRequest Data);
        public Task<DTODataTablesResponse<DTOReportResponse>> GetReportData(DTODataTablesRequestForReport dTO);
        public Task<DTOReportDashboardCountResponse> GetReportDashboardCount(DTOMHierarchyRequest dTO);

    }
}
