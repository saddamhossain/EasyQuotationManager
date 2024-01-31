using EasyQuotationManager.Model.ApiModels.Contact;
using EasyQuotationManager.Repository.IRepository;
using EasyQuotationManager.Shared.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EasyQuotationManager.Repository.Repository
{
    public class ContactRepository : IContactRepository
    {
        private readonly ITokenRepository _tokenRepository;

        public ContactRepository(ITokenRepository tokenRepository)
        {
            _tokenRepository = tokenRepository;
        }

        public async Task<CommonServiceResult<ContactListResponseDTO>> GetAll()
        {
            var request = new RequestModel
            {
                URL = "/contacts.list",
                RequestType = System.Net.Http.HttpMethod.Get,
                Token = await _tokenRepository.GetToken()

            };
            var createResponse = await RequestHelper<ContactListResponseDTO>.MakeRequest(request);
            if (createResponse != null)
            {
                return new CommonServiceResult<ContactListResponseDTO>(true, "Successfully retrived Contacts") { Data = createResponse };
            }
            return new CommonServiceResult<ContactListResponseDTO>(false, "Faild to retrive Contacts");
        }

        public async Task<CommonServiceResult<ContactDetailsResponseDTO>> Get(string id)
        {
            var request = new RequestModel
            {
                URL = "/contacts.info",
                RequstBody = new
                {
                    id = id
                },
                Token = await _tokenRepository.GetToken()

            };
            var createResponse = await RequestHelper<ContactDetailsResponseDTO>.MakeRequest(request);
            if (createResponse != null)
            {
                return new CommonServiceResult<ContactDetailsResponseDTO>(true, "Successfully retrived Contact") { Data = createResponse };
            }
            return new CommonServiceResult<ContactDetailsResponseDTO>(false, "Faild to retrive Contact");
        }
    }
}
