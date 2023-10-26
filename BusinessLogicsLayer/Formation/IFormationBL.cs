using DataTransferObject.Domain.Master;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicsLayer.Formation
{
    public interface IFormationBL : IGenericRepository<MFormation>
    {
        public Task<bool> GetByName(MFormation Data);
    }
}
