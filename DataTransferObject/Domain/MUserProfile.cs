using DataTransferObject.Domain.Master;
using DataTransferObject.Localize;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataTransferObject.Domain
{
    public class MUserProfile:Common
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [RegularExpression(@"^[\d]+$", ErrorMessage = "UserId is number.")]
        public int UserId { get; set; }

        [StringLength(10)]
        [Column(TypeName = "varchar(10)")]
        [Required(ErrorMessageResourceType = typeof(ErrorMessages), ErrorMessageResourceName = "RequiredError")]
        [MinLength(7, ErrorMessage = "Minimum length of Offr Army No is seven alphanumeric.")]
        [MaxLength(9,ErrorMessage = "Maximum length of Offr Army No is nine alphanumeric.")]
        [RegularExpression(@"^(?:[A-Z]{1,2}[0-9]{5,6}|[A-Z]{3}[0-9]{5})[A-Z]$",ErrorMessage = "Please enter a valid Offr Army No. Examples: V12345X, IC123456L, NTR12345X.")]
        public string ArmyNo { get; set; } = string.Empty;
        
        [Required]
        [ForeignKey("MRank")]
        [RegularExpression(@"^[\d]+$", ErrorMessage = "RankId is number.")]
        public short RankId { get; set; }
        public MRank? MRank { get; set; }


        [StringLength(50)]
        [Column(TypeName = "varchar(50)")]
        [Required(ErrorMessageResourceType = typeof(ErrorMessages), ErrorMessageResourceName = "RequiredError")]
        [MinLength(1, ErrorMessageResourceType = typeof(ErrorMessages), ErrorMessageResourceName = "MinLengthError")]
        [MaxLength(50, ErrorMessageResourceType = typeof(ErrorMessages), ErrorMessageResourceName = "MaxLengthError")]
        [RegularExpression(@"^[A-Za-z]+(?: [A-Za-z]+)*$", ErrorMessage = "Only letters are allowed, with a single space between words.")]
        public string Name { get; set; } = string.Empty;


        [StringLength(50)]
        [Column(TypeName = "varchar(50)")]
        [RegularExpression(@"^[\w]+$", ErrorMessageResourceType = typeof(ErrorMessages), ErrorMessageResourceName = "SpecialChars")]
        public string? Thumbprint { get; set; }
        public bool IsToken { get; set; }=true;
        
        [RegularExpression(@"^[\d]+$", ErrorMessage = "ArmedId is number.")]
        public byte ArmedId { get; set; }
        public MArmedType? Armed { get; set; }
        
        [Column(TypeName = "varchar(100)")]
        [MaxLength(100, ErrorMessageResourceType = typeof(ErrorMessages), ErrorMessageResourceName = "MaxLengthError")]
        [RegularExpression(@"^[\w\&\.\-\; ]*$", ErrorMessageResourceType = typeof(ErrorMessages), ErrorMessageResourceName = "SpecialChars")]
        public string? ReasonTokenWaiver { get; set; }
        public bool IsTokenWaiver { get; set; } = false;
        public bool IsWithTokenApply { get; set; } = true;
    }
}
