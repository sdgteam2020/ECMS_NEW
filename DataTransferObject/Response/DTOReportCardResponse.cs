namespace DataTransferObject.Response
{
    public class DTOReportCardResponse
    {
        public int TotalFilteredRecords { get; set; }
        public int? RequestId { get; set; }
        public string? NameAsPerRecord { get; set; }
        public string? Name { get; set; }
        public string FName { get; set; } = string.Empty;
        public string? LName { get; set; }
        public string? ServiceNo { get; set; }
        public string? RankName { get; set; }
        public string? ArmedAbbreviation { get; set; }
        public DateTime? ActionOn { get; set; }

        public string FromRankName { get; set; } = string.Empty;
        public string ToRankName { get; set; } = string.Empty;
        public string FromName { get; set; } = string.Empty;
        public string ToName { get; set; } = string.Empty;
        public string FromServiceNo { get; set; } = string.Empty;
        public string ToServiceNo { get; set; } = string.Empty;
        public string FromDID { get; set; } = string.Empty;
        public string ToDID { get; set; } = string.Empty;

        public string? FName_1 { get; set; }
        public string? LName_1 { get; set; }

        public string? FName_2 { get; set; }
        public string? LName_2 { get; set; }

    }
}
