using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyQuotationManager.Model.ApiModels.Product
{
    public class ProductListResponseDTO
    {
        public List<ProductListItemResponseDTO> data { get; set; }
    }
}
