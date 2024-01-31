using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EasyQuotationManager.SeedData
{
    public interface IDbInitializer
    {
        Task Initialize();
    }
}
