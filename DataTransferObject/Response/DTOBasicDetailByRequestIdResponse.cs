using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace DataTransferObject.Response
{
    public class DTOBasicDetailByRequestIdResponse
    {
        public int BasicDetailId { get; set; }
        public string FName { get; set; } = string.Empty;
        public string? LName { get; set; }
        public string NameAsPerRecord { get; set; } = string.Empty;

        [JsonIgnore]
        public short RankId { get; set; }
        public string? RankName { get; set; }

        [JsonIgnore]
        public byte ArmedId { get; set; }
        public string? ArmedName { get; set; }
        public string ServiceNo { get; set; } = string.Empty;

        [DataType(DataType.Date)]
        public DateTime DOB { get; set; }
        public int Height { get; set; }
        public string AadhaarNo { get; set; } = string.Empty;

        [ScaffoldColumn(false)]
        [JsonIgnore]
        public byte BloodGroupId { get; set; }
        public string? BloodGroup { get; set; }
        public string? PlaceOfIssue { get; set; }
        public DateTime DateOfIssue { get; set; }

        [ScaffoldColumn(false)]
        [JsonIgnore]
        public int UploadId { get; set; }
        public string SignatureImagePath { get; set; } = string.Empty;
        public string PhotoImagePath { get; set; } = string.Empty;
        [DataType(DataType.Date)]
        public DateTime DateOfCommissioning { get; set; }
        public string PermanentAddress { get; set; } = string.Empty;
        public byte? RegimentalId { get; set; }
        public byte ApplyForId { get; set; }
        public byte RegistrationId { get; set; }
        public byte TypeId { get; set; }

        [ScaffoldColumn(false)]
        [JsonIgnore]
        public int AddressId { get; set; }
        public string State { get; set; } = string.Empty;
        public string District { get; set; } = string.Empty;
        public string? PS { get; set; }
        public string? PO { get; set; }
        public string? Tehsil { get; set; }
        public string? Village { get; set; }
        public int? PinCode { get; set; }

        [ScaffoldColumn(false)]
        [JsonIgnore]
        public int InfoId { get; set; }
        public string IdenMark1 { get; set; } = string.Empty;
        public string? IdenMark2 { get; set; } = string.Empty;

        [ScaffoldColumn(false)]
        [JsonIgnore]
        public int UnitId { get; set; }
        public string IssuingAuthorityName { get; set; } = string.Empty;
        public string UnitName { get; set; } = string.Empty;
        public string? RegimentalName { get; set; }
        public string? ExistingSignatureInBase64 { get; set; }
        public string? ExistingPhotoInBase64 { get; set; }
    }
}
