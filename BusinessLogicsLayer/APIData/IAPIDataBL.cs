using DataAccessLayer;
using DataTransferObject.Domain.Model;
using DataTransferObject.Requests;
using DataTransferObject.Response;

namespace BusinessLogicsLayer.APIData
{
    public interface IapiDataBl : IGenericRepositoryDL<MApiData>
    {

        public Task<DTOApiPersDataResponse> GetByIC(DTOAPIDataRequest Data);
        public Task<DTOApiPersDataResponse> GetByoffrsIC(DTOAPIDataRequest Data);
        public Task<bool> apiLogin(string accessKey);
    }
}
