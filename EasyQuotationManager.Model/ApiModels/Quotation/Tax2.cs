using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyQuotationManager.Model.ApiModels.Quotation
{
    public class Tax2
    {
        public double rate { get; set; }
        public Taxable taxable { get; set; }
        public Tax tax { get; set; }
    }
}
