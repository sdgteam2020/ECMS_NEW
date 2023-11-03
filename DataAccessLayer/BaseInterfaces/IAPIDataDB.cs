using DataTransferObject.Domain.Master;
using DataTransferObject.Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.BaseInterfaces
{
    public interface IAPIDataDB : IGenericRepositoryDL<MApiData>
    {
        public Task<MApiData> GetByIC(string ICNo);
    }
}
