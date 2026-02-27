using DataTransferObject.Localize;
using DataTransferObject.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObject.Requests
{
    public class DTOTrnDestructionCardSaveRequest
    {

        [Range(1, int.MaxValue, ErrorMessage = "Invalid RequestId")]
        public int RequestId { get; set; }
        public List<int> RemarksIds { get; set; } = new List<int>();

        [RegularExpression(@"^[\w \.]*$", ErrorMessageResourceType = typeof(ErrorMessages), ErrorMessageResourceName = "SpecialChars")]
        [MaxLength(100, ErrorMessage = "Maximum length of Remark is hundred character.")]
        public string? Remark { get; set; }

        [Required(ErrorMessage = "Destruction date is required.")]
        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy HH:mm}", ApplyFormatInEditMode = true)]
        [DestructionDateValidationAttribute]
        public DateTime? DestructedOn { get; set; }
    }

}
