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
    public interface IComdDB : IGenericRepositoryDL<MComd>
    {
        public Task<bool> GetByName(MComd Dto);
        public Task<int> GetByMaxOrder();
        public Task<int> GetComdIdbyOrderby(int OrderBy);
        public Task<IEnumerable<DataTransferObject.Domain.Master.MComd>> GetAllByorder();

    }
}
