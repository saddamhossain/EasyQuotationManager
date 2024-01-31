using EasyQuotationManager.Data;
using EasyQuotationManager.Model;
using EasyQuotationManager.Repository.IRepository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EasyQuotationManager.Repository.Repository
{
    public class AuditLogRepository : IAuditLogRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IInternalLogRepository _internalLogRepository;
        public AuditLogRepository(ApplicationDbContext context, UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager, IHttpContextAccessor httpContextAccessor, IInternalLogRepository internalLogRepository)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
            _httpContextAccessor = httpContextAccessor;
            _internalLogRepository = internalLogRepository;
        }

        public async Task<IQueryable<AuditLog>> GetAllAuditLog()
        {
            try
            {
                var result = _context.AuditLogs;
                return result;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<AuditLog> GetAuditLogById(int id)
        {
            try
            {
                var result = await _context.AuditLogs.FirstOrDefaultAsync(s => s.Id == id);
                return result;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<bool> DeleteAuditLog(int id)
        {
            try
            {
                if (id == 0)
                {
                    throw new ArgumentException("Covid ID can't be empty");
                }

                var current_logged_in_user_name = _httpContextAccessor.HttpContext.User.Identity.Name.ToLower();
                var user_details = await _userManager.FindByNameAsync(current_logged_in_user_name);

                var existingAuditLog = await _context.AuditLogs.FindAsync(id);
                if (existingAuditLog != null)
                {
                    _context.AuditLogs.Remove(existingAuditLog);
                    await _context.SaveChangesAsync(user_details.Id);
                    return true;
                }
                return false;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<bool> IsIdExist(int id)
        {
            try
            {
                var result = await _context.AuditLogs.FirstOrDefaultAsync(s => s.Id == id);
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
