using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyQuotationManager.Model
{
    public class QuotationLine
    {
        public int QuotationLineId { set; get; }
        public int QuotationId { set; get; }
        public int ProductId { set; get; }
        public int Qty { set; get; }
        public double Price { set; get; }
        public string QuotationLineSequence { set; get; }
        public string ExtendedDescription { set; get; }
        public bool IsSubHeader { set; get; }
        public string Description { set; get; }
        public double VATPct { set; get; }
        public double DiscountPct { set; get; }
    }
}
