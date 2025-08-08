using DataTransferObject.Domain.Identitytable;
using DataTransferObject.Domain.Master;
using DataTransferObject.Domain;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataTransferObject.Localize;
using DataTransferObject.Validation;
using Microsoft.AspNetCore.Http;

namespace DataTransferObject.Requests
{
    public class DTODispatchOutRequestWithoutIFormFile
    {
        [RegularExpression(@"^[\d]+$", ErrorMessageResourceType = typeof(ErrorMessages), ErrorMessageResourceName = "OnlyNumber")]
        public int DispatchCardId { get; set; }

        [RegularExpression(@"^[\d]+$", ErrorMessageResourceType = typeof(ErrorMessages), ErrorMessageResourceName = "OnlyNumber")]
        [Range(1, 2, ErrorMessage = "Step must be between 1 and 2.")]
        public byte Step { get; set; } // 1 For AFSAC AND 2 FOR Regiment / Record

        [RegularExpression(@"^[\d]+$", ErrorMessageResourceType = typeof(ErrorMessages), ErrorMessageResourceName = "OnlyNumber")]
        [Range(1, 2, ErrorMessage = "ApplyForId must be between 1 and 2.")]
        public byte ApplyForId { get; set; }

        [RegularExpression(@"^[\d]+$", ErrorMessageResourceType = typeof(ErrorMessages), ErrorMessageResourceName = "OnlyNumber")]
        public byte? RegId { get; set; }

        [RegularExpression(@"^[\d]+$", ErrorMessageResourceType = typeof(ErrorMessages), ErrorMessageResourceName = "OnlyNumber")]
        public byte? RecordOfficeId { get; set; }
        public DateTime OutDate { get; set; }
        public DateTime? ReceiptDate { get; set; }
        public DateTime DispatchDate { get; set; }

        [RegularExpression(@"^[\d]+$", ErrorMessageResourceType = typeof(ErrorMessages), ErrorMessageResourceName = "OnlyNumber")]
        public byte DispatchModeId { get; set; }

        [RegularExpression(@"^[\w \.]*$", ErrorMessageResourceType = typeof(ErrorMessages), ErrorMessageResourceName = "SpecialChars")]
        [MaxLength(50, ErrorMessage = "Maximum length of Ref Of Dispatch is fifty character.")]
        public string RefOfDispatch { get; set; } = string.Empty;

        [RegularExpression(@"^[\w \.]*$", ErrorMessageResourceType = typeof(ErrorMessages), ErrorMessageResourceName = "SpecialChars")]
        [MaxLength(50, ErrorMessage = "Maximum length of Name Of Courier Incharge is fifty character.")]
        public string NameOfCourierIncharge { get; set; } = string.Empty;

        [StringLength(100)]
        [Column(TypeName = "VARCHAR(100)")]
        public string UploadFilePath { get; set; } = string.Empty;

        [RegularExpression(@"^[\w \.]*$", ErrorMessageResourceType = typeof(ErrorMessages), ErrorMessageResourceName = "SpecialChars")]
        [MaxLength(100, ErrorMessage = "Maximum length of Remark is hundred character.")]
        public string? FromRemark { get; set; }

        [RegularExpression(@"^[\w \.]*$", ErrorMessageResourceType = typeof(ErrorMessages), ErrorMessageResourceName = "SpecialChars")]
        [MaxLength(100, ErrorMessage = "Maximum length of Remark is hundred character.")]
        public string? ToRemark { get; set; }

        [RegularExpression(@"^[\d]+$", ErrorMessageResourceType = typeof(ErrorMessages), ErrorMessageResourceName = "OnlyNumber")]
        public int FromUnitId { get; set; }

        [RegularExpression(@"^[\d]+$", ErrorMessageResourceType = typeof(ErrorMessages), ErrorMessageResourceName = "OnlyNumber")]
        public int ToUnitId { get; set; }

        [RegularExpression(@"^[\d]+$", ErrorMessageResourceType = typeof(ErrorMessages), ErrorMessageResourceName = "OnlyNumber")]
        public int ToUserId { get; set; }

        [RegularExpression(@"^[\d]+$", ErrorMessageResourceType = typeof(ErrorMessages), ErrorMessageResourceName = "OnlyNumber")]
        public int FromUserId { get; set; }

        [RegularExpression(@"^[\d]+$", ErrorMessageResourceType = typeof(ErrorMessages), ErrorMessageResourceName = "OnlyNumber")]
        public int FromAspNetUsersId { get; set; }
        
        [RegularExpression(@"^[\d]+$", ErrorMessageResourceType = typeof(ErrorMessages), ErrorMessageResourceName = "OnlyNumber")]
        public int ToAspNetUsersId { get; set; }
        public bool IsComplete { get; set; } = false;
        public bool IsActive { get; set; } = true;
        public int Updatedby { get; set; }
        public DateTime UpdatedOn { get; set; }


    }
    public class DTODispatchOutRequest : DTODispatchOutRequestWithoutIFormFile
    {
        [Required(ErrorMessage = "File is required!")]
        [SecureFile(allowedExtensions: new[] { ".csv" },
        allowedMimeTypes: new[] { "text/csv", "application/vnd.ms-excel" },
        expectedHeaders: new[] { "ChipNo" },
        maxFileSize: 5 * 1024 * 1024)]
        public required IFormFile CSVFile { get; set; }
    }
}
