using EasyQuotationManager.Data;
using EasyQuotationManager.Repository.IRepository;
using EasyQuotationManager.Shared.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace EasyQuotationManager.Repository.Repository
{
    public class RoleRepository : IRoleRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IInternalLogRepository _internalLogRepository;
        public RoleRepository(ApplicationDbContext context, UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager, IHttpContextAccessor httpContextAccessor, IInternalLogRepository internalLogRepository)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
            _httpContextAccessor = httpContextAccessor;
            _internalLogRepository = internalLogRepository;
        }

        public async Task<IEnumerable<IdentityRole>> GetAllRoles()
        {
            try
            {
                var current_logged_in_user_name = _httpContextAccessor.HttpContext.User.Identity.Name.ToLower();
                var user_details = await _userManager.FindByNameAsync(current_logged_in_user_name);

                #region Following information using for Internal Log 
                var serviceName = "RoleService";
                var razorPageName = @"Role\Index.razor";
                var actionName = @"GetAllRoles";
                var log_description = $" User {current_logged_in_user_name} view all the roles.";
                await _internalLogRepository.InsertInternalLog(log_description, null, serviceName, actionName, razorPageName, _httpContextAccessor.HttpContext.Request.Method.ToString(),
                                                    user_details.Id, user_details.UserName, _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.Role).Value,
                                                    _httpContextAccessor.HttpContext.Session.Id.ToString(), _httpContextAccessor.HttpContext.Connection.Id.ToString(),
                                                    _httpContextAccessor.HttpContext.TraceIdentifier.ToString(), _httpContextAccessor.HttpContext.Connection.LocalIpAddress.ToString(),
                                                    _httpContextAccessor.HttpContext.Connection.LocalPort.ToString(), _httpContextAccessor.HttpContext.Connection.RemoteIpAddress.ToString(),
                                                    _httpContextAccessor.HttpContext.Connection.RemotePort.ToString());
                #endregion

                var result = await _context.Roles.OrderBy(s => s.NormalizedName).ToListAsync();
                return result;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<IdentityRole> GetRolesById(string id)
        {
            try
            {
                var current_logged_in_user_name = _httpContextAccessor.HttpContext.User.Identity.Name.ToLower();
                var user_details = await _userManager.FindByNameAsync(current_logged_in_user_name);

                #region Following information using for Internal Log 
                var serviceName = "RoleService";
                var razorPageName = @"Role\View.razor";
                var actionName = $"GetRolesById\\{id}";
                var log_description = $"User {current_logged_in_user_name} view the role with Id {id}.";
                await _internalLogRepository.InsertInternalLog(log_description, null, serviceName, actionName, razorPageName, _httpContextAccessor.HttpContext.Request.Method.ToString(),
                                                    user_details.Id, user_details.UserName, _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.Role).Value,
                                                    _httpContextAccessor.HttpContext.Session.Id.ToString(), _httpContextAccessor.HttpContext.Connection.Id.ToString(),
                                                    _httpContextAccessor.HttpContext.TraceIdentifier.ToString(), _httpContextAccessor.HttpContext.Connection.LocalIpAddress.ToString(),
                                                    _httpContextAccessor.HttpContext.Connection.LocalPort.ToString(), _httpContextAccessor.HttpContext.Connection.RemoteIpAddress.ToString(),
                                                    _httpContextAccessor.HttpContext.Connection.RemotePort.ToString());
                #endregion

                var result = await _context.Roles.Where(s => s.Id == id).FirstOrDefaultAsync();
                return result;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public IList<ManageUserRolesViewModel> GetAllRolesInViewModel()
        {
            try
            {
                var model = new List<ManageUserRolesViewModel>();
                foreach (var role in _roleManager.Roles)
                {
                    var userRolesViewModel = new ManageUserRolesViewModel
                    {
                        RoleId = role.Id,
                        RoleName = role.Name,
                    };
                    model.Add(userRolesViewModel);
                }
                return model;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<bool> InsertRole(IdentityRole identityRole)
        {
            try
            {
                var current_logged_in_user_name = _httpContextAccessor.HttpContext.User.Identity.Name.ToLower();
                var user_details = await _userManager.FindByNameAsync(current_logged_in_user_name);

                if (!await _roleManager.RoleExistsAsync(identityRole.Name.Trim()))
                {
                    await _roleManager.CreateAsync(new IdentityRole(identityRole.Name.Trim()));

                    #region Following information using for Internal Log 
                    var serviceName = "RoleService";
                    var razorPageName = @"Role\Create.razor";
                    var actionName = $"InsertRole";
                    var log_description = $"User {current_logged_in_user_name} created the role name is {identityRole.Name.Trim()}.";
                    await _internalLogRepository.InsertInternalLog(log_description, null, serviceName, actionName, razorPageName, _httpContextAccessor.HttpContext.Request.Method.ToString(),
                                                        user_details.Id, user_details.UserName, _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.Role).Value,
                                                        _httpContextAccessor.HttpContext.Session.Id.ToString(), _httpContextAccessor.HttpContext.Connection.Id.ToString(),
                                                        _httpContextAccessor.HttpContext.TraceIdentifier.ToString(), _httpContextAccessor.HttpContext.Connection.LocalIpAddress.ToString(),
                                                        _httpContextAccessor.HttpContext.Connection.LocalPort.ToString(), _httpContextAccessor.HttpContext.Connection.RemoteIpAddress.ToString(),
                                                        _httpContextAccessor.HttpContext.Connection.RemotePort.ToString());
                    #endregion

                    return true;
                }
                throw new ArgumentException("Role already exist.");
            }
            catch (Exception)
            {
                throw;
            }
        }

        #region IDisposable Support
        private bool disposed = false;
        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    _context.Dispose();
                }
            }
            this.disposed = true;
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}
