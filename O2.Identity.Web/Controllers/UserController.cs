using System.Linq;
using System.Threading.Tasks;
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

        [HttpGet]
        public async Task<IActionResult>  GetUsers()
        {
            var users = await _userManager.Users.Where(x => x.IsSpecialist).ToListAsync();
            return Ok(users);
        }
    }
}