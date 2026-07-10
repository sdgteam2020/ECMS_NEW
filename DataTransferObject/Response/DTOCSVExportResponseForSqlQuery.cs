namespace DataTransferObject.Response
{
    public class DTOCSVExportResponseForSqlQuery
    {
        public string ServiceNo { get; set; } = string.Empty;
        public string NameAsPerRecord { get; set; } = string.Empty;
        public DateTime DOB { get; set; }
        public DateTime DateOfCommissioning { get; set; }
        public string PermanentAddress { get; set; } = string.Empty;
        public string RankAbbreviation { get; set; } = string.Empty;
        public string FName { get; set; } = string.Empty;
        public string? LName { get; set; }
        public string UnitName { get; set; } = string.Empty;
        public int ApplId { get; set; }
        public string ApplyFor { get; set; } = string.Empty;
        public string ICardType { get; set; } = string.Empty;
        public string? State { get; set; }
        public string? District { get; set; }
        public string? PS { get; set; }
        public string? PO { get; set; }
        public string? Tehsil { get; set; }
        public string? Village { get; set; }
        public int PinCode { get; set; }

        public string? FName_1 { get; set; }
        public string? LName_1 { get; set; }

        public string? FName_2 { get; set; }
        public string? LName_2 { get; set; }

        public string? NameAsPerRecord_1 { get; set; }
        public string? NameAsPerRecord_2 { get; set; }

        public DateTime? DOB_1 { get; set; }
        public DateTime? DOB_2 { get; set; }

        public string State_1 { get; set; } = string.Empty;
        public string District_1 { get; set; } = string.Empty;
        public string? PS_1 { get; set; }
        public string? PO_1 { get; set; }
        public string? Tehsil_1 { get; set; }
        public string? Village_1 { get; set; }
        public int? PinCode_1 { get; set; }

        public string State_2 { get; set; } = string.Empty;
        public string District_2 { get; set; } = string.Empty;
        public string? PS_2 { get; set; }
        public string? PO_2 { get; set; }
        public string? Tehsil_2 { get; set; }
        public string? Village_2 { get; set; }
        public int? PinCode_2 { get; set; }
    }
}
