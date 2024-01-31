using EasyQuotationManager.Data;
using EasyQuotationManager.Model;
using EasyQuotationManager.Model.ApiModels.Product;
using EasyQuotationManager.Repository.IRepository;
using EasyQuotationManager.Shared.Utility;
using EasyQuotationManager.Shared.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EasyQuotationManager.Repository.Repository
{
    public class ProductRepository : IProductRepository
    {
        private readonly ITokenRepository _tokenRepository;
        private readonly ApplicationDbContext _context;

        public ProductRepository(ITokenRepository tokenRepository, ApplicationDbContext context)
        {
            _tokenRepository = tokenRepository;
            _context = context;
        }
        public async Task<CommonServiceResult<ProductListResponseDTO>> GetAll()
        {
            var request = new RequestModel
            {
                URL = "/products.list",
                Token = await _tokenRepository.GetToken()
            };
            var createResponse = await RequestHelper<ProductListResponseDTO>.MakeRequest(request);
            if (createResponse != null)
            {
                return new CommonServiceResult<ProductListResponseDTO>(true, "Successfully retrive products") { Data = createResponse };
            }
            return new CommonServiceResult<ProductListResponseDTO>(false, "Faild to retrive Products");
        }

        public async Task<CommonServiceResult<ProductDetailsResponseDTO>> Get(string id)
        {
            var request = new RequestModel
            {
                URL = "/products.info",
                RequstBody = new
                {
                    id = id
                },
                Token = await _tokenRepository.GetToken()
            };
            var createResponse = await RequestHelper<ProductDetailsResponseDTO>.MakeRequest(request);
            if (createResponse != null)
            {
                return new CommonServiceResult<ProductDetailsResponseDTO>(true, "Successfully retrive product") { Data = createResponse };
            }
            return new CommonServiceResult<ProductDetailsResponseDTO>(false, "Faild to retrive Product");
        }

        public async Task<bool> InsertCombinedProducts(string combinedProductIdOrName, int combinedProductQty, List<SubProductDetailsViewModel> subProductDetailsViewModels)
        {
            try
            {
                foreach (var item in subProductDetailsViewModels)
                {
                    ProductComposition obj = new();
                    obj.CombinedProductIdOrName = combinedProductIdOrName;
                    obj.CombinedProductQty = combinedProductQty;

                    obj.SubProductName = item.SubProductName;
                    obj.SubProductQty = item.quantity;

                    await _context.ProductCompositions.AddAsync(obj);
                    await _context.SaveChangesAsync();
                }
                return true;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<IQueryable<ProductComposition>> GetCombinedProducts()
        {
            try
            {
                var result = _context.ProductCompositions.AsQueryable();
                return result;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<IQueryable<ProductComposition>> GetCombinedProductById(string combinedProductIdOrName)
        {
            try
            {
                var result = _context.ProductCompositions.Where(s => s.CombinedProductIdOrName.ToLower() == combinedProductIdOrName.ToLower());
                return result;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<bool> DeleteCombinedProduct(string combinedProductIdOrName)
        {
            try
            {
                var productComposition = await _context.ProductCompositions.Where(s=>s.CombinedProductIdOrName.ToLower() == combinedProductIdOrName.ToLower()).ToListAsync();
                if (productComposition != null)
                {
                    _context.ProductCompositions.RemoveRange(productComposition);
                    await _context.SaveChangesAsync();
                    return true;
                }
                return false;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
