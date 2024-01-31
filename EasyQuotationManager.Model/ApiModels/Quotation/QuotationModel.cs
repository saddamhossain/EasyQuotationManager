using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyQuotationManager.Model.ApiModels.Quotation
{
    public class QuotationModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string IntroductionText { get; set; }
        public string DealId { get; set; }
        public string Currency { get; set; }
        public string Tax { get; set; } = "excluding";

        public List<LineItemsDetailsViewModel> LineItems { get; set; }
    }
    public class LineItemsDetailsViewModel
    {
        public double SubTotal { set; get; }
        public double Quantity { get; set; }
        public string Description { get; set; }
        public string ExtendedDescription { get; set; }
        public string TaxRateId { get; set; }
        public double? Discount { get; set; }
        public double Price { get; set; }
        public string Type { get; set; }
    }
}
