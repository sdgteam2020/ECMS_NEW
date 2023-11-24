using DataTransferObject.Domain.Master;
using DataTransferObject.Response;
using DataTransferObject.Response.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicsLayer.Master
{
    public interface IRegimentalBL : IGenericRepository<MRegimental>
    {

        public Task<bool> GetByName(MRegimental DTo);
        public Task<List<DTORegimentalResponse>> GetAllData();
    }
}
