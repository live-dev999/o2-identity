using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using IdentityModel;
using IdentityServer4;
using IdentityServer4.Extensions;
using IdentityServer4.Models;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.JsonWebTokens;
using O2.Identity.Web.Models;

namespace O2.Identity.Web.Services
{ public class ProfileService : IProfileService
        {
            private readonly UserManager<O2User> _userManager;

        public ProfileService(UserManager<O2User> userManager)
        {
            _userManager = userManager;
        }

        async public Task GetProfileDataAsync(ProfileDataRequestContext context)
        {
            var subject = context.Subject ?? throw new ArgumentNullException(nameof(context.Subject));

            var subjectId = subject.Claims.Where(x => x.Type == "sub").FirstOrDefault().Value;

            var user = await _userManager.FindByIdAsync(subjectId);
            if (user == null)
                throw new ArgumentException("Invalid subject identifier");

            var claims = GetClaimsFromUser(user);
            context.IssuedClaims = claims.ToList();
        }

        async public Task IsActiveAsync(IsActiveContext context)
        {
            var subject = context.Subject ?? throw new ArgumentNullException(nameof(context.Subject));

            var subjectId = subject.Claims.Where(x => x.Type == "sub").FirstOrDefault().Value;
            var user = await _userManager.FindByIdAsync(subjectId);

            context.IsActive = false;

            if (user != null)
            {
                if (_userManager.SupportsUserSecurityStamp)
                {
                    var security_stamp = subject.Claims.Where(c => c.Type == "security_stamp").Select(c => c.Value).SingleOrDefault();
                    if (security_stamp != null)
                    {
                        var db_security_stamp = await _userManager.GetSecurityStampAsync(user);
                        if (db_security_stamp != security_stamp)
                            return;
                    }
                }

                context.IsActive =
                    !user.LockoutEnabled ||
                    !user.LockoutEnd.HasValue ||
                    user.LockoutEnd <= DateTime.Now;
            }
        }

        private IEnumerable<Claim> GetClaimsFromUser(O2User user)
        {
            var claims = new List<Claim>
            {
                var subject = context.Subject ?? throw new ArgumentNullException(nameof(context.Subject));

                var subjectId = subject.Claims.Where(x => x.Type == "sub").FirstOrDefault().Value;

                var user = await _userManager.FindByIdAsync(subjectId);
                if (user == null)
                    throw new ArgumentException("Invalid subject identifier");

                var claims = GetClaimsFromUser(user);
                context.IssuedClaims = claims.ToList();
            }

            private IEnumerable<Claim> GetClaimsFromUser(O2User user)
            {
                var claims = new List<Claim>
                {
                    new Claim(JwtClaimTypes.Subject, user.Id),
                    new Claim(JwtClaimTypes.PreferredUserName, user.UserName),
                    new Claim(JwtRegisteredClaimNames.UniqueName, user.UserName)
                };
                if (!string.IsNullOrWhiteSpace(user.Firstname))
                    claims.Add(new Claim(JwtClaimTypes.Name, user.Firstname));

                if (!string.IsNullOrWhiteSpace(user.Lastname))
                    claims.Add(new Claim(JwtClaimTypes.FamilyName, user.Lastname));
                
                if (!string.IsNullOrWhiteSpace(user.Lastname))
                    claims.Add(new Claim(JwtClaimTypes.Picture, user.ProfilePhoto ?? ""));
                
               
                claims.Add(new Claim("is_specialist", user.IsSpecialist.ToString()));
                var role = user.IsSpecialist ? "Member" : "Client";
                
                claims.Add(new Claim(JwtClaimTypes.Role, role));
                // context.IssuedClaims = claims.ToList();
                //
                // claims = claims.Where(claim => context.RequestedClaimTypes.Contains(claim.Type)).ToList();
                // // claims.Remove(claims.Single(x => x.Type == JwtClaimTypes.Name));
                // claims.Add(new Claim(JwtClaimTypes.Name, user.Firstname));
                // claims.Add(new Claim(IdentityServerConstants.StandardScopes.Email, user.Email));
                // // claims.Add(new Claim(IdentityServerConstants.StandardScopes.Phone ));
                // claims.Add(new Claim(JwtClaimTypes.FamilyName, user.Lastname));
                // claims.Add(new Claim(JwtClaimTypes.PhoneNumber, user.PhoneNumber));
                // claims.Add(new Claim(JwtClaimTypes.Picture, user.ProfilePhoto ?? ""));
                // claims.Add(new Claim("is_specialist", user.IsSpecialist.ToString()));
                //
                //
                // //Get user claims from AspNetUserClaims table
                // var userClaims = await _userManager.GetClaimsAsync(user);
                //
                // // Add custom claims in token here based on user properties or any other source
                // claims.AddRange(userClaims);
                //
                // Console.WriteLine("========= claims ==========");
                // foreach (var claim in claims)
                // {
                //     Console.WriteLine(claim.Type,claim.Value);
                // }
                // Console.WriteLine("======= end claims ========");
                // context.IssuedClaims = claims;
                if (_userManager.SupportsUserEmail)
                {
                    claims.AddRange(new[]
                    {
                        new Claim(JwtClaimTypes.Email, user.Email),
                        new Claim(JwtClaimTypes.EmailVerified, user.EmailConfirmed ? "true" : "false", ClaimValueTypes.Boolean)
                    });
                }

                if (_userManager.SupportsUserPhoneNumber && !string.IsNullOrWhiteSpace(user.PhoneNumber))
                {
                    claims.AddRange(new[]
                    {
                        new Claim(JwtClaimTypes.PhoneNumber, user.PhoneNumber),
                        new Claim(JwtClaimTypes.PhoneNumberVerified, user.PhoneNumberConfirmed ? "true" : "false", ClaimValueTypes.Boolean)
                    });
                }

                return claims;
            }

            if (_userManager.SupportsUserPhoneNumber && !string.IsNullOrWhiteSpace(user.PhoneNumber))
            {
                claims.AddRange(new[]
                {
                    new Claim(JwtClaimTypes.PhoneNumber, user.PhoneNumber),
                    new Claim(JwtClaimTypes.PhoneNumberVerified, user.PhoneNumberConfirmed ? "true" : "false", ClaimValueTypes.Boolean)
                });
            }

            return claims;
        }
            // private readonly IUserClaimsPrincipalFactory<O2User> _claimsFactory;
            // private readonly UserManager<O2User> _userManager;
            //
            // public ProfileService(UserManager<O2User> userManager, IUserClaimsPrincipalFactory<O2User> claimsFactory)
            // {
            //     _userManager = userManager;
            //     _claimsFactory = claimsFactory;
            // }
            //
            // public async Task GetProfileDataAsync(ProfileDataRequestContext context)
            // {
            //     var sub = context.Subject.GetSubjectId();
            //     var user = await _userManager.FindByIdAsync(sub);
            //     var principal = await _claimsFactory.CreateAsync(user);
            //
            //     var claims = principal.Claims.ToList();
            //     claims = claims.Where(claim => context.RequestedClaimTypes.Contains(claim.Type)).ToList();
            //     claims.Add(new Claim(JwtClaimTypes.GivenName, user.UserName));
            //     claims.Add(new Claim(IdentityServerConstants.StandardScopes.Email, user.Email));
            //     // claims.Add(new Claim(IdentityServerConstants.StandardScopes.Phone ));
            //     claims.Add(new Claim(JwtClaimTypes.FamilyName, user.Lastname));
            //     claims.Add(new Claim(JwtClaimTypes.PhoneNumber, user.PhoneNumber));
            //     claims.Add(new Claim(JwtClaimTypes.Picture, user.ProfilePhoto));
            //     claims.Add(new Claim("is_specialist", user.IsSpecialist.ToString()));
            //     
            //     
            //     //Get user claims from AspNetUserClaims table
            //     var userClaims = await _userManager.GetClaimsAsync(user);
            //     
            //     // Add custom claims in token here based on user properties or any other source
            //     claims.AddRange(userClaims);
            //
            //     Console.WriteLine("========= claims ==========");
            //     foreach (var claim in claims)
            //     {
            //         Console.WriteLine(claim.Type,claim.Value);
            //     }
            //     Console.WriteLine("======= end claims ========");
            //     context.IssuedClaims = claims;
            // }
            //
            // public async Task IsActiveAsync(IsActiveContext context)
            // {
            //     var sub = context.Subject.GetSubjectId();
            //     var user = await _userManager.FindByIdAsync(sub);
            //     context.IsActive = user != null;
            // }
        }
    }