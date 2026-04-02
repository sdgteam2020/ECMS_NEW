using DataTransferObject.Domain.Model;
using DataTransferObject.Requests;
using DataTransferObject.Response;

namespace DataAccessLayer.BaseInterfaces
{
    public interface ITrnFwnDB : IGenericRepositoryDL<MTrnFwd>
    {
        public Task<bool> UpdateFieldBYTrnFwdId(int TrnFwdId);
        public Task<DTOGenericResponse<string>> SaveInternalFwd(DTOSaveInternalFwdRequest dTO, List<DTOCheckRequestIdsBeforeInternalFwdResponse> dTOChecks);
        public Task<DTORequestRejectDetailResponse?> RequestRejectDetail(int RequestId);
        public Task<DTORequestFwdDetailResponse?> RequestFwdDetail(int RequestId);
        public Task<bool> ActionOnRequest(DTOActionOnRequest data, byte StepId);
        public Task<DTOCheckUserIdBeforeInternalFwdResponse> CheckUserIdBeforeInternalFwd(int ToAspNetUsersId, int UnitId);
        public Task<List<DTOCheckRequestIdsBeforeInternalFwdResponse>> CheckRequestIdsBeforeInternalFwd(int[] RequestIds, int FromAspNetUsersId);
    }
}
