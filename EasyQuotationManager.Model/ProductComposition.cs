using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyQuotationManager.Model
{
    public class ProductComposition
    {
        public int ProductCompositionId { set; get; }
        public string CombinedProductIdOrName { set; get; }
        public int CombinedProductQty { set; get; }
        public string SubProductName { set; get; }
        public int SubProductQty { set; get; }
    }
}
