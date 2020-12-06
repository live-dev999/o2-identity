using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using IdentityModel;
using IdentityServer4;
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
                claims.Add(new Claim(JwtClaimTypes.GivenName, user.UserName));
                claims.Add(new Claim(IdentityServerConstants.StandardScopes.Email, user.Email));
                // claims.Add(new Claim(IdentityServerConstants.StandardScopes.Phone ));
                claims.Add(new Claim(JwtClaimTypes.FamilyName, user.Lastname));
                claims.Add(new Claim(JwtClaimTypes.PhoneNumber, user.PhoneNumber));
                claims.Add(new Claim(JwtClaimTypes.Picture, user.ProfilePhoto));
                claims.Add(new Claim("is_specialist", user.IsSpecialist.ToString()));
                
                
                //Get user claims from AspNetUserClaims table
                var userClaims = await _userManager.GetClaimsAsync(user);
                
                // Add custom claims in token here based on user properties or any other source
                claims.AddRange(userClaims);

                Console.WriteLine("========= claims ==========");
                foreach (var claim in claims)
                {
                    Console.WriteLine(claim.Type,claim.Value);
                }
                Console.WriteLine("======= end claims ========");
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