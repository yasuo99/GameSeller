using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DichVuGame.Utility
{
    public class PayPalConfig
    {
        public string PayPalMode { get; set; }
        public string PayPalConnectionTimeout { get; set; }
        public string PayPalRequestEntries { get; set; }
        public string PayPalClientId { get; set; }
        public string PayPalClientSecret { get; set; }
        public Dictionary<string,string> GetConfig()
        {
            Dictionary<string, string> config = new Dictionary<string, string>();
            config.Add("mode", PayPalMode);
            config.Add("connectionTimeout", PayPalConnectionTimeout);
            config.Add("requestEntries", PayPalRequestEntries);
            config.Add("clientId", PayPalClientId);
            config.Add("clientSecret", PayPalClientSecret);
            return config;
        }
    }
}
