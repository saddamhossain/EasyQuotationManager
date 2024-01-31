using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyQuotationManager.Shared.Utility
{
    public class EmailOptions
    {
        public string Host { set; get; }
        public int Port { get; set; }
        public bool EnableSSL { set; get; }
        public string MailFrom { set; get; }
        public string UserName { set; get; }
        public string Password { set; get; }
    }
}
