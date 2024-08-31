using DataTransferObject.Domain;
using DataTransferObject.Localize;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using DataTransferObject.Validation;

namespace DataTransferObject.Requests
{
    public class DTOBasicDetailTempRequest : Common
    {
        [RegularExpression(@"^[\d]+$", ErrorMessage = "BasicDetailTempId is number.")]
        public int BasicDetailTempId { get; set; }

        [DisplayName("First Name")]
        [Required(ErrorMessageResourceType = typeof(ErrorMessages), ErrorMessageResourceName = "RequiredError")]
        [RegularExpression(@"^[\w ]*$", ErrorMessage = "Only Alphabets ,Numbers allowed.")]
        [MaxLength(18, ErrorMessage = "Maximum length of First Name is eighteen character.")]
        public string FName { get; set; } = string.Empty;

        [DisplayName("Last Name")]
        [RegularExpression(@"^[\w ]*$", ErrorMessage = "Only Alphabets ,Numbers allowed.")]
        [MaxLength(18, ErrorMessage = "Maximum length of Last Name is eighteen character.")]
        public string? LName { get; set; }

        [DisplayName("Name As Per Record")]
        [Required(ErrorMessageResourceType = typeof(ErrorMessages), ErrorMessageResourceName = "RequiredError")]
        [RegularExpression(@"^[\w ]*$", ErrorMessage = "Only Alphabets ,Numbers allowed.")]
        [MaxLength(36, ErrorMessage = "Maximum length of Name As Per Record is thirty six character.")]
        public string NameAsPerRecord { get; set; } = string.Empty;


        [Required(ErrorMessageResourceType = typeof(ErrorMessages), ErrorMessageResourceName = "RequiredError")]
        [MinLength(8, ErrorMessage = "Minimum length of Army No is eight character.")]
        [MaxLength(9, ErrorMessage = "Maximum length of Army No is nine character.")]
        public string ServiceNo { get; set; } = string.Empty;


        [DataType(DataType.Date)]
        [Required(ErrorMessageResourceType = typeof(ErrorMessages), ErrorMessageResourceName = "RequiredError")]
        public DateTime DOB { get; set; }


        [DisplayName("Dt of Commissioning/ Enrolment")]
        [Required(ErrorMessageResourceType = typeof(ErrorMessages), ErrorMessageResourceName = "RequiredError")]
        [DataType(DataType.Date)]
        public DateTime DateOfCommissioning { get; set; }


        [Display(Name = "Observations", ResourceType = typeof(Resource))]
        [RegularExpression(@"^[\w \.]*$", ErrorMessageResourceType = typeof(ErrorMessages), ErrorMessageResourceName = "SpecialChars")]
        public string? Observations { get; set; }

        [DisplayName("Arms, Service & Corps")]
        [Required(ErrorMessageResourceType = typeof(ErrorMessages), ErrorMessageResourceName = "RequiredError")]
        [RegularExpression(@"^[\d]+$", ErrorMessage = "Arms, Service & Corps is number.")]
        public byte ArmedId { get; set; }


        [Display(Name = "Rank", ResourceType = typeof(Resource))]
        [Required(ErrorMessageResourceType = typeof(ErrorMessages), ErrorMessageResourceName = "RequiredError")]
        [RegularExpression(@"^[\d]+$", ErrorMessage = "RankId is number.")]
        public short RankId { get; set; }
        /// <summary>
        /// address
        /// </summary>
        [RegularExpression(@"^[\w\-\.\/ ]*$", ErrorMessageResourceType = typeof(ErrorMessages), ErrorMessageResourceName = "SpecialChars")]
        public string? PermanentAddress { get; set; }

        [DisplayName("State")]
        [RegularExpression(@"^[\w ]*$", ErrorMessageResourceType = typeof(ErrorMessages), ErrorMessageResourceName = "SpecialChars")]
        [MaxLength(50, ErrorMessage = "Maximum length of State is fifty character.")]
        [Required(ErrorMessageResourceType = typeof(ErrorMessages), ErrorMessageResourceName = "RequiredError")]
        public string State { get; set; } = string.Empty;

        [Required(ErrorMessageResourceType = typeof(ErrorMessages), ErrorMessageResourceName = "RequiredError")]
        [RegularExpression(@"^[\w ]*$", ErrorMessageResourceType = typeof(ErrorMessages), ErrorMessageResourceName = "SpecialChars")]
        [MaxLength(50, ErrorMessage = "Maximum length of District is fifty character.")]
        public string District { get; set; } = string.Empty;

        [RegularExpression(@"^[\w ]*$", ErrorMessageResourceType = typeof(ErrorMessages), ErrorMessageResourceName = "SpecialChars")]
        public string? PS { get; set; }

        [RegularExpression(@"^[\w ]*$", ErrorMessageResourceType = typeof(ErrorMessages), ErrorMessageResourceName = "SpecialChars")]
        [MaxLength(50, ErrorMessage = "Maximum length of PO is fifty character.")]
        public string? PO { get; set; }

        [RegularExpression(@"^[\w ]*$", ErrorMessageResourceType = typeof(ErrorMessages), ErrorMessageResourceName = "SpecialChars")]
        [MaxLength(50, ErrorMessage = "Maximum length of Tehsil is fifty character.")]
        public string? Tehsil { get; set; }

        [RegularExpression(@"^[\w\-\.\/ ]*$", ErrorMessageResourceType = typeof(ErrorMessages), ErrorMessageResourceName = "SpecialChars")]
        [MaxLength(50, ErrorMessage = "Maximum length of Village is fifty character.")]
        public string? Village { get; set; }

        [ValidInteger("Pin Code")]
        [Range(typeof(int), "100000", "999999", ErrorMessage = "Value for {0} must be between {1} and {2}.")]
        [Required(ErrorMessage = "Pin Code is required")]
        public int PinCode { get; set; }
        /// <summary>
        /// end address
        /// </summary> 
        
        [RegularExpression(@"^[\w\.\/ ]*$", ErrorMessageResourceType = typeof(ErrorMessages), ErrorMessageResourceName = "SpecialChars")]
        public string? IdenMark1 { get; set; } = string.Empty;
        
        //[RegularExpression(@"^[\w ]*$", ErrorMessageResourceType = typeof(ErrorMessages), ErrorMessageResourceName = "SpecialChars")]
        public string? IdenMark2 { get; set; } = string.Empty;

        [RegularExpression(@"^[\d]+$", ErrorMessage = "AadhaarNo is number.")]
        public string? AadhaarNo { get; set; }
        //[NotMapped]
        //public int? Height { get; set; }

        [RegularExpression(@"^[\d]+$", ErrorMessage = "BloodGroupId is number.")]
        public byte BloodGroupId { get; set; }

        [RegularExpression(@"^[\w\+\- ]*$", ErrorMessageResourceType = typeof(ErrorMessages), ErrorMessageResourceName = "SpecialChars")]
        public string? BloodGroup { get; set; }

        [NotMapped]
        public string? EncryptedId { get; set; }

        [NotMapped]
        [Display(Name = "Sno", ResourceType = typeof(Resource))]
        public int Sno { get; set; }

        [RegularExpression(@"^[\w ]*$", ErrorMessageResourceType = typeof(ErrorMessages), ErrorMessageResourceName = "SpecialChars")]
        public string? Remarks2 { get; set; }
        
        [RegularExpression(@"^[\w ]*$", ErrorMessageResourceType = typeof(ErrorMessages), ErrorMessageResourceName = "SpecialChars")]
        public string? ApplyType { get; set; }
        
        [RegularExpression(@"^[\w ]*$", ErrorMessageResourceType = typeof(ErrorMessages), ErrorMessageResourceName = "SpecialChars")]
        public string? RegistrationName { get; set; }
        
        [RegularExpression(@"^[\w ]*$", ErrorMessageResourceType = typeof(ErrorMessages), ErrorMessageResourceName = "SpecialChars")]
        public string? CardType { get; set; }
        
        [RegularExpression(@"^[\w ]*$", ErrorMessageResourceType = typeof(ErrorMessages), ErrorMessageResourceName = "SpecialChars")]
        public string? DomainId { get; set; }
        
        [RegularExpression(@"^[\w ]*$", ErrorMessageResourceType = typeof(ErrorMessages), ErrorMessageResourceName = "SpecialChars")]
        public string? UnitName { get; set; }
        
        [RegularExpression(@"^[\w ]*$", ErrorMessageResourceType = typeof(ErrorMessages), ErrorMessageResourceName = "SpecialChars")]
        public string? Suffix { get; set; }
        
        [RegularExpression(@"^[\w ]*$", ErrorMessageResourceType = typeof(ErrorMessages), ErrorMessageResourceName = "SpecialChars")]
        public string? Sus_no { get; set; }
        
        [RegularExpression(@"^[\w ]*$", ErrorMessageResourceType = typeof(ErrorMessages), ErrorMessageResourceName = "SpecialChars")]
        public string? OffName { get; set; }
        
        [RegularExpression(@"^[\w ]*$", ErrorMessageResourceType = typeof(ErrorMessages), ErrorMessageResourceName = "SpecialChars")]
        public string? RankAbbreviation { get; set; }
        
        [RegularExpression(@"^[\w ]*$", ErrorMessageResourceType = typeof(ErrorMessages), ErrorMessageResourceName = "SpecialChars")]
        public string? RankName { get; set; }
        
        [RegularExpression(@"^[\w ]*$", ErrorMessageResourceType = typeof(ErrorMessages), ErrorMessageResourceName = "SpecialChars")]
        public string? ArmyNo { get; set; }

        [RegularExpression(@"^[\d]+$", ErrorMessage = "ApplyForId is number.")]
        public byte ApplyForId { get; set; }
        
        [RegularExpression(@"^[\d]+$", ErrorMessage = "RegistrationId is number.")]
        public byte RegistrationId { get; set; }
        
        [RegularExpression(@"^[\d]+$", ErrorMessage = "TypeId is number.")]
        public byte TypeId { get; set; }
    }
}
