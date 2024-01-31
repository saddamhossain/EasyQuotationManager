using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace EasyQuotationManager.Shared.Utility
{
    public class RequestHelper<T> where T : class
    {
        public static async Task<T> MakeRequest(RequestModel model)
        {
            try
            {
                var url = $"https://api.focus.teamleader.eu{model.URL}";
                if (!model.IsApiRequest)
                {
                    url = $"https://focus.teamleader.eu{model.URL}";
                }
                var request = new HttpRequestMessage(model.RequestType, url);
                //auth
                if (!string.IsNullOrEmpty(model.Token))
                {
                    request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", model.Token);
                }
                //common headers
                request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                request.Headers.AcceptEncoding.Add(new StringWithQualityHeaderValue("gzip"));
                request.Headers.AcceptEncoding.Add(new StringWithQualityHeaderValue("deflate"));
                request.Headers.AcceptEncoding.Add(new StringWithQualityHeaderValue("br"));
                var bodyJson = "{}";
                //request body
                if (model.RequstBody != null)
                {
                    bodyJson = JsonConvert.SerializeObject(model.RequstBody, new JsonSerializerSettings { DefaultValueHandling = DefaultValueHandling.Ignore }); //don't send null value
                }
                request.Content = new StringContent(bodyJson, Encoding.UTF8,
                            "application/json");

                using (var client = new HttpClient(new HttpClientHandler
                {
                    AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
                }))
                {
                    var response = await client.SendAsync(request);
                    if (response.IsSuccessStatusCode)
                    {
                        var responseStream = await response.Content.ReadAsStringAsync();
                        //check for no content and return empty result
                        if (response.StatusCode == HttpStatusCode.NoContent)
                        {
                            return JsonConvert.DeserializeObject<T>("{}");
                        }
                        var result = JsonConvert.DeserializeObject<T>(responseStream);
                        return result;
                    }
                    if (response.StatusCode == HttpStatusCode.BadRequest)
                    {
                        var responseStream = await response.Content.ReadAsStringAsync();
                        var result = JsonConvert.DeserializeObject<ResponseCommonBadRequestModel>(responseStream);
                        if (result != null && result.errors != null)
                        {
                            throw new AppException(result.errors.FirstOrDefault()?.title);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                throw new AppException(e.Message);
            }
            return null;
        }
    }
}
