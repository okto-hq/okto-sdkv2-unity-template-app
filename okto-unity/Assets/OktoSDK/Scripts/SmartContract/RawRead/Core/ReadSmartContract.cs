using UnityEngine;
using UnityEngine.Networking;
using TMPro;
using Newtonsoft.Json;
using System.Text;
using System.Threading.Tasks;
using System;
using OktoSDK.Auth;

namespace OktoSDK.Features.SmartContract
{
    public static class ReadSmartContract
    {
        /// <summary>
        /// Asynchronously reads smart contract data by delegating to SmartContractReader.
        /// </summary>
        /// <param name="network">The blockchain network.</param>
        /// <param name="abiInput">The ABI JSON string.</param>
        /// <param name="contractAddress">The smart contract address.</param>
        /// <param name="endpoint">The API endpoint URL.</param>
        /// <param name="authToken">The authorization token.</param>
        /// <param name="functionName">The smart contract function name (optional).</param>
        /// <param name="functionArgs">The function arguments as JSON (optional).</param>
        /// <returns>A task that returns the response string from the smart contract call.</returns>
        public static async Task<string> ReadContractAsync(string network,
            string abi,string contractAddress,
             string endpoint, string authToken, string functionArgs = null)
        {
            try
            {
                var abiJson = JsonConvert.DeserializeObject(abi);
                object argsJson = string.IsNullOrWhiteSpace(functionArgs)
                    ? new { }
                    : JsonConvert.DeserializeObject(functionArgs);

                var apiPayload = new
                {
                    caip2Id = network,
                    data = new
                    {
                        contractAddress = contractAddress,
                        abi = abiJson,
                        args = argsJson
                    }
                };

                var response = await SmartContractReader.ReadContractData(apiPayload, endpoint, authToken);

                if (string.IsNullOrWhiteSpace(response))
                    throw new Exception("Empty response from smart contract API.");

                // Check status manually
                if (response.Contains("\"status\":\"success\""))
                {
                    var success = JsonConvert.DeserializeObject<SmartContractSuccessResponse>(response);
                    return success.data != null && success.data.Length > 0 ? success.data[0] : "No data returned.";
                }
                else if (response.Contains("\"status\":\"error\""))
                {
                    var error = JsonConvert.DeserializeObject<SmartContractErrorResponse>(response);
                    throw new Exception(
                        $"Smart contract call failed: {error.error.message} " +
                        $"(Code: {error.error.code}, Trace ID: {error.error.trace_id}, Details: {error.error.details})"
                    );
                }
                else
                {
                    throw new Exception($"Unexpected response format: {response}");
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Exception in ReadSmartContractAsync: {ex.Message}", ex);
            }
        }

    }
}
