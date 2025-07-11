using DataTransferObject.Domain.Master;
using DataTransferObject.Domain.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataTransferObject.Domain;
using DataTransferObject.Localize;

namespace DataTransferObject.Requests
{
    public class DTOFaultyCardRequest : Common
    {
        [RegularExpression(@"^[\d]+$", ErrorMessageResourceType = typeof(ErrorMessages), ErrorMessageResourceName = "OnlyNumber")]
        public int TrnFaultyCardId { get; set; }

        [MaxLength(100, ErrorMessage = "Maximum length of RemarksIds is hundred character.")]
        public string RemarksIds { get; set; } = string.Empty;

        [RegularExpression(@"^[\w \&\.\-\;]*$", ErrorMessageResourceType = typeof(ErrorMessages), ErrorMessageResourceName = "SpecialChars")]
        [MaxLength(100, ErrorMessage = "Maximum length of Faulty Remark is hundred character.")]
        public string? FromRemark { get; set; }

        [RegularExpression(@"^[\w \&\.\-\;]*$", ErrorMessageResourceType = typeof(ErrorMessages), ErrorMessageResourceName = "SpecialChars")]
        [MaxLength(100, ErrorMessage = "Maximum length of Faulty Remark is hundred character.")]
        public string? ToRemark { get; set; }


        [Required(ErrorMessageResourceType = typeof(ErrorMessages), ErrorMessageResourceName = "RequiredError")]
        [RegularExpression(@"^[\d]+$", ErrorMessageResourceType = typeof(ErrorMessages), ErrorMessageResourceName = "OnlyNumber")]
        public byte CategoryId { get; set; }
        public MCategory? MCategory { get; set; }

        [Required(ErrorMessageResourceType = typeof(ErrorMessages), ErrorMessageResourceName = "RequiredError")]
        [RegularExpression(@"^[\d]+$", ErrorMessageResourceType = typeof(ErrorMessages), ErrorMessageResourceName = "OnlyNumber")]
        public int RequestId { get; set; }
        public MTrnICardRequest? MTrnICardRequest { get; set; }

        [RegularExpression(@"^[\d]+$", ErrorMessageResourceType = typeof(ErrorMessages), ErrorMessageResourceName = "OnlyNumber")]
        public int UserId { get; set; }
        public MUserProfile? MUserProfile { get; set; }

        [RegularExpression(@"^[\d]+$", ErrorMessageResourceType = typeof(ErrorMessages), ErrorMessageResourceName = "OnlyNumber")]
        public byte Choice { get; set; }
        public int TrnFwdId { get; set; }

        public bool IsEditAction { get; set; }
        public bool IsComplete { get; set; } = false;

    }
}
