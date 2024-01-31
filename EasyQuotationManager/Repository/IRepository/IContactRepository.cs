using EasyQuotationManager.Model.ApiModels.Contact;
using EasyQuotationManager.Shared.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EasyQuotationManager.Repository.IRepository
{
    public interface IContactRepository
    {
        Task<CommonServiceResult<ContactListResponseDTO>> GetAll();
        Task<CommonServiceResult<ContactDetailsResponseDTO>> Get(string id);
    }
}
