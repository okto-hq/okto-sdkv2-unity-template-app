using System;
using System.Threading.Tasks;
using UnityEngine.Networking;
using Newtonsoft.Json;
using OktoSDK.BFF;

/*
 * GatewayClientRepository Class
 *
 * This class handles communication with the Okto SDK authentication gateway using JSON-RPC requests.
 * It sends authentication payloads and processes responses, including error handling.
 *
 * Features:
 * - Constructs and sends JSON-RPC requests.
 * - Handles authentication via the `Authenticate` method.
 * - Parses and logs server responses.
 * - Manages error handling and displays responses.
 */

namespace OktoSDK.Auth
{
    public class GatewayClientRepository
    {
        private class RpcRequest<T>
        {
            [JsonProperty("method")]
            public string Method { get; set; }

            [JsonProperty("jsonrpc")]
            public string Jsonrpc { get; set; } = "2.0";

            [JsonProperty("id")]
            public string Id { get; set; } = Guid.NewGuid().ToString("D");

            [JsonProperty("params")]
            public T[] Params { get; set; }
        }

        //private class RpcResponse<T>
        //{
        //    public string Jsonrpc { get; set; }
        //    public string Id { get; set; }
        //    public T Result { get; set; }
        //    public OktoSDK.BFF.Error Error { get; set; }
        //}

        private class RpcResponse<T>
        {
            public T data { get; set; }
            public OktoSDK.BFF.Error Error { get; set; }
        }

        public class AuthenticateResponse
        {
            public string status { get; set; }
            public AuthenticateResult data { get; set; }
        }

        public static async Task<AuthenticateResult> Authenticate(
        OktoClient client,
        AuthenticatePayloadParam payload)
        {
            //var request = new RpcRequest<AuthenticatePayloadParam>
            //{
            //    Method = "authenticate",
            //    Params = new[] { payload }
            //};

            var json = JsonConvert.SerializeObject(payload, new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            });

            CustomLogger.Log($"Sending request: {json}"); // Log the request payload
            RpcResponse<AuthenticateResult> response = null;

            using (var webRequest = new UnityWebRequest(client.Env.BffBaseUrl + "/api/oc/v1/authenticate", "POST"))
            {
                webRequest.uploadHandler = new UploadHandlerRaw(
                    System.Text.Encoding.UTF8.GetBytes(json));
                webRequest.downloadHandler = new DownloadHandlerBuffer();
                webRequest.SetRequestHeader("Content-Type", "application/json");
                webRequest.SetRequestHeader("Accept", "application/json");


                string escapedJson = json.Replace("\"", "\\\"");

                string curlCommand = $"curl -X POST \"{client.Env.BffBaseUrl}/rpc\" " +
                                     "-H \"Content-Type: application/json\" " +
                                     "-H \"Accept: application/json\" " +
                                     $"-d \"{escapedJson}\"";

                CustomLogger.Log($"Generated curl command:\n{curlCommand}");


                await webRequest.SendWebRequest();

                var responseText = webRequest.downloadHandler.text;
                CustomLogger.Log($"Response: {responseText}"); // Log the response

                try
                {
                    if (webRequest.result != UnityWebRequest.Result.Success)
                    {
                        // Try to parse error response to get details
                        var errorResponse = JsonConvert.DeserializeObject<RpcResponse<AuthenticateResult>>(responseText);
                        if (errorResponse?.Error != null)
                        {
                            throw new RpcError("", "", errorResponse.Error);
                        }
                        throw new Exception(webRequest.error);
                    }

                    response = JsonConvert.DeserializeObject<RpcResponse<AuthenticateResult>>(responseText);

                    if (response.Error != null)
                    {
                        ResponsePanel.SetResponse(responseText);
                        throw new RpcError("", "", response.Error);
                    }

                    return response.data;
                }
                catch (Exception e)
                {
                    CustomLogger.LogError($"Exception during authentication: {e}");
                    ResponsePanel.SetResponse(responseText);
                }

                return response.data;

            }
        }
    }
}
