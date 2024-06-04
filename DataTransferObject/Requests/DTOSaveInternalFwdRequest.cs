using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataTransferObject.Domain.Identitytable;
using DataTransferObject.Localize;

namespace DataTransferObject.Requests
{
    public class DTOSaveInternalFwdRequest
    {
        //[RegularExpression(@"^[\w\,\'' ]*$", ErrorMessageResourceType = typeof(ErrorMessages), ErrorMessageResourceName = "SpecialChars")]
        public required int[] TrnFwdIds { get; set; }
        
        [RegularExpression(@"^[\d]+$", ErrorMessage = "ToUserId is number.")]
        public int ToUserId { get; set; }

        [RegularExpression(@"^[\d]+$", ErrorMessage = "FromUserId is number.")]
        public int FromUserId { get; set; }

        [RegularExpression(@"^[\d]+$", ErrorMessage = "FromAspNetUsersId is number.")]
        public int? FromAspNetUsersId { get; set; }

        [RegularExpression(@"^[\d]+$", ErrorMessage = "ToAspNetUsersId is number.")]
        public int? ToAspNetUsersId { get; set; }

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

        [Column(TypeName = "varchar(100)")]
        [RegularExpression(@"^[\w\,]*$", ErrorMessageResourceType = typeof(ErrorMessages), ErrorMessageResourceName = "SpecialChars")]
        public string? RemarksIds { get; set; }

        [RegularExpression(@"^[a-zA-Z]*$", ErrorMessageResourceType = typeof(ErrorMessages), ErrorMessageResourceName = "SpecialChars")]
        public bool IsActive { get; set; } = true;

        [RegularExpression(@"^[\d]+$", ErrorMessage = "Updatedby is number.")]
        public int? Updatedby { get; set; }
        public DateTime? UpdatedOn { get; set; }
    }
}
