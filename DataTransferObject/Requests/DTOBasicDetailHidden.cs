using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObject.Requests
{
    public class DTOBasicDetailHidden
    {
        public int BasicDetailId { get; set; }
        public string NameAsPerRecord { get; set; } = string.Empty;
        public string? ExistingPhotoImagePath { get; set; }
        public string? ExistingPhotoInBase64 { get; set; }
        public string? ExistingSignatureImagePath { get; set; }
        public string? ExistingSignatureInBase64 { get; set; }
        public DateTime DOB { get; set; }
        public DateTime DateOfCommissioning { get; set; }
        public string? EncryptedId { get; set; }
        public byte ApplyForId { get; set; }
        public string? OldServiceNo { get; set; } = string.Empty;
        public int? PreviousBasicDetailId { get; set; }
        public byte RegistrationId { get; set; }
        public byte TypeId { get; set; }
        public string State { get; set; } = string.Empty;
        public string District { get; set; } = string.Empty;
        public string? PS { get; set; }
        public string? PO { get; set; }
        public string? Tehsil { get; set; }
        public string? Village { get; set; }
        public int? PinCode { get; set; }
        public string? IdenMark2 { get; set; } = string.Empty;
        public int AddressId { get; set; }
        public int UploadId { get; set; }
        public int InfoId { get; set; }
    }
}
