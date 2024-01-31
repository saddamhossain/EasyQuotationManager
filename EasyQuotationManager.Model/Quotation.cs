using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyQuotationManager.Model
{
    public class Quotation
    {
        public int QuotationId { set; get; }
        public DateTime QuotationDate { set; get; }
        public string QuotationNumber { set; get; }
        public int ContactId { set; get; }
        public int DealId { set; get; }
    }
}
