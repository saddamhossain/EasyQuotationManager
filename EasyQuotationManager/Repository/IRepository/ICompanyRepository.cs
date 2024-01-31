using EasyQuotationManager.Model.ApiModels.Company;
using EasyQuotationManager.Shared.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EasyQuotationManager.Repository.IRepository
{
    public interface ICompanyRepository
    {
        Task<CommonServiceResult<CompanyListResponseDTO>> GetAll();
        Task<CommonServiceResult<CompanyDetailsResponseDTO>> Get(string id);
    }
}
