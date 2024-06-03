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
       
        public Task<List<DTOReportReturnCount>> GetRecordOffOffers();
        public Task<List<DTOReportReturnCount>> GetRecordOffOffersCount(DTOMHierarchyRequest Data);
         
        public Task<List<DTOReportReturnCount>> GetRecordJco();
        public Task<List<DTOReportReturnCount>> GetRecordJcoCount(DTOMHierarchyRequest Data,int IsComplete);
        public Task<List<DTOReportReturnListResponse>> GetRecordHistory(DTOMHierarchyRequest Data,int ApplyForId,int StepId, int IsApproveId);

    }
}
