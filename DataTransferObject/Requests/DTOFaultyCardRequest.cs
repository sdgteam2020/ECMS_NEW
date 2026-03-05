using DataTransferObject.Domain;
using DataTransferObject.Domain.Identitytable;
using DataTransferObject.Domain.Master;
using DataTransferObject.Domain.Model;
using DataTransferObject.Localize;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObject.Requests
{
    public class DTOFaultyCardRequest
    {
        [RegularExpression("^[0-9]+$", ErrorMessage = "Numbers allowed.")]
        public int TrnFaultyCardId { get; set; }

        public List<int> RemarksIds { get; set; } = new List<int>();

        [RegularExpression(@"^[\w \&\.\-\;]*$", ErrorMessageResourceType = typeof(ErrorMessages), ErrorMessageResourceName = "SpecialChars")]
        [MaxLength(100, ErrorMessage = "Maximum length of Faulty Remark is hundred character.")]
        public string? FromRemark { get; set; }

        [RegularExpression(@"^[\w \&\.\-\;]*$", ErrorMessageResourceType = typeof(ErrorMessages), ErrorMessageResourceName = "SpecialChars")]
        [MaxLength(100, ErrorMessage = "Maximum length of Faulty Remark is hundred character.")]
        public string? ToRemark { get; set; }


        [Required(ErrorMessageResourceType = typeof(ErrorMessages), ErrorMessageResourceName = "RequiredError")]
        [Range(1, 2, ErrorMessage = "Category Id must be between 1 and 2.")]
        public byte CategoryId { get; set; }

        [Required(ErrorMessage = "Application Id is required.")]
        [RegularExpression("^[0-9]+$", ErrorMessage = "Numbers allowed.")]
        public int RequestId { get; set; }

        [RegularExpression("^[0-9]+$", ErrorMessage = "Numbers allowed.")]
        public int UserId { get; set; }

        [Required(ErrorMessage = "Choice is required.")]
        [Range(1, 3, ErrorMessage = "Choice must be between 1 and 3.")]
        public byte Choice { get; set; }

        [RegularExpression("^[0-9]+$", ErrorMessage = "Numbers allowed.")]
        public int? TrnFwdId { get; set; }
        
        [RegularExpression("^[a-zA-Z]*$", ErrorMessage = "Only Alphabets allowed.")]
        public bool IsEditAction { get; set; }

        [RegularExpression("^[a-zA-Z]*$", ErrorMessage = "Only Alphabets allowed.")]
        public bool IsComplete { get; set; } = false;

        [RegularExpression("^[a-zA-Z]*$", ErrorMessage = "Only Alphabets allowed.")]
        public bool Claim { get; set; } = false;

        [RegularExpression("^[0-9]+$", ErrorMessage = "Numbers allowed.")]
        public int UnitId { get; set; }

        [RegularExpression("^[a-zA-Z]*$", ErrorMessage = "Only Alphabets allowed.")]
        public bool IsActive { get; set; } = true;

        [RegularExpression("^[0-9]+$", ErrorMessage = "Numbers allowed.")]
        public int? Updatedby { get; set; }

        [DataType(DataType.Date)]
        [Column(TypeName = "datetime")]
        public DateTime? UpdatedOn { get; set; } = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("India Standard Time"));

    }
}
