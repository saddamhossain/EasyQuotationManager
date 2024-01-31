using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyQuotationManager.Model.ApiModels.Quotation
{
    public class TaxInclusiveBeforeDiscount
    {
        public double amount { get; set; }
        public string currency { get; set; }
    }
}
