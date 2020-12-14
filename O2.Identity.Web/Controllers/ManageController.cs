using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using O2.Identity.Web.Extensions;
using O2.Identity.Web.Models;
using O2.Identity.Web.Models.ManageViewModels;
using O2.Identity.Web.Resources;
using O2.Identity.Web.Services;

namespace O2.Identity.Web.Controllers
{
    [Authorize]
    [Route("[controller]/[action]")]
    public class ManageController : Controller
    {
        private readonly UserManager<O2User> _userManager;
        private readonly SignInManager<O2User> _signInManager;
        private readonly IEmailSender _emailSender;
        private readonly ILogger _logger;
        private readonly UrlEncoder _urlEncoder;
        private readonly IStringLocalizer<IndexViewModel> _localizer;
        private readonly IStringLocalizer<SharedResource> _sharedLocalizer;
        private Cloudinary _cloudinary;

        private const string AuthenicatorUriFormat = "otpauth://totp/{0}:{1}?secret={2}&issuer={0}&digits=6";

        public ManageController(
            UserManager<O2User> userManager,
            SignInManager<O2User> signInManager,
            IEmailSender emailSender,
            ILogger<ManageController> logger,
            UrlEncoder urlEncoder,
            IOptions<CloudinarySettings> _cloudinaryConfig,
            IStringLocalizer<IndexViewModel> localizer,
            IStringLocalizer<SharedResource> sharedLocalizer
        )
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _emailSender = emailSender;
            _logger = logger;
            _urlEncoder = urlEncoder;
            _localizer = localizer;
            _sharedLocalizer = sharedLocalizer;

            Account acc = new Account(
                _cloudinaryConfig.Value.CloudName,
                _cloudinaryConfig.Value.ApiKey,
                _cloudinaryConfig.Value.ApiSecret
            );

            _cloudinary = new Cloudinary(acc);
        }

        public class CloudinarySettings
        {
            public string CloudName { get; set; }
            public string ApiKey { get; set; }
            public string ApiSecret { get; set; }
        }

        [TempData] public string StatusMessage { get; set; }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            var model = new IndexViewModel
            {
                Id = user.Id,
                Username = user.UserName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                IsEmailConfirmed = user.EmailConfirmed,
                StatusMessage = StatusMessage,
                ProfilePhoto = user.ProfilePhoto,
                Firstname = user.Firstname,
                Lastname = user.Lastname,
                Country = user.Country,
                City = user.City,
                Birthday = user.Birthday,
                RegistrationDate = user.RegistrationDate
            };

            return View(model);
        }

        [HttpGet]
        public IActionResult PaymentAndBilling(int page = 1)
        {
            var invoicesViewModels = new List<InvoiceViewModel>()
            {
                new InvoiceViewModel()
                {
                    InvoiceID = 1,
                    InvoiceDate = DateTime.Now,
                    DueDate = DateTime.Now.AddDays(30),
                    BillingPeriod = "9/8/2020 - 9/30/2020",
                    AmountDue = "50 USD",
                    TotalAmount = "50 USD",
                    Status = "Paid on 9/26/2020"
                },
                new InvoiceViewModel()
                {
                    InvoiceID = 2,
                    InvoiceDate = DateTime.Now.AddDays(30),
                    DueDate = DateTime.Now.AddDays(60),
                    BillingPeriod = "10/8/2020 - 10/30/2020",
                    AmountDue = "100 USD",
                    TotalAmount = "100 USD",
                    Status = "Paid on 10/26/2020"
                },
                new InvoiceViewModel()
                {
                    InvoiceID = 3,
                    InvoiceDate = DateTime.Now.AddDays(30),
                    DueDate = DateTime.Now.AddDays(60),
                    BillingPeriod = "11/8/2020 - 11/30/2020",
                    AmountDue = "100 USD",
                    TotalAmount = "100 USD",
                    Status = "Paid on 11/26/2020"
                }
            };
            
            int pageSize = 10;   // количество элементов на странице

            var source = invoicesViewModels;
            var count =  source.Count();
            var items =  source.Skip((page - 1) * pageSize).Take(pageSize).ToList();
             
            PageViewModel pageViewModel = new PageViewModel(count, page, pageSize);
            InvoicesViewModel viewModel = new InvoicesViewModel
            {
                PageViewModel = pageViewModel,
                Invoices = items
            };
            return View(viewModel);
        }
        
        [HttpGet]
        public IActionResult ServicesAndSubscriptions(int page = 1)
        {
            var list = new List<SubscriptionViewModel>()
            {
                new SubscriptionViewModel()
                {
                    AppName = "PFR Community",
                    Cost = 0,
                    Term = true,
                    Description = "ПФР сообщество. Платформа. Позволяет реализовать связь между клиентами и специалистами ПФР."
                },
                new SubscriptionViewModel()
                {
                    AppName = "O2 Account",
                    Cost = 0,
                    Term = true,
                    Description = "O2 Аккаунт  - специальзированный аккаунт для входа во все системы платформы O2."
                },
                new SubscriptionViewModel()
                {
                    AppName = "O2 OrdLine",
                    Cost = 0,
                    Term = true,
                    Description = "O2 OrdLine  - Приложения для связи социальных сетей. Landing Page - Аккаунт."
                }
            };
            
             int pageSize = 10;   // количество элементов на странице

             var source = list;
            var count =  source.Count();
            var items =  source.Skip((page - 1) * pageSize).Take(pageSize).ToList();
 
            PageViewModel pageViewModel = new PageViewModel(count, page, pageSize);
            SubscriptionsViewModel viewModel = new SubscriptionsViewModel
            {
                PageViewModel = pageViewModel,
                Subscriptions = items
            };
                        
                        
            return View(viewModel);
        }

        [HttpGet]
        // [ValidateAntiForgeryToken]
        //[ActionName("Users")]
        public async Task<IActionResult> GetUsers(int page=1)
        {
            
            int pageSize = 10;   // количество элементов на странице
             
            var source = _userManager.Users.ToList();
            var speciaCount = source.Count(x => x.IsSpecialist);
            var clientsCount = source.Count(x => x.IsSpecialist==false);
            var count =  source.Count();
            var items =  source.Skip((page - 1) * pageSize).Take(pageSize).ToList();
            
            PageViewModel pageViewModel = new PageViewModel(count, page, pageSize);
            var user =await _userManager.GetUserAsync(User);
            UsersViewModel viewModel = new UsersViewModel
            {
                SpecialistCount = speciaCount,
                ClientCount= clientsCount,
                PageViewModel = pageViewModel,
                Users = items,
                UserId = user.Id
            };
            
            return View(viewModel);
        }

        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(IndexViewModel model, string submit)
        {
            if (submit=="copyId")
            {
                return View(model);
            }
            
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            var email = user.Email;
            if (model.Email != email)
            {
                var setEmailResult = await _userManager.SetEmailAsync(user, model.Email);
                if (!setEmailResult.Succeeded)
                {
                    throw new ApplicationException($"Unexpected error occurred setting email for user with ID '{user.Id}'.");
                }
            }

            var phoneNumber = user.PhoneNumber;
            if (model.PhoneNumber != phoneNumber)
            {
                var setPhoneResult = await _userManager.SetPhoneNumberAsync(user, model.PhoneNumber);
                if (!setPhoneResult.Succeeded)
                {
                    throw new ApplicationException($"Unexpected error occurred setting phone number for user with ID '{user.Id}'.");
                }
            }

            var profilePhoto = user.ProfilePhoto;
            if (model.ProfilePhoto != profilePhoto || profilePhoto==null)
            {
                var uploadResult = new ImageUploadResult();
                var file = model.FormFile;
                if (file != null)
                {
                    if (file.Length > 0)
                    {
                        using (var stream = file.OpenReadStream())
                        {
                            var uploadParams = new ImageUploadParams()
                            {
                                File = new FileDescription(user.Id + "_" + file.Name, stream),
                                Transformation = new Transformation()
                                    .Width(500).Height(500).Crop("fill").Gravity("face")
                            };

                            uploadResult = _cloudinary.Upload(uploadParams);
                        }
                    }

                    var newPhoto = new Photo();
                    newPhoto.Url = uploadResult.Uri.ToString();
                    newPhoto.PublicId = uploadResult.PublicId;
                    newPhoto.IsMain = true;
                    //
                    if (user.Photos == null)
                        user.Photos = new List<Photo>();
                    user.Photos.Add(newPhoto);
                    //
                    user.ProfilePhoto = user.Photos.Single(x => x.IsMain).Url;

                    var setUpdateUser = await _userManager.UpdateAsync(user);
                    if (!setUpdateUser.Succeeded)
                    {
                        throw new ApplicationException(
                            $"Unexpected error occurred setting phone number for user with ID '{user.Id}'.");
                    }
                }

            }
            
            var firstname = user.Firstname;
            if (model.Firstname != firstname)
            {
                user.Firstname = model.Firstname;
                var setUpdateUser = await _userManager.UpdateAsync(user);
                if (!setUpdateUser.Succeeded)
                {
                    throw new ApplicationException($"Unexpected error occurred setting phone number for user with ID '{user.Id}'.");
                }
            }
            
            var lastname = user.Lastname;
            if (model.Lastname != lastname)
            {
                user.Lastname = model.Lastname;
                var setUpdateUser = await _userManager.UpdateAsync(user);
                if (!setUpdateUser.Succeeded)
                {
                    throw new ApplicationException($"Unexpected error occurred setting phone number for user with ID '{user.Id}'.");
                }
            }
        
            var birthday = user.Birthday;
            if (model.Birthday != birthday)
            {
                user.Birthday = model.Birthday;
                var setUpdateUser = await _userManager.UpdateAsync(user);
                if (!setUpdateUser.Succeeded)
                {
                    throw new ApplicationException($"Unexpected error occurred setting phone number for user with ID '{user.Id}'.");
                }
                
            }
            
            var country = user.Country;
            if (model.Country != country)
            {
                user.Country = model.Country;
                var setUpdateUser = await _userManager.UpdateAsync(user);
                if (!setUpdateUser.Succeeded)
                {
                    throw new ApplicationException($"Unexpected error occurred setting phone number for user with ID '{user.Id}'.");
                }
            }
            var city = user.City;
            if (model.City != city)
            {
                user.City = model.City;
                var setUpdateUser = await _userManager.UpdateAsync(user);
                if (!setUpdateUser.Succeeded)
                {
                    throw new ApplicationException($"Unexpected error occurred setting phone number for user with ID '{user.Id}'.");
                }
                
            }
            StatusMessage = "Ваш профиль has been updated";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SendVerificationEmail(IndexViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var callbackUrl = Url.EmailConfirmationLink(user.Id, code, Request.Scheme);
            var email = user.Email;
            await _emailSender.SendEmailConfirmationAsync(email, callbackUrl);

            StatusMessage = "Verification email sent. Please check your email.";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> ChangePassword()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            var hasPassword = await _userManager.HasPasswordAsync(user);
            if (!hasPassword)
            {
                return RedirectToAction(nameof(SetPassword));
            }

            var model = new ChangePasswordViewModel { StatusMessage = StatusMessage };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            var changePasswordResult = await _userManager.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);
            if (!changePasswordResult.Succeeded)
            {
                AddErrors(changePasswordResult);
                return View(model);
            }

            await _signInManager.SignInAsync(user, isPersistent: false);
            _logger.LogInformation("User changed their password successfully.");
            StatusMessage = "Your password has been changed.";

            return RedirectToAction(nameof(ChangePassword));
        }

        [HttpGet]
        public async Task<IActionResult> SetPassword()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            var hasPassword = await _userManager.HasPasswordAsync(user);

            if (hasPassword)
            {
                return RedirectToAction(nameof(ChangePassword));
            }

            var model = new SetPasswordViewModel { StatusMessage = StatusMessage };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SetPassword(SetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            var addPasswordResult = await _userManager.AddPasswordAsync(user, model.NewPassword);
            if (!addPasswordResult.Succeeded)
            {
                AddErrors(addPasswordResult);
                return View(model);
            }

            await _signInManager.SignInAsync(user, isPersistent: false);
            StatusMessage = "Your password has been set.";

            return RedirectToAction(nameof(SetPassword));
        }

        [HttpGet]
        public async Task<IActionResult> ExternalLogins()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            var model = new ExternalLoginsViewModel { CurrentLogins = await _userManager.GetLoginsAsync(user) };
            model.OtherLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync())
                .Where(auth => model.CurrentLogins.All(ul => auth.Name != ul.LoginProvider))
                .ToList();
            model.ShowRemoveButton = await _userManager.HasPasswordAsync(user) || model.CurrentLogins.Count > 1;
            model.StatusMessage = StatusMessage;

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LinkLogin(string provider)
        {
            // Clear the existing external cookie to ensure a clean login process
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

            // Request a redirect to the external login provider to link a login for the current user
            var redirectUrl = Url.Action(nameof(LinkLoginCallback));
            var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl, _userManager.GetUserId(User));
            return new ChallengeResult(provider, properties);
        }

        [HttpGet]
        public async Task<IActionResult> LinkLoginCallback()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            var info = await _signInManager.GetExternalLoginInfoAsync(user.Id);
            if (info == null)
            {
                throw new ApplicationException($"Unexpected error occurred loading external login info for user with ID '{user.Id}'.");
            }

            var result = await _userManager.AddLoginAsync(user, info);
            if (!result.Succeeded)
            {
                throw new ApplicationException($"Unexpected error occurred adding external login for user with ID '{user.Id}'.");
            }

            // Clear the existing external cookie to ensure a clean login process
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

            StatusMessage = "The external login was added.";
            return RedirectToAction(nameof(ExternalLogins));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveLogin(RemoveLoginViewModel model)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            var result = await _userManager.RemoveLoginAsync(user, model.LoginProvider, model.ProviderKey);
            if (!result.Succeeded)
            {
                throw new ApplicationException($"Unexpected error occurred removing external login for user with ID '{user.Id}'.");
            }

            await _signInManager.SignInAsync(user, isPersistent: false);
            StatusMessage = "The external login was removed.";
            return RedirectToAction(nameof(ExternalLogins));
        }

        [HttpGet]
        public async Task<IActionResult> TwoFactorAuthentication()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            var model = new TwoFactorAuthenticationViewModel
            {
                HasAuthenticator = await _userManager.GetAuthenticatorKeyAsync(user) != null,
                Is2faEnabled = user.TwoFactorEnabled,
                RecoveryCodesLeft = await _userManager.CountRecoveryCodesAsync(user),
            };

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Disable2faWarning()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            if (!user.TwoFactorEnabled)
            {
                throw new ApplicationException($"Unexpected error occured disabling 2FA for user with ID '{user.Id}'.");
            }

            return View(nameof(Disable2fa));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Disable2fa()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            var disable2faResult = await _userManager.SetTwoFactorEnabledAsync(user, false);
            if (!disable2faResult.Succeeded)
            {
                throw new ApplicationException($"Unexpected error occured disabling 2FA for user with ID '{user.Id}'.");
            }

            _logger.LogInformation("User with ID {UserId} has disabled 2fa.", user.Id);
            return RedirectToAction(nameof(TwoFactorAuthentication));
        }

        [HttpGet]
        public async Task<IActionResult> EnableAuthenticator()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            var unformattedKey = await _userManager.GetAuthenticatorKeyAsync(user);
            if (string.IsNullOrEmpty(unformattedKey))
            {
                await _userManager.ResetAuthenticatorKeyAsync(user);
                unformattedKey = await _userManager.GetAuthenticatorKeyAsync(user);
            }

            var model = new EnableAuthenticatorViewModel
            {
                SharedKey = FormatKey(unformattedKey),
                AuthenticatorUri = GenerateQrCodeUri(user.Email, unformattedKey)
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EnableAuthenticator(EnableAuthenticatorViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            // Strip spaces and hypens
            var verificationCode = model.Code.Replace(" ", string.Empty).Replace("-", string.Empty);

            var is2faTokenValid = await _userManager.VerifyTwoFactorTokenAsync(
                user, _userManager.Options.Tokens.AuthenticatorTokenProvider, verificationCode);

            if (!is2faTokenValid)
            {
                ModelState.AddModelError("model.Code", "Verification code is invalid.");
                return View(model);
            }

            await _userManager.SetTwoFactorEnabledAsync(user, true);
            _logger.LogInformation("User with ID {UserId} has enabled 2FA with an authenticator app.", user.Id);
            return RedirectToAction(nameof(GenerateRecoveryCodes));
        }

        [HttpGet]
        public IActionResult ResetAuthenticatorWarning()
        {
            return View(nameof(ResetAuthenticator));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetAuthenticator()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            await _userManager.SetTwoFactorEnabledAsync(user, false);
            await _userManager.ResetAuthenticatorKeyAsync(user);
            _logger.LogInformation("User with id '{UserId}' has reset their authentication app key.", user.Id);

            return RedirectToAction(nameof(EnableAuthenticator));
        }

        [HttpGet]
        public async Task<IActionResult> GenerateRecoveryCodes()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            if (!user.TwoFactorEnabled)
            {
                throw new ApplicationException($"Cannot generate recovery codes for user with ID '{user.Id}' as they do not have 2FA enabled.");
            }

            var recoveryCodes = await _userManager.GenerateNewTwoFactorRecoveryCodesAsync(user, 10);
            var model = new GenerateRecoveryCodesViewModel { RecoveryCodes = recoveryCodes.ToArray() };

            _logger.LogInformation("User with ID {UserId} has generated new 2FA recovery codes.", user.Id);

            return View(model);
        }

        #region Helpers

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }

        private string FormatKey(string unformattedKey)
        {
            var result = new StringBuilder();
            int currentPosition = 0;
            while (currentPosition + 4 < unformattedKey.Length)
            {
                result.Append(unformattedKey.Substring(currentPosition, 4)).Append(" ");
                currentPosition += 4;
            }
            if (currentPosition < unformattedKey.Length)
            {
                result.Append(unformattedKey.Substring(currentPosition));
            }

            return result.ToString().ToLowerInvariant();
        }

        private string GenerateQrCodeUri(string email, string unformattedKey)
        {
            return string.Format(
                AuthenicatorUriFormat,
                _urlEncoder.Encode("O2.Identity.Web"),
                _urlEncoder.Encode(email),
                unformattedKey);
        }

        #endregion

        
    }
}
