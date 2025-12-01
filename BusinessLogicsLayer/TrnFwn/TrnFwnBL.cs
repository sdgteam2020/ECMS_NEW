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
        public async Task<bool> AddTrnFwdWithIsCompleteUpdate(MTrnFwd data)
        {
            return await _ITrnFwnDB.AddTrnFwdWithIsCompleteUpdate(data);
        }

        public Task<bool> UpdateAllBYRequestId(int RequestId)
        {
            return _ITrnFwnDB.UpdateAllBYRequestId(RequestId);
        }
        public async Task<bool> UpdateFieldBYTrnFwdId(int TrnFwdId)
        {
            return await _ITrnFwnDB.UpdateFieldBYTrnFwdId(TrnFwdId);
        }
        public async Task<bool?> SaveInternalFwd(DTOSaveInternalFwdRequest dTO)
        {
            return await _ITrnFwnDB.SaveInternalFwd(dTO);
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
    }
}
