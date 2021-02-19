using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using O2.Identity.Web.Models;

namespace O2.Identity.Web.Filters
{
    public class VerifyFilter: IAsyncResourceFilter
    {
        
        private readonly UserManager<O2User> _manager;

        public VerifyFilter(UserManager<O2User> manager)
        {
            _manager = manager;
        }

        public async Task OnResourceExecutionAsync(ResourceExecutingContext context, ResourceExecutionDelegate next)
        {
            if (!context.HttpContext.User.Identity.IsAuthenticated)
            {
                throw new Exception("Authentication required before verification");
            }
            
            var user = await _manager.GetUserAsync(context.HttpContext.User);
            if (!user.Verified)
            {
                context.Result = new RedirectResult("/Identity/Account/Verify");
            }
            else
            {
                await next();
            }
        }
    }
}