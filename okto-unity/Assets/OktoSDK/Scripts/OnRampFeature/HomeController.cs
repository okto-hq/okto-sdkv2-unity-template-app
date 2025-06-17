using UnityEngine;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using UnityEngine.Networking;
using OktoSDK;
using OktoSDK.Auth;

namespace OktoSDK.OnRamp
{
    public class HomeController : MonoBehaviour
    {
        /// <summary>
        /// Requests a transaction token from the Okto API.
        /// </summary>
        /// <param name="_authToken">The user's auth token.</param>
        /// <param name="_deviceToken">The user's device token.</param>
        /// <returns>The transaction token string, or null if failed.</returns>
        public async Task<string> GetTransactionToken(string authToken, string _deviceToken)
        {
            string url = OktoAuthManager.GetOktoClient().Env.BffBaseUrl + "/api/v2/transaction_token";

            using (UnityWebRequest request = new UnityWebRequest(url, "POST"))
            {
                request.downloadHandler = new DownloadHandlerBuffer();

                // Log headers and Authorization token being sent
                request.SetRequestHeader("Authorization", $"Bearer {authToken}");
                request.SetRequestHeader("x-source", "okto_wallet_web");
                request.SetRequestHeader("x-version", "okto_plus");

                // Construct and log equivalent curl command
                string curl = $"curl -X POST \"{url}\" " +
                              $"-H \"Authorization: Bearer {authToken}\" " +
                              "-H \"x-source: okto_wallet_web\"";

                CustomLogger.Log("[GetTransactionToken] cURL: " + curl);

                var operation = request.SendWebRequest();
                while (!operation.isDone)
                    await Task.Yield();


                if (request.result != UnityWebRequest.Result.Success)
                {
                    CustomLogger.LogError($"[GetTransactionToken] Error: {request.error}");
                    return "";
                }

                try
                {
                    var json = JObject.Parse(request.downloadHandler.text);
                    return json["data"]?["transactionToken"]?.ToString();
                }
                catch (System.Exception ex)
                {
                    CustomLogger.LogError($"[GetTransactionToken] JSON Parse Error: {ex.Message}");
                    return "";
                }
            }
        }
    }
}