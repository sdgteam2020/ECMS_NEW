using DataTransferObject.Localize;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObject.Requests
{
    public class DTOSaveUnitWithMappingByAdminRequest
    {
        [RegularExpression(@"^[\d]+$", ErrorMessage = "UnitId is number.")]
        public int UnitId { get; set; }

        [RegularExpression(@"^[\d]+$", ErrorMessage = "UnitMapId is number.")]
        public int UnitMapId { get; set; }

        [Required(ErrorMessageResourceType = typeof(ErrorMessages), ErrorMessageResourceName = "RequiredError")]
        //[RegularExpression("^[0-9]{7}[a-zA-Z]{1}$", ErrorMessage = "Use Seven digit numbers and one alphabet in capital letter without space")]
        [RegularExpression("^[0-9]{7}$", ErrorMessage = "Use Seven digit numbers")]
        [MaxLength(7, ErrorMessage = "Maximum length of SUS No is seven digit.")]
        public string Sus_no { get; set; } = string.Empty;

        [Required(ErrorMessageResourceType = typeof(ErrorMessages), ErrorMessageResourceName = "RequiredError")]
        [RegularExpression("^[A-Z]{1}$", ErrorMessage = "Alphabet in Capital letter.")]
        [MaxLength(1, ErrorMessage = "Maximum length of Suffix is one character.")]
        public string Suffix { get; set; } = string.Empty;

        //[Required(ErrorMessageResourceType = typeof(ErrorMessages), ErrorMessageResourceName = "RequiredError")]
        //[RegularExpression("^[a-zA-Z0-9 ]*$", ErrorMessage = "Only Alphabets and Numbers allowed.")]
        //[MaxLength(100, ErrorMessage = "Maximum length of Suffix is one hundred character.")]
        //public string UnitName { get; set; } = string.Empty;

        public bool IsVerify { get; set; }

        [RegularExpression(@"^[\d]+$", ErrorMessageResourceType = typeof(ErrorMessages), ErrorMessageResourceName = "OnlyNumber")]
        [Range(typeof(int), "1", "3", ErrorMessage = "Invalid Unit Type.")]
        public int UnitType { get; set; }

        [RegularExpression(@"^[\d]+$", ErrorMessage = "ComdId is number.")]
        [Required(ErrorMessageResourceType = typeof(ErrorMessages), ErrorMessageResourceName = "RequiredError")]
        public byte ComdId { get; set; }

        [RegularExpression(@"^[\d]+$", ErrorMessage = "CorpsId is number.")]
        [Required(ErrorMessageResourceType = typeof(ErrorMessages), ErrorMessageResourceName = "RequiredError")]
        public byte CorpsId { get; set; }

        [RegularExpression(@"^[\d]+$", ErrorMessage = "DivId is number.")]
        [Required(ErrorMessageResourceType = typeof(ErrorMessages), ErrorMessageResourceName = "RequiredError")]
        public byte DivId { get; set; }

        [RegularExpression(@"^[\d]+$", ErrorMessage = "BdeId is number.")]
        [Required(ErrorMessageResourceType = typeof(ErrorMessages), ErrorMessageResourceName = "RequiredError")]
        public byte BdeId { get; set; }

        [RegularExpression(@"^[\d]+$", ErrorMessage = "PsoId is number.")]
        [Required(ErrorMessageResourceType = typeof(ErrorMessages), ErrorMessageResourceName = "RequiredError")]
        public byte PsoId { get; set; }

        [RegularExpression(@"^[\d]+$", ErrorMessage = "FmnBranchID is number.")]
        [Required(ErrorMessageResourceType = typeof(ErrorMessages), ErrorMessageResourceName = "RequiredError")]
        public byte FmnBranchID { get; set; }

        [RegularExpression(@"^[\d]+$", ErrorMessage = "SubDteId is number.")]
        [Required(ErrorMessageResourceType = typeof(ErrorMessages), ErrorMessageResourceName = "RequiredError")]
        public byte SubDteId { get; set; }

        [RegularExpression(@"^[\d]+$", ErrorMessage = "Updatedby is number.")]
        public int Updatedby { get; set; }

        [DataType(DataType.Date)]
        public DateTime? UpdatedOn { get; set; }
    }
}
