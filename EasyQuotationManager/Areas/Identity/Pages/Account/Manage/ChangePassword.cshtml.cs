using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using EasyQuotationManager.Repository.IRepository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
namespace EasyQuotationManager.Areas.Identity.Pages.Account.Manage
{
    public class ChangePasswordModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly ILogger<ChangePasswordModel> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IInternalLogRepository _internalLogRepository;

        public ChangePasswordModel(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager, ILogger<ChangePasswordModel> logger, IHttpContextAccessor httpContextAccessor, IInternalLogRepository internalLogRepository)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
            _internalLogRepository = internalLogRepository;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        [TempData]
        public string StatusMessage { get; set; }

        public class InputModel
        {
            [Required]
            [DataType(DataType.Password)]
            [Display(Name = "Current password")]
            public string OldPassword { get; set; }

            [Required]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "New password")]
            public string NewPassword { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "Confirm new password")]
            [Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; }
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            var hasPassword = await _userManager.HasPasswordAsync(user);
            if (!hasPassword)
            {
                return RedirectToPage("./SetPassword");
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            var changePasswordResult = await _userManager.ChangePasswordAsync(user, Input.OldPassword, Input.NewPassword);
            if (!changePasswordResult.Succeeded)
            {
                foreach (var error in changePasswordResult.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                return Page();
            }

            await _signInManager.RefreshSignInAsync(user);
            _logger.LogInformation("User changed their password successfully.");
            TempData["StatusMessage"] = "Your password has been changed.";
            // StatusMessage = "Your password has been changed.";
            #region Following information using for Internal Log 
            var current_logged_in_user_name = _httpContextAccessor.HttpContext.User.Identity.Name.ToLower();
            var user_details = await _userManager.FindByNameAsync(current_logged_in_user_name);
            var razorPageName = @"Identity/Account/Manage";
            var actionName = @"OnPostAsync";
            var log_description = $"{current_logged_in_user_name} changed his/her password successfully.";
            await _internalLogRepository.InsertInternalLog(log_description, null, null, actionName, razorPageName, _httpContextAccessor.HttpContext.Request.Method.ToString(),
                                                user_details.Id, user_details.UserName, _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.Role).Value,
                                                _httpContextAccessor.HttpContext.Session.Id.ToString(), _httpContextAccessor.HttpContext.Connection.Id.ToString(),
                                                _httpContextAccessor.HttpContext.TraceIdentifier.ToString(), _httpContextAccessor.HttpContext.Connection.LocalIpAddress.ToString(),
                                                _httpContextAccessor.HttpContext.Connection.LocalPort.ToString(), _httpContextAccessor.HttpContext.Connection.RemoteIpAddress.ToString(),
                                                _httpContextAccessor.HttpContext.Connection.RemotePort.ToString());
            #endregion
            return RedirectToPage();
        }
    }
}
