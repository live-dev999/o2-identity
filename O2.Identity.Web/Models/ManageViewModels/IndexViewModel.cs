using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace O2.Identity.Web.Models.ManageViewModels
{
    public class IndexViewModel
    {
        public string Username { get; set; }

        public bool IsEmailConfirmed { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Phone]
        [Display(Name = "Phone number")]
        public string PhoneNumber { get; set; }

        public string StatusMessage { get; set; }
        
        [Display(Name = "ProfilePhoto")]
        public string ProfilePhoto { get; set; }

        [Required]
        [Display(Name="File")]
        public IFormFile FormFile { get; set; }
    }
}
