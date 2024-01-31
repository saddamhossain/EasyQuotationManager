using System;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using EasyQuotationManager.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;

namespace EasyQuotationManager.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class ResendEmailConfirmationModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IEmailSender _emailSender;
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _host;

        public ResendEmailConfirmationModel(UserManager<IdentityUser> userManager, IEmailSender emailSender, ApplicationDbContext context, IWebHostEnvironment host)
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

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var user = await _userManager.FindByEmailAsync(Input.Email);
            if (user == null)
            {
                TempData["user_not_found"] = "Email Not Exist!";
                return Page();
            }

            var userId = await _userManager.GetUserIdAsync(user);

            var user_id = await _context.ApplicationUser.FindAsync(user.Id);
            var firstName = user_id.FirstName;
            var lastName = user_id.LastName;
            var content_root = _host.WebRootPath;

            var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));

            var callbackUrl = Url.Page("/Account/ConfirmEmail", pageHandler: null, values: new { userId = userId, code = code }, protocol: Request.Scheme);

            // Integrate email template
            StreamReader reader_TemplateConfirmationAccount = new StreamReader(content_root + ("/Templates/EmailTemplates/ConfirmRegistration.html"));
            string readFile_TemplateConfirmationAccount = reader_TemplateConfirmationAccount.ReadToEnd();
            string strContent_TemplateConfirmationAccount = "";
            strContent_TemplateConfirmationAccount = readFile_TemplateConfirmationAccount;
            strContent_TemplateConfirmationAccount = strContent_TemplateConfirmationAccount.Replace("[fullName]", firstName + " " + lastName);
            strContent_TemplateConfirmationAccount = strContent_TemplateConfirmationAccount.Replace("[link]", HtmlEncoder.Default.Encode(callbackUrl));
            string mailBody_TemplateConfirmationAccount = strContent_TemplateConfirmationAccount;

            await _emailSender.SendEmailAsync(Input.Email, "Easy Quotation Manager: Confirm your Email", mailBody_TemplateConfirmationAccount);

            ModelState.AddModelError(string.Empty, "Verification email sent. Please check your email.");
            return Page();
        }
    }
}
