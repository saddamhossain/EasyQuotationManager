using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyQuotationManager.Model
{
    public class InternalLog
    {
        [Key]
        public int Id { set; get; }
        public string ControllerName { set; get; }
        public string ServiceName { set; get; }
        public string ActionName { set; get; }
        public string LogType { set; get; }
        public string RazorPageName { set; get; }
        public string LogDescription { set; get; }
        public string UserId { set; get; }
        public string UserName { set; get; }
        public string RoleName { set; get; }
        public string SessionId { set; get; }
        public string ConnectionId { set; get; }
        public string TraceIdentifier { set; get; }
        public string LocalIpAddress { set; get; }
        public string LocalPort { set; get; }
        public string RemoteIpAddress { set; get; }
        public string RemotePort { set; get; }
        public DateTime CreatedDate { set; get; }
    }
}
