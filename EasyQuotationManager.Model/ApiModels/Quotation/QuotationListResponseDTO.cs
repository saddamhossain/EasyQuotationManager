using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyQuotationManager.Model.ApiModels.Quotation
{
    public class QuotationListResponseDTO
    {
        public List<QuotationListItemResponseDTO> data { get; set; }
    }

    public class QuotationListItemResponseDTO
    {
        public string id { get; set; }
        public Deal deal { get; set; }
        public CurrencyExchangeRate currency_exchange_rate { get; set; }
        public Total total { get; set; }
        public DateTime created_at { get; set; }
        public DateTime updated_at { get; set; }
        public string status { get; set; }
    }
}
