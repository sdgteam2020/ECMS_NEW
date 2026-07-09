namespace DataTransferObject.Response
{
    public class DTODispatchCardForCSVResponse
    {
        public int ApplId { get; set; }
        public string ServiceNo { get; set; } = string.Empty;
        public string RankName { get; set; } = string.Empty;
        public string FName { get; set; } = string.Empty;
        public string? LName { get; set; } = string.Empty;
        public string? FName_1 { get; set; }
        public string? LName_1 { get; set; }
        public string? FName_2 { get; set; }
        public string? LName_2 { get; set; }
        public string? ChipNo { get; set; } 
        public string? CardSerialNo { get; set; }
    }
}
