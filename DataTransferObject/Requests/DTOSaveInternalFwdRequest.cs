using DataTransferObject.Localize;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataTransferObject.Requests
{
    public class DTOSaveInternalFwdRequest
    {
        public required int[] RequestIds { get; set; }
        
        [Range(1, int.MaxValue, ErrorMessage = "Receiver UserId must be greater than 0.")]
        [Required(ErrorMessage = "Receiver UserId is required.")]
        public int ToUserId { get; set; }

        [RegularExpression(@"^[\d]+$", ErrorMessage = "FromUserId is number.")]
        public int FromUserId { get; set; }

        [RegularExpression(@"^[\d]+$", ErrorMessage = "FromAspNetUsersId is number.")]
        public int FromAspNetUsersId { get; set; }

        
        [Range(1, int.MaxValue, ErrorMessage = "Receiver Id must be greater than 0.")]
        [Required(ErrorMessage = "Receiver Id is required.")]
        public int ToAspNetUsersId { get; set; }

        [RegularExpression(@"^[\d]+$", ErrorMessage = "UnitId is number.")]
        public int UnitId { get; set; }

        [StringLength(100)]
        [Column(TypeName = "varchar(100)")]
        [RegularExpression(@"^[\w\. ]*$", ErrorMessageResourceType = typeof(ErrorMessages), ErrorMessageResourceName = "SpecialChars")]
        public string? Remark { get; set; } = string.Empty;

        [RegularExpression(@"^[\d]+$", ErrorMessage = "FwdStatusId is number.")]
        public byte FwdStatusId { get; set; }

        [RegularExpression(@"^[\d]+$", ErrorMessage = "TypeId is number.")]
        public byte TypeId { get; set; }

        [RegularExpression(@"^[a-zA-Z]*$", ErrorMessageResourceType = typeof(ErrorMessages), ErrorMessageResourceName = "SpecialChars")]
        public bool IsComplete { get; set; } = false;

        public List<int> Remarks { get; set; } = new();

        [Column(TypeName = "varchar(100)")]
        public string? RemarksIds { get; set; }

        [RegularExpression(@"^[a-zA-Z]*$", ErrorMessageResourceType = typeof(ErrorMessages), ErrorMessageResourceName = "SpecialChars")]
        public bool IsActive { get; set; } = true;

        [RegularExpression(@"^[\d]+$", ErrorMessage = "Updatedby is number.")]
        public int? Updatedby { get; set; }
        public DateTime? UpdatedOn { get; set; }
    }
}
