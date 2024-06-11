using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObject.Response
{
    public class DTOXMLDigitalResponse
    {
        public DTOXMLDigitalSignResponse? Header { get; set; }
    }
    public class DTOXMLDigitalSignResponse
    {

        public ApplicationDetails? applicationDetails { get; set; }
        public Profiledtls? profiledtls { get; set; }

    }
    public class ApplicationDetails
    {
        public string PaperIcardNo { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string? RankName { get; set; }
        public string? ArmedName { get; set; }
        public string ServiceNo { get; set; } = string.Empty;
        public DateTime DOB { get; set; }
        public string? Height { get; set; }
        public string AadhaarNo { get; set; } = string.Empty;
        public string? BloodGroup { get; set; }
        public string? RegimentalName { get; set; }
        public string? UnitName { get; set; }
        public string PlaceOfIssue { get; set; } = string.Empty;
        public DateTime? DateOfIssue { get; set; }
        public string IssuingAuth { get; set; } = string.Empty;
        public int UploadId { get; set; }
        public string SignatureImagePath { get; set; } = string.Empty;
        public string PhotoImagePath { get; set; } = string.Empty;
        public DateTime DateOfCommissioning { get; set; }
        public string PermanentAddress { get; set; } = string.Empty;
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
        public string IdenMark2 { get; set; } = string.Empty;
        public string? ICardType { get; set; }
        public DateTime XmlCreatedOn { get; set; }


    }
    public class Profiledtls
    {
        public string? ProApplyFor { get; set; }
        public string? ProRegistraion { get; set; }
        public string? ProType { get; set; }
        public string? ProDomainId { get; set; }
        public string? ProUnitName { get; set; }
        public string? ProSuffix { get; set; }
        public string? ProSUSNO { get; set; }
        public string? ProName { get; set; }
        public string? ProRankName { get; set; }
        public string? ProArmyName { get; set; }
    }
}
