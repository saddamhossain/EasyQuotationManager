using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyQuotationManager.Model.ApiModels.Quotation
{
    public class Total
    {
        public TaxExclusive tax_exclusive { get; set; }
        public TaxExclusiveBeforeDiscount tax_exclusive_before_discount { get; set; }
        public TaxInclusive tax_inclusive { get; set; }
        public TaxInclusiveBeforeDiscount tax_inclusive_before_discount { get; set; }
        public List<Tax2> taxes { get; set; }
        public PurchasePrice purchase_price { get; set; }
    }
}
