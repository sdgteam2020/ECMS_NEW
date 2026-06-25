namespace DataTransferObject.Response
{
    public class DTONotificationResponse
    {
        public int TotalFilteredRecords { get; set; }
        public int DisplayId { get; set; }
        public string Spanname { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public string DomainId { get; set; } = string.Empty;
        public string RankAbbreviation { get; set; } = string.Empty;
        public string FName { get; set; } = string.Empty;
        public string? LName { get; set; }
        public string ServiceNo { get; set; } = string.Empty;
        public string PhotoImagePath { get; set; } = string.Empty;
        public string ExistingPhotoInBase64 { get; set; } = string.Empty;
        public string ApplId { get; set; } = string.Empty;
        public string Url { get; set; } = string.Empty;
        public DateTime UpdatedOn { get; set; }

        public string? FName_1 { get; set; }
        public string? LName_1 { get; set; }

        public string? FName_2 { get; set; }
        public string? LName_2 { get; set; }

        public string? PhotoImagePath_1 { get; set; }
        public string? PhotoImagePath_2 { get; set; }
    }
    public class DTONotificationResult
    {
        public int TotalCount { get; set; }
        public List<DTONotificationResponse> Items { get; set; } = new();
    }
}
