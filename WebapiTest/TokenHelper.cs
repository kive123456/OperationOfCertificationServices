using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace WebapiTest
{
    public class TokenHelper
    {
        private readonly string api = "http://localhost:31225/api/token";        

        public string GetToken()
        {
            Uri server = new Uri(api);
            HttpClient httpClient = new HttpClient();            

            HttpResponseMessage responseMessage = httpClient.GetAsync(server).Result;

            if (responseMessage.IsSuccessStatusCode)
            {
                string content = responseMessage.Content.ReadAsStringAsync().Result;     
                           
                return content;

            }
            return null;
        }
    }
}