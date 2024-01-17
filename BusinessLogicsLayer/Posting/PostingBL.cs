using BusinessLogicsLayer.Bde;
using DataAccessLayer;
using DataAccessLayer.BaseInterfaces;
using DataTransferObject.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicsLayer.Posting
{
    public class PostingBL : IPostingBL
    {
        private readonly IPostingDB postingDB;
        public PostingBL(IPostingDB _postingDB)
        {
            postingDB = _postingDB;
        }
        public async Task<DTOPostingInResponse> GetArmyDataForPostingIn(string ArmyNo)
        {
           return await postingDB.GetArmyDataForPostingIn(ArmyNo);
        }
    }
}
