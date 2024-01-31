using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyQuotationManager.Model.ApiModels.Company
{
    public class PrimaryAddress
    {
        public string line_1 { get; set; }
        public string postal_code { get; set; }
        public string city { get; set; }
        public string country { get; set; }
        public AreaLevelTwo area_level_two { get; set; }
    }
}
