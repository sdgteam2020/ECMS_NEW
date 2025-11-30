using DataTransferObject.Domain.Model;
using DataTransferObject.Requests;
using DataTransferObject.Response;

namespace DataAccessLayer.BaseInterfaces
{
    public interface ITrnFwnDB : IGenericRepositoryDL<MTrnFwd>
    {
        public Task<bool> AddTrnFwdWithIsCompleteUpdate(MTrnFwd data);
        public Task<bool> UpdateAllBYRequestId(int RequestId);
        public Task<bool> UpdateFieldBYTrnFwdId(int TrnFwdId);
        public Task<bool?> SaveInternalFwd(DTOSaveInternalFwdRequest dTO);
        public Task<DTORequestRejectDetailResponse?> RequestRejectDetail(int RequestId);

    }
}
