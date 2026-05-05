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
        [Required(ErrorMessage = "Rank is required.")]
        [Range(1, short.MaxValue, ErrorMessage = "RankId must be a positive number.")]
        public short RankId { get; set; }

        [Required(ErrorMessage = "Armed is required.")]
        [Range(1, short.MaxValue, ErrorMessage = "ArmedId must be a positive number.")]
        public byte ArmedId { get; set; }

        [Required(ErrorMessageResourceType = typeof(ErrorMessages), ErrorMessageResourceName = "RequiredError")]
        [MaxLength(50, ErrorMessageResourceType = typeof(ErrorMessages), ErrorMessageResourceName = "MaxLengthError")]
        [MinLength(1, ErrorMessageResourceType = typeof(ErrorMessages), ErrorMessageResourceName = "MinLengthError")]
        [RegularExpression(@"^[A-Za-z]+(?: [A-Za-z]+)*$", ErrorMessage = "Only letters are allowed, with a single space between words.")]
        public string Name { get; set; } = string.Empty;


        [Required(ErrorMessageResourceType = typeof(ErrorMessages), ErrorMessageResourceName = "RequiredError")]
        [Range(1, short.MaxValue, ErrorMessage = "ApptId must be a positive number.")]
        public short ApptId { get; set; }

        [Required(ErrorMessageResourceType = typeof(ErrorMessages), ErrorMessageResourceName = "RequiredError")]
        [Range(1, short.MaxValue, ErrorMessage = "UnitMapId must be a positive number.")]
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
        [RegularExpression("^[a-zA-Z]*$", ErrorMessage = "Only Alphabets allowed.")]
        public string? Thumbprint { get; set; }
        public DateTime UpdatedOn { get; set; }

    }
}
