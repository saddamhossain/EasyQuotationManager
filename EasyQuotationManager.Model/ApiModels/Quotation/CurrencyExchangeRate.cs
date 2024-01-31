using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyQuotationManager.Model.ApiModels.Quotation
{
    public class CurrencyExchangeRate
    {
        public string from { get; set; }
        public string to { get; set; }
        public double rate { get; set; }
    }
}
