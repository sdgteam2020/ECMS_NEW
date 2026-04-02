using BusinessLogicsLayer.Bde;
using DataAccessLayer;
using DataAccessLayer.BaseInterfaces;
using DataTransferObject.Domain.Model;
using DataTransferObject.Requests;
using DataTransferObject.Response;

namespace BusinessLogicsLayer.BdeCate
{
    public class TrnFwnBL : GenericRepositoryDL<MTrnFwd>, ITrnFwnBL
    {

        private readonly ITrnFwnDB _ITrnFwnDB;

      
        public TrnFwnBL(ApplicationDbContext context, ITrnFwnDB iTrnFwnDB) : base(context)
        {
            _ITrnFwnDB = iTrnFwnDB;
        }
        public async Task<bool> UpdateFieldBYTrnFwdId(int TrnFwdId)
        {
            return await _ITrnFwnDB.UpdateFieldBYTrnFwdId(TrnFwdId);
        }
        public async Task<DTOGenericResponse<string>> SaveInternalFwd(DTOSaveInternalFwdRequest dTO, List<DTOCheckRequestIdsBeforeInternalFwdResponse> dTOChecks)
        {
            return await _ITrnFwnDB.SaveInternalFwd(dTO, dTOChecks);
        }
        public async Task<DTORequestRejectDetailResponse?> RequestRejectDetail(int RequestId)
        {
            return await _ITrnFwnDB.RequestRejectDetail(RequestId);
        }
        public async Task<DTORequestFwdDetailResponse?> RequestFwdDetail(int RequestId)
        {
            return await _ITrnFwnDB.RequestFwdDetail(RequestId);
        }
        public async Task<bool> ActionOnRequest(DTOActionOnRequest data, byte StepId)
        {
            return await _ITrnFwnDB.ActionOnRequest(data, StepId);
        }
        public async Task<DTOCheckUserIdBeforeInternalFwdResponse> CheckUserIdBeforeInternalFwd(int ToAspNetUsersId, int UnitId)
        {
            return await _ITrnFwnDB.CheckUserIdBeforeInternalFwd(ToAspNetUsersId, UnitId);

        }
        public async Task<List<DTOCheckRequestIdsBeforeInternalFwdResponse>> CheckRequestIdsBeforeInternalFwd(int[] RequestIds, int FromAspNetUsersId)
        {
            return await _ITrnFwnDB.CheckRequestIdsBeforeInternalFwd(RequestIds, FromAspNetUsersId);
        }
    }
}
