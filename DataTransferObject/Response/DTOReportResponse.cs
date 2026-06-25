namespace DataTransferObject.Response
{
    public class DTOReportResponse
    {
        public int TotalFilteredRecords { get; set; }
        public int? RequestId { get; set; }
        public int? StepId { get; set; }
        public string? NameAsPerRecord { get; set; }
        public string? Name { get; set; }
        public string? FName { get; set; }
        public string? LName { get; set; }
        public string? ServiceNo { get; set; }
        public string? RankName { get; set; }
        public string? Status { get; set; }
        public string? ArmedAbbreviation { get; set; }
        public string? RegimentalName { get; set; }
        public string? ApplyFor { get; set; }
        
        
        public string? UnitAbbreviation { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public string? FromRemark { get; set; }
        public string? ToRemark { get; set; }
        public string? RemarksNameList { get; set; }

        public DateTime? LostOn { get; set; }
        public string? SupportDocName { get; set; }
        public bool IsFIRLogged { get; set; }

        public string? FName_1 { get; set; }
        public string? LName_1 { get; set; }

        public string? FName_2 { get; set; } = string.Empty;
        public string? LName_2 { get; set; }
    }
}
