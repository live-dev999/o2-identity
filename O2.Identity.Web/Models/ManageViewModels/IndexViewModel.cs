using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace O2.Identity.Web.Models.ManageViewModels
{
    public class IndexViewModel
    {
        public string Id { get; set; }
        public string Lastname { get; set; }
        public string Firstname { get; set; }
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

        [Display(Name = "Profile Photo")] 
        public IFormFile FormFile { get; set; }

        public DateTime Birthday { get; set; }
        public  string Country { get; set; }
        public string City { get; set; }
        public DateTime? RegistrationDate { get; set; }
        public string AboutMe { get; set; }
    }
}
