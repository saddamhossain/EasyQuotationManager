using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyQuotationManager.Shared.Utility
{
    public class ResponseCommonBadRequestModel
    {
        public List<Error> errors { get; set; }
    }

    public class Meta
    {
        public string field { get; set; }
    }

    public class Error
    {
        public int code { get; set; }
        public string title { get; set; }
        public int status { get; set; }
        public Meta meta { get; set; }
    }
 
}
