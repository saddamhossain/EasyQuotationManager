using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyQuotationManager.Shared.Utility
{ 
    /// <summary>
  /// only for empty result for avoiding deserilization exception
  /// </summary>
    public class EmptyResultModel
    {
        public string EmptyString { get; set; }
    }
}
