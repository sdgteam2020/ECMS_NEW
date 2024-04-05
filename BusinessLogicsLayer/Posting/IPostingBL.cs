using DataTransferObject.Domain.Master;
using DataTransferObject.Domain.Model;
using DataTransferObject.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicsLayer.Posting
{
    public interface IPostingBL : IGenericRepository<TrnPostingOut>
    {
        
        public Task<DTOPostingInResponse> GetArmyDataForPostingOut(string ArmyNo);
        public Task<List<DTOPostingOutDetilsResponse>> GetAllPostingHistory(int AspNetUsersId);
        public Task<List<DTOPostingOutDetilsResponse>> GetPostingOutWithType(int AspNetUsersId,int Type,string PostingTy);
        public Task<bool> UpdateForPosting(TrnPostingOut Data);

    }
}
