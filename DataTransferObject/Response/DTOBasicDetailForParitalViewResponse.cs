using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        public string? RegimentalName { get; set; }
        public string UnitName { get; set; } = string.Empty;
        public string RankName { get; set; } = string.Empty;
        public string? ArmedName { get; set; }
        public int RequestId { get; set; }
        public DateTime RequestDate { get; set; }
        public string ApplyFor { get; set; } = string.Empty;
        public string PhotoImagePath { get; set; } = string.Empty;
        public string SignatureImagePath { get; set; } = string.Empty;
        public string? ModifiedServiceNo { get; set; }
        public string CardSerialNo { get; set; } = string.Empty;
        public string ChipNo { get; set; } = string.Empty;

        public string? FName_1 { get; set; }
        public string? LName_1 { get; set; }

        public string? FName_2 { get; set; }
        public string? LName_2 { get; set; }

        public string? NameAsPerRecord_1 { get; set; }
        public string? NameAsPerRecord_2 { get; set; }

        public string? PlaceOfIssue_1 { get; set; }
        public string? PlaceOfIssue_2 { get; set; }

        public DateTime? DOB_1 { get; set; }
        public DateTime? DOB_2 { get; set; }

        public string? AadhaarNo_1 { get; set; }
        public string? AadhaarNo_2 { get; set; }

        public DateTime? DateOfIssue_1 { get; set; }
        public DateTime? DateOfIssue_2 { get; set; }

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
