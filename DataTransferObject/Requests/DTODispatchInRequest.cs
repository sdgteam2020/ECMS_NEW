using DataTransferObject.Localize;
using System.ComponentModel.DataAnnotations;

namespace DataTransferObject.Requests
{
    public class DTODispatchInRequest
    {
        [RegularExpression(@"^[\d]+$", ErrorMessageResourceType = typeof(ErrorMessages), ErrorMessageResourceName = "OnlyNumber")]
        public int DispatchCardId { get; set; }

        [RegularExpression(@"^[\w \.]*$", ErrorMessageResourceType = typeof(ErrorMessages), ErrorMessageResourceName = "SpecialChars")]
        [MaxLength(100, ErrorMessage = "Maximum length of Remark is hundred character.")]
        public string ToRemark { get; set; }=string.Empty;
        
        public DateTime ReceiptDate { get; set; }

        [RegularExpression("^[a-zA-Z]*$", ErrorMessage = "Only Alphabets allowed.")]
        public bool IsComplete { get; set; } = false;

        [RegularExpression("^[0-9]+$", ErrorMessage = "Numbers allowed.")]
        public byte ClaimValue { get; set; }

        [RegularExpression("^[0-9]+$", ErrorMessage = "Numbers allowed.")]
        public int UnitId { get; set; }

        [RegularExpression("^[0-9]+$", ErrorMessage = "Numbers allowed.")]
        public int TDMId { get; set; }

        [RegularExpression(@"^[\d]+$", ErrorMessage = "StepId is number.")]
        public byte StepId { get; set; }

        [RegularExpression(@"^[\d]+$", ErrorMessage = "RegId is number.")]
        public byte? RegId { get; set; }

        [RegularExpression(@"^[\d]+$", ErrorMessage = "RecordOfficeId is number.")]
        public byte? RecordOfficeId { get; set; }
    }
}
