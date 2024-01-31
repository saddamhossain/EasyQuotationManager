using EasyQuotationManager.Model.ApiModels.Quotation;
using EasyQuotationManager.Shared.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EasyQuotationManager.Repository.IRepository
{
    public interface IQuotationRepository
    {
        Task<CommonServiceResult<QuotationListResponseDTO>> GetAll();
        Task<CommonServiceResult<QuotationDetailsResponseDTO>> Get(string id);
        Task<CommonServiceResult<EmptyResultModel>> Create(QuotationModel model);
    }
}
