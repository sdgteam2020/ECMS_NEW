namespace DataTransferObject.Response
{
    public class DTOLoginAPIResponse
    {
        //public int code { get; set; }
        //public bool error { get; set; }
        //public string msg { get; set; }
        //public int timestamp { get; set; }
        //public string jwt { get; set; }

        public string token { get; set; } = string.Empty;
        public string expiration { get; set; } = string.Empty;
        public bool Status { get; set; } = false;
        public string Message { get; set; } = string.Empty;

    }
    public class DTOLoginAPIResponseData
    {
        public DTOLoginAPIResponse ValidateRequest { get; set; }
    }
}
