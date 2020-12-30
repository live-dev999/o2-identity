using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using O2.Identity.Web.Models;

namespace O2.Identity.Web.Controllers
{
    [Route("api/users")]
    public class UserController: Controller
    {
        private readonly UserManager<O2User> _userManager;

        public UserController( UserManager<O2User> userManager)
        {
            _userManager = userManager;
        }
    
        // ToDo: added security
        [HttpGet]
        public async Task<IActionResult>  GetUsers()
        {
            var users = await _userManager.Users.Where(x => x.IsSpecialist).ToListAsync();
            return Ok(users);
        }
        
        // ToDo: added security
        [HttpGet("referrals/{userId}")]
        public async Task<IActionResult>  GetReferrals(string userId)
        {
            //string userId
             var users = await _userManager.Users.Where(x => x.SpecialistId==userId).ToListAsync();
            //var users = await _userManager.Users.Where(x => x.IsSpecialist).ToListAsync();
            return Ok(users);
        }

        [HttpGet("{userId}")]
        public IActionResult GetUser(string userId)
        {
            var user = _userManager.Users.SingleOrDefault(x=>userId != null && x.Id==userId);
            if (user == null)
            {
                throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            var model = new UserViewModel
            {
                Id = user.Id,
                Username = user.UserName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                IsEmailConfirmed = user.EmailConfirmed,
                IsSpecialist = user.IsSpecialist,
                ProfilePhoto = user.ProfilePhoto,
                Firstname = user.Firstname,
                Lastname = user.Lastname,
                Country = user.Country,
                City = user.City,
                Birthday = user.Birthday,
                RegistrationDate = user.RegistrationDate
            };

            return Ok(model);
        }
    }

    public class UserViewModel
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
        public bool IsSpecialist { get; set; }
    }
}