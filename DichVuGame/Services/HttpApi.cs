using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Policy;
using System.Threading.Tasks;

namespace DichVuGame.Services
{
    public class HttpApi
    {
        public string ApiUrl { get; set; }
        public HttpApi(string url)
        {
            ApiUrl = url;
        }
        public HttpClient init()
        {
            var client = new HttpClient();
            client.BaseAddress = new Uri(ApiUrl);
            return client;
        }
    }
}
