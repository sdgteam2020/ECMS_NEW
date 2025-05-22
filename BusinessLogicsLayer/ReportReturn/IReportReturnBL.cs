using DataTransferObject.Requests;
using DataTransferObject.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicsLayer.ReportReturn
{
    public interface IReportReturnBL
    {
        public Task<DTOReportReturnCountlst> GetMstepCount(DTOMHierarchyRequest Data, short ArmedIdForORO);
        public Task<DTODataTablesResponse<DTOReportReturnListResponse>> GetRecordHistory(DTORecordHistory dTORecord);
        public Task<List<DTOReportReturnListResponse>> GetReportForm11(DTOMHierarchyRequest Data);
        public Task<DTODataTablesResponse<DTOReportResponse>> GetReportData(DTODataTablesRequestForReport dTO);
        public Task<DTOReportDashboardCountResponse> GetReportDashboardCount(DTODataTablesRequestForReport dTO);
    }
}
