using DataTransferObject.Domain.Master;
using DataTransferObject.Domain.Model;
using DataTransferObject.Requests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.BaseInterfaces
{
    public interface IAPIDataDB : IGenericRepositoryDL<MApiData>
    {
        public Task<MApiData> GetByIC(DTOAPIDataRequest Data);
        public Task<MApiDataOffrs> GetByoffrsIC(DTOAPIDataRequest Data);
    }
}
