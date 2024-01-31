using EasyQuotationManager.Shared.ViewModels;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EasyQuotationManager.Repository.IRepository
{
    public interface IRoleRepository : IDisposable
    {
        Task<IEnumerable<IdentityRole>> GetAllRoles();
        IList<ManageUserRolesViewModel> GetAllRolesInViewModel();
        Task<IdentityRole> GetRolesById(string id);
        Task<bool> InsertRole(IdentityRole identityRole);
    }
}
