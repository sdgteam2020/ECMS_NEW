using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataTransferObject.Domain.Master;
using DataTransferObject.Localize;

namespace DataTransferObject.Domain.Model
{
    public class BasicDetailTemp:Common
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int BasicDetailTempId { get; set; }

        [StringLength(36)]
        [Column(TypeName = "varchar(36)")]
        public string Name { get; set; } = string.Empty;

        [StringLength(10)]
        [Column(TypeName = "varchar(10)")]
        [Index("IX_BasicDetails_ServiceNo", IsClustered = false, IsUnique = true, Order = 1)]
        public string ServiceNo { get; set; } = string.Empty;

        public DateTime DOB { get; set; }
        
        [Display(Name = "Rank", ResourceType = typeof(Resource))]
        [Required(ErrorMessageResourceType = typeof(ErrorMessages), ErrorMessageResourceName = "RequiredError")]
        [RegularExpression(@"^[\d]+$", ErrorMessage = "RankId is number.")]
        [ForeignKey("MRank")]
        public short RankId { get; set; }
        
        public MRank? Rank { get; set; }

        public DateTime DateOfCommissioning { get; set; }

        [StringLength(50)]
        [Column(TypeName = "varchar(50)")]
        public string? State { get; set; }

        [StringLength(50)]
        [Column(TypeName = "varchar(50)")]
        public string? District { get; set; }

        [StringLength(50)]
        [Column(TypeName = "varchar(50)")]
        public string? PS { get; set; }

        [StringLength(50)]
        [Column(TypeName = "varchar(50)")]
        public string? PO { get; set; }

        [StringLength(50)]
        [Column(TypeName = "varchar(50)")]
        public string? Tehsil { get; set; }

        [StringLength(50)]
        [Column(TypeName = "varchar(50)")]
        public string? Village { get; set; }

        [RegularExpression(@"^[\d]+$", ErrorMessage = "PinCode is number.")]
        public int? PinCode { get; set; }


        [StringLength(100)]
        [Column(TypeName = "varchar(100)")]
        public string? Observations { get; set; } = string.Empty;

        [NotMapped]
        public string? EncryptedId { get; set; }

        [StringLength(100)]
        [Column(TypeName = "varchar(100)")]
        public string RemarksIds { get; set; } = string.Empty;

        [ForeignKey("MApplyFor"), DatabaseGenerated(DatabaseGeneratedOption.None)]
        public byte ApplyForId { get; set; }
        public MApplyFor? MApplyFor { get; set; }

        [ForeignKey("Registration"), DatabaseGenerated(DatabaseGeneratedOption.None)]
        public byte RegistrationId { get; set; }
        public MRegistration? Registration { get; set; }

        [ForeignKey("MICardType"), DatabaseGenerated(DatabaseGeneratedOption.None)]
        public byte TypeId { get; set; }
        public MICardType? MICardType { get; set; }
    }
}
