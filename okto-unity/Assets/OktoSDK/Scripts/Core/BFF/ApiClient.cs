using System;
using System.Threading.Tasks;
using UnityEngine.Networking;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Text;
using OktoSDK.Auth;

/*
 * ApiClient Class
 * 
 * This class serves as an API client for interacting with web services using HTTP requests. 
 * It provides methods for sending GET and POST requests, handling authentication, and managing request/response data.
 *
 * Features:
 * - GET and POST request support with optional query parameters and request bodies.
 * - Uses Unity's UnityWebRequest for network communication.
 * - Automatically retrieves and includes an authorization token for authenticated requests.
 * - Logs detailed request and response data for debugging.
 * - Handles error responses gracefully by logging and throwing exceptions.
 *
 * Methods:
 * - Get<T>(string endpoint, Dictionary<string, string> queryParams = null): 
 *   Sends a GET request to the specified endpoint with optional query parameters.
 * 
 * - Post<T>(string endpoint, object data = null): 
 *   Sends a POST request to the specified endpoint with an optional request body.
 *
 * - SetupRequest(UnityWebRequest request): 
 *   Configures the request with necessary headers, including authentication.
 *
 * - SendRequest<T>(UnityWebRequest request): 
 *   Sends the request, handles errors, and deserializes the response into the specified type.
 *
 * - BuildUrl(string endpoint, Dictionary<string, string> queryParams = null): 
 *   Constructs the complete request URL with query parameters.
 *
 * Usage:
 * - Initialize with a base URL and an OktoClient instance for authentication.
 * - Use Get<T>() and Post<T>() to communicate with APIs.
 * - Ensure OktoClient is properly set up to provide authentication tokens.
 */

namespace OktoSDK.BFF
{
    public class ApiClient
    {
        private readonly string baseUrl;
        private readonly OktoClient oktoClient;

        public ApiClient(string baseUrl, OktoClient oktoClient)
        {
            this.baseUrl = baseUrl;
            this.oktoClient = oktoClient;
        }

        public async Task<T> Get<T>(string endpoint, Dictionary<string, string> queryParams = null)
        {
            string url = BuildUrl(endpoint, queryParams);
            CustomLogger.Log($"Making GET request to: {url}");

            using (UnityWebRequest request = UnityWebRequest.Get(url))
            {
                await SetupRequest(request);
                return await SendRequest<T>(request);
            }
        }

        public async Task<T> Post<T>(string endpoint, object data = null)
        {
            using (UnityWebRequest request = new UnityWebRequest(BuildUrl(endpoint), "POST"))
            {
                if (data != null)
                {
                    string jsonData = JsonConvert.SerializeObject(
                        data,
                        new JsonSerializerSettings
                        {
                            NullValueHandling = NullValueHandling.Ignore
                        }
                    );
                    byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);
                    request.uploadHandler = new UploadHandlerRaw(bodyRaw);
                    request.downloadHandler = new DownloadHandlerBuffer();
                }

                await SetupRequest(request);
                return await SendRequest<T>(request);
            }
        }

        private async Task SetupRequest(UnityWebRequest request)
        {
            try
            {
                // Set common headers
                request.SetRequestHeader("Content-Type", "application/json");

                // Get and set authorization token
                string token = await oktoClient.GetAuthorizationToken();
                if (token == null)
                {
                    ResponsePanel.SetResponse("You are not Logged In!");
                    return;
                }

                CustomLogger.Log($"Authorization Token: {token}");
                request.SetRequestHeader("Authorization", $"Bearer {token}");

                // Log headers individually since we can't get them all at once
                CustomLogger.Log("Request Headers:");
                CustomLogger.Log($"Content-Type: {request.GetRequestHeader("Content-Type")}");
                CustomLogger.Log($"Authorization: {request.GetRequestHeader("Authorization")}");

                if (request.method == "POST" && request.uploadHandler == null)
                {
                    request.uploadHandler = new UploadHandlerRaw(new byte[0]);
                }

                request.downloadHandler = new DownloadHandlerBuffer();
            }
            catch (Exception ex)
            {
                CustomLogger.LogError($"Error setting up request: {ex.Message}\nStack trace: {ex.StackTrace}");
                throw;
            }
        }

        private async Task<T> SendRequest<T>(UnityWebRequest request)
        {
            try
            {
                CustomLogger.Log($"Sending request to: {request.url}");

                // Log request details before sending
                CustomLogger.Log($"Request Method: {request.method}");
                CustomLogger.Log("Request Headers:");
                CustomLogger.Log($"Content-Type: {request.GetRequestHeader("Content-Type")}");
                CustomLogger.Log($"Authorization: {request.GetRequestHeader("Authorization")}");

                if (request.uploadHandler != null && request.uploadHandler.data != null)
                {
                    string requestBody = Encoding.UTF8.GetString(request.uploadHandler.data);
                    CustomLogger.Log($"Request Body: {requestBody}");
                }

                var json = JsonConvert.SerializeObject(request, new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore
                });

                CustomLogger.Log($"Sending request: {json}"); // Log the request payload
                                                              // Build a curl command for debugging
                var curlBuilder = new StringBuilder();
                curlBuilder.Append($"curl -X {request.method} \"{request.url}\"");

                var contentType = request.GetRequestHeader("Content-Type");
                if (!string.IsNullOrEmpty(contentType))
                    curlBuilder.Append($" -H \"Content-Type: {contentType}\"");

                var authorization = request.GetRequestHeader("Authorization");
                if (!string.IsNullOrEmpty(authorization))
                    curlBuilder.Append($" -H \"Authorization: {authorization}\"");

                if (request.uploadHandler != null && request.uploadHandler.data != null)
                {
                    string requestBody = Encoding.UTF8.GetString(request.uploadHandler.data);
                    // Escape double quotes for safe curl usage
                    string escapedBody = requestBody.Replace("\"", "\\\"");
                    curlBuilder.Append($" -d \"{escapedBody}\"");
                }

                CustomLogger.Log($"[CURL] {curlBuilder}");

                await request.SendWebRequest();

                if (request.result != UnityWebRequest.Result.Success)
                {
                    string errorMessage = $"API request failed: {request.error}";
                    if (!string.IsNullOrEmpty(request.downloadHandler.text))
                    {
                        errorMessage += $"\nResponse: {request.downloadHandler.text}";

                        // Check if the error code is 401=401 session expired
                        if (request.responseCode == 401)
                        {
                            OktoAuthManager.HandleSessionExpired();
                            throw new Exception("Session Expired,please login Again");
                        }
                    }
                    CustomLogger.Log(errorMessage);
                    throw new Exception(errorMessage);
                }

                string responseText = request.downloadHandler.text;
                CustomLogger.Log($"Response received: {responseText}");

                return JsonConvert.DeserializeObject<T>(
                    responseText,
                    new JsonSerializerSettings
                    {
                        ContractResolver = new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver()
                    }
                );
            }
            catch (Exception ex)
            {
                CustomLogger.Log($"Request failed: {ex.Message}\nStack trace: {ex.StackTrace}");
                throw;
            }
            finally
            {
                request.Dispose();
            }
        }

        private string BuildUrl(string endpoint, Dictionary<string, string> queryParams = null)
        {
            string url = baseUrl.TrimEnd('/') + "/" + endpoint.TrimStart('/');

            if (queryParams != null && queryParams.Count > 0)
            {
                url += "?";
                foreach (var param in queryParams)
                {
                    url += $"{UnityWebRequest.EscapeURL(param.Key)}={UnityWebRequest.EscapeURL(param.Value)}&";
                }
                url = url.TrimEnd('&');
            }

            return url;
        }


    }

}
