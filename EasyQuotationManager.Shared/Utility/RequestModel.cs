using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace EasyQuotationManager.Shared.Utility
{
    public class RequestModel
    {
        public RequestModel()
        {
            RequestType = HttpMethod.Get;
            IsApiRequest = true;
        }
        /// <summary>
        /// Network request type GET/POST by default it's GET
        /// </summary>
        public HttpMethod RequestType { get; set; }
        public string URL { get; set; }
        public object RequstBody { get; set; }
        public string Token { get; set; }
        //Checking for api or auth Request
        public bool IsApiRequest { get; set; }
    }
}
