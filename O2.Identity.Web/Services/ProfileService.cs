using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using IdentityServer4.Extensions;
using IdentityServer4.Models;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Identity;
using O2.Identity.Web.Models;

namespace O2.Identity.Web.Services
{ public class ProfileService : IProfileService
        {
            private readonly IUserClaimsPrincipalFactory<O2User> _claimsFactory;
            private readonly UserManager<O2User> _userManager;

            public ProfileService(UserManager<O2User> userManager, IUserClaimsPrincipalFactory<O2User> claimsFactory)
            {
                _userManager = userManager;
                _claimsFactory = claimsFactory;
            }

            public async Task GetProfileDataAsync(ProfileDataRequestContext context)
            {
                var sub = context.Subject.GetSubjectId();
                var user = await _userManager.FindByIdAsync(sub);
                var principal = await _claimsFactory.CreateAsync(user);

                var claims = principal.Claims.ToList();
                claims = claims.Where(claim => context.RequestedClaimTypes.Contains(claim.Type)).ToList();

                // Add custom claims in token here based on user properties or any other source
                // Added custom field - isSpecialist
                claims.Add(new Claim("is_specialist", user.IsSpecialist.ToString()));

                context.IssuedClaims = claims;
            }

            public async Task IsActiveAsync(IsActiveContext context)
            {
                var sub = context.Subject.GetSubjectId();
                var user = await _userManager.FindByIdAsync(sub);
                context.IsActive = user != null;
            }
        }
    }