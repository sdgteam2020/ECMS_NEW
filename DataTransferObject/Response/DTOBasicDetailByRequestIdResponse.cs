using DataTransferObject.Domain.Master;
using DataTransferObject.Localize;
using DataTransferObject.Validation;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        [ScaffoldColumn(false)]
        [JsonIgnore]
        public string? FName_1 { get; set; }

        [ScaffoldColumn(false)]
        [JsonIgnore]
        public string? LName_1 { get; set; }

        [ScaffoldColumn(false)]
        [JsonIgnore]
        public string? FName_2 { get; set; }

        [ScaffoldColumn(false)]
        [JsonIgnore]
        public string? LName_2 { get; set; }
        
        [ScaffoldColumn(false)]
        [JsonIgnore]
        public string? NameAsPerRecord_1 { get; set; }

        [ScaffoldColumn(false)]
        [JsonIgnore]
        public string? NameAsPerRecord_2 { get; set; }

        [ScaffoldColumn(false)]
        [JsonIgnore]
        public string? PlaceOfIssue_1 { get; set; }

        [ScaffoldColumn(false)]
        [JsonIgnore]
        public string? PlaceOfIssue_2 { get; set; }

        [ScaffoldColumn(false)]
        [JsonIgnore]
        public DateTime? DOB_1 { get; set; }

        [ScaffoldColumn(false)]
        [JsonIgnore]
        public DateTime? DOB_2 { get; set; }

        [ScaffoldColumn(false)]
        [JsonIgnore]
        public string? AadhaarNo_1 { get; set; }

        [ScaffoldColumn(false)]
        [JsonIgnore]
        public string? AadhaarNo_2 { get; set; }

        [ScaffoldColumn(false)]
        [JsonIgnore]
        public DateTime? DateOfIssue_1 { get; set; }

        [ScaffoldColumn(false)]
        [JsonIgnore]
        public DateTime? DateOfIssue_2 { get; set; }

        [ScaffoldColumn(false)]
        [JsonIgnore]
        public string State_1 { get; set; } = string.Empty;

        [ScaffoldColumn(false)]
        [JsonIgnore]
        public string District_1 { get; set; } = string.Empty;

        [ScaffoldColumn(false)]
        [JsonIgnore]
        public string? PS_1 { get; set; }

        [ScaffoldColumn(false)]
        [JsonIgnore]
        public string? PO_1 { get; set; }

        [ScaffoldColumn(false)]
        [JsonIgnore]
        public string? Tehsil_1 { get; set; }

        [ScaffoldColumn(false)]
        [JsonIgnore]
        public string? Village_1 { get; set; }

        [ScaffoldColumn(false)]
        [JsonIgnore]
        public int? PinCode_1 { get; set; }

        [ScaffoldColumn(false)]
        [JsonIgnore]
        public string State_2 { get; set; } = string.Empty;

        [ScaffoldColumn(false)]
        [JsonIgnore]
        public string District_2 { get; set; } = string.Empty;

        [ScaffoldColumn(false)]
        [JsonIgnore]
        public string? PS_2 { get; set; }

        [ScaffoldColumn(false)]
        [JsonIgnore]
        public string? PO_2 { get; set; }

        [ScaffoldColumn(false)]
        [JsonIgnore]
        public string? Tehsil_2 { get; set; }

        [ScaffoldColumn(false)]
        [JsonIgnore]
        public string? Village_2 { get; set; }

        [ScaffoldColumn(false)]
        [JsonIgnore]
        public int? PinCode_2 { get; set; }
    }
}
