using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyQuotationManager.Model.ApiModels.Quotation
{
    public class GroupedLine
    {
        public Section section { get; set; }
        public List<LineItem> line_items { get; set; }
    }
}
