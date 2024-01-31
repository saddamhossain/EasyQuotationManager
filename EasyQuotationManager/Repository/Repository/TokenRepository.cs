using EasyQuotationManager.Data;
using EasyQuotationManager.Model;
using EasyQuotationManager.Repository.IRepository;
using EasyQuotationManager.Shared.Utility;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace EasyQuotationManager.Repository.Repository
{
    public class TokenRepository : ITokenRepository
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ApplicationDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IConfiguration _configuration;

        public TokenRepository(IConfiguration configuration, UserManager<IdentityUser> userManager, ApplicationDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _configuration = configuration;
            _userManager = userManager;
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<string> GetToken()
        {
            var current_logged_in_user_name = _httpContextAccessor.HttpContext.User.Identity.Name.ToLower();
            var user_details = await _userManager.FindByNameAsync(current_logged_in_user_name);
            var user = await _context.ApplicationUser.Where(s => s.Id == user_details.Id).FirstOrDefaultAsync();
            if (user == null)
            {
                throw new AppException("User not found");
            }
            else if (string.IsNullOrEmpty(user.TeamLeaderAuthCode))
            {
                throw new AppException("User in not authorized in Teamleader");
            }

            //Check for existing token within valid time
            var nowTime = DateTime.UtcNow;
            if (user.TeamLeaderTokenExpireInUtc != null && nowTime <= user.TeamLeaderTokenExpireInUtc)
            {
                return user.TeamLeaderAccessToken;
            }

            //Check for existing token
            if (string.IsNullOrEmpty(user.TeamLeaderAccessToken))
            {
                var newTokeRes = await CreateNewToken(user);
                return newTokeRes.access_token;
            }

            //refresh token
            var tokenResponse = await CreateRefreshToken(user);
            return tokenResponse.access_token;
        }

        public async Task<TokenResponseModel> CreateNewToken(ApplicationUser user)
        {
            var request = new RequestModel
            {
                URL = "/oauth2/access_token",
                RequstBody = new
                {
                    client_id = _configuration["Teamleader:client_id"],
                    client_secret = _configuration["Teamleader:client_secret"],
                    code = user.TeamLeaderAuthCode,
                    grant_type = "authorization_code",
                    redirect_uri = _configuration["Teamleader:redirect_uri"],
                },
                IsApiRequest = false,
                RequestType = System.Net.Http.HttpMethod.Post

            };
            var tokeResponse = await RequestHelper<TokenResponseModel>.MakeRequest(request);
            if (tokeResponse == null)
            {
                throw new AppException("Faild to create token");
            }
            await UdateUserToken(user, tokeResponse, true);
            return tokeResponse;
        }

        public async Task<TokenResponseModel> CreateRefreshToken(ApplicationUser user)
        {
            var request = new RequestModel
            {
                URL = "/oauth2/access_token",
                RequstBody = new
                {
                    client_id = _configuration["Teamleader:client_id"],
                    client_secret = _configuration["Teamleader:client_secret"],
                    refresh_token = user.TeamLeaderRefreshToken,
                    grant_type = "refresh_token",
                },
                IsApiRequest = false,
                RequestType = System.Net.Http.HttpMethod.Post
            };
            var tokeResponse = await RequestHelper<TokenResponseModel>.MakeRequest(request);
            if (tokeResponse == null)
            {
                throw new AppException("Faild to create token");
            }
            await UdateUserToken(user, tokeResponse);
            return tokeResponse;
        }

        public async Task UdateUserToken(ApplicationUser user, TokenResponseModel tokenResponseModel, bool isNewToken = false)
        {
            var todayTime = DateTime.UtcNow;
            user.TeamLeaderTokenExpireInUtc = todayTime.AddSeconds(tokenResponseModel.expires_in);
            user.TeamLeaderAccessToken = tokenResponseModel.access_token;
            user.TeamLeaderRefreshToken = tokenResponseModel.refresh_token;
            if (isNewToken)
            {
                user.TeamLeaderTokenCreatedUtc = todayTime;
            }
            await _context.SaveChangesAsync(user.Id);
        }

    }
}
