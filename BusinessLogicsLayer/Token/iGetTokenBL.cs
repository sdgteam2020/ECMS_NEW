using DataTransferObject.Response;

namespace BusinessLogicsLayer.Token
{
    public interface iGetTokenBL
    {
        public DTOTokenResponse? GetTokenDetails(DTOTokenResponse Data);
    }
}
