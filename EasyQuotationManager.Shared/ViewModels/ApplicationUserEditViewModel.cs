using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyQuotationManager.Shared.ViewModels
{
    public class ApplicationUserEditViewModel : IdentityUser
    {
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [Display(Name = "License Expiration Date")]
        public DateTime LicenseExpirationDate { set; get; }
        public string Role { set; get; }
        public string UpdatedBy { set; get; }
        public DateTime? UpdatedDate { set; get; }
    }
}
