using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyQuotationManager.Shared.ViewModels
{
    public class ManageUserRolesViewModel
    {
        public string ApplicationUserId { set; get; }
        public string UserName { set; get; }
        public string RoleId { get; set; }
        public string RoleName { get; set; }
        public bool Selected { get; set; }
    }
}
