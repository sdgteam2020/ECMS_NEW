using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace DataTransferObject.Response
{
    public class DTOGetICardPrintPreviewByRequestIdResponse
    {
        public string FName { get; set; } = string.Empty;
        public string? LName { get; set; }
        public string? RankName { get; set; }
        public string? ArmedName { get; set; }
        public string ServiceNo { get; set; } = string.Empty;

        [DataType(DataType.Date)]
        public DateTime DOB { get; set; }
        public int Height { get; set; }
        public string AadhaarNo { get; set; } = string.Empty;
        public string? BloodGroup { get; set; }
        public string? PlaceOfIssue { get; set; }
        public DateTime DateOfIssue { get; set; }
        public string SignatureImagePath { get; set; } = string.Empty;
        public string PhotoImagePath { get; set; } = string.Empty;
        
        [DataType(DataType.Date)]
        public DateTime DateOfCommissioning { get; set; }
        public string IdenMark1 { get; set; } = string.Empty;
        public string IssuingAuthorityName { get; set; } = string.Empty;
        public string? ExistingSignatureInBase64 { get; set; }
        public string? ExistingPhotoInBase64 { get; set; }

    }
}
