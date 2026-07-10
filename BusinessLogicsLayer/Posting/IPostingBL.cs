using DataAccessLayer;
using DataTransferObject.Domain.Model;
using DataTransferObject.Requests;
using DataTransferObject.Response;

namespace BusinessLogicsLayer.Posting
{
    public interface IPostingBL : IGenericRepositoryDL<TrnPostingOut>
    {
        
        public Task<DTOPostingInResponse> GetArmyDataForPostingOut(string ArmyNo);
        public Task<DTODataTablesResponse<DTOPostingOutDetilsResponse>> GetPostingOutWithType(DTODataTablesRequest dTO,int AspNetUsersId, int UnitMapId, int Type,string PostingTy);
        public Task<bool> UpdateForPosting(TrnPostingOut Data);
        public Task<DTODataTablesResponse<DTOAppClosedListResponse>> GetAppClosedList(DTODataTableRequestForAppCloseList dTORecord);
        Task<DTOPostingOutDetailByIdResponse> GetPostingDetailById(string Id);
        public Task<DTOBeforePostingOutCheckedInputDataResponse> BeforePostingOutCheckedInputData(TrnPostingOut trnPostingOut);

    }
}
