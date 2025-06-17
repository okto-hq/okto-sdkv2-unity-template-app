using Newtonsoft.Json;
using System;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.Networking;
using UserOpType = OktoSDK.UserOp.UserOp;
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

                string authToken = OktoAuthManager.GetOktoClient().GetAuthorizationToken();

                //var request = new JsonRpcRequest
                //{
                //    method = "execute",
                //    jsonrpc = "2.0",
                //    id = Guid.NewGuid().ToString(),
                //    @params = new object[] { userOp }
                //};

                // Format JSON exactly like web version
                string jsonBody = JsonConvert.SerializeObject(userOp, new JsonSerializerSettings
                {
                    Formatting = Formatting.None,
                    NullValueHandling = NullValueHandling.Ignore
                });

                using (UnityWebRequest webRequest = new UnityWebRequest(OktoAuthManager.GetOktoClient().Env.BffBaseUrl + "/api/oc/v1/execute", "POST"))
                {
                    // Important: Set headers in correct order
                    webRequest.SetRequestHeader("Accept", "application/json, text/plain, */*");
                    webRequest.SetRequestHeader("Content-Type", "application/json");
                    webRequest.SetRequestHeader("Authorization", $"Bearer {authToken}");
                    byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonBody);
                    webRequest.uploadHandler = new UploadHandlerRaw(bodyRaw);
                    webRequest.downloadHandler = new DownloadHandlerBuffer();
                    webRequest.timeout = 60; // 60 seconds timeout

                    var json = JsonConvert.SerializeObject(userOp, new JsonSerializerSettings
                    {
                        NullValueHandling = NullValueHandling.Ignore
                    });


                    // Send request
                    await webRequest.SendWebRequest();

                    string responseText = webRequest.downloadHandler.text;

                    return JsonConvert.DeserializeObject<BFF.JsonRpcResponse<ExecuteResult>>(responseText);
                }
            }
            catch (Exception ex)
            {
                CustomLogger.LogError($"Error executing UserOp: {ex.Message}");
                throw;
            }

        }
    }
}
