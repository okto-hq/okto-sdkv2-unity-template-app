using Newtonsoft.Json;
using System;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using UserOpType = OktoSDK.UserOp.UserOp;
using OktoSDK.UserOp;
using OktoSDK.BFF;
using ExecuteResult = OktoSDK.BFF.ExecuteResult;
using OktoSDK.Auth;

/*
 * The UserOpExecute class is responsible for executing User Operations (UserOp) 
 * via an RPC request to the Okto Gateway. It includes functionalities such as:
 * 
 * 1. ExecuteUserOp:
 *    - Sends a UserOp request to the Okto Gateway.
 *    - Retrieves the authorization token.
 *    - Constructs the request payload following the JSON-RPC 2.0 specification.
 *    - Sends the request via UnityWebRequest and logs the response.
 * 
 * 2. GetCurlCommand:
 *    - Generates a cURL command for debugging and manual execution.
 *    - Formats headers and request body similar to the UnityWebRequest payload.
 * 
 * 3. LogCompletePayload:
 *    - Logs the full transaction payload including:
 *      - URL and method details.
 *      - Headers used in the request.
 *      - JSON request body with UserOp details.
 * 
 * The class ensures proper request formatting, logging, and debugging support 
 * to facilitate smooth execution of User Operations.
 */

namespace OktoSDK.Features.Transaction
{
    public class UserOpExecute
    {
        // Step 8: Execute UserOp
        public static async Task<BFF.JsonRpcResponse<ExecuteResult>> ExecuteUserOp(UserOpType userOp, string signature)
        {

            try
            {
                CustomLogger.Log($"UserOp being sent: {JsonConvert.SerializeObject(userOp, Formatting.Indented)}");

                string authToken = await OktoAuthManager.GetOktoClient().GetAuthorizationToken();
                GetCurlCommand(userOp, authToken);

                var request = new JsonRpcRequest
                {
                    method = "execute",
                    jsonrpc = "2.0",
                    id = Guid.NewGuid().ToString(),
                    @params = new object[] { userOp }
                };

                // Format JSON exactly like web version
                string jsonBody = JsonConvert.SerializeObject(request, new JsonSerializerSettings
                {
                    Formatting = Formatting.None,
                    NullValueHandling = NullValueHandling.Ignore
                });

                using (UnityWebRequest webRequest = new UnityWebRequest(OktoAuthManager.GetOktoClient().Env.GatewayBaseUrl + "/rpc", "POST"))
                {
                    // Important: Set headers in correct order
                    webRequest.SetRequestHeader("Accept", "application/json, text/plain, */*");
                    webRequest.SetRequestHeader("Content-Type", "application/json");
                    webRequest.SetRequestHeader("Authorization", $"Bearer {authToken}");
                    byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonBody);
                    webRequest.uploadHandler = new UploadHandlerRaw(bodyRaw);
                    webRequest.downloadHandler = new DownloadHandlerBuffer();
                    webRequest.timeout = 60; // 60 seconds timeout

                    var json = JsonConvert.SerializeObject(request, new JsonSerializerSettings
                    {
                        NullValueHandling = NullValueHandling.Ignore
                    });

                    CustomLogger.Log($"Sending request: {json}"); // Log the request payload

                    // Send request
                    await webRequest.SendWebRequest();

                    string responseText = webRequest.downloadHandler.text;
                    CustomLogger.Log($"Response: {responseText}");

                    return JsonConvert.DeserializeObject<BFF.JsonRpcResponse<ExecuteResult>>(responseText);
                }
            }
            catch (Exception ex)
            {
                CustomLogger.LogError($"Error executing UserOp: {ex.Message}");
                throw;
            }

        }

        public static string GetCurlCommand(UserOpType userOp, string authToken)
        {
            try
            {
                var request = new JsonRpcRequest
                {
                    method = "execute",
                    jsonrpc = "2.0",
                    id = Guid.NewGuid().ToString(),
                    @params = new object[] { userOp }
                };
                string jsonBody = JsonConvert.SerializeObject(request, Formatting.None);

                // Build cURL command
                var curlCommand = $@"curl -X POST '{OktoAuthManager.GetOktoClient().Env.GatewayBaseUrl} + ""/rpc""' \
            -H 'Accept: application/json, text/plain, */*' \
            -H 'Content-Type: application/json' \
            -H 'Authorization: Bearer {authToken}' \
            -d '{jsonBody}'";

                CustomLogger.Log("\n=== CURL COMMAND ===\n");
                CustomLogger.Log(curlCommand);
                CustomLogger.Log("\n=== END CURL COMMAND ===\n");

                return curlCommand;
            }
            catch (Exception ex)
            {
                CustomLogger.LogError($"Error generating curl command: {ex.Message}");
                throw;
            }
        }
        public static void LogCompletePayload(UserOpType userOp, string authToken)
        {
            try
            {
                CustomLogger.Log("\n=== COMPLETE TRANSACTION PAYLOAD ===\n");

                // 1. HTTP Request Details
                CustomLogger.Log("URL: " + OktoAuthManager.GetOktoClient().Env.GatewayBaseUrl + "/rpc");
                CustomLogger.Log("Method: POST");

                // 2. Headers
                CustomLogger.Log("\nHeaders:");
                CustomLogger.Log("{");
                CustomLogger.Log("  \"Accept\": \"application/json, text/plain, */*\",");
                CustomLogger.Log("  \"Content-Type\": \"application/json\",");
                CustomLogger.Log($"  \"Authorization\": \"Bearer {authToken}\"");
                CustomLogger.Log("}");

                // 3. Request Body
                var requestBody = new JsonRpcRequest
                {
                    method = "execute",
                    jsonrpc = "2.0",
                    id = Guid.NewGuid().ToString(),
                    @params = new object[] { userOp }
                };

                CustomLogger.Log("\nData:");
                CustomLogger.Log(JsonConvert.SerializeObject(requestBody, Formatting.Indented));

                CustomLogger.Log("\n=== END PAYLOAD ===\n");
            }
            catch (Exception ex)
            {
                CustomLogger.LogError($"Error logging payload: {ex.Message}");
            }
        }
    }
}
