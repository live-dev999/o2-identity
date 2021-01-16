using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using O2.Identity.Web.Models;

namespace O2.Identity.Web.Controllers
{
    [Route("api/system-count")]
    public class CounterController: Controller
    {
        private readonly UserManager<O2User> _userManager;

        public CounterController( UserManager<O2User> userManager)
        {
            _userManager = userManager;
        }
        
        [HttpGet]
        public async Task<IActionResult> GetSystemCount()
        {
            int countUsers = await _userManager.Users.CountAsync();
            int countSpecialist = await _userManager.Users.Where(x=>x.IsSpecialist).CountAsync();
            var countViewModel = new CountViewModel()
            {
                CountUsers=countUsers+110,
                CountSpecialist = countSpecialist
            };
            return Ok(countViewModel);
        }
        [HttpGet("/real")]
        public async Task<IActionResult> GetSystemRealCount()
        {
            int countUsers = await _userManager.Users.CountAsync();
            int countSpecialist = await _userManager.Users.Where(x=>x.IsSpecialist).CountAsync();
            var countViewModel = new CountViewModel()
            {
                CountUsers=countUsers,
                CountSpecialist = countSpecialist
            };
            return Ok(countViewModel);
        }
    }

    public class CountViewModel
    {
        public int CountUsers { get; set; }
        public int CountSpecialist { get; set; }
    }
}