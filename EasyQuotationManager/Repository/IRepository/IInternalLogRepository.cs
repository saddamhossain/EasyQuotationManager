using EasyQuotationManager.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EasyQuotationManager.Repository.IRepository
{
    public interface IInternalLogRepository : IDisposable
    {
        Task<IQueryable<InternalLog>> GetAllInternalLog();
        Task<InternalLog> GetInternalLogById(int id);
        Task InsertInternalLog(string LogDescription, string ControllerName, string ServiceName, string ActionName, string RazorPageName, string LogType, string UserId, string UserName, string RoleName, string SessionId, string ConnectionId, string TraceIdentifier, string LocalIpAddress, string LocalPort, string RemoteIpAddress, string RemotePort);
        Task<bool> DeleteInternalLog(int id);
        Task<bool> IsIdExist(int id);
    }
}
