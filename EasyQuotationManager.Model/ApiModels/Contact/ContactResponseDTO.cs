using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyQuotationManager.Model.ApiModels.Contact
{
    public class ContactResponseDTO
    {
        public Data data { get; set; }
    }

    public class Data
    {
        public string id { get; set; }
        public string first_name { get; set; }
        public string last_name { get; set; }
        public string salutation { get; set; }
        public string vat_number { get; set; }
        public List<Email> emails { get; set; }
        public List<Telephone> telephones { get; set; }
        public string website { get; set; }
        public List<Address> addresses { get; set; }
        public string gender { get; set; }
        public string birthdate { get; set; }
        public string iban { get; set; }
        public string bic { get; set; }
        public string national_identification_number { get; set; }
        public List<Company> companies { get; set; }
        public string language { get; set; }
        public PaymentTerm payment_term { get; set; }
        public InvoicingPreferences invoicing_preferences { get; set; }
        public string remarks { get; set; }
        public List<string> tags { get; set; }
        public List<CustomFieldGet> custom_fields { get; set; }
        public bool marketing_mails_consent { get; set; }
        public DateTime added_at { get; set; }
        public DateTime updated_at { get; set; }
        public string web_url { get; set; }
    }
}
