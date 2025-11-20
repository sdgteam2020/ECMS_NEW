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
        public bool IsComplete { get; set; } = false;
    }
}
