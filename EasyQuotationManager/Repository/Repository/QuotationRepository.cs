using EasyQuotationManager.Model.ApiModels.Quotation;
using EasyQuotationManager.Repository.IRepository;
using EasyQuotationManager.Shared.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EasyQuotationManager.Repository.Repository
{
    public class QuotationRepository : IQuotationRepository
    {
        private readonly ITokenRepository _tokenRepository;

        public QuotationRepository(ITokenRepository tokenRepository)
        {
            _tokenRepository = tokenRepository;
        }

        public async Task<CommonServiceResult<QuotationListResponseDTO>> GetAll()
        {
            var request = new RequestModel
            {
                URL = "/quotations.list",
                RequestType = System.Net.Http.HttpMethod.Get,
                Token = await _tokenRepository.GetToken()

            };
            var createResponse = await RequestHelper<QuotationListResponseDTO>.MakeRequest(request);
            if (createResponse != null)
            {
                return new CommonServiceResult<QuotationListResponseDTO>(true, "Successfully retrived Contacts") { Data = createResponse };
            }
            return new CommonServiceResult<QuotationListResponseDTO>(false, "Faild to retrive Contacts");
        }

        public async Task<CommonServiceResult<QuotationDetailsResponseDTO>> Get(string id)
        {
            var request = new RequestModel
            {
                URL = "/quotations.info",
                RequstBody = new
                {
                    id = id
                },
                Token = await _tokenRepository.GetToken()

            };
            var createResponse = await RequestHelper<QuotationDetailsResponseDTO>.MakeRequest(request);
            if (createResponse != null)
            {
                return new CommonServiceResult<QuotationDetailsResponseDTO>(true, "Successfully retrived Contact") { Data = createResponse };
            }
            return new CommonServiceResult<QuotationDetailsResponseDTO>(false, "Faild to retrive Contact");
        }
        public async Task<CommonServiceResult<EmptyResultModel>> Create(QuotationModel model)
        {
            var request = new RequestModel
            {
                URL = "/quotations.create",
                RequstBody = MapModel(model),
                RequestType = System.Net.Http.HttpMethod.Post,
                Token = await _tokenRepository.GetToken()

            };
            var createResponse = await RequestHelper<EmptyResultModel>.MakeRequest(request);
            if (createResponse != null)
            {
                return new CommonServiceResult<EmptyResultModel>(true, "quotation created successfully") { Data = createResponse };
            }
            return new CommonServiceResult<EmptyResultModel>(false, "Faild to create quotation");
        }

        public QuotationCreateOrUpdateRequestDTO MapModel(QuotationModel model)
        {
            var obj = new QuotationCreateOrUpdateRequestDTO()
            {
                deal_id = "044a25fb-54a4-0981-956c-7f0a7febeef0", // Static Deal ID
                text = model.IntroductionText
            };
            var lineItems = new List<QuotationCreateOrUpdateRequestDTO.LineItems>();
            foreach (var item in model.LineItems)
            {
                var lineItem = new QuotationCreateOrUpdateRequestDTO.LineItems
                {
                    description = item.Description,
                    quantity = item.Quantity,
                    extended_description = item.ExtendedDescription,
                    tax_rate_id = "fefd67b7-37aa-050e-8643-6780c8bcc381",
                    unit_price = new QuotationCreateOrUpdateRequestDTO.UnitPrice
                    {
                        amount = 111,
                        currency = "EUR",
                        tax = "excluding"
                    },
                };
                //if (item.Discount.HasValue && item.Discount > 0)
                //{
                //    lineItem.discount = new QuotationCreateOrUpdateRequestDTO.ItemDiscount
                //    {
                //        value = item.Discount ?? 0
                //    };
                //}
                lineItems.Add(lineItem);
            }

            obj.grouped_lines = new List<QuotationCreateOrUpdateRequestDTO.GroupedLine>()
                {
                    new QuotationCreateOrUpdateRequestDTO.GroupedLine
                    {
                        section = new QuotationCreateOrUpdateRequestDTO.Section
                        {
                            title = model.Name
                        },
                        line_items = lineItems
                    }
                };
            return obj;
        }
    }
}
