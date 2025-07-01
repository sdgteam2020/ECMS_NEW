using DataAccessLayer;
using DataTransferObject.Domain.Master;
using DataTransferObject.Requests;
using DataTransferObject.Response;
using DataTransferObject.Response.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicsLayer.Bde
{
    public interface IRankBL : IGenericRepositoryDL<MRank>
    {

        public Task<bool> GetByName(MRank Dto);
        public Task<short> GetByMaxOrder();
        public Task<int> OrderByChange(MRank Dto);
        public Task<IEnumerable<MRank>> GetAllByorder();
        public Task<IEnumerable<MRank>> GetAllByType(int Type);
        public Task<DTORankIdCheckInFKTableResponse?> RankIdCheckInFKTable(short RankId);
    }
}
