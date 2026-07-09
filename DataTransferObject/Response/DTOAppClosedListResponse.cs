namespace DataTransferObject.Response
{
    public class DTOAppClosedListResponse
    {
        public int TotalFilteredRecords { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public int Updatedby { get; set; }
        public string ServiceNo { get; set; } = string.Empty;
        public string RankName { get; set; } = string.Empty;
        public string FName { get; set; } = string.Empty;
        public string? LName { get; set; }
        public string Reason { get; set; } = string.Empty;
        public string ApplyFor { get; set; } = string.Empty;
        public string Authority { get; set; } = string.Empty;
        public string Remarks { get; set; } = string.Empty;
        public int Id { get; set; }
        public int BasicDetailId { get; set; }
        public byte ReasonId { get; set; }
        public int RequestId { get; set; }
    }
}
