using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using EasyQuotationManager.Model;
using EasyQuotationManager.Repository.IRepository;
using EasyQuotationManager.Shared.Constant;
using EasyQuotationManager.Shared.Enums;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;

namespace EasyQuotationManager.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class RegisterModel : PageModel
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ILogger<RegisterModel> _logger;
        private readonly IEmailSender _emailSender;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IWebHostEnvironment _host;
        private readonly IInternalLogRepository _internalLogRepository;

        public RegisterModel(
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager,
            ILogger<RegisterModel> logger,
            IEmailSender emailSender,
            RoleManager<IdentityRole> roleManager,
            IWebHostEnvironment host,
            IInternalLogRepository internalLogRepository)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _emailSender = emailSender;
            _roleManager = roleManager;
            _host = host;
            _internalLogRepository = internalLogRepository;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public string ReturnUrl { get; set; }

        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        /* The Input Model we are using for Registration process. If we need more property we have to add here
         and also we have to add that property in our ApplicationUser model class then we have to update the database migration
         becuase for user related things we are using Identity, thats why this way we have to do this.
      */
        public class InputModel
        {

            [Required]
            [EmailAddress]
            [Display(Name = "Email")]
            public string Email { get; set; }

            [Required]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]
            public string Password { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "Confirm password")]
            [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; }

            [Required]
            [Display(Name = "First Name")]
            public string FirstName { get; set; }

            [Required]
            [Display(Name = "Last Name")]
            public string LastName { get; set; }

            [Required]
            [Display(Name = "License Expiration Date")]
            public DateTime LicenseExpirationDate { set; get; }
        }

        public async Task OnGetAsync(string returnUrl = null)
        {
            ReturnUrl = returnUrl;
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            // here at first we keep the role in role variable. so if someone/admin logged in then it has some value and without logged in this value will be null. 
            string role = Request.Form["rdUserRole"].ToString();
            var content_root = _host.WebRootPath;

            returnUrl ??= Url.Content("~/");
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
            if (ModelState.IsValid)
            {
                // var user = new IdentityUser { UserName = Input.Email, Email = Input.Email };
                var user = new ApplicationUser
                {
                    UserName = Input.Email,
                    Email = Input.Email,
                    FirstName = Input.FirstName,
                    LastName = Input.LastName,
                    LicenseExpirationDate = Input.LicenseExpirationDate,
                    CreatedBy = Input.Email,
                    CreatedDate = DateTime.Now
                };

                var result = await _userManager.CreateAsync(user, Input.Password);
                if (result.Succeeded)
                {
                    _logger.LogInformation("User created a new account with password.");

                    #region If any role exist or not, will be run the following code and role will be created as per logic implement here.
                    if (!await _roleManager.RoleExistsAsync(SD.AdminUser))
                    {
                        await _roleManager.CreateAsync(new IdentityRole(SD.AdminUser));
                    }
                    if (!await _roleManager.RoleExistsAsync(SD.RegisteredUser))
                    {
                        await _roleManager.CreateAsync(new IdentityRole(SD.RegisteredUser));
                    }
                    #endregion

                    if (role == SD.AdminUser)
                    {
                        await _userManager.AddToRoleAsync(user, SD.AdminUser);
                    }
                    else
                    {
                        await _userManager.AddToRoleAsync(user, SD.RegisteredUser);

                        // generate email confirmation token
                        var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);

                        // Build email confirmation  link
                        code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                        var callbackUrl = Url.Page("/Account/ConfirmEmail", pageHandler: null, values: new { area = "Identity", userId = user.Id, code = code, returnUrl = returnUrl }, protocol: Request.Scheme);

                        // Integrate email template
                        StreamReader reader_TemplateConfirmationAccount = new StreamReader(content_root + ("/Templates/EmailTemplates/ConfirmRegistration.html"));
                        string readFile_TemplateConfirmationAccount = reader_TemplateConfirmationAccount.ReadToEnd();
                        string strContent_TemplateConfirmationAccount = "";
                        strContent_TemplateConfirmationAccount = readFile_TemplateConfirmationAccount;
                        strContent_TemplateConfirmationAccount = strContent_TemplateConfirmationAccount.Replace("[fullName]", Input.FirstName + " " + Input.LastName);
                        strContent_TemplateConfirmationAccount = strContent_TemplateConfirmationAccount.Replace("[link]", HtmlEncoder.Default.Encode(callbackUrl));
                        string mailBody_TemplateConfirmationAccount = strContent_TemplateConfirmationAccount;

                        await _emailSender.SendEmailAsync(Input.Email, "Easy Quotation Manager: Confirm your Email", mailBody_TemplateConfirmationAccount);

                        #region Following information using for Internal Log 
                        var razorPageName = @"Identity/Account/Register.cshtml";
                        var actionName = @"OnPostAsync";
                        var log_description = $"{Input.Email} new user registration at {DateTime.Now}.";
                        await _internalLogRepository.InsertInternalLog(log_description, null, null, actionName, razorPageName, AuditType.Create.ToString(), null, null, null, null, null, null, null, null, null, null);
                        #endregion


                        if (_userManager.Options.SignIn.RequireConfirmedAccount)
                        {
                            return RedirectToPage("RegisterConfirmation", new { email = Input.Email, returnUrl = returnUrl });
                        }
                        else
                        {
                            await _signInManager.SignInAsync(user, isPersistent: false);
                            return LocalRedirect(returnUrl);
                        }
                    }
                    return RedirectToAction("Index", "User");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            // If we got this far, something failed, redisplay form
            return Page();
        }
    }
}
