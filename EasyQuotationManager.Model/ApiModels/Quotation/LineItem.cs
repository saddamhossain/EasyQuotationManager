using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyQuotationManager.Model.ApiModels.Quotation
{
    public class LineItem
    {
        public Product product { get; set; }
        public string product_category { get; set; }
        public double quantity { get; set; }
        public string description { get; set; }
        public string extended_description { get; set; }
        public UnitPrice unit_price { get; set; }
        public Tax3 tax { get; set; }
        public Discount discount { get; set; }
        public PurchasePrice purchase_price { get; set; }
        public LineItemTotalTaxDetails total { get; set; }
    }
    public class Tax3
    {
        public string type { get; set; }
        public string id { get; set; }
    }
    public class LineItemTotalTaxDetails
    {
        public TaxExclusive tax_exclusive { get; set; }
        public TaxExclusiveBeforeDiscount tax_exclusive_before_discount { get; set; }
        public TaxInclusive tax_inclusive { get; set; }
        public TaxInclusiveBeforeDiscount tax_inclusive_before_discount { get; set; }
    }


}
