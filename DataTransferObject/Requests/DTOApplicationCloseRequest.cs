using DataTransferObject.Localize;
using System.ComponentModel.DataAnnotations;

namespace DataTransferObject.Requests
{
    public class DTOApplicationCloseRequest
    {
        [RegularExpression(@"^[\d]+$", ErrorMessage = "Id is number.")]
        public int Id { get; set; }

        [RegularExpression(@"^[\d]+$", ErrorMessage = "BasicDetailId is number.")]
        public int BasicDetailId { get; set; }

        [RegularExpression(@"^[\d]+$", ErrorMessage = "ReasonId is number.")]
        [Required(ErrorMessage = "ReasonId is required.")]
        public byte ReasonId { get; set; }

        [RegularExpression(@"^[\w\. ]*$", ErrorMessageResourceType = typeof(ErrorMessages), ErrorMessageResourceName = "SpecialChars")]
        [Required(ErrorMessage = "Authority is required.")]
        public string Authority { get; set; } = string.Empty;

        [RegularExpression(@"^[\w\. ]*$", ErrorMessageResourceType = typeof(ErrorMessages), ErrorMessageResourceName = "SpecialChars")]
        [Required(ErrorMessage = "Remarks is required.")]
        public string Remarks { get; set; } = string.Empty;

        [RegularExpression(@"^[\d]+$", ErrorMessage = "RequestId is number.")]
        [Required(ErrorMessage = "RequestId is required.")]
        public int RequestId { get; set; }

        [RegularExpression(@"^[\d]+$", ErrorMessage = "UserId is number.")]
        public int UserId { get; set; }

        [RegularExpression("^[a-zA-Z]*$", ErrorMessage = "Only Alphabets allowed.")]
        public bool IsActive { get; set; }

        [RegularExpression(@"^[\d]+$", ErrorMessage = "Updatedby is number.")]
        public int Updatedby { get; set; }

        public DateTime UpdatedOn { get; set; }

        [RegularExpression(@"^[\d]+$", ErrorMessage = "UnitId is number.")]
        public int UnitId { get; set; }

    }
}
