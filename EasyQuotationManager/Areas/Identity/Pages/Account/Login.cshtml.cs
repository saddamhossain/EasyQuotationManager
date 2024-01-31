using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Hosting;
using EasyQuotationManager.Data;
using EasyQuotationManager.Repository.IRepository;
using EasyQuotationManager.Shared.Enums;
using Microsoft.AspNetCore.WebUtilities;
using System.Text;
using System.IO;

namespace EasyQuotationManager.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class LoginModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly ILogger<LoginModel> _logger;
        private readonly IInternalLogRepository _internalLogRepository;
        private readonly IEmailSender _emailSender;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IWebHostEnvironment _host;
        private readonly ApplicationDbContext _context;

        public LoginModel(SignInManager<IdentityUser> signInManager, ILogger<LoginModel> logger, UserManager<IdentityUser> userManager, IInternalLogRepository internalLogRepository, IEmailSender emailSender, RoleManager<IdentityRole> roleManager, IWebHostEnvironment host, ApplicationDbContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _internalLogRepository = internalLogRepository;
            _emailSender = emailSender;
            _roleManager = roleManager;
            _host = host;
            _context = context;
        }
        [BindProperty]
        public InputModel Input { get; set; }

        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        public string ReturnUrl { get; set; }

        [TempData]
        public string ErrorMessage { get; set; }

        public class InputModel
        {
            [Required]
            [EmailAddress]
            public string Email { get; set; }

            [Required]
            [DataType(DataType.Password)]
            public string Password { get; set; }

            [Display(Name = "Remember me?")]
            public bool RememberMe { get; set; }
        }

        public async Task OnGetAsync(string returnUrl = null)
        {
            if (!string.IsNullOrEmpty(ErrorMessage))
            {
                ModelState.AddModelError(string.Empty, ErrorMessage);
            }

            returnUrl ??= Url.Content("~/");

            // Clear the existing external cookie to ensure a clean login process
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            ReturnUrl = returnUrl;
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");

            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            if (ModelState.IsValid)
            {
                // Here we are checking to check LicenseExpirationDate
                var user = await _userManager.FindByEmailAsync(Input.Email);
                if (user != null)
                {
                    var applicationUser = await _context.ApplicationUser.FindAsync(user.Id);
                    var today = DateTime.UtcNow.Date;
                    if (today > applicationUser.LicenseExpirationDate.Date)
                    {
                        TempData["LicenseExpirationDate"] = "Your License Expired! Please contact with Administrator.";
                        return Page();
                    }
                }

                var result = await _signInManager.PasswordSignInAsync(Input.Email, Input.Password, Input.RememberMe, lockoutOnFailure: true);
                if (result.Succeeded)
                {
                    _logger.LogInformation("User logged in.");
                    #region Following information using for Internal Log 
                    var razorPageName = @"Identity/Account/Login.cshtml";
                    var actionName = @"OnPostAsync";
                    var log_description = $"{Input.Email} successfully logged in at {DateTime.Now}.";
                    await _internalLogRepository.InsertInternalLog(log_description, null, null, actionName, razorPageName, AuditType.Get.ToString(), null, null, null, null, null, null, null, null, null, null);
                    #endregion
                    return LocalRedirect(returnUrl);
                }
                if (result.RequiresTwoFactor)
                {
                    return RedirectToPage("./LoginWith2fa", new { ReturnUrl = returnUrl, RememberMe = Input.RememberMe });
                }
                if (result.IsNotAllowed)
                {
                    TempData["EmailConfirmed"] = "Verification email sent. Please check your email.";
                    #region Following information using for Internal Log 
                    var razorPageName = @"Identity/Account/Index.cshtml";
                    var actionName = @"OnPostAsync";
                    var log_description = $"{Input.Email} Please confirm your email at {DateTime.Now}.";
                    await _internalLogRepository.InsertInternalLog(log_description, null, null, actionName, razorPageName, AuditType.Get.ToString(), null, null, null, null, null, null, null, null, null, null);
                    #endregion
                    return Page();
                }
                if (result.IsLockedOut)
                {
                    _logger.LogWarning("User account locked out.");
                    /* If user lockout, user should wait until the lockout time End OR user can reset his/her password.
                      So, user will see Lockout page and get a email. There user get Reset Password link. */
                    _logger.LogWarning("User account locked out.");

                    //  user = await _userManager.FindByEmailAsync(Input.Email);
                    var content_root = _host.WebRootPath;

                    // generate password reset token
                    var code = await _userManager.GeneratePasswordResetTokenAsync(user);

                    var user_id = await _context.ApplicationUser.FindAsync(user.Id);
                    var firstName = user_id.FirstName;
                    var lastName = user_id.LastName;

                    // Build password reset link
                    code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                    var callbackUrl = Url.Page("/Account/ResetPassword", pageHandler: null, values: new { area = "Identity", code }, protocol: Request.Scheme);

                    // Integrate email template
                    StreamReader reader_TemplateIsLockedOut = new StreamReader(content_root + ("/Templates/EmailTemplates/IsLockedOut.html"));
                    string readFile_TemplateIsLockedOut = reader_TemplateIsLockedOut.ReadToEnd();
                    string strContent_TemplateIsLockedOut = "";
                    strContent_TemplateIsLockedOut = readFile_TemplateIsLockedOut;
                    strContent_TemplateIsLockedOut = strContent_TemplateIsLockedOut.Replace("[fullName]", firstName + " " + lastName);
                    strContent_TemplateIsLockedOut = strContent_TemplateIsLockedOut.Replace("[link]", HtmlEncoder.Default.Encode(callbackUrl));
                    string mailBody_TemplateIsLockedOut = strContent_TemplateIsLockedOut;

                    await _emailSender.SendEmailAsync(Input.Email, "Easy Quotation Manager: IsLockedOut? Reset Password", mailBody_TemplateIsLockedOut);

                    #region Following information using for Internal Log 
                    var razorPageName = @"Identity/Account/Lockout.cshtml";
                    var actionName = @"OnPostAsync";
                    var log_description = $"{Input.Email} account locked out at {DateTime.Now}.";
                    await _internalLogRepository.InsertInternalLog(log_description, null, null, actionName, razorPageName, AuditType.Get.ToString(), null, null, null, null, null, null, null, null, null, null);
                    #endregion
                    return RedirectToPage("./Lockout");
                }
                else
                {
                    // ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                    TempData["Invalidloginattempt"] = "Invalid login attempt!";
                    #region Following information using for Internal Log 
                    var razorPageName = @"Identity/Account/Index.cshtml";
                    var actionName = @"OnPostAsync";
                    var log_description = $"{Input.Email} Invalid login attempt at {DateTime.Now}.";
                    await _internalLogRepository.InsertInternalLog(log_description, null, null, actionName, razorPageName, AuditType.Get.ToString(), null, null, null, null, null, null, null, null, null, null);
                    #endregion
                    return Page();
                }
            }

            // If we got this far, something failed, redisplay form
            return Page();
        }
    }
}
