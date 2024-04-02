using DataTransferObject.Domain.Model;
using DataTransferObject.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.BaseInterfaces
{
    public interface IPostingDB
    {
        public Task<DTOPostingInResponse> GetArmyDataForPostingOut(string ArmyNo);
        public Task<List<DTOPostingOutDetilsResponse>> GetAllPostingHistory(int AspNetUsersId);
        public Task<List<DTOPostingOutDetilsResponse>> GetPostingOutWithType(int AspNetUsersId, int Type);
        public Task<bool> UpdateForPosting(TrnPostingOut Data);
    }
}
