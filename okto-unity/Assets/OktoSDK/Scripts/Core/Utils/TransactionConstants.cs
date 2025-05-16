using Nethereum.ABI.Model;
using Newtonsoft.Json;
using System.Text;
using System.Threading.Tasks;
using System;
using UnityEngine.Networking;
using OktoSDK.BFF;
using UserOpNamespace = OktoSDK.UserOp;
using NetworkData = OktoSDK.BFF.NetworkData;
using OktoSDK.Auth;

namespace OktoSDK
{
    public static class TransactionConstants
    {
        private static NetworkData _currentChain;
        private static string _feePayerAddress;

        public static string FeePayerAddress
        {
            get
            {
                if (string.IsNullOrEmpty(_feePayerAddress))
                {
                    _feePayerAddress = Constants.FEE_PAYER_ADDRESS;
                }
                return _feePayerAddress;
            }
            set => _feePayerAddress = value;
        }

        public static NetworkData CurrentChain
        {
            get => _currentChain;
            set
            {
                if (value != null)
                {
                    _currentChain = new NetworkData
                    {
                        caipId = value.caipId,
                        networkName = value.networkName,
                        chainId = value.chainId,
                        logo = value.logo,
                        sponsorshipEnabled = value.sponsorshipEnabled,
                        gsnEnabled = value.gsnEnabled,
                        type = value.type,
                        networkId = value.networkId,
                        onRampEnabled = value.onRampEnabled,
                        whitelisted = value.whitelisted
                    };
                }
                else
                {
                    _currentChain = null;
                }
            }
        }

        public static readonly Parameter[] INTENT_ABI = new[]
        {
            new Parameter("uint256", "_jobId"),
            new Parameter("address", "_clientSWA"),
            new Parameter("address", "_userSWA"),
            new Parameter("address", "_feePayerAddress"),
            new Parameter("bytes", "_policyInfo"),
            new Parameter("bytes", "_gsnData"),
            new Parameter("bytes", "_jobParameters"),
            new Parameter("string", "_intentType")
        };

        public static async Task<UserOpNamespace.JsonRpcResponse<UserOpNamespace.UserOperationGasPriceResult>> GetUserOperationGasPriceAsync()
        {
            //try
            //{
                string authToken = await OktoAuthManager.GetOktoClient().GetAuthorizationToken();
                string gatewayUrl = OktoAuthManager.GetOktoClient().Env.GatewayBaseUrl + "/rpc";

                var request = new JsonRpcRequest
                {
                    method = "getUserOperationGasPrice",
                    jsonrpc = "2.0",
                    id = Guid.NewGuid().ToString(),
                    @params = new object[] { }
                };

                string jsonBody = JsonConvert.SerializeObject(request, new JsonSerializerSettings
                {
                    Formatting = Formatting.None,
                    NullValueHandling = NullValueHandling.Ignore
                });

                // Log the cURL command
                string curl = $"curl -X POST \"{gatewayUrl}\" " +
                              $"-H \"Accept: application/json, text/plain, */*\" " +
                              $"-H \"Content-Type: application/json\" " +
                              $"-H \"Authorization: Bearer {authToken}\" " +
                              $"--data '{jsonBody.Replace("'", "\\'")}'";
                CustomLogger.Log("[cURL]\n" + curl);

                using (UnityWebRequest webRequest = new UnityWebRequest(gatewayUrl, "POST"))
                {
                    webRequest.SetRequestHeader("Accept", "application/json, text/plain, */*");
                    webRequest.SetRequestHeader("Content-Type", "application/json");
                    webRequest.SetRequestHeader("Authorization", $"Bearer {authToken}");
                    webRequest.uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(jsonBody));
                    webRequest.downloadHandler = new DownloadHandlerBuffer();
                    webRequest.timeout = 60;

                    CustomLogger.Log($"Sending request: {jsonBody}");

                    await webRequest.SendWebRequest();

                    string responseText = webRequest.downloadHandler.text;
                    CustomLogger.Log($"Response: {responseText}");

                    return JsonConvert.DeserializeObject<UserOpNamespace.JsonRpcResponse<UserOpNamespace.UserOperationGasPriceResult>>(responseText);
                }
            //}
            //catch (Exception ex)
            //{
            //    CustomLogger.LogError($"Error fetching gas price: {ex.Message}");
            //    throw;
            //}
        }

    }
}
