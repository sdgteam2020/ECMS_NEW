using BusinessLogicsLayer.Bde;
using DataAccessLayer;
using DataAccessLayer.BaseInterfaces;
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
        public async Task<List<DTOPostingOutDetilsResponse>> GetPostingOutWithType(int AspNetUsersId, int Type)
        {
            return await postingDB.GetPostingOutWithType(AspNetUsersId,Type);
        }

        public async Task<DTOPostingInResponse> GetArmyDataForPostingOut(string ArmyNo)
        {
           return await postingDB.GetArmyDataForPostingOut(ArmyNo);
        }

        public async Task<bool> UpdateForPosting(TrnPostingOut Date)
        {
            return await postingDB.UpdateForPosting(Date);
        }
    }
}
