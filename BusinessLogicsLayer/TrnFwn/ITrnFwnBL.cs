using DataAccessLayer;
using DataTransferObject.Domain.Model;
using DataTransferObject.Requests;
using DataTransferObject.Response;

namespace BusinessLogicsLayer.Bde
{
    public interface ITrnFwnBL : IGenericRepositoryDL<MTrnFwd>
    {
        public Task<bool> UpdateFieldBYTrnFwdId(int TrnFwdId);
        public Task<bool?> SaveInternalFwd(DTOSaveInternalFwdRequest dTO);
        public Task<DTORequestRejectDetailResponse?> RequestRejectDetail(int RequestId);
        public Task<DTORequestFwdDetailResponse?> RequestFwdDetail(int RequestId);
        public Task<bool> ActionOnRequest(DTOActionOnRequest data, byte StepId);
        public Task<DTOCheckUserIdBeforeInternalFwdResponse> CheckUserIdBeforeInternalFwd(int ToAspNetUsersId, int UnitId);

    }
}
