using EasyQuotationManager.Model;
using EasyQuotationManager.Shared.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EasyQuotationManager.Repository.IRepository
{
    public interface ITokenRepository
    {
        Task<string> GetToken();
        Task<TokenResponseModel> CreateNewToken(ApplicationUser user);
        Task<TokenResponseModel> CreateRefreshToken(ApplicationUser user);
        Task UdateUserToken(ApplicationUser user, TokenResponseModel tokenResponseModel, bool isNewToken = false);
    }
}
