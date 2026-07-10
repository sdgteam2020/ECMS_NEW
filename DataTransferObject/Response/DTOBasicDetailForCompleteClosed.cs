using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObject.Response
{
    public class DTOBasicDetailForCompleteClosed
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
        public short RankId { get; set; }
        public string RankName { get; set; } = string.Empty;
        public string? ArmedName { get; set; }
        public int RequestId { get; set; }
        public byte ApplyForId { get; set; }
        public string ApplyFor { get; set; } = string.Empty;
        public string CardSerialNo { get; set; } = string.Empty;
        public string ChipNo { get; set; } = string.Empty;
    }
}
