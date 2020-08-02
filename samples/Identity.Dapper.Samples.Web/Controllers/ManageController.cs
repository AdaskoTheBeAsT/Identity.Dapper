using System;
using System.Linq;
using System.Threading.Tasks;
using Identity.Dapper.Samples.Web.Entities;
using Identity.Dapper.Samples.Web.Models.ManageViewModels;
using Identity.Dapper.Samples.Web.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Identity.Dapper.Samples.Web.Controllers
{
    [Authorize]
    public class ManageController : Controller
    {
        private readonly UserManager<CustomUser> _userManager;
        private readonly SignInManager<CustomUser> _signInManager;
        private readonly IEmailSender _emailSender;
        private readonly ISmsSender _smsSender;
        private readonly ILogger _logger;
        private readonly IAuthenticationSchemeProvider _authenticationSchemeProvider;

        public ManageController(
        UserManager<CustomUser> userManager,
        SignInManager<CustomUser> signInManager,
        IEmailSender emailSender,
        ISmsSender smsSender,
        ILoggerFactory loggerFactory,
        IAuthenticationSchemeProvider authenticationSchemeProvider)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _emailSender = emailSender;
            _smsSender = smsSender;
            _authenticationSchemeProvider = authenticationSchemeProvider;
            _logger = loggerFactory.CreateLogger<ManageController>();
        }

        public enum ManageMessageId
        {
            AddPhoneSuccess,
            AddLoginSuccess,
            ChangePasswordSuccess,
            SetTwoFactorSuccess,
            SetPasswordSuccess,
            RemoveLoginSuccess,
            RemovePhoneSuccess,
            Error,
        }

        // GET: /Manage/Index
        [HttpGet]
        public async Task<IActionResult> Index(ManageMessageId? message = null)
        {
            ViewData["StatusMessage"] = message switch
            {
                ManageMessageId.ChangePasswordSuccess => "Your password has been changed.",
                ManageMessageId.SetPasswordSuccess => "Your password has been set.",
                ManageMessageId.SetTwoFactorSuccess => "Your two-factor authentication provider has been set.",
                ManageMessageId.Error => "An error has occurred.",
                ManageMessageId.AddPhoneSuccess => "Your phone number was added.",
                ManageMessageId.RemovePhoneSuccess => "Your phone number was removed.",
                _ => string.Empty,
            };

            var user = await GetCurrentUserAsync();
            var model = new IndexViewModel
            {
                HasPassword = await _userManager.HasPasswordAsync(user),
                PhoneNumber = await _userManager.GetPhoneNumberAsync(user),
                TwoFactor = await _userManager.GetTwoFactorEnabledAsync(user),
                Logins = await _userManager.GetLoginsAsync(user),
                BrowserRemembered = await _signInManager.IsTwoFactorClientRememberedAsync(user),
            };
            return View(model);
        }

        // POST: /Manage/RemoveLogin
        [HttpPost]
        [ValidateAntiForgeryToken]
#pragma warning disable S4457 // Parameter validation in "async"/"await" methods should be wrapped
        public async Task<IActionResult> RemoveLogin(RemoveLoginViewModel account)
#pragma warning restore S4457 // Parameter validation in "async"/"await" methods should be wrapped
        {
            if (account is null)
            {
                throw new ArgumentNullException(nameof(account));
            }

            ManageMessageId? message = ManageMessageId.Error;
            var user = await GetCurrentUserAsync();
            if (user != null)
            {
                var result = await _userManager.RemoveLoginAsync(user, account.LoginProvider, account.ProviderKey);
                if (result.Succeeded)
                {
                    await _signInManager.SignInAsync(user, isPersistent: false);
                    message = ManageMessageId.RemoveLoginSuccess;
                }
            }

            return RedirectToAction(nameof(ManageLogins), new { Message = message });
        }

        // GET: /Manage/AddPhoneNumber
        public IActionResult AddPhoneNumber()
        {
            return View();
        }

        // POST: /Manage/AddPhoneNumber
        [HttpPost]
        [ValidateAntiForgeryToken]
#pragma warning disable S4457 // Parameter validation in "async"/"await" methods should be wrapped
        public async Task<IActionResult> AddPhoneNumber(AddPhoneNumberViewModel model)
#pragma warning restore S4457 // Parameter validation in "async"/"await" methods should be wrapped
        {
            if (model is null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Generate the token and send it
            var user = await GetCurrentUserAsync();
            var code = await _userManager.GenerateChangePhoneNumberTokenAsync(user, model.PhoneNumber);
            await _smsSender.SendSmsAsync(model.PhoneNumber ?? string.Empty, "Your security code is: " + code);
            return RedirectToAction(nameof(VerifyPhoneNumber), new { model.PhoneNumber });
        }

        // POST: /Manage/EnableTwoFactorAuthentication
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EnableTwoFactorAuthentication()
        {
            var user = await GetCurrentUserAsync();
            if (user != null)
            {
                await _userManager.SetTwoFactorEnabledAsync(user, true);
                await _signInManager.SignInAsync(user, isPersistent: false);
                _logger.LogInformation(1, "User enabled two-factor authentication.");
            }

            return RedirectToAction(nameof(Index), "Manage");
        }

        // POST: /Manage/DisableTwoFactorAuthentication
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DisableTwoFactorAuthentication()
        {
            var user = await GetCurrentUserAsync();
            if (user != null)
            {
                await _userManager.SetTwoFactorEnabledAsync(user, false);
                await _signInManager.SignInAsync(user, isPersistent: false);
                _logger.LogInformation(2, "User disabled two-factor authentication.");
            }

            return RedirectToAction(nameof(Index), "Manage");
        }

        // GET: /Manage/VerifyPhoneNumber
        [HttpGet]
        public async Task<IActionResult> VerifyPhoneNumber(string phoneNumber)
        {
            await _userManager.GenerateChangePhoneNumberTokenAsync(await GetCurrentUserAsync(), phoneNumber);

            // Send an SMS to verify the phone number
            return phoneNumber == null ? View("Error") : View(new VerifyPhoneNumberViewModel { PhoneNumber = phoneNumber });
        }

        // POST: /Manage/VerifyPhoneNumber
        [HttpPost]
        [ValidateAntiForgeryToken]
#pragma warning disable S4457 // Parameter validation in "async"/"await" methods should be wrapped
        public async Task<IActionResult> VerifyPhoneNumber(VerifyPhoneNumberViewModel model)
#pragma warning restore S4457 // Parameter validation in "async"/"await" methods should be wrapped
        {
            if (model is null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await GetCurrentUserAsync();
            if (user != null)
            {
                var result = await _userManager.ChangePhoneNumberAsync(user, model.PhoneNumber, model.Code);
                if (result.Succeeded)
                {
                    await _signInManager.SignInAsync(user, isPersistent: false);
                    return RedirectToAction(nameof(Index), new { Message = ManageMessageId.AddPhoneSuccess });
                }
            }

            // If we got this far, something failed, redisplay the form
            ModelState.AddModelError(string.Empty, "Failed to verify phone number");
            return View(model);
        }

        // POST: /Manage/RemovePhoneNumber
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemovePhoneNumber()
        {
            var user = await GetCurrentUserAsync();
            if (user != null)
            {
                var result = await _userManager.SetPhoneNumberAsync(user, null);
                if (result.Succeeded)
                {
                    await _signInManager.SignInAsync(user, isPersistent: false);
                    return RedirectToAction(nameof(Index), new { Message = ManageMessageId.RemovePhoneSuccess });
                }
            }

            return RedirectToAction(nameof(Index), new { Message = ManageMessageId.Error });
        }

        // GET: /Manage/ChangePassword
        [HttpGet]
        public IActionResult ChangePassword()
        {
            return View();
        }

        // POST: /Manage/ChangePassword
        [HttpPost]
        [ValidateAntiForgeryToken]
#pragma warning disable S4457 // Parameter validation in "async"/"await" methods should be wrapped
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
#pragma warning restore S4457 // Parameter validation in "async"/"await" methods should be wrapped
        {
            if (model is null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await GetCurrentUserAsync();
            if (user != null)
            {
                var result = await _userManager.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);
                if (result.Succeeded)
                {
                    await _signInManager.SignInAsync(user, isPersistent: false);
                    _logger.LogInformation(3, "User changed their password successfully.");
                    return RedirectToAction(nameof(Index), new { Message = ManageMessageId.ChangePasswordSuccess });
                }

                AddErrors(result);
                return View(model);
            }

            return RedirectToAction(nameof(Index), new { Message = ManageMessageId.Error });
        }

        // GET: /Manage/SetPassword
        [HttpGet]
        public IActionResult SetPassword()
        {
            return View();
        }

        // POST: /Manage/SetPassword
        [HttpPost]
        [ValidateAntiForgeryToken]
#pragma warning disable S4457 // Parameter validation in "async"/"await" methods should be wrapped
        public async Task<IActionResult> SetPassword(SetPasswordViewModel model)
#pragma warning restore S4457 // Parameter validation in "async"/"await" methods should be wrapped
        {
            if (model is null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await GetCurrentUserAsync();
            if (user != null)
            {
                var result = await _userManager.AddPasswordAsync(user, model.NewPassword);
                if (result.Succeeded)
                {
                    await _signInManager.SignInAsync(user, isPersistent: false);
                    return RedirectToAction(nameof(Index), new { Message = ManageMessageId.SetPasswordSuccess });
                }

                AddErrors(result);
                return View(model);
            }

            return RedirectToAction(nameof(Index), new { Message = ManageMessageId.Error });
        }

        // GET: /Manage/ManageLogins
        [HttpGet]
        public async Task<IActionResult> ManageLogins(ManageMessageId? message = null)
        {
            ViewData["StatusMessage"] = message switch
            {
                ManageMessageId.RemoveLoginSuccess => "The external login was removed.",
                ManageMessageId.AddLoginSuccess => "The external login was added.",
                ManageMessageId.Error => "An error has occurred.",
                _ => string.Empty,
            };

            var user = await GetCurrentUserAsync();
            if (user == null)
            {
                return View("Error");
            }

            var userLogins = await _userManager.GetLoginsAsync(user);
            var schemes = await _authenticationSchemeProvider.GetAllSchemesAsync();
            var otherLogins = schemes.Where(
                    auth => userLogins.All(
                        ul => !auth.Name.Equals(ul.LoginProvider, StringComparison.OrdinalIgnoreCase)))
                .ToList();
            ViewData["ShowRemoveButton"] = user.PasswordHash != null || userLogins.Count > 1;
            return View(new ManageLoginsViewModel
            {
                CurrentLogins = userLogins,
                OtherLogins = otherLogins,
            });
        }

        // POST: /Manage/LinkLogin
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult LinkLogin(string provider)
        {
            // Request a redirect to the external login provider to link a login for the current user
            var redirectUrl = Url.Action(nameof(LinkLoginCallback), "Manage");
            var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl, _userManager.GetUserId(User));
            return new ChallengeResult(provider, properties);
        }

        // GET: /Manage/LinkLoginCallback
        [HttpGet]
        public async Task<ActionResult> LinkLoginCallback()
        {
            var user = await GetCurrentUserAsync();
            if (user == null)
            {
                return View("Error");
            }

            var info = await _signInManager.GetExternalLoginInfoAsync(await _userManager.GetUserIdAsync(user));
            if (info == null)
            {
                return RedirectToAction(nameof(ManageLogins), new { Message = ManageMessageId.Error });
            }

            var result = await _userManager.AddLoginAsync(user, info);
            var message = result.Succeeded ? ManageMessageId.AddLoginSuccess : ManageMessageId.Error;
            return RedirectToAction(nameof(ManageLogins), new { Message = message });
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }

        private Task<CustomUser> GetCurrentUserAsync()
        {
            return _userManager.GetUserAsync(HttpContext.User);
        }
    }
}
