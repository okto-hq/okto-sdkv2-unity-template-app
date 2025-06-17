using System.Collections.Generic;
using System;
using System.Linq;

namespace OktoSDK.OnRamp
{
    [Serializable]
    public class AddFundsData
    {
        public string walletAddress;
        public string walletBalance;
        public string tokenId;
        public string networkId;
        public string userId;
        public string email;
        public string countryCode;
        public string payToken;
        public string theme = "light";
        public string app_version = "500000";
        public string platform = "web";
        public string app = "okto_web";
        public string screen_source;
        public string tokenName;
        public string chainId;
        public string paymentId = "";   // optional
        public string isDirectFlow = ""; // optional
        public string host = "sdk"; // required for sdk

        public string GenerateUrl(string baseUrl)
        {
            var queryParams = ToQueryParameters();
            string queryString = string.Join("&",
                queryParams.Where(kvp => !string.IsNullOrEmpty(kvp.Value))
                .Select(kvp => $"{Uri.EscapeDataString(kvp.Key)}={Uri.EscapeDataString(kvp.Value)}"));

            return $"{baseUrl}?{queryString}";
        }

        public Dictionary<string, string> ToQueryParameters()
        {
            return new Dictionary<string, string>
            {
                { "walletAddress", walletAddress },
                { "walletBalance", walletBalance },
                { "tokenId", tokenId },
                { "networkId", networkId },
                { "email", email },
                { "countryCode", countryCode },
                { "theme", theme },
                { "app_version", app_version },
                { "platform", platform },
                { "app", app },
                { "userId", userId },
                { "screen_source", screen_source},
                { "payToken", payToken },
                { "chain", chainId},
                { "tokenName", tokenName },
                { "host", host }
            };
        }
    }
}