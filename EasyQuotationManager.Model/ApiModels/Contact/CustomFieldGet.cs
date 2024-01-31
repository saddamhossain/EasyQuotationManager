using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyQuotationManager.Model.ApiModels.Contact
{
    public class CustomFieldGet
    {
        public Definition definition { get; set; }
        public object value { get; set; }
    }
}
