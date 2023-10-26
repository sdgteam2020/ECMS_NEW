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
    public interface IComdDB : IGenericRepositoryDL<Comd>
    {
        public Task<bool> GetByName(Comd Dto);
    }
}
