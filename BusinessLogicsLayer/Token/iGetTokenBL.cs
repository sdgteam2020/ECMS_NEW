using DataTransferObject.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicsLayer.Token
{
    public interface iGetTokenBL
    {
        public Task<DTOTokenResponse> GetTokenDetails(DTOTokenResponse Data);
    }
}
