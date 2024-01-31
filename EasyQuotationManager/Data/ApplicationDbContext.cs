using EasyQuotationManager.Model;
using EasyQuotationManager.Shared.Utility;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace EasyQuotationManager.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, IHttpContextAccessor httpContextAccessor) : base(options)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public DbSet<ApplicationUser> ApplicationUser { set; get; }
        public DbSet<InternalLog> InternalLogs { set; get; }
        public DbSet<AuditLog> AuditLogs { set; get; }

        public DbSet<Product> Products { set; get; }
        public DbSet<ProductComposition> ProductCompositions { set; get; }
        public DbSet<Contact> Contacts { set; get; }
        public DbSet<Quotation> Quotations { set; get; }
        public DbSet<QuotationLine> QuotationLines { set; get; }

        public virtual async Task<int> SaveChangesAsync(string userId = null)
        {
            OnBeforeSaveChanges(userId);
            var result = await base.SaveChangesAsync();
            return result;
        }

        private void OnBeforeSaveChanges(string userId)
        {
            ChangeTracker.DetectChanges();
            var auditEntries = new List<AuditEntry>();

            foreach (var item in ChangeTracker.Entries())
            {
                if (item.Entity is AuditLog || item.State == EntityState.Detached || item.State == EntityState.Unchanged)
                {
                    continue;
                }
                var auditEntry = new AuditEntry(item);
                auditEntry.TableName = item.Entity.GetType().Name;
                auditEntry.UserId = userId;
                if (userId != null)
                {
                    auditEntry.UserName = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.Name).Value;
                    auditEntry.RoleName = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.Role).Value;
                }
                else
                {
                    auditEntry.UserName = null;
                    auditEntry.RoleName = null;
                }

                auditEntries.Add(auditEntry);
                foreach (var property in item.Properties)
                {
                    string propertyName = property.Metadata.Name;
                    if (property.Metadata.IsPrimaryKey())
                    {
                        auditEntry.KeyValues[propertyName] = property.CurrentValue;
                        continue;
                    }
                    switch (item.State)
                    {
                        case EntityState.Added:
                            auditEntry.AuditType = Shared.Enums.AuditType.Create;
                            auditEntry.NewValues[propertyName] = property.OriginalValue;
                            break;

                        case EntityState.Deleted:
                            auditEntry.AuditType = Shared.Enums.AuditType.Delete;
                            auditEntry.OldValues[propertyName] = property.OriginalValue;
                            break;

                        case EntityState.Modified:
                            if (property.IsModified)
                            {
                                auditEntry.ChangedColumns.Add(propertyName);
                                auditEntry.AuditType = Shared.Enums.AuditType.Update;
                                auditEntry.OldValues[propertyName] = property.OriginalValue;
                                auditEntry.NewValues[propertyName] = property.CurrentValue;
                            }
                            break;
                    }
                }
            }

            foreach (var item in auditEntries)
            {
                AuditLogs.Add(item.ToAudit());
            }
        }
    }
}
