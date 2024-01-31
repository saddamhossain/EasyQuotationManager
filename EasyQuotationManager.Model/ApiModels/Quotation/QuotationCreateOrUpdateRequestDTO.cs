using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyQuotationManager.Model.ApiModels.Quotation
{
    public class QuotationCreateOrUpdateRequestDTO
    {
        public string deal_id { get; set; }

        public List<GroupedLine> grouped_lines { get; set; }
        public string text { get; set; }


        public class GroupedLine
        {
            public Section section { get; set; }
            public List<LineItems> line_items { get; set; }

        }
        public class Section
        {
            public string title { get; set; }
        }
        public class LineItems
        {
            public double quantity { get; set; }
            public string description { get; set; }
            public string extended_description { get; set; }
            public UnitPrice unit_price { get; set; }
            public string tax_rate_id { get; set; }
            public ItemDiscount discount { get; set; }
        }

        public class UnitPrice
        {
            public double amount { get; set; }
            public string currency { get; set; }
            public string tax { get; set; }
        }
        public class ItemDiscount
        {
            public double value { get; set; }
            public string type => "percentage";
        }
    }
}
