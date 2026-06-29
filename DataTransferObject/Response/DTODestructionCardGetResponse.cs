namespace DataTransferObject.Response
{
    public class DTODestructionCardGetResponse
    {
        public int TotalFilteredRecords { get; set; }
        public string FName { get; set; } = string.Empty;
        public string? LName { get; set; }
        public string ServiceNo { get; set; } = string.Empty;
        public string UnitAbbreviation { get; set; } = string.Empty;
        public string RankName { get; set; } = string.Empty;
        public int RequestId { get; set; }
        public DateTime UpdatedOn { get; set; }
        public DateTime DestructedOn { get; set; }
        public string ApplyFor { get; set; } = string.Empty;
        public int DestructedCardId { get; set; }
        public string RemarksNameList { get; set; } = string.Empty;
        public string? Remark { get; set; }

        public string? FName_1 { get; set; }
        public string? LName_1 { get; set; }

        public string? FName_2 { get; set; }
        public string? LName_2 { get; set; }
    }
}
