using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json;
using OktoSDK.BFF;
using OktoSDK.Auth;
using static OktoSDK.BFF.BffClientRepository;

namespace OktoSDK.OnRamp
{
    public static class OnRampApi
    {
        public static async Task<UserPortfolioData> GetPortfolioAsync(string authToken)
        {
            string url = OktoAuthManager.GetOktoClient().Env.BffBaseUrl + BffClientRepository.Routes.GetPortfolio;

            using (UnityWebRequest request = UnityWebRequest.Get(url))
            {
                request.SetRequestHeader("Authorization", $"Bearer {authToken}");
                request.SetRequestHeader("Content-Type", "application/json");
                var operation = request.SendWebRequest();
                while (!operation.isDone) await Task.Yield();

                CustomLogger.Log(request.downloadHandler.text);
                if (request.result == UnityWebRequest.Result.Success)
                {
                    ApiResponse<UserPortfolioData> response = JsonConvert.DeserializeObject<ApiResponse<UserPortfolioData>>(request.downloadHandler.text);
                    if (response.status == "error" || response.data == null) throw new System.Exception("Failed to retrieve portfolio");
                    return response.data;
                }
                else
                {
                    CustomLogger.LogError($"Error fetching portfolio: {request.error}");
                    return null;
                }
            }
        }

        public static async Task<UserFromToken> GetUserFromTokenAsync(string authToken)
        {
            string url = OktoAuthManager.GetOktoClient().Env.BffBaseUrl + "/api/oc/v1/verify-session";

            using (UnityWebRequest request = UnityWebRequest.Get(url))
            {
                request.SetRequestHeader("Authorization", $"Bearer {authToken}");
                request.SetRequestHeader("Content-Type", "application/json");
                var operation = request.SendWebRequest();
                while (!operation.isDone) await Task.Yield();

                CustomLogger.Log(request.downloadHandler.text);
                if (request.result == UnityWebRequest.Result.Success)
                {
                    var response = JsonConvert.DeserializeObject<ApiResponse<UserFromToken>>(request.downloadHandler.text);
                    if (response.status == "error" || response.data == null) throw new System.Exception("Failed to retrieve user data");
                    return response.data;
                }
                else
                {
                    CustomLogger.LogError($"Error fetching user data: {request.error}");
                    return null;
                }
            }
        }

        public static async Task<List<Wallet>> GetWalletAsync(string authToken, string deviceToken)
        {
            string url = OktoAuthManager.GetOktoClient().Env.BffBaseUrl + BffClientRepository.Routes.GetWallets;

            using (UnityWebRequest request = UnityWebRequest.Get(url))
            {
                request.SetRequestHeader("Authorization", $"Bearer {authToken}");
                request.SetRequestHeader("Content-Type", "application/json");
                request.SetRequestHeader("x-source", "okto_wallet_web");
                var operation = request.SendWebRequest();
                while (!operation.isDone) await Task.Yield();

                CustomLogger.Log(request.downloadHandler.text);
                if (request.result == UnityWebRequest.Result.Success)
                {
                    var response = JsonConvert.DeserializeObject<ApiResponse<List<Wallet>>>(request.downloadHandler.text);
                    if (response.status == "error" || response.data == null) throw new System.Exception("Failed to retrieve portfolio");
                    return response.data;
                }
                else
                {
                    CustomLogger.LogError($"Error fetching wallet: {request.error}");
                    return null;
                }
            }
        }
    }
}