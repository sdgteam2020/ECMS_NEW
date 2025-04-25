using DataTransferObject.Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.BaseInterfaces
{
    public interface IMapUnitChangeDB:IGenericRepositoryDL<TrnMapUnitChangeRequest>
    {
        public Task<bool> FindUnitIdMapped(int UnitMapId);
    }
}
