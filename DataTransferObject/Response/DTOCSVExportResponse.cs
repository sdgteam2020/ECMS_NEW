namespace DataTransferObject.Response
{

    public class DTOCSVExportResponse
    {
        public int Sno { get; set; }
        public int ApplId { get; set; }
        public string ApplyFor { get; set; } = string.Empty;
        public string ServiceNo { get; set; } = string.Empty;
        public string NameAsPerRecord { get; set; } = string.Empty;
        public DateOnly DOB { get; set; }
        public DateOnly DateOfCommissioning { get; set; }
        public string PermanentAddress { get; set; } = string.Empty;
        public string RankAbbreviation { get; set; } = string.Empty;
        public string FName { get; set; } = string.Empty;
        public string? LName { get; set; }
        public string UnitName { get; set; } = string.Empty;
        public string ICardType { get; set; } = string.Empty;
        public string? State { get; set; }
        public string? District { get; set; }
        public string? PS { get; set; }
        public string? PO { get; set; }
        public string? Tehsil { get; set; }
        public string? Village { get; set; }
        public int PinCode { get; set; }
    }
}
