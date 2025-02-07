using DataTransferObject.Domain.Model;
using DataTransferObject.Requests;
using DataTransferObject.Response;

namespace BusinessLogicsLayer.APIData
{
    public interface IAPIDataBL : IGenericRepository<MApiData>
    {

        public Task<DTOApiPersDataResponse> GetByIC(DTOAPIDataRequest Data);
        public Task<DTOApiPersDataResponse> GetByoffrsIC(DTOAPIDataRequest Data);
        public Task<bool> apiLogin(string accessKey);
    }
}
