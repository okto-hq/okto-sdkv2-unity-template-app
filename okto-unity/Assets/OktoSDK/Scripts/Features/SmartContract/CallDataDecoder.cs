using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Nethereum.ABI.FunctionEncoding;
using Nethereum.ABI.Model;
using Newtonsoft.Json.Linq;
using UnityEngine;
using OktoSDK.Features.Order;
using OktoSDK.BFF;

namespace OktoSDK.Features.SmartContract
{
    /// <summary>
    /// Decodes transaction call data based on ABI information and order details
    /// </summary>
    public class CallDataDecoder : MonoBehaviour
    {
        [SerializeField] private OrderPrefab orderPrefab;
        
        // Event that fires when call data is decoded successfully
        public event Action<DecodedCallData> OnCallDataDecoded;
        
        // Event that fires when call data decoding fails
        public event Action<string> OnDecodingFailed;

        private readonly HashSet<string> supportedTypes = new HashSet<string>
        {
            "address",
            "uint256", "uint", "uint8", "uint16", "uint32", "uint64", "uint128", "uint192", "uint224",
            "int256", "int", "int8", "int16", "int32", "int64", "int128", "int192", "int224",
            "bool",
            "string",
            "bytes", "bytes1", "bytes2", "bytes3", "bytes4", "bytes8", "bytes16", "bytes20", "bytes32"
        };
        
        /// <summary>
        /// Structure to hold decoded call data information
        /// </summary>
        [Serializable]
        public class DecodedCallData
        {
            public string FunctionName { get; set; }
            public Dictionary<string, object> Parameters { get; set; } = new Dictionary<string, object>();
            public string RawCallData { get; set; }
            public string ContractAddress { get; set; }
            public string FromAddress { get; set; }
            public string Value { get; set; }
            public string OrderType { get; set; }
            public string OrderId { get; set; }
            public bool IsSuccessful { get; set; }
            public string ErrorMessage { get; set; }

            public override string ToString()
            {
                var sb = new StringBuilder();
                sb.AppendLine($"Function: {FunctionName}");
                sb.AppendLine($"Contract: {ContractAddress}");
                sb.AppendLine($"From: {FromAddress}");
                sb.AppendLine($"Value: {Value}");
                sb.AppendLine($"Order Type: {OrderType}");
                sb.AppendLine($"Order ID: {OrderId}");
                sb.AppendLine($"Status: {(IsSuccessful ? "Success" : "Failed")}");
                
                if (!string.IsNullOrEmpty(ErrorMessage))
                {
                    sb.AppendLine($"Error: {ErrorMessage}");
                }
                
                if (Parameters != null && Parameters.Count > 0)
                {
                    sb.AppendLine("Parameters:");
                    foreach (var param in Parameters)
                    {
                        string valueStr = FormatParameterValue(param.Value);
                        sb.AppendLine($"  {param.Key}: {valueStr}");
                    }
                }
                
                return sb.ToString();
            }

            private string FormatParameterValue(object value)
            {
                if (value == null)
                    return "null";
                
                if (value is byte[] bytes)
                    return $"0x{BitConverter.ToString(bytes).Replace("-", "")}";
                
                if (value is Array array)
                {
                    var elements = new List<string>();
                    foreach (var item in array)
                    {
                        elements.Add(FormatParameterValue(item));
                    }
                    return $"[{string.Join(", ", elements)}]";
                }
                
                return value.ToString();
            }
        }
        
        /// <summary>
        /// Decodes call data from an order based on its intent ID
        /// </summary>
        /// <param name="intentId">The order intent ID</param>
        /// <param name="abiJson">The ABI JSON string for decoding</param>
        /// <returns>Task returning the decoded call data</returns>
        public async Task<DecodedCallData> DecodeCallDataFromOrderAsync(string intentId, string abiJson)
        {
            try
            {
                if (orderPrefab == null)
                {
                    string error = "OrderPrefab reference is missing. Please assign it in the inspector.";
                    CustomLogger.LogError(error);
                    OnDecodingFailed?.Invoke(error);
                    return new DecodedCallData { 
                        IsSuccessful = false, 
                        ErrorMessage = error 
                    };
                }
                
                if (string.IsNullOrEmpty(intentId))
                {
                    string error = "Intent ID cannot be empty.";
                    CustomLogger.LogError(error);
                    OnDecodingFailed?.Invoke(error);
                    return new DecodedCallData { 
                        IsSuccessful = false, 
                        ErrorMessage = error 
                    };
                }
                
                // Get order details based on intent ID
                var (order, txHash) = await orderPrefab.GetOrderDetailsByIntentId(intentId);
                
                if (order == null)
                {
                    string error = $"Order with intent ID {intentId} not found.";
                    CustomLogger.LogError(error);
                    OnDecodingFailed?.Invoke(error);
                    return new DecodedCallData { 
                        IsSuccessful = false, 
                        ErrorMessage = error 
                    };
                }
                
                // Check if order has details
                if (order.Details == null)
                {
                    string error = "Order details are missing.";
                    CustomLogger.LogError(error);
                    OnDecodingFailed?.Invoke(error);
                    return new DecodedCallData { 
                        IsSuccessful = false, 
                        ErrorMessage = error 
                    };
                }
                
                // Extract transaction data using the TransactionDataExtractor
                var transactionData = TransactionDataExtractor.ExtractTransactionData(order);
                
                string callData = transactionData.CallData;
                string contractAddress = transactionData.ContractAddress;
                string fromAddress = transactionData.FromAddress;
                string value = transactionData.Value;
                
                if (string.IsNullOrEmpty(callData) || callData == "0x")
                {
                    // This is a simple transfer without calldata
                    var decodedData = new DecodedCallData
                    {
                        FunctionName = "transfer",
                        Parameters = new Dictionary<string, object>(),
                        RawCallData = callData,
                        ContractAddress = contractAddress,
                        FromAddress = fromAddress,
                        Value = value,
                        OrderType = order.IntentType,
                        OrderId = order.IntentId,
                        IsSuccessful = true
                    };
                    
                    OnCallDataDecoded?.Invoke(decodedData);
                    return decodedData;
                }
                
                // For actual calldata, we need to decode it using the ABI
                if (string.IsNullOrEmpty(abiJson))
                {
                    string error = "ABI JSON is required to decode call data.";
                    CustomLogger.LogError(error);
                    OnDecodingFailed?.Invoke(error);
                    return new DecodedCallData { 
                        IsSuccessful = false, 
                        ErrorMessage = error,
                        RawCallData = callData 
                    };
                }
                
                // Decode the calldata
                var decodedCallData = DecodeCallData(callData, abiJson);
                decodedCallData.ContractAddress = contractAddress;
                decodedCallData.FromAddress = fromAddress;
                decodedCallData.Value = value;
                decodedCallData.OrderType = order.IntentId;
                decodedCallData.OrderId = order.IntentId;
                
                OnCallDataDecoded?.Invoke(decodedCallData);
                return decodedCallData;
            }
            catch (Exception ex)
            {
                string error = $"Error decoding call data: {ex.Message}";
                CustomLogger.LogError(error);
                OnDecodingFailed?.Invoke(error);
                return new DecodedCallData { 
                    IsSuccessful = false, 
                    ErrorMessage = error 
                };
            }
        }
        
        /// <summary>
        /// Decodes raw calldata using the provided ABI
        /// </summary>
        /// <param name="callData">The hex string calldata to decode</param>
        /// <param name="abiJson">The ABI JSON string</param>
        /// <returns>Decoded call data object</returns>
        public DecodedCallData DecodeCallData(string callData, string abiJson)
        {
            try
            {
                if (string.IsNullOrEmpty(callData) || callData == "0x")
                {
                    return new DecodedCallData
                    {
                        FunctionName = "transfer",
                        Parameters = new Dictionary<string, object>(),
                        RawCallData = callData,
                        IsSuccessful = true
                    };
                }
                
                // Parse the ABI JSON
                var abiArray = JArray.Parse(abiJson);
                
                // Extract the function signature (first 4 bytes of the calldata)
                if (callData.Length < 10)
                {
                    return new DecodedCallData
                    {
                        FunctionName = "Unknown",
                        Parameters = new Dictionary<string, object> { { "rawData", callData } },
                        RawCallData = callData,
                        IsSuccessful = false,
                        ErrorMessage = "Invalid calldata: too short"
                    };
                }
                
                string functionSignature = callData.Substring(0, 10); // "0x" + 8 characters (4 bytes)
                
                // Find the matching function in the ABI
                FunctionABI matchingFunction = null;
                foreach (var item in abiArray)
                {
                    if (item["type"]?.ToString() != "function")
                        continue;
                    
                    string functionName = item["name"]?.ToString();
                    if (string.IsNullOrEmpty(functionName))
                        continue;
                    
                    var inputs = item["inputs"] as JArray;
                    if (inputs == null)
                        continue;
                    
                    // Create a function ABI definition
                    var parameters = new List<Parameter>();
                    foreach (var input in inputs)
                    {
                        string type = input["type"]?.ToString();
                        if (string.IsNullOrEmpty(type))
                            continue;
                            
                        // Check if type is supported or is an array of supported type
                        string baseType = type.EndsWith("[]") ? type.Substring(0, type.Length - 2) : type;
                        if (!supportedTypes.Contains(baseType))
                            continue;
                            
                        parameters.Add(new Parameter(
                            type,
                            input["name"]?.ToString() ?? string.Empty
                        ));
                    }
                    
                    var functionAbi = new FunctionABI(
                        functionName, 
                        (bool)(item["constant"] ?? false), 
                        false
                    )
                    {
                        InputParameters = parameters.ToArray()
                    };
                    
                    // Check if the signature matches
                    if (functionAbi.Sha3Signature.ToLower() == functionSignature.ToLower())
                    {
                        matchingFunction = functionAbi;
                        break;
                    }
                }
                
                if (matchingFunction == null)
                {
                    // Could not find matching function
                    return new DecodedCallData
                    {
                        FunctionName = "Unknown",
                        Parameters = new Dictionary<string, object> { { "rawData", callData } },
                        RawCallData = callData,
                        IsSuccessful = false,
                        ErrorMessage = "Could not find matching function signature in ABI"
                    };
                }
                
                // Decode the parameters
                try
                {
                    var functionCallDecoder = new FunctionCallDecoder();
                    string parametersData = callData.Substring(10); // Skip the function signature
                    var parameterValues = functionCallDecoder.DecodeDefaultData(
                        parametersData,
                        matchingFunction.InputParameters
                    );
                    
                    // Create result dictionary
                    var parameters = new Dictionary<string, object>();
                    
                    for (int i = 0; i < matchingFunction.InputParameters.Length; i++)
                    {
                        var param = matchingFunction.InputParameters[i];
                        var value = parameterValues[i];
                        
                        // Add to results - use param name if available, otherwise use param index
                        parameters[string.IsNullOrEmpty(param.Name) ? $"param{i}" : param.Name] = value;
                    }
                    
                    return new DecodedCallData
                    {
                        FunctionName = matchingFunction.Name,
                        Parameters = parameters,
                        RawCallData = callData,
                        IsSuccessful = true
                    };
                }
                catch (Exception ex)
                {
                    return new DecodedCallData
                    {
                        FunctionName = matchingFunction.Name,
                        Parameters = new Dictionary<string, object>(),
                        RawCallData = callData,
                        IsSuccessful = false,
                        ErrorMessage = $"Error decoding parameters: {ex.Message}"
                    };
                }
            }
            catch (Exception ex)
            {
                CustomLogger.LogError($"Error in DecodeCallData: {ex.Message}");
                return new DecodedCallData
                {
                    FunctionName = "Unknown",
                    Parameters = new Dictionary<string, object>(),
                    RawCallData = callData,
                    IsSuccessful = false,
                    ErrorMessage = ex.Message
                };
            }
        }

        /// <summary>
        /// Directly decodes call data using the provided ABI JSON and call data
        /// </summary>
        /// <param name="callData">The call data to decode</param>
        /// <param name="abiJson">The ABI JSON for the function</param>
        /// <returns>The decoded call data</returns>
        public DecodedCallData DecodeCallDataDirectWrapper(string callData, string abiJson)
        {
            try
            {
                return DecodeCallData(callData, abiJson);
            }
            catch (Exception ex)
            {
                string error = $"Error directly decoding call data: {ex.Message}";
                CustomLogger.LogError(error);
                OnDecodingFailed?.Invoke(error);
                return new DecodedCallData { 
                    IsSuccessful = false, 
                    ErrorMessage = error,
                    RawCallData = callData
                };
            }
        }
    }
} 
