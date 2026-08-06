
namespace DataTransferObject.Response
{
    public class DTOBasicDetailForParitalViewResponse
    {
        public string PaperIcardNo { get; set; } = string.Empty;
        public string NameAsPerRecord { get; set; } = string.Empty;
        public string FName { get; set; } = string.Empty;
        public string? LName { get; set; }
        public string ServiceNo { get; set; } = string.Empty;
        public DateTime DOB { get; set; }
        public DateTime? DateOfIssue { get; set; }
        public DateTime DateOfCommissioning { get; set; }
        public string? PlaceOfIssue { get; set; }
        public string IssuingAuthorityName { get; set; } = string.Empty;
        public string State { get; set; } = string.Empty;
        public string District { get; set; } = string.Empty;
        public string? PS { get; set; }
        public string? PO { get; set; }
        public string? Tehsil { get; set; }
        public string? Village { get; set; }
        public int? PinCode { get; set; }
        public string IdenMark1 { get; set; } = string.Empty;
        public string AadhaarNo { get; set; } = string.Empty;
        public int Height { get; set; }
        public string? BloodGroup { get; set; }
        public string RankName { get; set; } = string.Empty;
        public string? ArmedName { get; set; }
        public string PhotoImagePath { get; set; } = string.Empty;
        public string SignatureImagePath { get; set; } = string.Empty;
        public string CardSerialNo { get; set; } = string.Empty;
        public string ChipNo { get; set; } = string.Empty;
        public string? SignatureInBase64 { get; set; }
        public string? PhotoInBase64 { get; set; }

    }
}
