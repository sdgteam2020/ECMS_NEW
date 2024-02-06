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
    public interface IRankBL : IGenericRepository<MRank>
    {

        public Task<bool> GetByName(MRank DTo);
        public Task<short> GetByMaxOrder();
        public Task<int> OrderByChange(MRank DTo);
        public Task<IEnumerable<MRank>> GetAllByorder();
        public Task<IEnumerable<MRank>> GetAllByType(int Type);
    }
}
