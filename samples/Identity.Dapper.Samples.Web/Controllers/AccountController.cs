using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Identity.Dapper.Samples.Web.Entities;
using Identity.Dapper.Samples.Web.Models.AccountViewModels;
using Identity.Dapper.Samples.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;

namespace Identity.Dapper.Samples.Web.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private readonly UserManager<CustomUser> _userManager;
        private readonly SignInManager<CustomUser> _signInManager;
        private readonly IEmailSender _emailSender;
        private readonly ISmsSender _smsSender;
        private readonly ILogger _logger;

        public AccountController(
            UserManager<CustomUser> userManager,
            SignInManager<CustomUser> signInManager,
            IEmailSender emailSender,
            ISmsSender smsSender,
            ILoggerFactory loggerFactory)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _emailSender = emailSender;
            _smsSender = smsSender;
            _logger = loggerFactory.CreateLogger<AccountController>();
        }

        // GET: /Account/Login
        [HttpGet]
        [AllowAnonymous]
#pragma warning disable CA1054 // Uri parameters should not be strings
        public IActionResult Login(string? returnUrl = null)
#pragma warning restore CA1054 // Uri parameters should not be strings
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
#pragma warning disable CA1054 // Uri parameters should not be strings
#pragma warning disable S4457 // Parameter validation in "async"/"await" methods should be wrapped
        public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
#pragma warning restore S4457 // Parameter validation in "async"/"await" methods should be wrapped
#pragma warning restore CA1054 // Uri parameters should not be strings
        {
            if (model is null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            ViewData["ReturnUrl"] = returnUrl;
            if (ModelState.IsValid)
            {
                // This doesn't count login failures towards account lockout
                // To enable password failures to trigger account lockout, set lockoutOnFailure: true
#pragma warning disable SEC0018 // Identity Password Lockout Disabled
                var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, lockoutOnFailure: false);
#pragma warning restore SEC0018 // Identity Password Lockout Disabled
                if (result.Succeeded)
                {
                    _logger.LogInformation(1, "User logged in.");
                    return RedirectToLocal(returnUrl ?? string.Empty);
                }

                if (result.RequiresTwoFactor)
                {
                    return RedirectToAction(
                        nameof(SendCode),
                        new
                        {
                            ReturnUrl = returnUrl,
                            model.RememberMe,
                        });
                }

                if (result.IsLockedOut)
                {
                    _logger.LogWarning(2, "User account locked out.");
                    return View("Lockout");
                }

                ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                return View(model);
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        // GET: /Account/Register
        [HttpGet]
        [AllowAnonymous]
#pragma warning disable CA1054 // Uri parameters should not be strings
#pragma warning disable S4144 // Methods should not have identical implementations
        public IActionResult Register(string? returnUrl = null)
#pragma warning restore S4144 // Methods should not have identical implementations
#pragma warning restore CA1054 // Uri parameters should not be strings
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        // POST: /Account/Register
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
#pragma warning disable CA1054 // Uri parameters should not be strings
#pragma warning disable S4457 // Parameter validation in "async"/"await" methods should be wrapped
        public async Task<IActionResult> Register(RegisterViewModel model, string? returnUrl = null)
#pragma warning restore S4457 // Parameter validation in "async"/"await" methods should be wrapped
#pragma warning restore CA1054 // Uri parameters should not be strings
        {
            if (model is null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            ViewData["ReturnUrl"] = returnUrl;
            if (ModelState.IsValid)
            {
                var user = new CustomUser { UserName = model.Email ?? string.Empty, Email = model.Email ?? string.Empty, Address = model.Address };
                var result = await _userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
#pragma warning disable S125 // Sections of code should not be commented out
                    // For more information on how to enable account confirmation and password reset please visit http://go.microsoft.com/fwlink/?LinkID=532713
                    // Send an email with this link
                    // var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    // var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: HttpContext.Request.Scheme);
                    // await _emailSender.SendEmailAsync(model.Email, "Confirm your account",
                    //    $"Please confirm your account by clicking this link: <a href='{callbackUrl}'>link</a>");
                    await _signInManager.SignInAsync(user, isPersistent: false);
#pragma warning restore S125 // Sections of code should not be commented out
                    _logger.LogInformation(3, "User created a new account with password.");
                    return RedirectToLocal(returnUrl ?? string.Empty);
                }

                AddErrors(result);
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        // POST: /Account/LogOff
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LogOff()
        {
            await _signInManager.SignOutAsync();
            _logger.LogInformation(4, "User logged out.");
            return RedirectToAction(nameof(HomeController.Index), "Home");
        }

        // POST: /Account/ExternalLogin
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
#pragma warning disable CA1054 // Uri parameters should not be strings
        public IActionResult ExternalLogin(string provider, string? returnUrl = null)
#pragma warning restore CA1054 // Uri parameters should not be strings
        {
            // Request a redirect to the external login provider.
            var redirectUrl = Url.Action(nameof(ExternalLoginCallback), "Account", new { ReturnUrl = returnUrl });
            var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
            return new ChallengeResult(provider, properties);
        }

        // GET: /Account/ExternalLoginCallback
        [HttpGet]
        [AllowAnonymous]
#pragma warning disable CA1054 // Uri parameters should not be strings
        public async Task<IActionResult> ExternalLoginCallback(string? returnUrl = null, string? remoteError = null)
#pragma warning restore CA1054 // Uri parameters should not be strings
        {
            if (remoteError != null)
            {
                ModelState.AddModelError(string.Empty, $"Error from external provider: {remoteError}");
                return View(nameof(Login));
            }

            var info = await _signInManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                return RedirectToAction(nameof(Login));
            }

            // Sign in the user with this external login provider if the user already has a login.
            var result = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: false);
            if (result.Succeeded)
            {
                _logger.LogInformation(5, "User logged in with {Name} provider.", info.LoginProvider);
                return RedirectToLocal(returnUrl ?? string.Empty);
            }

            if (result.RequiresTwoFactor)
            {
                return RedirectToAction(nameof(SendCode), new { ReturnUrl = returnUrl });
            }

            if (result.IsLockedOut)
            {
                return View("Lockout");
            }
            else
            {
                // If the user does not have an account, then ask the user to create an account.
                ViewData["ReturnUrl"] = returnUrl;
                ViewData["LoginProvider"] = info.LoginProvider;
                var email = info.Principal.FindFirstValue(ClaimTypes.Email);
                return View(nameof(ExternalLoginConfirmation), new ExternalLoginConfirmationViewModel { Email = email });
            }
        }

        // POST: /Account/ExternalLoginConfirmation
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
#pragma warning disable CA1054 // Uri parameters should not be strings
#pragma warning disable S4457 // Parameter validation in "async"/"await" methods should be wrapped
        public async Task<IActionResult> ExternalLoginConfirmation(ExternalLoginConfirmationViewModel model, string? returnUrl = null)
#pragma warning restore S4457 // Parameter validation in "async"/"await" methods should be wrapped
#pragma warning restore CA1054 // Uri parameters should not be strings
        {
            if (model is null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            if (ModelState.IsValid)
            {
                // Get the information about the user from the external login provider
                var info = await _signInManager.GetExternalLoginInfoAsync();
                if (info == null)
                {
                    return View("ExternalLoginFailure");
                }

                var user = new CustomUser { UserName = model.Email ?? string.Empty, Email = model.Email ?? string.Empty };
                var result = await _userManager.CreateAsync(user);
                if (result.Succeeded)
                {
                    result = await _userManager.AddLoginAsync(user, info);
                    if (result.Succeeded)
                    {
                        await _signInManager.SignInAsync(user, isPersistent: false);
                        _logger.LogInformation(6, "User created an account using {Name} provider.", info.LoginProvider);
                        return RedirectToLocal(returnUrl ?? string.Empty);
                    }
                }

                AddErrors(result);
            }

            ViewData["ReturnUrl"] = returnUrl;
            return View(model);
        }

        // GET: /Account/ConfirmEmail
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> ConfirmEmail(string userId, string code)
        {
            if (userId == null || code == null)
            {
                return View("Error");
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return View("Error");
            }

            var result = await _userManager.ConfirmEmailAsync(user, code);
            return View(result.Succeeded ? nameof(ConfirmEmail) : "Error");
        }

        // GET: /Account/ForgotPassword
        [HttpGet]
        [AllowAnonymous]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        // POST: /Account/ForgotPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
#pragma warning disable S4457 // Parameter validation in "async"/"await" methods should be wrapped
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
#pragma warning restore S4457 // Parameter validation in "async"/"await" methods should be wrapped
        {
            if (model is null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByNameAsync(model.Email);
                if (user == null || !await _userManager.IsEmailConfirmedAsync(user))
                {
                    // Don't reveal that the user does not exist or is not confirmed
                    return View(nameof(ForgotPasswordConfirmation));
                }

#pragma warning disable S125 // Sections of code should not be commented out

                // For more information on how to enable account confirmation and password reset please visit http://go.microsoft.com/fwlink/?LinkID=532713
                // Send an email with this link
                // var code = await _userManager.GeneratePasswordResetTokenAsync(user);
                // var callbackUrl = Url.Action("ResetPassword", "Account", new { userId = user.Id, code = code }, protocol: HttpContext.Request.Scheme);
                // await _emailSender.SendEmailAsync(model.Email, "Reset Password",
                //   $"Please reset your password by clicking here: <a href='{callbackUrl}'>link</a>");
                // return View("ForgotPasswordConfirmation");
#pragma warning restore S125 // Sections of code should not be commented out
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        // GET: /Account/ForgotPasswordConfirmation
        [HttpGet]
        [AllowAnonymous]
        public IActionResult ForgotPasswordConfirmation()
        {
            return View();
        }

        // GET: /Account/ResetPassword
        [HttpGet]
        [AllowAnonymous]
        public IActionResult ResetPassword(string? code = null)
        {
            return code == null ? View("Error") : View();
        }

        // POST: /Account/ResetPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
#pragma warning disable S4457 // Parameter validation in "async"/"await" methods should be wrapped
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
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

            var user = await _userManager.FindByNameAsync(model.Email);
            if (user == null)
            {
                // Don't reveal that the user does not exist
                return RedirectToAction(nameof(AccountController.ResetPasswordConfirmation), "Account");
            }

            var result = await _userManager.ResetPasswordAsync(user, model.Code, model.Password);
            if (result.Succeeded)
            {
                return RedirectToAction(nameof(AccountController.ResetPasswordConfirmation), "Account");
            }

            AddErrors(result);
            return View();
        }

        // GET: /Account/ResetPasswordConfirmation
        [HttpGet]
        [AllowAnonymous]
        public IActionResult ResetPasswordConfirmation()
        {
            return View();
        }

        // GET: /Account/SendCode
        [HttpGet]
        [AllowAnonymous]
#pragma warning disable CA1054 // Uri parameters should not be strings
        public async Task<ActionResult> SendCode(string? returnUrl = null, bool rememberMe = false)
#pragma warning restore CA1054 // Uri parameters should not be strings
        {
            var user = await _signInManager.GetTwoFactorAuthenticationUserAsync();
            if (user == null)
            {
                return View("Error");
            }

            var userFactors = await _userManager.GetValidTwoFactorProvidersAsync(user);
            var factorOptions = userFactors.Select(purpose => new SelectListItem { Text = purpose, Value = purpose }).ToList();
            return View(new SendCodeViewModel { Providers = factorOptions, ReturnUrl = returnUrl ?? string.Empty, RememberMe = rememberMe });
        }

        // POST: /Account/SendCode
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
#pragma warning disable S4457 // Parameter validation in "async"/"await" methods should be wrapped
        public async Task<IActionResult> SendCode(SendCodeViewModel model)
#pragma warning restore S4457 // Parameter validation in "async"/"await" methods should be wrapped
        {
            if (model is null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            if (!ModelState.IsValid)
            {
                return View();
            }

            var user = await _signInManager.GetTwoFactorAuthenticationUserAsync();
            if (user == null)
            {
                return View("Error");
            }

            // Generate the token and send it
            var code = await _userManager.GenerateTwoFactorTokenAsync(user, model.SelectedProvider);
            if (string.IsNullOrWhiteSpace(code))
            {
                return View("Error");
            }

            var message = "Your security code is: " + code;
            if (model.SelectedProvider != null && model.SelectedProvider.Equals("Email", StringComparison.OrdinalIgnoreCase))
            {
                await _emailSender.SendEmailAsync(await _userManager.GetEmailAsync(user), "Security Code", message);
            }
            else if (model.SelectedProvider != null && model.SelectedProvider.Equals("Phone", StringComparison.OrdinalIgnoreCase))
            {
                await _smsSender.SendSmsAsync(await _userManager.GetPhoneNumberAsync(user), message);
            }

            return RedirectToAction(
                nameof(VerifyCode),
                new
                {
                    Provider = model.SelectedProvider,
                    model.ReturnUrl,
                    model.RememberMe,
                });
        }

        // GET: /Account/VerifyCode
        [HttpGet]
        [AllowAnonymous]
#pragma warning disable CA1054 // Uri parameters should not be strings
        public async Task<IActionResult> VerifyCode(string provider, bool rememberMe, string? returnUrl = null)
#pragma warning restore CA1054 // Uri parameters should not be strings
        {
            // Require that the user has already logged in via username/password or external login
            var user = await _signInManager.GetTwoFactorAuthenticationUserAsync();
            if (user == null)
            {
                return View("Error");
            }

            return View(new VerifyCodeViewModel { Provider = provider, ReturnUrl = returnUrl ?? string.Empty, RememberMe = rememberMe });
        }

        // POST: /Account/VerifyCode
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
#pragma warning disable S4457 // Parameter validation in "async"/"await" methods should be wrapped
        public async Task<IActionResult> VerifyCode(VerifyCodeViewModel model)
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

            // The following code protects for brute force attacks against the two factor codes.
            // If a user enters incorrect codes for a specified amount of time then the user account
            // will be locked out for a specified amount of time.
            var result = await _signInManager.TwoFactorSignInAsync(model.Provider, model.Code, model.RememberMe, model.RememberBrowser);
            if (result.Succeeded)
            {
                return RedirectToLocal(model.ReturnUrl ?? string.Empty);
            }

            if (result.IsLockedOut)
            {
                _logger.LogWarning(7, "User account locked out.");
                return View("Lockout");
            }

            ModelState.AddModelError(string.Empty, "Invalid code.");
            return View(model);
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

        private IActionResult RedirectToLocal(string returnUrl)
        {
            return Url.IsLocalUrl(returnUrl)
                ? Redirect(returnUrl)
                : (IActionResult)RedirectToAction(nameof(HomeController.Index), "Home");
        }
    }
}
