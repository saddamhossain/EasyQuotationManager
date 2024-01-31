using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyQuotationManager.Model
{
    public class Product
    {
        public int ProductId { set; get; }
        public string Name { set; get; }
        public string VATPct { set; get; }
        public double Price { set; get; }
        public string Unit { set; get; }
    }
}
