using Newtonsoft.Json;
using System;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

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

namespace OktoSDK
{
    public class UserOpExecute
    {
        // Step 8: Execute UserOp
        public static async Task<JsonRpcResponse<ExecuteResult>> ExecuteUserOp(UserOp userOp, string signature)
        {

            try
            {
                Debug.Log($"UserOp being sent: {JsonConvert.SerializeObject(userOp, Formatting.Indented)}");

                string authToken = await OktoAuthExample.getOktoClient().GetAuthorizationToken();
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

                using (UnityWebRequest webRequest = new UnityWebRequest(OktoAuthExample.getOktoClient().Env.GatewayBaseUrl + "/rpc", "POST"))
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

                    Debug.Log($"Sending request: {json}"); // Log the request payload

                    // Send request
                    await webRequest.SendWebRequest();

                    string responseText = webRequest.downloadHandler.text;
                    Debug.Log($"Response: {responseText}");

                    return JsonConvert.DeserializeObject<JsonRpcResponse<ExecuteResult>>(responseText);
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error executing UserOp: {ex.Message}");
                throw;
            }

        }

        public static string GetCurlCommand(UserOp userOp, string authToken)
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
                var curlCommand = $@"curl -X POST '{OktoAuthExample.getOktoClient().Env.GatewayBaseUrl} + ""/rpc""' \
            -H 'Accept: application/json, text/plain, */*' \
            -H 'Content-Type: application/json' \
            -H 'Authorization: Bearer {authToken}' \
            -d '{jsonBody}'";

                Debug.Log("\n=== CURL COMMAND ===\n");
                Debug.Log(curlCommand);
                Debug.Log("\n=== END CURL COMMAND ===\n");

                return curlCommand;
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error generating curl command: {ex.Message}");
                throw;
            }
        }
        public static void LogCompletePayload(UserOp userOp, string authToken)
        {
            try
            {
                Debug.Log("\n=== COMPLETE TRANSACTION PAYLOAD ===\n");

                // 1. HTTP Request Details
                Debug.Log("URL: " + OktoAuthExample.getOktoClient().Env.GatewayBaseUrl + "/rpc");
                Debug.Log("Method: POST");

                // 2. Headers
                Debug.Log("\nHeaders:");
                Debug.Log("{");
                Debug.Log("  \"Accept\": \"application/json, text/plain, */*\",");
                Debug.Log("  \"Content-Type\": \"application/json\",");
                Debug.Log($"  \"Authorization\": \"Bearer {authToken}\"");
                Debug.Log("}");

                // 3. Request Body
                var requestBody = new JsonRpcRequest
                {
                    method = "execute",
                    jsonrpc = "2.0",
                    id = Guid.NewGuid().ToString(),
                    @params = new object[] { userOp }
                };

                Debug.Log("\nData:");
                Debug.Log(JsonConvert.SerializeObject(requestBody, Formatting.Indented));

                Debug.Log("\n=== END PAYLOAD ===\n");
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error logging payload: {ex.Message}");
            }
        }
    }
}