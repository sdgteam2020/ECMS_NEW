using DataTransferObject.Requests;
using DataTransferObject.Response;

namespace BusinessLogicsLayer.API
{
     public interface IAPIBL
    {
        public Task<DTOLoginAPIResponse> Getauthentication(DTOAPILoginRequest Data);
        public Task<DTOApiPersDataResponse> GetData(DTOPersDataRequest Data);
        public Task<DTOApiPersDataResponse> GetDataOffrs(DTOPersDataRequest Data);
    }
}
