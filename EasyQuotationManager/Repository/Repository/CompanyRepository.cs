using EasyQuotationManager.Model.ApiModels.Company;
using EasyQuotationManager.Repository.IRepository;
using EasyQuotationManager.Shared.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EasyQuotationManager.Repository.Repository
{
    public class CompanyRepository : ICompanyRepository
    {
        private readonly ITokenRepository _tokenRepository;

        public CompanyRepository(ITokenRepository tokenRepository)
        {
            _tokenRepository = tokenRepository;
        }

        public async Task<CommonServiceResult<CompanyListResponseDTO>> GetAll()
        {
            var request = new RequestModel
            {
                URL = "/companies.list",
                RequestType = System.Net.Http.HttpMethod.Get,
                Token = await _tokenRepository.GetToken()

            };
            var createResponse = await RequestHelper<CompanyListResponseDTO>.MakeRequest(request);
            if (createResponse != null)
            {
                return new CommonServiceResult<CompanyListResponseDTO>(true, "Successfully retrived Contacts") { Data = createResponse };
            }
            return new CommonServiceResult<CompanyListResponseDTO>(false, "Faild to retrive Contacts");
        }

        public async Task<CommonServiceResult<CompanyDetailsResponseDTO>> Get(string id)
        {
            var request = new RequestModel
            {
                URL = "/companies.info",
                RequstBody = new
                {
                    id = id
                },
                Token = await _tokenRepository.GetToken()

            };
            var createResponse = await RequestHelper<CompanyDetailsResponseDTO>.MakeRequest(request);
            if (createResponse != null)
            {
                return new CommonServiceResult<CompanyDetailsResponseDTO>(true, "Successfully retrived Contact") { Data = createResponse };
            }
            return new CommonServiceResult<CompanyDetailsResponseDTO>(false, "Faild to retrive Contact");
        }
    }
}
