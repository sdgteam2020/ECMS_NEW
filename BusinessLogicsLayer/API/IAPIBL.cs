using DataTransferObject.Requests;
using DataTransferObject.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicsLayer.API
{
     public interface IAPIBL
    {
        public Task<DTOLoginAPIResponse> Getauthentication(DTOAPILoginRequest Data);
        public Task<DTOApiPersDataResponse> GetData(DTOPersDataRequest Data);
        public Task<DTOApiPersDataResponse> GetDataOffrs(DTOPersDataRequest Data);
    }
}
