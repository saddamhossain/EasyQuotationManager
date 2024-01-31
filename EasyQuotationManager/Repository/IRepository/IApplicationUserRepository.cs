using EasyQuotationManager.Model;
using EasyQuotationManager.Shared.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EasyQuotationManager.Repository.IRepository
{
    public interface IApplicationUserRepository
    {
        Task<IQueryable<ApplicationUser>> GetAllUser();
        Task<ApplicationUser> GetUserById(string id);
        Task<ApplicationUser> GetUserInfo(string userId);
        Task<ApplicationUserEditViewModel> GetUserByIdInViewModel(string id);
        Task<bool> InsertUser(ApplicationUserViewModel user, IList<ManageUserRolesViewModel> model);
        Task<bool> UpdateUser(ApplicationUserEditViewModel user, string id, IList<ManageUserRolesViewModel> model);
        Task<bool> DeleteUser(ApplicationUser user, string id);
        Task<bool> UserIsLocked(string id);
        Task<bool> UserIsUnLocked(string id);
        Task<bool> IsIdExist(string id);
        Task<bool> IsUserNameExist(string userName);
        Task<bool> IsEmailExist(string email);
        Task<IList<ManageUserRolesViewModel>> GetManageUserRoles(string id);
        Task<bool> UpdateManageUserRoles(IList<ManageUserRolesViewModel> model, string id);

        Task<bool> UpdateTeamLeaderAuthCode(string userId, string code);
    }
}
