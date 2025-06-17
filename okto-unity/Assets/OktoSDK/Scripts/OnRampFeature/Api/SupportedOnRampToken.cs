using Newtonsoft.Json;
using OktoSDK.Auth;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using static OktoSDK.BFF.BffClientRepository;

namespace OktoSDK.OnRamp
{
    public static class SupportedOnRampToken
    {
        private const string rpc = "/api/v2/supported_ramp_tokens?country_code=IN&side=onramp";

        public static async Task<List<WhitelistedToken>> FetchStatus(string authToken)
        {
            string apiUrl = OktoAuthManager.GetOktoClient().Env.BffBaseUrl + rpc;
            using (UnityWebRequest request = UnityWebRequest.Get(apiUrl))
            {
                request.SetRequestHeader("Accept", "application/json, text/plain, */*");
                request.SetRequestHeader("Authorization", "Bearer " + authToken);
                request.SetRequestHeader("Content-Type", "application/json");
                string curlCommand = $"curl -X GET \"{apiUrl}\" " +
                                     $"-H \"Accept: application/json, text/plain, */*\" " +
                                     $"-H \"Authorization: Bearer {authToken}\" " +
                                     $"-H \"Content-Type: application/json\"";
                CustomLogger.Log("CURL Equivalent:\n" + curlCommand);

                var asyncOperation = request.SendWebRequest();

                // Await the completion of the request
                while (!asyncOperation.isDone)
                {
                    await Task.Yield(); // Ensures the async operation runs without blocking the main thread
                }

                CustomLogger.Log("request.downloadHandler.text " + request.downloadHandler.text);

                if (request.result == UnityWebRequest.Result.Success)
                {
                    string json = request.downloadHandler.text;
                    var response = JsonConvert.DeserializeObject<ApiResponseWithCount<WhitelistedToken>>(json);

                    if (response.data == null || response.data.onramp_tokens.Count == 0)
                    {
                        ResponsePanel.SetResponse("OnRamp is not supported for this token!");
                        return null;
                    }

                    return response.data.onramp_tokens;
                }
                else
                {
                    ResponsePanel.SetResponse(request.downloadHandler.text);
                    return null;
                }
            }
        }

    }
}