using DataTransferObject.Localize;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataTransferObject.Requests
{
    public class DTOActionOnRequest
    {
        //For Step Counter Update fields
        [Required(ErrorMessage = "RequestId is required.")]
        [RegularExpression(@"^[\d]+$", ErrorMessage = "RequestId is number.")]
        public int RequestId { get; set; }

        [RegularExpression(@"^[\d]+$", ErrorMessage = "StepId is number.")]
        public byte StepId { get; set; }

        [RegularExpression(@"^[\d]+$", ErrorMessage = "ApplyForId is number.")]
        public byte ApplyForId { get; set; }
        public string UnitName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Please select choice Fwd oblique Reject is required.")]
        [MaxLength(1, ErrorMessage = "Maximum length of Flag is one character.")]
        public string Flag { get; set; } = string.Empty;
        public int Updatedby { get; set; }
        public DateTime UpdatedOn { get; set; } = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("India Standard Time"));

        //For Forwarding/Reject Action fields
        [RegularExpression(@"^[\d]+$", ErrorMessage = "TrnFwdId is number.")]
        public int TrnFwdId { get; set; }

        [RegularExpression(@"^[\d]+$", ErrorMessage = "ToUserId is number.")]
        public int ToUserId { get; set; }

        [RegularExpression(@"^[\d]+$", ErrorMessage = "FromUserId is number.")]
        public int FromUserId { get; set; }

        [RegularExpression(@"^[\d]+$", ErrorMessage = "FromAspNetUsersId is number.")]
        public int FromAspNetUsersId { get; set; }

        [RegularExpression(@"^[\d]+$", ErrorMessage = "ToAspNetUsersId is number.")]
        public int ToAspNetUsersId { get; set; }
        
        [RegularExpression(@"^[\d]+$", ErrorMessage = "ToAspNetUsersId is number.")]
        public int UnitId { get; set; }

        [StringLength(100)]
        [Column(TypeName = "varchar(100)")]
        [RegularExpression(@"^[\w\. ]*$", ErrorMessageResourceType = typeof(ErrorMessages), ErrorMessageResourceName = "SpecialChars")]
        public string? Remark { get; set; } = string.Empty;

        [RegularExpression(@"^[\d]+$", ErrorMessage = "FwdStatusId is number.")]
        public byte FwdStatusId { get; set; }
        
        [RegularExpression(@"^[\d]+$", ErrorMessage = "TypeId is number.")]
        public byte TypeId { get; set; }

        [RegularExpression("^[a-zA-Z]*$", ErrorMessage = "Only Alphabets allowed.")]
        public bool IsComplete { get; set; } = false;

        [Column(TypeName = "varchar(100)")]
        [RegularExpression(@"^[\w\,]*$", ErrorMessageResourceType = typeof(ErrorMessages), ErrorMessageResourceName = "SpecialChars")]
        public string? RemarksIds { get; set; }

        [RegularExpression("^[a-zA-Z]*$", ErrorMessage = "Only Alphabets allowed.")]
        public bool IsActive { get; set; } = true;

        [RegularExpression("^[a-zA-Z]*$", ErrorMessage = "Only Alphabets allowed.")]
        public bool IsLock { get; set; }
    }
}
