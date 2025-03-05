using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json;


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

namespace OktoSDK
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

        private class RpcResponse<T>
        {
            public string Jsonrpc { get; set; }
            public string Id { get; set; }
            public T Result { get; set; }
            public Error Error { get; set; }
        }


        public static async Task<AuthenticateResult> Authenticate(
            OktoClient client, 
            AuthenticatePayloadParam payload)
        {
            var request = new RpcRequest<AuthenticatePayloadParam>
            {
                Method = "authenticate",
                Params = new[] { payload }
            };

            var json = JsonConvert.SerializeObject(request, new JsonSerializerSettings 
            { 
                NullValueHandling = NullValueHandling.Ignore 
            });
            
            Debug.Log($"Sending request: {json}"); // Log the request payload
            RpcResponse<AuthenticateResult> response = null;

            using (var webRequest = new UnityWebRequest(client.Env.GatewayBaseUrl + "/rpc", "POST"))
            {
                webRequest.uploadHandler = new UploadHandlerRaw(
                    System.Text.Encoding.UTF8.GetBytes(json));
                webRequest.downloadHandler = new DownloadHandlerBuffer();
                webRequest.SetRequestHeader("Content-Type", "application/json");
                webRequest.SetRequestHeader("Accept", "application/json");

                await webRequest.SendWebRequest();

                var responseText = webRequest.downloadHandler.text;
                Debug.Log($"Response: {responseText}"); // Log the response

                try
                {
                    if (webRequest.result != UnityWebRequest.Result.Success)
                    {
                        var errorResponse = JsonConvert.DeserializeObject<RpcResponse<AuthenticateResult>>(responseText);
                        if (errorResponse?.Error != null)
                        {
                            Debug.Log("------webRequest.result != UnityWebRequest.Result.Success---------");

                            throw new RpcError(errorResponse.Jsonrpc, errorResponse.Id, errorResponse.Error);
                        }
                    }

                     response = JsonConvert.DeserializeObject<RpcResponse<AuthenticateResult>>(responseText);
                    if (response.Error != null)
                    {
                        Debug.Log("---------------");
                        ResponsePanel.SetResponse(responseText);

                    }
                    return response.Result;

                }

                catch (Exception e)
                {
                    ResponsePanel.SetResponse(responseText);
                }
                return response.Result;

            }
        }
    }
} 