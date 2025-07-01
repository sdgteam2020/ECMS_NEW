using DataTransferObject.Response;
using Microsoft.Extensions.Logging;

namespace BusinessLogicsLayer.Token
{
    public class GetTokenBL : iGetTokenBL
    {
        private readonly ILogger<GetTokenBL> _logger;
        public GetTokenBL(ILogger<GetTokenBL> logger)
        {
            _logger = logger;
        }
        public DTOTokenResponse? GetTokenDetails(DTOTokenResponse Data)
        {
            try
            {
                if (Data.subject != null)
                {
                    var subdata = Data.subject.Split(",");

                    Data.Name = subdata[0].Replace("CN=", "");
                    Data.ArmyNo = subdata[1].Replace("SERIALNUMBER=", "");
                }

                return Data;
            }
            catch (Exception ex)
            {
                _logger.LogError(1001, ex, "GetTokenBL->GetTokenBL");
                return null;
            }
        }
    }
}
