using DataAccessLayer;
using DataAccessLayer.BaseInterfaces;
using DataTransferObject.Domain.Model;


namespace BusinessLogicsLayer.Posting
{
    public class ApplCloseBL : GenericRepositoryDL<TrnApplClose>, IApplCloseBL
    {
        private readonly IApplCloseDB _iApplCloseDB;
        public ApplCloseBL(ApplicationDbContext context, IApplCloseDB iApplCloseDB) : base(context)
        {
            _iApplCloseDB = iApplCloseDB;   
        }
        public async Task<bool> RequestIdExists(TrnApplClose DTo)
        {
          return  await _iApplCloseDB.RequestIdExists(DTo);   
        }
        public async Task<bool> ApplCloseWithUpdateStatus(TrnApplClose Data)
        {
            return await _iApplCloseDB.ApplCloseWithUpdateStatus(Data);
        }
    }
}
