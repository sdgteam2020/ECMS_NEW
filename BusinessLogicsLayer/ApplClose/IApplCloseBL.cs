using DataTransferObject.Domain.Model;

namespace BusinessLogicsLayer.Posting
{
    public interface IApplCloseBL : IGenericRepository<TrnApplClose>
    {
        public Task<bool> RequestIdExists(TrnApplClose DTo);
        public Task<bool> ApplCloseWithUpdateStatus(TrnApplClose Data);
    }
}
