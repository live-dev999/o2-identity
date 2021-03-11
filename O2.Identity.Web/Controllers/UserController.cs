using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using O2.Identity.Web.Helpers;
using O2.Identity.Web.Models;
using SendGrid;
using SendGrid.Helpers.Mail;

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

        public class UserParam
        {
            private const int MaxPageSize = 50;
            public int PageNumber { get; set; } = 1;
            private int pageSize = 10;

            public int PageSize
            {
                get { return pageSize; }
                set { pageSize = (value > MaxPageSize) ? MaxPageSize : value; }
            }
        }

        public class UserViewM
        {
            public string Id { get; set; }
            public string Email { get; set; }
            public string Firstname { get; set; }
            public string Lastname { get; set; }
            public DateTime? RegistrationDate { get; set; }
            public string ProfilePhoto { get; set; }
        }
        
        
        [HttpGet("all/{userId}")]
        public async Task<IActionResult> GetUsersAll(string userId)
        {
             var user = _userManager.Users.SingleOrDefault(x=>userId != null && x.Id==userId);
            if (user == null)
            {
                throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            var model = new UserViewM
            {
               Id= user.Id,
                            Email = user.Email,
                            Firstname = user.Firstname,
                            Lastname = user.Lastname,
                            RegistrationDate = user.RegistrationDate,
                            ProfilePhoto = user.ProfilePhoto
            };

            return Ok(model);
        }

        [HttpGet("all")]
        public async Task<IActionResult> GetUsersAll([FromQuery] UserParam certificateParam)
        {
            
            List<O2User> users = null;
        
            List<UserViewM> usersView = new List<UserViewM>();
                users  =  _userManager.Users.ToList();
                foreach (var user in users)
                {
                    usersView.Add(
                        new UserViewM()
                        {
                            Id= user.Id,
                            Email = user.Email,
                            Firstname = user.Firstname,
                            Lastname = user.Lastname,
                            RegistrationDate = user.RegistrationDate,
                            ProfilePhoto = user.ProfilePhoto
                        }
                        );
                }
               var paginator = await PagedList<UserViewM>.CreateAsync(
                    usersView.OrderByDescending(x => x.RegistrationDate).AsQueryable(), certificateParam.PageNumber,
                    certificateParam.PageSize);

            Response.AddPagination(paginator.CurrentPage, paginator.PageSize,
                paginator.TotalCount, paginator.TotalPages);
            return Ok(usersView);
            
        }

        [HttpGet("send-email")]
        public async Task<IActionResult> Send()
        {
            try
            {
     var apiKey = "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";
                // var apiKey = Environment.GetEnvironmentVariable("NAME_OF_THE_ENVIRONMENT_VARIABLE_FOR_YOUR_SENDGRID_KEY");
                var client = new SendGridClient(apiKey);
                var from = new EmailAddress("test@example.com", "Example User");
                var subject = "Sending with SendGrid is Fun";
                var to = new EmailAddress("test@example.com", "Example User");
                var plainTextContent = "and easy to do anywhere, even with C#";
                var htmlContent = "<strong>and easy to do anywhere, even with C#</strong>";
                var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);
                var response = await client.SendEmailAsync(msg);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }
           

            return Ok("ok");
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
                AboutMe = user.AboutMe,
                Birthday = user.Birthday,
                RegistrationDate = user.RegistrationDate,
                Education = user.Education,
                Specialization = user.Specialization,
                LanguageProficiency= user.LanguageProficiency,
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
        public string AboutMe { get; set; }
        public string Education { get; set; }
        public string Specialization { get; set; }
        public string LanguageProficiency { get; set; }
    }
}