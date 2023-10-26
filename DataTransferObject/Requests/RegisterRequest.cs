using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObject.Requests
{
    public class RegisterRequest
    {
        [Required]
        [Display(Name = "Username")]
        //[RegularExpression("^[a-zA-Z0-9 _ ]*$", ErrorMessage = "Only Alphabets and Numbers allowed.")]
        public string Username { get; set; }

        [Required]
        //[StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
       // [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        //[DataType(DataType.Password)]
        //[Display(Name = "Confirm password")]
        //[Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        //public string ConfirmPassword { get; set; }


        //[Display(Name = "DE Fwd Auth")]
        //public string DEFwdAuth { get; set; }


        //[Display(Name = "GEB Fwd Auth")]
        //public string GEBFwdAuth { get; set; }

        [Display(Name = "Status")]
        public string Active { get; set; }

        //[Display(Name = "Unit / Fmn/ Dte")]
        //public string EstablishmentFull { get; set; }
        [Display(Name = "Unit_ID")]
        public int Unit_ID { get; set; }

        //[Required]
        //[RegularExpression("^[a-zA-Z0-9-&)(  ]*$", ErrorMessage = "Only Alphabets, Numbers,AND and brackets allowed.")]
        //[Display(Name = "Unit Name")]
        //public string EstablishmentAbbreviation { get; set; }

        //[Required]
        //public string Appointment { get; set; }

        //[Required]
        //[Display(Name = "Pers Number")]
        //[RegularExpression("^[a-zA-Z0-9 ]*$", ErrorMessage = "Only Alphabets and Numbers allowed.")]
        //public string Number { get; set; }

        [Required]
        public string RankId { get; set; }

        [Required]
        //[RegularExpression("^[a-zA-Z0-9 ]*$", ErrorMessage = "Only Alphabets and Numbers allowed.")]
        [Display(Name = "Full Name")]
        public string FullName { get; set; }

        [Required]
        //[RegularExpression(@"^[0-9]+$", ErrorMessage = "ASCON No can only be numbers.")]
        [Display(Name = "ASCON Number")]
        public string ASCON { get; set; }

        //[Required]
        //[Display(Name = "Roles")]
        //public string Roles { get; set; }
        [Required]
        [Display(Name = "UserType")]
        public int UserTypeId { get; set; }
        //[Required]
        //[Display(Name = "ParentId")]
        //public int ParentId { get; set; }
    }
}
