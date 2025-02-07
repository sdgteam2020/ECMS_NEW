using DataTransferObject.Domain.Master;
using DataTransferObject.Domain.Model;
using DataTransferObject.Requests;
using DataTransferObject.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.BaseInterfaces
{
    public interface IAPIDataDB : IGenericRepositoryDL<MApiData>
    {
        public Task<DTOApiPersDataResponse> GetByIC(DTOAPIDataRequest Data);
        public Task<DTOApiPersDataResponse> GetByoffrsIC(DTOAPIDataRequest Data);
        public Task<bool> apiLogin(string accessKey);
    }
}
