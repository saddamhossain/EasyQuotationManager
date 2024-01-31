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
    public class InternalLogRepository : IInternalLogRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public InternalLogRepository(ApplicationDbContext context, UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<IQueryable<InternalLog>> GetAllInternalLog()
        {
            try
            {
                var result = _context.InternalLogs;
                return result;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<InternalLog> GetInternalLogById(int id)
        {
            try
            {
                var result = await _context.InternalLogs.FirstOrDefaultAsync(s => s.Id == id);
                return result;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task InsertInternalLog(string LogDescription, string ControllerName, string ServiceName, string ActionName, string RazorPageName, string LogType, string UserId, string UserName, string RoleName, string SessionId, string ConnectionId, string TraceIdentifier, string LocalIpAddress, string LocalPort, string RemoteIpAddress, string RemotePort)
        {
            try
            {
                InternalLog obj = new InternalLog
                {
                    LogDescription = LogDescription,
                    ControllerName = ControllerName,
                    ServiceName = ServiceName,
                    ActionName = ActionName,
                    RazorPageName = RazorPageName,
                    LogType = LogType,
                    UserId = UserId,
                    UserName = UserName,
                    RoleName = RoleName,
                    SessionId = SessionId,
                    ConnectionId = ConnectionId,
                    TraceIdentifier = TraceIdentifier,
                    LocalIpAddress = LocalIpAddress,
                    LocalPort = LocalPort,
                    RemoteIpAddress = RemoteIpAddress,
                    RemotePort = RemotePort,
                    CreatedDate = DateTime.Now
                };
                _context.InternalLogs.Add(obj);
                await _context.SaveChangesAsync();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<bool> DeleteInternalLog(int id)
        {
            try
            {
                if (id == 0)
                {
                    throw new ArgumentException("Internal Log ID can't be empty");
                }

                var current_logged_in_user_name = _httpContextAccessor.HttpContext.User.Identity.Name.ToLower();
                var user_details = await _userManager.FindByNameAsync(current_logged_in_user_name);

                var existingInternalLog = await _context.InternalLogs.FindAsync(id);
                if (existingInternalLog != null)
                {
                    _context.InternalLogs.Remove(existingInternalLog);
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
                var result = await _context.InternalLogs.FirstOrDefaultAsync(s => s.Id == id);
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
