using DataAccessLayer;
using DataTransferObject.Domain.Model;

namespace BusinessLogicsLayer.Posting
{
    public interface IApplCloseBL : IGenericRepositoryDL<TrnApplClose>
    {
        public Task<bool> RequestIdExists(TrnApplClose DTo);
        public Task<bool> ApplCloseWithUpdateStatus(TrnApplClose Data);
    }
}
