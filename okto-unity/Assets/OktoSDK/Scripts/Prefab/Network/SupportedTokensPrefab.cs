using Newtonsoft.Json;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using OktoSDK.BFF;

namespace OktoSDK
{
    public class SupportedTokensPrefab : MonoBehaviour
    {
        // Fetches the SupportedTokensApi
        public async Task<List<Token>> SupportedTokensApi()
        {
            var bffTokens = await BffClientRepository.GetSupportedTokens();
            
            // Convert BFF tokens to model tokens
            List<Token> modelTokens = new List<Token>();
            foreach (var token in bffTokens)
            {
                modelTokens.Add(new Token
                {
                    address = token.address,
                    symbol = token.symbol,
                    name = token.name,
                    networkId = token.networkId,
                    networkName = token.networkName
                });
            }
            
            string json = JsonConvert.SerializeObject(modelTokens, Formatting.Indented);
            CustomLogger.Log("Tokens: " + json);
            return modelTokens;
        }
    }
}