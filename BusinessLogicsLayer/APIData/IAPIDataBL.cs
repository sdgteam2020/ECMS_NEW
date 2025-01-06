using DataTransferObject.Domain.Model;
using DataTransferObject.Requests;

namespace BusinessLogicsLayer.APIData
{
    public interface IAPIDataBL : IGenericRepository<MApiData>
    {

        public Task<MApiData?> GetByIC(DTOAPIDataRequest Data);
        public Task<MApiDataOffrs?> GetByoffrsIC(DTOAPIDataRequest Data);
        public Task<bool> apiLogin(DTOAPILoginRequest Data);
    }
}
