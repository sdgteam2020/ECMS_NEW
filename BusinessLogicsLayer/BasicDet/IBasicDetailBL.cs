using DataTransferObject.Domain.Model;
using DataTransferObject.Requests;
using DataTransferObject.Response;
using DataTransferObject.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicsLayer.BasicDet
{
    public interface IBasicDetailBL:IGenericRepository<BasicDetail>
    {
        public Task<List<BasicDetailVM>> GetALLBasicDetail(int UserId,int step,int type);
        public Task<List<BasicDetailVM>> GetALLForIcardSttaus(int UserId,int step,int type);
        public Task<List<DTOICardTypeRequest>> GetAllICardType();
        public Task<BasicDetail?> FindServiceNo(string ServiceNo);
        public Task<BasicDetailCrtAndUpdVM> GetByBasicDetailsId(int BasicDetailId);
        public Task<List<ICardHistoryResponse>> ICardHistory(int RequestId);
        public Task<bool> SaveBasicDetailsWithAll(BasicDetail Data, MTrnAddress address,MTrnUpload trnUpload, MTrnIdentityInfo mTrnIdentityInfo, MTrnICardRequest mTrnICardRequest, MStepCounter mStepCounter);
        public Task<DTOICardTaskCountResponse> GetTaskCountICardRequest(int UserId, int Type);
        public Task<List<DTONotificationResponse>> GetNotification(int UserId, int Type);
        public Task<List<DTONotificationResponse>> GetNotificationRequestId(int UserId, int Type);

    }
}
