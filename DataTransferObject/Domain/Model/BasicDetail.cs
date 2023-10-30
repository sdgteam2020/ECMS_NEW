using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataTransferObject.Domain.Master;

namespace DataTransferObject.Domain.Model
{
    public class BasicDetail: Common
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int BasicDetailId { get; set; }

        [StringLength(36)]
        [Column(TypeName = "nvarchar(36)")]
        public string? Name { get; set; }

        [StringLength(50)]
        [Column(TypeName = "nvarchar(50)")]
        public string? Rank { get; set; }

        [StringLength(50)]
        [Column(TypeName = "nvarchar(50)")]
        public string? ArmService { get; set; }

        [Index("IX_BasicDetails_ServiceNo", IsClustered = false, IsUnique = true, Order = 1)]
        [StringLength(50)]
        [Column(TypeName = "nvarchar(50)")]
        public string? ServiceNo { get; set; }

        [StringLength(20)]
        [Column(TypeName = "nvarchar(20)")]
        public string? IdentityMark { get; set; }
        public DateTime? DOB { get; set; }
        public decimal? Height { get; set; }

        [StringLength(12)]
        [Column(TypeName = "nvarchar(12)")]
        public string? AadhaarNo { get; set; }

        [StringLength(3)]
        [Column(TypeName = "nvarchar(3)")]
        public string? BloodGroup { get; set; }

        [StringLength(50)]
        [Column(TypeName = "nvarchar(50)")]
        public string? PlaceOfIssue { get; set; }
        public DateTime? DateOfIssue { get; set; }

        [StringLength(50)]
        [Column(TypeName = "nvarchar(50)")]
        public string? IssuingAuth { get; set; }

        [StringLength(200)]
        [Column(TypeName = "nvarchar(200)")]
        public string? SignatureImagePath { get; set; }

        [StringLength(200)]
        [Column(TypeName = "nvarchar(200)")]
        public string? PhotoImagePath { get; set; }
        public DateTime? DateOfCommissioning { get; set; }

        [StringLength(100)]
        [Column(TypeName = "nvarchar(100)")]
        public string? PermanentAddress { get; set; }
        public bool IsDeleted { get; set; }

        public int Step { get; set; }
        public bool IsSubmit { get; set; }
        public int? DistrictId { get; set; }
        public District? District { get; set; }

        [NotMapped]
        public bool success { get; set; }

        [NotMapped]
        public string? message { get; set; } = string.Empty;

        [NotMapped]
        public string? EncryptedId { get; set; }
    }
}
