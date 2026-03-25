namespace DataTransferObject.Requests
{
    public class DTOCardDispatchCheckRequest
    {
        public string ChipNo { get; set; }=string.Empty;
        public int ApplId { get; set; }
        public bool IsValid { get; set; } = true;
        public string Status { get; set; } = "Valid";
        public string Remarks { get; set; } = string.Empty;
    }
}
