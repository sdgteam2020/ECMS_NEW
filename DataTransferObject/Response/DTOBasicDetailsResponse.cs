using DataTransferObject.Constants;
using DataTransferObject.Domain.Master;
using DataTransferObject.Localize;
using DataTransferObject.Validation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObject.Response
{
    public class DTOBasicDetailsResponse
    {
        
        public int BasicDetailId { get; set; }
        public RegistrationType RegistrationType { get; set; }
        public string Name { get; set; } = string.Empty;
        public int RankId { get; set; }
        public string RankName { get; set; }
        public int ArmedId { get; set; }
        public string ArmedType { get; set; }
        public string ServiceNo { get; set; } = string.Empty;
        public string IdentityMark { get; set; } = string.Empty;
        public DateTime DOB { get; set; }
        public int Height { get; set; }
        public string AadhaarNo { get; set; } = string.Empty;
        public string BloodGroup { get; set; } = string.Empty;
        public string? PlaceOfIssue { get; set; } = string.Empty;
        public DateTime DateOfIssue { get; set; }
        public string IssuingAuth { get; set; } = string.Empty;
        public IFormFile Signature_ { get; set; }
        public string? SignatureImagePath { get; set; }
        public IFormFile Photo_ { get; set; }
        public string? PhotoImagePath { get; set; }
        public DateTime DateOfCommissioning { get; set; }
        public string PermanentAddress { get; set; } = string.Empty;
        public byte StatusLevel { get; set; }
        public int? RegimentalId { get; set; }
        public bool IsSubmit { get; set; }
        public bool success { get; set; }
        public string? message { get; set; }
        public string? EncryptedId { get; set; }
        public int Sno { get; set; }
        public int StepCounter { get; set; }
        
        public int StepId { get; set; }
        
        [Display(Name = "Request For")]
        public string? ICardType { get; set; }
        
        public int RequestId { get; set; }
    }
}
