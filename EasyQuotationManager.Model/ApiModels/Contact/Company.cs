using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyQuotationManager.Model.ApiModels.Contact
{
    public class Company
    {
        public string position { get; set; }
        public bool decision_maker { get; set; }
        public Company company { get; set; }
    }
}
