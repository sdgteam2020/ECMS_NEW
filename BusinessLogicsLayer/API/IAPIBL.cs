using DataTransferObject.Requests;
using DataTransferObject.Response;

namespace BusinessLogicsLayer.API
{
    public interface IaPiBl
    {
        public Task<DTOLoginAPIResponse> Getauthentication(DTOAPILoginRequest Data);
        public Task<DTOApiPersDataResponse> GetData(DTOPersDataRequest Data);

    }
}
