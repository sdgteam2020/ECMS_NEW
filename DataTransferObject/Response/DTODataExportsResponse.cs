using DataTransferObject.Domain.Master;
using DataTransferObject.Localize;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataTransferObject.Validation;
using Microsoft.AspNetCore.Http;

namespace DataTransferObject.Response
{
    public class DTODataExportsResponse
    {

        public string PaperIcardNo { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string? RankName { get; set; }
        public string? ArmedName { get; set; }
        public string ServiceNo { get; set; } = string.Empty;
        public DateTime DOB { get; set; }
        public int Height { get; set; }
        public string AadhaarNo { get; set; } = string.Empty;
        public string BloodGroup { get; set; }
        public string PlaceOfIssue { get; set; } = string.Empty;
        public DateTime DateOfIssue { get; set; }
        public string IssuingAuth { get; set; } = string.Empty;
        public int UploadId { get; set; }
        //public string SignatureImagePath { get; set; } = string.Empty;
        //public string PhotoImagePath { get; set; } = string.Empty;
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



    }
}