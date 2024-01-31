using EasyQuotationManager.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EasyQuotationManager.Repository.IRepository
{
  public  interface IAuditLogRepository : IDisposable
    {
        Task<IQueryable<AuditLog>> GetAllAuditLog();
        Task<AuditLog> GetAuditLogById(int id);
        Task<bool> DeleteAuditLog(int id);
        Task<bool> IsIdExist(int id);
    }
}
