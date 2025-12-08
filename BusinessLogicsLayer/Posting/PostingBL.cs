using DataAccessLayer;
using DataAccessLayer.BaseInterfaces;
using DataTransferObject.Domain.Model;
using DataTransferObject.Requests;
using DataTransferObject.Response;

namespace BusinessLogicsLayer.Posting
{
    public class PostingBL : GenericRepositoryDL<TrnPostingOut>, IPostingBL
    {
        private readonly IPostingDB postingDB;
        public PostingBL(ApplicationDbContext context, IPostingDB _postingDB) : base(context)
        {
            postingDB = _postingDB;
        }

        public async Task<List<DTOPostingOutDetilsResponse>> GetAllPostingHistory(int AspNetUsersId)
        {
            return await postingDB.GetAllPostingHistory(AspNetUsersId);
        }
        public async Task<DTODataTablesResponse<DTOPostingOutDetilsResponse>> GetPostingOutWithType(DTODataTablesRequest dTO,int AspNetUsersId, int UnitMapId, int Type, string PostingTy)
        {
            return await postingDB.GetPostingOutWithType(dTO, AspNetUsersId, UnitMapId, Type, PostingTy);
        }

        public async Task<DTOPostingInResponse> GetArmyDataForPostingOut(string ArmyNo)
        {
           return await postingDB.GetArmyDataForPostingOut(ArmyNo);
        }

        public async Task<bool> UpdateForPosting(TrnPostingOut Data)
        {
            return await postingDB.UpdateForPosting(Data);
        }
        public async Task<List<DTOAppClosedListResponse>> GetAppClosedList(int UnitMapId, int apply)
        {
            return await postingDB.GetAppClosedList(UnitMapId, apply);
        }
        public async Task<DTOPostingOutDetailByIdResponse> GetPostingDetailById(string Id) { 
            return await postingDB.GetPostingDetailById(Id);
        }
        public async Task<DTOBeforePostingOutCheckedInputDataResponse> BeforePostingOutCheckedInputData(TrnPostingOut trnPostingOut)
        {
            return await postingDB.BeforePostingOutCheckedInputData(trnPostingOut);
        }
    }
}
