using EasyQuotationManager.Model;
using EasyQuotationManager.Model.ApiModels.Product;
using EasyQuotationManager.Shared.Utility;
using EasyQuotationManager.Shared.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EasyQuotationManager.Repository.IRepository
{
    public interface IProductRepository
    {
        Task<CommonServiceResult<ProductListResponseDTO>> GetAll();
        Task<CommonServiceResult<ProductDetailsResponseDTO>> Get(string id);
        Task<bool> InsertCombinedProducts(string combinedProductIdOrName, int combinedProductQty, List<SubProductDetailsViewModel> subProductDetailsViewModels);
        Task<IQueryable<ProductComposition>> GetCombinedProducts();
        Task<IQueryable<ProductComposition>> GetCombinedProductById(string combinedProductIdOrName);
        Task<bool> DeleteCombinedProduct(string combinedProductIdOrName);
    }
}
