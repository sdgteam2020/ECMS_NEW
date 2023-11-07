using DataTransferObject.Domain.Identitytable;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObject.Requests
{
    public class DTORegisterRequest
    {
        [RegularExpression(@"^[a-zA-Z0-9-._@+]{5,30}$", ErrorMessage = "Invalid format")]
        [Required(ErrorMessage = "Domain Id is required.")]
        [MinLength(5, ErrorMessage = "Minimum length of Domain Id is 5 characters.")]
        [MaxLength(30, ErrorMessage = "Maximum length of Domain Id is 30 characters.")]
        [Display(Name = "Domain Id")]
        public string DomainId { get; set; } = string.Empty;

        [Display(Name = "UserRole")]
        [Required(ErrorMessage = "User Role is required.")]
        public string UserRole { get; set; } = string.Empty;

        public bool Active { get; set; } = false;
        public bool AdminFlag { get; set; } = false;

        [Display(Name = "Updated By")]
        public int Updatedby { get; set; }

        [Display(Name = "Updated On")]
        public DateTime UpdatedOn { get; set; }

        public string? Fd1 { get; set; }
        public string? Fd2 { get; set; }
        public string? hdns { get; set; }
    }
    public class DTORegisterListRequest : DTORegisterRequest
    {
        public int Id { get; set; }

        [Display(Name = "S No.")]
        public int Sno { get; set; }

        public string EncryptedId { get; set; } = string.Empty;

        [Display(Name = "Role Name")]
        public string RoleName { get; set; } = string.Empty;

    }
    public class DTOLoginRequest
    {
        [RegularExpression(@"^[a-zA-Z0-9-._@+]{5,30}$", ErrorMessage = "Invalid user format")]
        [Required(ErrorMessage = "User Id is required.")]
        [MinLength(5, ErrorMessage = "Minimum length of User Id is 5 characters.")]
        [MaxLength(30, ErrorMessage = "Maximum length of User Id is 30 characters.")]
        [Display(Name = "User Name")]
        public string UserName { get; set; } = string.Empty;
        [Required]
        [DataType(DataType.Password)]
        [RegularExpression(@"^[\w \.\,\?\;\:\""\''\[\]\!\@\#\$\%\&\*\(\)\-\=\+\\\/]*$", ErrorMessage = "This < >^| special chars not allowed for security reasons.")]
        public string Password { get; set; } = string.Empty;

        //[Display(Name = "Remember me")]
        //public bool RememberMe { get; set; }
        public string? hdns { get; set; }


    }
    public class DTOResetPasswordRequest
    {
        [RegularExpression(@"^[a-zA-Z0-9-._@+]{5,30}$", ErrorMessage = "Invalid user format")]
        [Required(ErrorMessage = "User Id is required.")]
        [MinLength(5, ErrorMessage = "Minimum length of User Id is 5 characters.")]
        [MaxLength(30, ErrorMessage = "Maximum length of User Id is 30 characters.")]
        [Display(Name = "User Name")]
        public string UserName { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "Password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; } = string.Empty;
        public string Token { get; set; } = string.Empty;
        public string? hdns { get; set; }

    }
    public class DTOSetPasswordRequest
    {
        [RegularExpression(@"^[a-zA-Z0-9-._@+]{5,30}$", ErrorMessage = "Invalid user format")]
        [Required(ErrorMessage = "User Id is required.")]
        [MinLength(5, ErrorMessage = "Minimum length of User Id is 5 characters.")]
        [MaxLength(30, ErrorMessage = "Maximum length of User Id is 30 characters.")]
        [Display(Name = "User Name")]
        public string UserName { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Password)]
        public string OldPassword { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "Password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; } = string.Empty;
        //public string Token { get; set; }
        public string? hdns { get; set; }

    }
    public class EditRoleViewModel
    {
        public EditRoleViewModel()
        {
            Users = new List<string>();
        }

        public string Id { get; set; }

        [Required(ErrorMessage = "Role Name is required")]
        public string RoleName { get; set; }

        public List<string> Users { get; set; }
    }
    public class UserClaimsViewModel
    {
        public UserClaimsViewModel()
        {
            Cliams = new List<UserClaim>();
        }

        public string UserId { get; set; }
        public List<UserClaim> Cliams { get; set; }
    }
    public class UserClaim
    {
        public string ClaimType { get; set; } = string.Empty;
        public bool IsSelected { get; set; }
    }
    public static class ClaimsStore
    {
        public static List<Claim> AllClaims = new List<Claim>()
        {
            new Claim("Create Role", "Create Role"),
            new Claim("Edit Role","Edit Role"),
            new Claim("Delete Role","Delete Role")
        };
    }
    public class UserRolesViewModel
    {
        public string RoleId { get; set; }=string.Empty;
        public string RoleName { get; set; } = string.Empty;
        public bool IsSelected { get; set; }
    }
    public class EditUserViewModel
    {
        public EditUserViewModel()
        {
            Claims = new List<string>();
            Roles = new List<string>();
        }

        public int Id { get; set; }
        public string EncryptedId { get; set; } = string.Empty;
        public string DomainId { get; set; } = string.Empty;
        public bool Active { get; set; } = false;
        public bool AdminFlag { get; set; } = false;

        public List<string> Claims { get; set; }

        public IList<string> Roles { get; set; }
    }
    public class CreateRoleViewModel
    {
        [Required]
        public string RoleName { get; set; } = string.Empty;
    }
    public class UserRoleViewModel
    {
        public int UserId { get; set; } 
        public string EncryptedId { get; set; } = string.Empty;
        public string DomainId { get; set; } = string.Empty;
        public bool IsSelected { get; set; }
    }
}
