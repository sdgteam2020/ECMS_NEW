using DataTransferObject.Domain.Master;
using DataTransferObject.Localize;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObject.Domain
{
    public class MUserProfile:Common
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int UserId { get; set; }

        [StringLength(20)]
        [Column(TypeName = "varchar(20)")]
        [Required(ErrorMessageResourceType = typeof(ErrorMessages), ErrorMessageResourceName = "RequiredError")]
        [MinLength(7, ErrorMessageResourceType = typeof(ErrorMessages), ErrorMessageResourceName = "MinLengthError")]
        [MaxLength(15, ErrorMessageResourceType = typeof(ErrorMessages), ErrorMessageResourceName = "MaxLengthError")]
        [RegularExpression(@"^[\w\-]+$", ErrorMessageResourceType = typeof(ErrorMessages), ErrorMessageResourceName = "SpecialChars")]
        public string ArmyNo { get; set; } = string.Empty;
        
        [Required]
        [ForeignKey("MRank")]
        public short RankId { get; set; }
        public MRank? MRank { get; set; }


        [StringLength(50)]
        [Column(TypeName = "varchar(50)")]
        [Required(ErrorMessageResourceType = typeof(ErrorMessages), ErrorMessageResourceName = "RequiredError")]
        [MinLength(1, ErrorMessageResourceType = typeof(ErrorMessages), ErrorMessageResourceName = "MinLengthError")]
        [MaxLength(50, ErrorMessageResourceType = typeof(ErrorMessages), ErrorMessageResourceName = "MaxLengthError")]
        [RegularExpression(@"^[\w ]+$", ErrorMessageResourceType = typeof(ErrorMessages), ErrorMessageResourceName = "SpecialChars")]
        public string Name { get; set; } = string.Empty;

        public Boolean IntOffr { get; set; }
        public Boolean IsIO { get; set; }
        public Boolean IsCO { get; set; }

        [StringLength(10)]
        [Column(TypeName = "varchar(10)")]
        public string? MobileNo { get; set; }
        
        
        [StringLength(6)]
        [Column(TypeName = "varchar(6)")]
        public string? DialingCode { get; set; }

        [StringLength(5)]
        [Column(TypeName = "varchar(5)")]
        public string? Extension { get; set; }

        [StringLength(50)]
        [Column(TypeName = "varchar(50)")]
        public string? Thumbprint { get; set; }
    }
}
