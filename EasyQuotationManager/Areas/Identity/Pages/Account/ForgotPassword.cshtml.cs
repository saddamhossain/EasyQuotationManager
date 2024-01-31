using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Encodings.Web;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using EasyQuotationManager.Data;
using Microsoft.AspNetCore.Hosting;
using System.IO;

namespace EasyQuotationManager.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class ForgotPasswordModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IEmailSender _emailSender;
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _host;

        public ForgotPasswordModel(UserManager<IdentityUser> userManager, IEmailSender emailSender, ApplicationDbContext context, IWebHostEnvironment host)
        {
            _userManager = userManager;
            _emailSender = emailSender;
            _context = context;
            _host = host;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            [Required]
            [EmailAddress]
            public string Email { get; set; }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(Input.Email);

                // here need to check if user not found, then it will redirect same page and show a message.
                if (user == null)
                {
                    TempData["user_not_found"] = "Email Not Exist!";
                    return Page();
                }

                if (!(await _userManager.IsEmailConfirmedAsync(user)))
                {
                    TempData["user_not_found"] = "Email Not Verified. Please verify your email first, then you can reset your password!";
                    return Page();
                }

                if (user != null)
                {
                    var user_info = await _context.ApplicationUser.FindAsync(user.Id);
                    var firstName = user_info.FirstName;
                    var lastName = user_info.LastName;
                    var content_root = _host.WebRootPath;

                    // generate password reset token
                    var code = await _userManager.GeneratePasswordResetTokenAsync(user);

                    // Build password reset link
                    code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                    var callbackUrl = Url.Page("/Account/ResetPassword", pageHandler: null, values: new { area = "Identity", code }, protocol: Request.Scheme);

                    // Integrate email template
                    StreamReader reader_TemplateConfirmationAccount = new StreamReader(content_root + ("/Templates/EmailTemplates/ResetPassword.html"));
                    string readFile_TemplateConfirmationAccount = reader_TemplateConfirmationAccount.ReadToEnd();
                    string strContent_TemplateConfirmationAccount = "";
                    strContent_TemplateConfirmationAccount = readFile_TemplateConfirmationAccount;
                    strContent_TemplateConfirmationAccount = strContent_TemplateConfirmationAccount.Replace("[fullName]", firstName + " " + lastName);
                    strContent_TemplateConfirmationAccount = strContent_TemplateConfirmationAccount.Replace("[link]", HtmlEncoder.Default.Encode(callbackUrl));
                    string mailBody_TemplateConfirmationAccount = strContent_TemplateConfirmationAccount;

                    await _emailSender.SendEmailAsync(Input.Email, "Easy Quotation Manager: Reset Password", mailBody_TemplateConfirmationAccount);
                    //  await _emailSender.SendEmailAsync(Input.Email, "Reset Password", $"Please reset your password by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");
                    return RedirectToPage("./ForgotPasswordConfirmation");
                }
            }

            return Page();
        }
    }
}
