
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyQuotationManager.Model
{
    public class ApplicationUser : IdentityUser
    {
        public string FirstName { set; get; }
        public string LastName { set; get; }
        public DateTime LicenseExpirationDate { set; get; }
        public string TeamLeaderAuthCode { get; set; }
        public string TeamLeaderAccessToken { get; set; }
        public string TeamLeaderRefreshToken { get; set; }
        public DateTime? TeamLeaderTokenCreatedUtc { get; set; }
        public DateTime? TeamLeaderTokenExpireInUtc { get; set; }
        public string CreatedBy { set; get; }
        public DateTime CreatedDate { set; get; }
        public string UpdatedBy { set; get; }
        public DateTime? UpdatedDate { set; get; }
    }
}
