using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyQuotationManager.Model
{
    public class AuditLog
    {
        [Key]
        public int Id { set; get; }
        public string UserId { set; get; }
        public string UserName { set; get; }
        public string RoleName { set; get; }
        public string AuditType { set; get; }
        public string TableName { set; get; }
        public DateTime DateTime { set; get; }
        public string PrimaryKey { set; get; }
        public string OldValues { set; get; }
        public string NewValues { set; get; }
        public string AffectedColumns { set; get; }
    }
}
