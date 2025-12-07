namespace DataTransferObject.Response
{
    public class DTOApplicationCloseResponse
    {
        public int BasicDetailId { get; set; }
        public int UnitId { get; set; }
        public byte StatusId { get; set; }
        public int? ApplCloseId { get; set; }
        public bool Result { get; set; }
        public string Message { get; set; } = string.Empty;
    }
}
