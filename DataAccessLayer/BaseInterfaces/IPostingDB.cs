using DataTransferObject.Domain.Model;
using DataTransferObject.Requests;
using DataTransferObject.Response;

namespace DataAccessLayer.BaseInterfaces
{
    public interface IPostingDB
    {
        public Task<DTOPostingInResponse> GetArmyDataForPostingOut(string ArmyNo);
        public Task<List<DTOPostingOutDetilsResponse>> GetAllPostingHistory(int AspNetUsersId);
        public Task<DTODataTablesResponse<DTOPostingOutDetilsResponse>> GetPostingOutWithType(DTODataTablesRequest dTO,int AspNetUsersId, int UnitMapId, int Type, string PostingTy);
        public Task<bool> UpdateForPosting(TrnPostingOut Data);
        public Task<List<DTOAppClosedListResponse>> GetAppClosedList(int Updatedby, int apply);
        public Task<DTOPostingOutDetailByIdResponse> GetPostingDetailById(string Id);
        public Task<DTOBeforePostingOutCheckedInputDataResponse> BeforePostingOutCheckedInputData(TrnPostingOut trnPostingOut);
    }
}
