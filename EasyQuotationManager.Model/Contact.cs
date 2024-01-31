using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyQuotationManager.Model
{
    public class Contact
    {
        public int ContactId { set; get; }
        public string FirstName { set; get; }
        public string LastName { set; get; }
        public string VATNumber { set; get; }
        public string Email { set; get; }
        public string Phone { set; get; }
        public string AddressStreet { set; get; }
        public string AddressNumber { set; get; }
        public string AddressZipCode { set; get; }
        public string AddressCity { set; get; }
        public string ContactType { set; get; }
        public string RecId { set; get; }
    }
}
