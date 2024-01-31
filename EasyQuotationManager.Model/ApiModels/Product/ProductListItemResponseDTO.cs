using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyQuotationManager.Model.ApiModels.Product
{
    public class ProductListItemResponseDTO
    {
        public string id { get; set; }
        public string name { get; set; }
        public string code { get; set; }
        public string description { get; set; }    
    }
}
