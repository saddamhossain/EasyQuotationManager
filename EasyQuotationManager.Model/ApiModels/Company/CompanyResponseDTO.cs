using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyQuotationManager.Model.ApiModels.Company
{
    public class CompanyResponseDTO
    {
        public Data data { get; set; }
    }
    public class Data
    {
        public string id { get; set; }
        public string name { get; set; }
        public BusinessType business_type { get; set; }
        public string vat_number { get; set; }
        public string national_identification_number { get; set; }
        public List<Email> emails { get; set; }
        public List<Telephone> telephones { get; set; }
        public string website { get; set; }
        public List<Address> addresses { get; set; }
        public string iban { get; set; }
        public string bic { get; set; }
        public string language { get; set; }
        public string preferred_currency { set; get; }
        public PaymentTerm payment_term { get; set; }
        public InvoicingPreferences invoicing_preferences { get; set; }
        public ResponsibleUser responsible_user { get; set; }
        public string remarks { get; set; }
        public DateTime added_at { get; set; }
        public DateTime updated_at { get; set; }
        public string web_url { get; set; }
        public List<string> tags { get; set; }
        public List<CustomFieldGet> custom_fields { get; set; }
        public bool marketing_mails_consent { get; set; }
    }
}
