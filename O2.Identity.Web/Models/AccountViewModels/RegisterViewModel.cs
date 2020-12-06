using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace O2.Identity.Web.Models.AccountViewModels
{
    public class RegisterViewModel
    {
        
        [Required(ErrorMessage = "RequiredLastname")]
        [Display(Name = "Lastname")]
        public string Lastname { get; set; }
        
        [Required(ErrorMessage = "RequiredFirstname")]
        [Display(Name = "Firstname")]
        public string Firstname { get; set; }
        
        [Required(ErrorMessage = "RequiredEmail")]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required(ErrorMessage = "RequiredPassword")]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "PasswordNotMatch")]
        public string ConfirmPassword { get; set; }
        
        [Display(Name = "ProfilePhoto")]
        public string ProfilePhoto { get; set; }

        // [Required]
        [Display(Name="Profile photo")]
        public IFormFile FormFile { get; set; }
        
        [Required(ErrorMessage = "RequiredPhoneNumber")]
        [Phone]
        [Display(Name="PhoneNumber")]
        public string PhoneNumber { get; set; }
        
        
    }
}
