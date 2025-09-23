namespace DataTransferObject.Response
{
    public class DTODispatchCardForCSVResponse
    {
        public int ApplId { get; set; }
        public string ServiceNo { get; set; } = string.Empty;
        public string RankName { get; set; } = string.Empty;
        public string FName { get; set; } = string.Empty;
        public string? LName { get; set; }
        public string? ChipNo { get; set; } 
        public string? CardSerialNo { get; set; }
    }
}
