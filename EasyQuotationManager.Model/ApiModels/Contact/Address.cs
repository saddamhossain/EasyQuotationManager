using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyQuotationManager.Model.ApiModels.Contact
{
    public class Address
    {
        public string type { get; set; }
        public Address address { get; set; }
    }
}
