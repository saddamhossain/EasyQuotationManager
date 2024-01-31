using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.Encodings.Web;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using EasyQuotationManager.Repository.IRepository;
using System.IO;
using System.Security.Claims;

namespace EasyQuotationManager.Areas.Identity.Pages.Account.Manage
{
    public partial class EmailModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly IEmailSender _emailSender;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IInternalLogRepository _internalLogRepository;
        private readonly IWebHostEnvironment _host;

        public EmailModel(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager, IEmailSender emailSender, IHttpContextAccessor httpContextAccessor, IInternalLogRepository internalLogRepository, IWebHostEnvironment host)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _emailSender = emailSender;
            _httpContextAccessor = httpContextAccessor;
            _internalLogRepository = internalLogRepository;
            _host = host;
        }
        public string Username { get; set; }

        public string Email { get; set; }

        public bool IsEmailConfirmed { get; set; }

        [TempData]
        public string StatusMessage { get; set; }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            [Required]
            [EmailAddress]
            [Display(Name = "New email")]
            public string NewEmail { get; set; }
        }

        private async Task LoadAsync(IdentityUser user)
        {
            var email = await _userManager.GetEmailAsync(user);
            Email = email;

            Input = new InputModel
            {
                NewEmail = email,
            };

            IsEmailConfirmed = await _userManager.IsEmailConfirmedAsync(user);
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            await LoadAsync(user);
            return Page();
        }

        public async Task<IActionResult> OnPostChangeEmailAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            if (!ModelState.IsValid)
            {
                await LoadAsync(user);
                return Page();
            }

            var email = await _userManager.GetEmailAsync(user);
            if (Input.NewEmail != email)
            {
                var userId = await _userManager.GetUserIdAsync(user);
                var code = await _userManager.GenerateChangeEmailTokenAsync(user, Input.NewEmail);
                code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                var callbackUrl = Url.Page("/Account/ConfirmEmailChange", pageHandler: null, values: new { userId = userId, email = Input.NewEmail, code = code }, protocol: Request.Scheme);

                var current_logged_in_user_name = _httpContextAccessor.HttpContext.User.Identity.Name.ToLower();
                var user_details = await _userManager.FindByNameAsync(current_logged_in_user_name);
                // Integrate email template
                var content_root = _host.WebRootPath;
                StreamReader reader_TemplateConfirmationAccount = new StreamReader(content_root + ("/Templates/EmailTemplates/ConfirmEmail.html"));
                string readFile_TemplateConfirmationAccount = reader_TemplateConfirmationAccount.ReadToEnd();
                string strContent_TemplateConfirmationAccount = "";
                strContent_TemplateConfirmationAccount = readFile_TemplateConfirmationAccount;
                strContent_TemplateConfirmationAccount = strContent_TemplateConfirmationAccount.Replace("[fullName]", Input.NewEmail);
                strContent_TemplateConfirmationAccount = strContent_TemplateConfirmationAccount.Replace("[link]", HtmlEncoder.Default.Encode(callbackUrl));
                string mailBody_TemplateConfirmationAccount = strContent_TemplateConfirmationAccount;

                await _emailSender.SendEmailAsync(Input.NewEmail, "Field Connect: Confirm your Email", mailBody_TemplateConfirmationAccount);
                // await _emailSender.SendEmailAsync(Input.NewEmail,"Confirm your email",$"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

               // StatusMessage = "Confirmation link to change email sent. Please check your email.";
                TempData["StatusMessage"] = "Confirmation link to change email sent. Please check your email.";
                #region Following information using for Internal Log 
                var razorPageName = @"Identity/Account/Manage/Email.cshtml";
                var actionName = @"OnPostAsync";
                var log_description = $"{current_logged_in_user_name} trying to update his/her Email/UserName.";
                await _internalLogRepository.InsertInternalLog(log_description, null, null, actionName, razorPageName, _httpContextAccessor.HttpContext.Request.Method.ToString(),
                                                    user_details.Id, user_details.UserName, _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.Role).Value,
                                                    _httpContextAccessor.HttpContext.Session.Id.ToString(), _httpContextAccessor.HttpContext.Connection.Id.ToString(),
                                                    _httpContextAccessor.HttpContext.TraceIdentifier.ToString(), _httpContextAccessor.HttpContext.Connection.LocalIpAddress.ToString(),
                                                    _httpContextAccessor.HttpContext.Connection.LocalPort.ToString(), _httpContextAccessor.HttpContext.Connection.RemoteIpAddress.ToString(),
                                                    _httpContextAccessor.HttpContext.Connection.RemotePort.ToString());
                #endregion
                return RedirectToPage();
            }

            StatusMessage = "Your email is unchanged.";
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostSendVerificationEmailAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            if (!ModelState.IsValid)
            {
                await LoadAsync(user);
                return Page();
            }

            var userId = await _userManager.GetUserIdAsync(user);
            var email = await _userManager.GetEmailAsync(user);
            var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
            var callbackUrl = Url.Page(
                "/Account/ConfirmEmail",
                pageHandler: null,
                values: new { area = "Identity", userId = userId, code = code },
                protocol: Request.Scheme);
            await _emailSender.SendEmailAsync(
                email,
                "Confirm your email",
                $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

            StatusMessage = "Verification email sent. Please check your email.";
            return RedirectToPage();
        }
    }
}
