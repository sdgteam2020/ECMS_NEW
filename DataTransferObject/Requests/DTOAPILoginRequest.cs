namespace DataTransferObject.Requests
{
    public class DTOAPILoginRequest
    {

        //public string? ClientKey { get; set; }
        //public string? ClientIP { get; set; }
        //public string? ClientURL { get; set; }
        //public string? ClientPW { get; set; }

        //public string ClientName { get; set; }
        public string ApiUrl { get; set; } = string.Empty;
        public string LoginUrl { get; set; } = string.Empty;
        public string accessKey { get; set; } = string.Empty;
        //public string? password { get; set; }
    }
}
