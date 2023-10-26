using DataTransferObject.Domain.Master;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.BaseInterfaces
{
    public interface IFormationDB : IGenericRepositoryDL<MFormation>
    {
        public Task<bool> GetByName(MFormation Dto);
    }
}
