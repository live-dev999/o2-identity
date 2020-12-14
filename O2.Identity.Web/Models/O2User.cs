using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace O2.Identity.Web.Models
{
    // Add profile data for application users by adding properties to the O2User class
    public class O2User : IdentityUser
    {
        public bool IsSpecialist { get; set; }   
        
        public string ProfilePhoto { get; set; }
        
        // maybe be other code in this model
        public ICollection<Photo> Photos { get; set; }
        
        public string Lastname { get; set; }
        public string Firstname { get; set; }
        public DateTime Birthday { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        
        public virtual DateTime? LastLoginTime { get; set; }
        public virtual DateTime? RegistrationDate { get; set; }
        
        public string SpecialistId { get; set; }
    }
}
