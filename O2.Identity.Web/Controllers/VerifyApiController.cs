using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using O2.Identity.Web.Models;
using O2.Identity.Web.Services;

namespace O2.Identity.Web.Controllers
{
    [Produces("application/json")]
    [Route("api/Verify")]
    public class VerifyApiController : Controller
    {
        
        private readonly IVerification _verification;
        private readonly UserManager<O2User> _userManager;

        public VerifyApiController(IVerification verification, UserManager<O2User> manager)
        {
            _verification = verification;
            _userManager = manager;
        }

        [HttpPost]
        public async Task<VerificationResult> Post(string channel)
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);

            if (!user.Verified)
            {
                return await _verification.StartVerificationAsync(user.PhoneNumber, channel);
            }

            return new VerificationResult(new List<string>{"Your phone number is already verified"});
        }
    }
}