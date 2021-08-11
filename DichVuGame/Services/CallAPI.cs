using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace DichVuGame.Services
{
    public class CallAPI
    {
        public string APIUrl { get; set; }
        public CallAPI(string url)
        {
            APIUrl = url;
        }
        public string GetResponse()
        {
            WebRequest request = WebRequest.Create(
              APIUrl);
            request.Credentials = CredentialCache.DefaultCredentials;
            WebResponse response = request.GetResponse();
            Stream dataStream = response.GetResponseStream();
            StreamReader reader = new StreamReader(dataStream);
            string responseFromServer = reader.ReadToEnd();
            return responseFromServer;
        }
    }
}
