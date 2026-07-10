using DataAccessLayer;
using DataAccessLayer.BaseInterfaces;
using DataTransferObject.Domain.Model;
using DataTransferObject.Requests;
using DataTransferObject.Response;


namespace BusinessLogicsLayer.Posting
{
    public class ApplCloseBL : GenericRepositoryDL<TrnApplClose>, IApplCloseBL
    {
        private readonly IApplCloseDB _iApplCloseDB;
        public ApplCloseBL(ApplicationDbContext context, IApplCloseDB iApplCloseDB) : base(context)
        {
            _iApplCloseDB = iApplCloseDB;   
        }
        public async Task<DTOApplicationCloseResponse> RequestIdExists(DTOApplicationCloseRequest DTo)
        {
          return  await _iApplCloseDB.RequestIdExists(DTo);   
        }
        public async Task<bool> ApplCloseWithUpdateStatus(TrnApplClose Data, ICardHistoryResponseAll? cardHistoryResponses)
        {         
            return await _iApplCloseDB.ApplCloseWithUpdateStatus(Data, cardHistoryResponses);
        }
    }
}
