using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using EasyQuotationManager.Data;
using EasyQuotationManager.Model;
using EasyQuotationManager.Repository.IRepository;
using EasyQuotationManager.Shared.Constant;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace EasyQuotationManager.Areas.Identity.Pages.Account.Manage
{
    public partial class IndexModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly ApplicationDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IInternalLogRepository _internalLogRepository;

        public IndexModel(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager, ApplicationDbContext context, IHttpContextAccessor httpContextAccessor, IInternalLogRepository internalLogRepository)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
            _httpContextAccessor = httpContextAccessor;
            _internalLogRepository = internalLogRepository;
        }

        public string Username { get; set; }

        [TempData]
        public string StatusMessage { get; set; }

        [TempData]
        public string ErrorMessage { get; set; }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            [Phone]
            [Display(Name = "Phone number")]
            public string PhoneNumber { get; set; }

            [Required]
            [Display(Name = "First Name")]
            public string FirstName { set; get; }

            [Required]
            [Display(Name = "Last Name")]
            public string LastName { set; get; }

            [Display(Name = "License Expiration Date")]
            public DateTime LicenseExpirationDate { set; get; }
        }

        private async Task LoadAsync(IdentityUser user)
        {
            if (!string.IsNullOrEmpty(ErrorMessage))
            {
                ModelState.AddModelError(string.Empty, ErrorMessage);
            }
            var userName = await _userManager.GetUserNameAsync(user);
            var phoneNumber = await _userManager.GetPhoneNumberAsync(user);

            var user_id = await _context.ApplicationUser.FindAsync(user.Id);
            var expirationDate = user_id.LicenseExpirationDate;

            Username = userName;

            var userInfo = _context.ApplicationUser.Where(s => s.UserName == userName).Select(s => new ApplicationUser
            {
                FirstName = s.FirstName,
                LastName = s.LastName
            });

            Input = new InputModel
            {
                PhoneNumber = phoneNumber,
                LicenseExpirationDate = expirationDate,
                FirstName = userInfo.FirstOrDefault().FirstName,
                LastName = userInfo.FirstOrDefault().LastName
            };
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

        public async Task<IActionResult> OnPostAsync()
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

            var current_logged_in_user_name = _httpContextAccessor.HttpContext.User.Identity.Name.ToLower();
            var user_details = await _userManager.FindByNameAsync(current_logged_in_user_name);

            var roles = await _userManager.GetRolesAsync(user_details);

            var user_id = await _context.ApplicationUser.FindAsync(user.Id);
            user_id.FirstName = Input.FirstName;
            user_id.LastName = Input.LastName;
            if (roles.Contains(SD.AdminUser))
            {
                user_id.LicenseExpirationDate = Input.LicenseExpirationDate;
            }
            else
            {
                user_id.LicenseExpirationDate = user_id.LicenseExpirationDate;
            }

            user_id.UpdatedBy = current_logged_in_user_name;
            user_id.UpdatedDate = DateTime.Now;
            await _context.SaveChangesAsync(user_details.Id);

            var phoneNumber = await _userManager.GetPhoneNumberAsync(user);
            if (Input.PhoneNumber != phoneNumber)
            {
                var setPhoneResult = await _userManager.SetPhoneNumberAsync(user, Input.PhoneNumber);
                if (!setPhoneResult.Succeeded)
                {
                    TempData["StatusMessage"] = "Unexpected error when trying to set phone number.";
                    // StatusMessage = "Unexpected error when trying to set phone number.";
                    return RedirectToPage();
                }
            }

            await _signInManager.RefreshSignInAsync(user);
            TempData["StatusMessage"] = "Your profile has been updated";
            StatusMessage = "Your profile has been updated";

            #region Following information using for Internal Log 
            var razorPageName = @"Identity/Account/Manage/Index.cshtml";
            var actionName = @"OnPostAsync";
            var log_description = $"{current_logged_in_user_name} updated his/her profile information.";
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
