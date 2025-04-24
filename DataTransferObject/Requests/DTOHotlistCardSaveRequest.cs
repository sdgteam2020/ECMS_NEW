using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataTransferObject.Domain;
using DataTransferObject.Localize;

namespace DataTransferObject.Requests
{
    public class DTOHotlistCardSaveRequest : Common
    {
        [Required(ErrorMessageResourceType = typeof(ErrorMessages), ErrorMessageResourceName = "RequiredError")]
        [RegularExpression(@"^[\d]+$", ErrorMessageResourceType = typeof(ErrorMessages), ErrorMessageResourceName = "OnlyNumber")]
        public int RequestId { get; set; }
        [MaxLength(100, ErrorMessage = "Maximum length of RemarksIds is hundred character.")]
        public string RemarksIds { get; set; } = string.Empty;

        [RegularExpression(@"^[\w \.]*$", ErrorMessageResourceType = typeof(ErrorMessages), ErrorMessageResourceName = "SpecialChars")]
        [MaxLength(100, ErrorMessage = "Maximum length of Faulty Remark is hundred character.")]
        public string? Remark { get; set; }
    }
}
