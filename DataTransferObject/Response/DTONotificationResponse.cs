namespace DataTransferObject.Response
{
    public class DTONotificationResponse
    { 
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

    }
}
