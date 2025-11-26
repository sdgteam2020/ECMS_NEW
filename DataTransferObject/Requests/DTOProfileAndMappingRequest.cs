using DataTransferObject.Domain.Master;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataTransferObject.Localize;

namespace DataTransferObject.Requests
{
    public class DTOProfileAndMappingRequest
    {
        [RegularExpression(@"^[\d]+$", ErrorMessage = "UserId is number.")]
        public int UserId { get; set; }
        
        [RegularExpression(@"^[\d]+$", ErrorMessage = "TDMId is number.")]
        public int TDMId { get; set; }

        [Required(ErrorMessageResourceType = typeof(ErrorMessages), ErrorMessageResourceName = "RequiredError")]
        [MinLength(8, ErrorMessage = "Minimum length of Offr Army No is eight character.")]
        [MaxLength(10, ErrorMessage = "Maximum length of Offr Army No is ten character.")]
        [RegularExpression(@"^[\w]+$", ErrorMessageResourceType = typeof(ErrorMessages), ErrorMessageResourceName = "SpecialChars")]
        public string ArmyNo { get; set; } = string.Empty;

        [Required(ErrorMessage = "Rank is required.")]
        [RegularExpression(@"^[\d]+$", ErrorMessage = "RankId is number.")]
        public short RankId { get; set; }

        [Required(ErrorMessage = "Armed is required.")]
        [RegularExpression(@"^[\d]+$", ErrorMessage = "ArmedId is number.")]
        public byte ArmedId { get; set; }

        [Required(ErrorMessageResourceType = typeof(ErrorMessages), ErrorMessageResourceName = "RequiredError")]
        [MaxLength(50, ErrorMessageResourceType = typeof(ErrorMessages), ErrorMessageResourceName = "MaxLengthError")]
        [MinLength(1, ErrorMessageResourceType = typeof(ErrorMessages), ErrorMessageResourceName = "MinLengthError")]
        [RegularExpression(@"^[\w ]+$", ErrorMessageResourceType = typeof(ErrorMessages), ErrorMessageResourceName = "SpecialChars")]
        public string Name { get; set; } = string.Empty;


        [Required(ErrorMessageResourceType = typeof(ErrorMessages), ErrorMessageResourceName = "RequiredError")]
        [RegularExpression(@"^[\d]+$", ErrorMessage = "ApptId is number.")]
        public short ApptId { get; set; }

        [Required(ErrorMessageResourceType = typeof(ErrorMessages), ErrorMessageResourceName = "RequiredError")]
        [RegularExpression(@"^[\d]+$", ErrorMessage = "UnitMapId is number.")]
        public int UnitMapId { get; set; }

        [RegularExpression("^[a-zA-Z]*$", ErrorMessage = "Only Alphabets allowed.")]
        public bool IsRO { get; set; }

        [RegularExpression("^[a-zA-Z]*$", ErrorMessage = "Only Alphabets allowed.")]
        public bool IsIO { get; set; }

        [RegularExpression("^[a-zA-Z]*$", ErrorMessage = "Only Alphabets allowed.")]
        public bool IsCO { get; set; }

        [RegularExpression("^[a-zA-Z]*$", ErrorMessage = "Only Alphabets allowed.")]
        public bool IsORO { get; set; }
        
        [RegularExpression("^[a-zA-Z]*$", ErrorMessage = "Only Alphabets allowed.")]
        public bool IsTokenWaiver { get; set; }

        [RegularExpression(@"^[\w \.]*$", ErrorMessageResourceType = typeof(ErrorMessages), ErrorMessageResourceName = "SpecialChars")]
        [MaxLength(100, ErrorMessage = "Maximum length of Token Waiver is hundred character.")]
        public string? ReasonTokenWaiver { get; set; }

        [StringLength(50)]
        [Column(TypeName = "varchar(50)")]
        public string? Thumbprint { get; set; }
        public DateTime UpdatedOn { get; set; }

    }
}
