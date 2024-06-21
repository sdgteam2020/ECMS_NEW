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
    public interface IRegimentalDB : IGenericRepositoryDL<MRegimental>
    {
        public Task<bool> GetByName(MRegimental Dto);
        public Task<List<DTORegimentalResponse>> GetByArmedId(byte ArmedId);
        public Task<List<DTORegimentalResponse>> GetAllData();
    }
}
