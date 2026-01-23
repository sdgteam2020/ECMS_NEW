namespace DataTransferObject.Response
{
    public class DTODistributeCardGetResponse
    {
        public int TotalFilteredRecords { get; set; }
        public string NameAsPerRecord { get; set; } = string.Empty;
        public string FName { get; set; } = string.Empty;
        public string? LName { get; set; }
        public string ServiceNo { get; set; } = string.Empty;
        public string? RegimentalName { get; set; }
        public string UnitName { get; set; } = string.Empty;
        public string UnitAbbreviation { get; set; } = string.Empty;
        public string RankName { get; set; } = string.Empty;
        public string? ArmedName { get; set; }
        public int RequestId { get; set; }
        public DateTime UpdatedOn { get; set; }
        public DateTime DistributedOn { get; set; }
        public string ApplyFor { get; set; } = string.Empty;
        public int DistributeCardId { get; set; }
        public string? Remark { get; set; }
        public bool IsActive { get; set; } = false;
        public string? EncryptedId { get; set; }
    }
}
