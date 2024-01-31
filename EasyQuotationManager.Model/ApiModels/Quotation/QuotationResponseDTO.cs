using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyQuotationManager.Model.ApiModels.Quotation
{
    public class QuotationResponseDTO
    {
        public Data data { get; set; }
    }

    public class Data
    {
        public string id { get; set; }
        public Deal deal { get; set; }
        public CurrencyExchangeRate currency_exchange_rate { get; set; }
        public List<GroupedLine> grouped_lines { get; set; }
        public TotalDetails total { get; set; }
        public List<Discount2> discounts { get; set; }
        public DateTime created_at { get; set; }
        public DateTime updated_at { get; set; }
        public string status { get; set; }
    }

    public class TotalDetails
    {
        public TaxExclusive tax_exclusive { get; set; }
        public TaxInclusive tax_inclusive { get; set; }
        public List<Tax2> taxes { get; set; }
        public PurchasePrice purchase_price { get; set; }
    }
}
