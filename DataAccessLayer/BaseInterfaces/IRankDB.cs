using DataTransferObject.Domain.Master;
using DataTransferObject.Response;
using DataTransferObject.Response.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.BaseInterfaces
{
    public interface IRankDB : IGenericRepositoryDL<MRank>
    {
        public Task<bool> GetByName(MRank Dto);
        public Task<int> GetByMaxOrder();
        public Task<int> GetRankIdbyOrderby(int OrderBy);
        public Task<IEnumerable<MRank>> GetAllByorder();
        public Task<IEnumerable<MRank>> GetAllByType(int Type);
    }
}
