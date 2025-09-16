using DataTransferObject.Validation;

namespace DataTransferObject.Response
{
    public class DTODataExportsResponse
    {
        [CsvIgnore]
        public string PaperIcardNo { get; set; } = string.Empty;
        public string FName { get; set; } = string.Empty;
        public string? LName { get; set; }
        public string NameAsPerRecord { get; set; } = string.Empty;
        public string? RankName { get; set; }
        public string? ArmedName { get; set; }
        public string ServiceNo { get; set; } = string.Empty;
        public DateTime DOB { get; set; }
        public int Height { get; set; }
        public string AadhaarNo { get; set; } = string.Empty;
        public string BloodGroup { get; set; } = string.Empty;
        public string PlaceOfIssue { get; set; } = string.Empty;
        public DateTime DateOfIssue { get; set; }
        public string IssuingAuth { get; set; } = string.Empty;
        
        [CsvIgnore]
        public int UploadId { get; set; }
        public string SignatureImagePath { get; set; } = string.Empty;
        public string PhotoImagePath { get; set; } = string.Empty;
        public DateTime DateOfCommissioning { get; set; }
        
        [CsvIgnore]
        public string PermanentAddress { get; set; } = string.Empty;
        
        [CsvIgnore]
        public byte StatusLevel { get; set; }
        public string ApplyFor { get; set; } = string.Empty;
        public string State { get; set; } = string.Empty;

        public string District { get; set; } = string.Empty;

        public string PS { get; set; } = string.Empty;

        public string PO { get; set; } = string.Empty;

        public string? Tehsil { get; set; } = string.Empty;

        public string? Village { get; set; } = string.Empty;

        public int PinCode { get; set; }


        public string IdenMark1 { get; set; } = string.Empty;

        [CsvIgnore]
        public string IdenMark2 { get; set; } = string.Empty;
        
        [CsvIgnore]
        public string? ICardType { get; set; }

        [CsvIgnore]
        public int RecordOfficeId { get; set; }
        public string? RecordOffice { get; set; }

        public int ApplId { get; set; }
        
        [CsvIgnore]
        public string RegimentalName { get; set; } = string.Empty;
        
        [CsvIgnore]
        public string RegimentalLocation { get; set; } = string.Empty;
        public string CardSerialNo { get; set; } = string.Empty;
        public string ChipNo { get; set; } = string.Empty;
    }
}