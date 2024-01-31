using EasyQuotationManager.Data;
using EasyQuotationManager.Model;
using EasyQuotationManager.Repository.IRepository;
using EasyQuotationManager.Shared.Constant;
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
    public class ApplicationUserRepository : IApplicationUserRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IInternalLogRepository _internalLogRepository;
        public ApplicationUserRepository(ApplicationDbContext context, UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager, IHttpContextAccessor httpContextAccessor, IInternalLogRepository internalLogRepository)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
            _httpContextAccessor = httpContextAccessor;
            _internalLogRepository = internalLogRepository;
        }

        public async Task<IQueryable<ApplicationUser>> GetAllUser()
        {
            try
            {
                var current_logged_in_user_name = _httpContextAccessor.HttpContext.User.Identity.Name.ToLower();
                //  var user_details =  _userManager.FindByNameAsync(current_logged_in_user_name);
                var result = _context.ApplicationUser.AsQueryable();

                #region Following information using for Internal Log 
                //var serviceName = "ApplicationUserService";
                //var razorPageName = @"User\Index.razor";
                //var actionName = @"GetAllUser";
                //var log_description = $"User {current_logged_in_user_name} view all the user.";
                // _internalLogRepository.InsertInternalLog(log_description, null, serviceName, actionName, razorPageName, _httpContextAccessor.HttpContext.Request.Method.ToString(),
                //                                   null, current_logged_in_user_name, _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.Role).Value,
                //                                    _httpContextAccessor.HttpContext.Session.Id.ToString(), _httpContextAccessor.HttpContext.Connection.Id.ToString(),
                //                                    _httpContextAccessor.HttpContext.TraceIdentifier.ToString(), _httpContextAccessor.HttpContext.Connection.LocalIpAddress.ToString(),
                //                                    _httpContextAccessor.HttpContext.Connection.LocalPort.ToString(), _httpContextAccessor.HttpContext.Connection.RemoteIpAddress.ToString(),
                //                                    _httpContextAccessor.HttpContext.Connection.RemotePort.ToString());
                #endregion
                return result;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<ApplicationUser> GetUserById(string id)
        {
            try
            {
                var result = await _context.ApplicationUser.AsNoTracking().Where(s => s.Id == id).FirstOrDefaultAsync();
                return result;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task<ApplicationUser> GetUserInfo(string userId)
        {
            try
            {
                var result = await (from a in _context.ApplicationUser
                                    where a.Id == userId
                                    select new ApplicationUser
                                    {
                                        Id = a.Id,
                                        UserName = a.UserName,
                                        Email = a.Email
                                    }).FirstOrDefaultAsync();
                return result;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task<ApplicationUserEditViewModel> GetUserByIdInViewModel(string id)
        {
            try
            {
                var user = await _context.ApplicationUser.FindAsync(id);
                var role = await _userManager.GetRolesAsync(user);
                var result = await (from s in _context.ApplicationUser
                                    where s.Id == id
                                    select new ApplicationUserEditViewModel
                                    {
                                        FirstName = s.FirstName,
                                        LastName = s.LastName,
                                        Email = s.Email,
                                        PhoneNumber = s.PhoneNumber,
                                        LicenseExpirationDate = s.LicenseExpirationDate
                                    }).FirstOrDefaultAsync();


                #region Following information using for Internal Log 
                var current_logged_in_user_name = _httpContextAccessor.HttpContext.User.Identity.Name.ToLower();
                var user_details = await _userManager.FindByNameAsync(current_logged_in_user_name);
                var serviceName = "ApplicationUserService";
                var razorPageName = @"User\Edit.razor";
                var actionName = @"GetUserByIdInViewModel";
                var log_description = $"{current_logged_in_user_name} updated the user information of name is {user_details.UserName}.";
                await _internalLogRepository.InsertInternalLog(log_description, null, serviceName, actionName, razorPageName, _httpContextAccessor.HttpContext.Request.Method.ToString(),
                                                    user_details.Id, user_details.UserName, _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.Role).Value,
                                                    _httpContextAccessor.HttpContext.Session.Id.ToString(), _httpContextAccessor.HttpContext.Connection.Id.ToString(),
                                                    _httpContextAccessor.HttpContext.TraceIdentifier.ToString(), _httpContextAccessor.HttpContext.Connection.LocalIpAddress.ToString(),
                                                    _httpContextAccessor.HttpContext.Connection.LocalPort.ToString(), _httpContextAccessor.HttpContext.Connection.RemoteIpAddress.ToString(),
                                                    _httpContextAccessor.HttpContext.Connection.RemotePort.ToString());
                #endregion

                return result;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<bool> InsertUser(ApplicationUserViewModel user, IList<ManageUserRolesViewModel> model)
        {
            try
            {
                if (!string.IsNullOrEmpty(user.Id))
                {
                    var current_logged_in_user_name = _httpContextAccessor.HttpContext.User.Identity.Name.ToLower();
                    var user_details = await _userManager.FindByNameAsync(current_logged_in_user_name);

                    var checkExistUser = await _userManager.FindByEmailAsync(user.Email);
                    if (checkExistUser != null)
                    {
                        throw new ArgumentException("User already exist!");
                    }

                    ApplicationUser obj = new ApplicationUser
                    {
                        FirstName = user.FirstName,
                        LastName = user.LastName,
                        LicenseExpirationDate = user.LicenseExpirationDate,
                        UserName = user.Email.ToLower(),
                        NormalizedUserName = user.Email.ToUpper(),
                        Email = user.Email.ToLower(),
                        NormalizedEmail = user.Email.ToUpper(),
                        EmailConfirmed = true, // Here admin created the user, thats why don't need to verified the email.
                        PhoneNumber = user.PhoneNumber,
                        PhoneNumberConfirmed = true,
                        TwoFactorEnabled = false,
                        LockoutEnd = null,
                        LockoutEnabled = true,
                        AccessFailedCount = 0,
                        CreatedBy = current_logged_in_user_name,
                        CreatedDate = DateTime.Now.Date
                    };

                    var result = await _userManager.CreateAsync(obj, user.Password);
                    if (result.Succeeded)
                    {
                        var roleCreation = await _userManager.AddToRolesAsync(obj, model.Where(s => s.Selected).Select(y => y.RoleName));
                        if (!roleCreation.Succeeded)
                        {
                            throw new ArgumentException($"Cannot add selected roles to this user.");
                        }

                        #region Following information using for Internal Log 
                        var serviceName = "ApplicationUserService";
                        var razorPageName = @"User\Create.razor";
                        var actionName = @"InsertUser";
                        var log_description = $"User {current_logged_in_user_name} created the user from the admin panel whose user name is {obj.UserName}.";
                        await _internalLogRepository.InsertInternalLog(log_description, null, serviceName, actionName, razorPageName, _httpContextAccessor.HttpContext.Request.Method.ToString(),
                                                            user_details.Id, user_details.UserName, _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.Role).Value,
                                                            _httpContextAccessor.HttpContext.Session.Id.ToString(), _httpContextAccessor.HttpContext.Connection.Id.ToString(),
                                                            _httpContextAccessor.HttpContext.TraceIdentifier.ToString(), _httpContextAccessor.HttpContext.Connection.LocalIpAddress.ToString(),
                                                            _httpContextAccessor.HttpContext.Connection.LocalPort.ToString(), _httpContextAccessor.HttpContext.Connection.RemoteIpAddress.ToString(),
                                                            _httpContextAccessor.HttpContext.Connection.RemotePort.ToString());
                        #endregion

                        return true;
                    }
                    return false;
                }
                throw new ArgumentException("User is empty!");
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<bool> UpdateUser(ApplicationUserEditViewModel user, string id, IList<ManageUserRolesViewModel> model)
        {
            try
            {
                var existingUser = await _context.ApplicationUser.Where(s => s.Id == id).FirstOrDefaultAsync();
                if (existingUser == null)
                {
                    throw new ArgumentException("User is empty!");
                }

                var current_logged_in_user_name = _httpContextAccessor.HttpContext.User.Identity.Name.ToLower();
                var user_details = await _userManager.FindByNameAsync(current_logged_in_user_name);

                existingUser.FirstName = user.FirstName;
                existingUser.LastName = user.LastName;
                existingUser.LicenseExpirationDate = user.LicenseExpirationDate;
                existingUser.PhoneNumber = user.PhoneNumber;
                existingUser.UpdatedBy = current_logged_in_user_name;
                existingUser.UpdatedDate = DateTime.Now.Date;
                await _context.SaveChangesAsync(user_details.Id);

                var roles = await _userManager.GetRolesAsync(existingUser);
                var result = await _userManager.RemoveFromRolesAsync(existingUser, roles);
                if (!result.Succeeded)
                {
                    throw new ArgumentException($"Cannot remove user existing roles.");
                }
                result = await _userManager.AddToRolesAsync(existingUser, model.Where(s => s.Selected).Select(y => y.RoleName));
                if (!result.Succeeded)
                {
                    throw new ArgumentException($"Cannot add selected roles to user.");
                }

                #region Following information using for Internal Log              
                var serviceName = "ApplicationUserService";
                var razorPageName = @"User\Edit.razor";
                var actionName = @"UpdateUser";
                var log_description = $"User {current_logged_in_user_name} updated the user information whose user name is {user.UserName}.";
                await _internalLogRepository.InsertInternalLog(log_description, null, serviceName, actionName, razorPageName, _httpContextAccessor.HttpContext.Request.Method.ToString(),
                                                    user_details.Id, user_details.UserName, _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.Role).Value,
                                                    _httpContextAccessor.HttpContext.Session.Id.ToString(), _httpContextAccessor.HttpContext.Connection.Id.ToString(),
                                                    _httpContextAccessor.HttpContext.TraceIdentifier.ToString(), _httpContextAccessor.HttpContext.Connection.LocalIpAddress.ToString(),
                                                    _httpContextAccessor.HttpContext.Connection.LocalPort.ToString(), _httpContextAccessor.HttpContext.Connection.RemoteIpAddress.ToString(),
                                                    _httpContextAccessor.HttpContext.Connection.RemotePort.ToString());
                #endregion

                return true;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<bool> DeleteUser(ApplicationUser user, string id)
        {
            try
            {
                if (id == null)
                {
                    throw new ArgumentException("User is empty!");
                }

                // Here we check the user exist or not based on given id
                var applicationUser = await _context.ApplicationUser.Where(s => s.Id == id).FirstOrDefaultAsync();
                if (applicationUser == null)
                {
                    throw new ArgumentException("User is empty!");
                }

                // If user want to Delete his/her own account, he simply can't do it.
                var current_logged_in_user_name = _httpContextAccessor.HttpContext.User.Identity.Name.ToLower();
                if (current_logged_in_user_name == applicationUser.UserName.ToLower())
                {
                    throw new ArgumentException("You can't Delete at your own account!");
                }

                // If user is available with that ID then when run the following line of code, that user will be deleted from database.
                var result = await _userManager.DeleteAsync(applicationUser);
                if (result.Succeeded)
                {
                    #region Following information using for Internal Log 
                    var user_details = await _userManager.FindByNameAsync(current_logged_in_user_name);
                    var serviceName = "ApplicationUserService";
                    var razorPageName = @"User\Delete.razor";
                    var actionName = @"DeleteUser";
                    var log_description = $"User {current_logged_in_user_name} deleted the user information whose user name is {user.UserName}.";
                    await _internalLogRepository.InsertInternalLog(log_description, null, serviceName, actionName, razorPageName, _httpContextAccessor.HttpContext.Request.Method.ToString(),
                                                        user_details.Id, user_details.UserName, _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.Role).Value,
                                                        _httpContextAccessor.HttpContext.Session.Id.ToString(), _httpContextAccessor.HttpContext.Connection.Id.ToString(),
                                                        _httpContextAccessor.HttpContext.TraceIdentifier.ToString(), _httpContextAccessor.HttpContext.Connection.LocalIpAddress.ToString(),
                                                        _httpContextAccessor.HttpContext.Connection.LocalPort.ToString(), _httpContextAccessor.HttpContext.Connection.RemoteIpAddress.ToString(),
                                                        _httpContextAccessor.HttpContext.Connection.RemotePort.ToString());
                    #endregion

                    return true;
                }
                return false;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<bool> UserIsLocked(string id)
        {
            try
            {
                if (id == null)
                {
                    throw new ArgumentException("User is empty!");
                }

                // Here we check the user exist or not based on given id
                var applicationUser = await _context.ApplicationUser.Where(s => s.Id == id).FirstOrDefaultAsync();
                if (applicationUser == null)
                {
                    throw new ArgumentException("User is empty!");
                }

                // If user want to Lock his/her own account, he simply can't do it.
                var current_logged_in_user_name = _httpContextAccessor.HttpContext.User.Identity.Name.ToLower();
                var user_details = await _userManager.FindByNameAsync(current_logged_in_user_name);

                if (current_logged_in_user_name == applicationUser.UserName.ToLower())
                {
                    throw new ArgumentException("You can't Lock at your own account!");
                }

                // If user value is not null, That means, if the user is exist the AspNetUsers tables LockoutEnd field value will be updated using following code. So, that user will be Locked. So user will be locked for 1000 years :)
                applicationUser.LockoutEnd = DateTime.Now.AddYears(1000);
                applicationUser.UpdatedBy = current_logged_in_user_name;
                applicationUser.UpdatedDate = DateTime.Now.Date;
                await _context.SaveChangesAsync(user_details.Id);

                #region Following information using for Internal Log 
                var serviceName = "ApplicationUserService";
                var razorPageName = @"User\Index.razor";
                var actionName = @"UserIsLocked";
                var log_description = $"User {current_logged_in_user_name} locked the user information whose user name is {applicationUser.UserName}.";
                await _internalLogRepository.InsertInternalLog(log_description, null, serviceName, actionName, razorPageName, _httpContextAccessor.HttpContext.Request.Method.ToString(),
                                                    user_details.Id, user_details.UserName, _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.Role).Value,
                                                    _httpContextAccessor.HttpContext.Session.Id.ToString(), _httpContextAccessor.HttpContext.Connection.Id.ToString(),
                                                    _httpContextAccessor.HttpContext.TraceIdentifier.ToString(), _httpContextAccessor.HttpContext.Connection.LocalIpAddress.ToString(),
                                                    _httpContextAccessor.HttpContext.Connection.LocalPort.ToString(), _httpContextAccessor.HttpContext.Connection.RemoteIpAddress.ToString(),
                                                    _httpContextAccessor.HttpContext.Connection.RemotePort.ToString());
                #endregion

                return true;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<bool> UserIsUnLocked(string id)
        {
            try
            {
                if (id == null)
                {
                    return false;
                }

                // Here we check the user exist or not based on given id
                var applicationUser = await _context.ApplicationUser.Where(s => s.Id == id).FirstOrDefaultAsync();
                if (applicationUser == null)
                {
                    return false;
                }

                // If user want to Unlock his/her own account, he/she simply can't do it.
                var current_logged_in_user_name = _httpContextAccessor.HttpContext.User.Identity.Name.ToLower();
                var user_details = await _userManager.FindByNameAsync(current_logged_in_user_name);

                if (current_logged_in_user_name == applicationUser.UserName.ToLower())
                {
                    throw new ArgumentException("You can't Unlock at your own account!");
                }

                // If user value is not null, That means, if the user is exist the AspNetUsers tables LockoutEnd field value will be updated using following code. So, that user will be UnLock.
                applicationUser.LockoutEnd = DateTime.Now.Date;
                applicationUser.UpdatedBy = current_logged_in_user_name;
                applicationUser.UpdatedDate = DateTime.Now.Date;
                await _context.SaveChangesAsync(user_details.Id);

                #region Following information using for Internal Log 
                var serviceName = "ApplicationUserService";
                var razorPageName = @"User\Index.razor";
                var actionName = @"UserIsUnLocked";
                var log_description = $"User {current_logged_in_user_name} locked the user information whose user name is {applicationUser.UserName}.";
                await _internalLogRepository.InsertInternalLog(log_description, null, serviceName, actionName, razorPageName, _httpContextAccessor.HttpContext.Request.Method.ToString(),
                                                    user_details.Id, user_details.UserName, _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.Role).Value,
                                                    _httpContextAccessor.HttpContext.Session.Id.ToString(), _httpContextAccessor.HttpContext.Connection.Id.ToString(),
                                                    _httpContextAccessor.HttpContext.TraceIdentifier.ToString(), _httpContextAccessor.HttpContext.Connection.LocalIpAddress.ToString(),
                                                    _httpContextAccessor.HttpContext.Connection.LocalPort.ToString(), _httpContextAccessor.HttpContext.Connection.RemoteIpAddress.ToString(),
                                                    _httpContextAccessor.HttpContext.Connection.RemotePort.ToString());
                #endregion

                return true;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<bool> IsIdExist(string id)
        {
            try
            {
                var result = await _context.ApplicationUser.FirstOrDefaultAsync(s => s.Id == id);
                if (result == null)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<bool> IsEmailExist(string email)
        {
            try
            {
                var result = await _context.ApplicationUser.FirstOrDefaultAsync(s => s.Email == email);
                if (result == null)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<bool> IsUserNameExist(string userName)
        {
            try
            {
                var result = await _context.ApplicationUser.FirstOrDefaultAsync(s => s.UserName == userName);
                if (result == null)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<IList<ManageUserRolesViewModel>> GetManageUserRoles(string id)
        {
            try
            {
                var current_logged_in_user_name = _httpContextAccessor.HttpContext.User.Identity.Name.ToLower();
                var user_details = await _userManager.FindByNameAsync(current_logged_in_user_name);

                var user = await _userManager.FindByIdAsync(id);
                if (user == null)
                {
                    throw new ArgumentException($"User with Id {id} cannot be found.");
                }
                var model = new List<ManageUserRolesViewModel>();
                foreach (var role in _roleManager.Roles)
                {
                    var userRolesViewModel = new ManageUserRolesViewModel
                    {
                        RoleId = role.Id,
                        RoleName = role.Name,
                        ApplicationUserId = user.Id,
                        UserName = user.UserName
                    };
                    if (await _userManager.IsInRoleAsync(user, role.Name))
                    {
                        userRolesViewModel.Selected = true;
                    }
                    else
                    {
                        userRolesViewModel.Selected = false;
                    }
                    model.Add(userRolesViewModel);
                }

                #region Following information using for Internal Log 
                var serviceName = "ApplicationUserService";
                var razorPageName = @"User\ManageUserRoles.razor";
                var actionName = @"GetManageUserRoles";
                var log_description = $"User {current_logged_in_user_name} view manage user roles.";
                await _internalLogRepository.InsertInternalLog(log_description, null, serviceName, actionName, razorPageName, _httpContextAccessor.HttpContext.Request.Method.ToString(),
                                                    user_details.Id, user_details.UserName, _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.Role).Value,
                                                    _httpContextAccessor.HttpContext.Session.Id.ToString(), _httpContextAccessor.HttpContext.Connection.Id.ToString(),
                                                    _httpContextAccessor.HttpContext.TraceIdentifier.ToString(), _httpContextAccessor.HttpContext.Connection.LocalIpAddress.ToString(),
                                                    _httpContextAccessor.HttpContext.Connection.LocalPort.ToString(), _httpContextAccessor.HttpContext.Connection.RemoteIpAddress.ToString(),
                                                    _httpContextAccessor.HttpContext.Connection.RemotePort.ToString());
                #endregion

                return model;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<bool> UpdateManageUserRoles(IList<ManageUserRolesViewModel> model, string id)
        {
            try
            {
                var current_logged_in_user_name = _httpContextAccessor.HttpContext.User.Identity.Name.ToLower();
                var user_details = await _userManager.FindByNameAsync(current_logged_in_user_name);

                var user = await _userManager.FindByIdAsync(id);
                if (user == null)
                {
                    throw new ArgumentException($"User with Id {id} cannot be found.");
                }
                var roles = await _userManager.GetRolesAsync(user);
                var result = await _userManager.RemoveFromRolesAsync(user, roles);
                if (!result.Succeeded)
                {
                    throw new ArgumentException($"Cannot remove user existing roles.");
                }
                result = await _userManager.AddToRolesAsync(user, model.Where(s => s.Selected).Select(y => y.RoleName));
                if (!result.Succeeded)
                {
                    throw new ArgumentException($"Cannot add selected roles to user.");
                }

                #region Following information using for Internal Log 
                var serviceName = "ApplicationUserService";
                var razorPageName = @"User\ManageUserRoles.razor";
                var actionName = @"UpdateManageUserRoles";
                var log_description = $"User {current_logged_in_user_name} updated the roles of user {user.UserName}";
                await _internalLogRepository.InsertInternalLog(log_description, null, serviceName, actionName, razorPageName, _httpContextAccessor.HttpContext.Request.Method.ToString(),
                                                    user_details.Id, user_details.UserName, _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.Role).Value,
                                                    _httpContextAccessor.HttpContext.Session.Id.ToString(), _httpContextAccessor.HttpContext.Connection.Id.ToString(),
                                                    _httpContextAccessor.HttpContext.TraceIdentifier.ToString(), _httpContextAccessor.HttpContext.Connection.LocalIpAddress.ToString(),
                                                    _httpContextAccessor.HttpContext.Connection.LocalPort.ToString(), _httpContextAccessor.HttpContext.Connection.RemoteIpAddress.ToString(),
                                                    _httpContextAccessor.HttpContext.Connection.RemotePort.ToString());
                #endregion
                return true;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<bool> UpdateTeamLeaderAuthCode(string userId, string code)
        {
            try
            {
                var existingUser = await _context.ApplicationUser.Where(s => s.Id == userId).FirstOrDefaultAsync();
                existingUser.TeamLeaderAuthCode = code;
                existingUser.UpdatedBy = existingUser.UserName;
                existingUser.UpdatedDate = DateTime.Now.Date;
                await _context.SaveChangesAsync(existingUser.Id);
                return true;
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
