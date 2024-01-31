using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyQuotationManager.Shared.Utility
{
    public class CommonServiceResult<T> where T : class
    {
        public CommonServiceResult()
        {

        }
        public CommonServiceResult(bool isSuccess, string message)
        {
            IsSuccess = isSuccess;
            Messages = new List<string> { message };
        }
        public T Data { get; set; }
        public bool IsSuccess { get; set; }
        public List<string> Messages { get; set; }
    }
}
