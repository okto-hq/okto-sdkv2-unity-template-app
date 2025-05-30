using System;
using System.Linq;
using Nethereum.ABI.FunctionEncoding;
using Nethereum.ABI.Model;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Text.RegularExpressions;

namespace OktoSDK.Features.SmartContract
{
    public static class DecodingCore
    {
        public class DecodedCallData
        {
            public bool IsSuccessful { get; set; }
            public string FunctionName { get; set; }
            public Dictionary<string, object> Parameters { get; set; }
            public string ErrorMessage { get; set; }

            public DecodedCallData()
            {
                Parameters = new Dictionary<string, object>();
                IsSuccessful = false;
                FunctionName = string.Empty;
                ErrorMessage = string.Empty;
            }

            public override string ToString()
            {
                if (!IsSuccessful)
                    return $"Decoding Failed: {ErrorMessage}";

                string result = $"Function: {FunctionName}\nParameters:";
                foreach (var param in Parameters)
                {
                    result += $"\n  {param.Key}: {param.Value}";
                }
                return result;
            }
        }

        public static DecodedCallData DecodeCallDataDirectWrapper(string callData, string abiJson)
        {
            var result = new DecodedCallData();
            
            try
            {
                // Parse the ABI to find matching function
                var abiArray = JArray.Parse(abiJson);
                
                // Extract function signature from calldata (first 4 bytes)
                if (callData.Length < 10)
                {
                    result.ErrorMessage = "Calldata too short - must be at least 10 characters (0x + 8 hex chars)";
                    return result;
                }
                
                string functionSignature = callData.Substring(0, 10);
                
                // Find matching function in ABI
                JToken matchingFunction = null;
                foreach (var func in abiArray)
                {
                    if (func["type"]?.ToString() == "function")
                    {
                        string name = func["name"]?.ToString();
                        if (string.IsNullOrEmpty(name)) continue;
                        
                        var inputParams = new List<Parameter>();
                        foreach (var input in func["inputs"])
                        {
                            inputParams.Add(new Parameter(
                                input["type"].ToString(),
                                input["name"]?.ToString() ?? string.Empty));
                        }
                        
                        var function = new FunctionABI(name, false, false) { InputParameters = inputParams.ToArray() };
                        string calculatedSignature = function.Sha3Signature;
                        
                        // Check if this function matches the calldata signature
                        if (callData.StartsWith("0x" + calculatedSignature))
                        {
                            matchingFunction = func;
                            break;
                        }
                    }
                }
                
                if (matchingFunction == null)
                {
                    result.ErrorMessage = "No matching function found in ABI for the provided calldata";
                    return result;
                }
                
                // We've found the matching function, so decode using that function's ABI
                string functionName = matchingFunction["name"].ToString();
                result.FunctionName = functionName;
                
                // Use DecodeFunctionInput with the matching function's ABI
                var functionInputs = matchingFunction["inputs"]
                    .Select(input => new Parameter(
                        input["type"].ToString(),
                        input["name"]?.ToString() ?? string.Empty))
                    .ToArray();
                
                var functionAbi = new FunctionABI(functionName, false, false) { InputParameters = functionInputs };
                var decoder = new FunctionCallDecoder();
                
                // Extract just the parameter data (remove function selector)
                string inputData = callData;
                if (callData.StartsWith("0x") && callData.Length >= 10)
                {
                    inputData = "0x" + callData.Substring(10); // Remove function selector but keep 0x prefix
                }
                
                // Use DecodeDefaultData to directly decode the parameters
                CustomLogger.Log($"Decoding parameter data: {inputData}");
                for (int i = 0; i < functionAbi.InputParameters.Length; i++)
                {
                    CustomLogger.Log($"Parameter {i}: {functionAbi.InputParameters[i].Name} ({functionAbi.InputParameters[i].Type})");
                }
                
                var parameterValues = decoder.DecodeDefaultData(
                    inputData,
                    functionAbi.InputParameters
                );
                
                CustomLogger.Log($"Decoded {parameterValues?.Count ?? 0} parameter values");
                
                if (parameterValues == null || parameterValues.Count == 0)
                {
                    result.ErrorMessage = "No parameters decoded";
                    return result;
                }

                // Store decoded parameters
                for (int i = 0; i < functionAbi.InputParameters.Length; i++)
                {
                    var param = functionAbi.InputParameters[i];
                    object value = parameterValues[i]?.Result;
                    result.Parameters.Add(param.Name, value);
                }
                
                result.IsSuccessful = true;
                return result;
            }
            catch (Exception ex)
            {
                result.ErrorMessage = $"Error decoding calldata: {ex.Message}";
                return result;
            }
        }

        public static string DecodeFunctionInputAsString(string abiJson, string functionName, string data)
        {
            try
            {
                if (string.IsNullOrEmpty(abiJson) || string.IsNullOrEmpty(functionName) || string.IsNullOrEmpty(data))
                {
                    throw new ArgumentException("ABI JSON, function name, and data are required.");
                }
                
                FunctionABI functionAbi = GetFunctionAbiDefinition(abiJson, functionName);
                if (functionAbi == null)
                {
                    throw new Exception($"Function '{functionName}' not found in ABI.");
                }
                
                if (functionAbi.InputParameters == null || functionAbi.InputParameters.Length == 0)
                {
                    return "Function has no input parameters.";
                }

                var decoder = new FunctionCallDecoder();
                
                // Extract just the parameter data (remove function selector)
                string inputData = data;
                if (data.StartsWith("0x") && data.Length >= 10)
                {
                    inputData = "0x" + data.Substring(10); // Remove function selector but keep 0x prefix
                }
                
                // Use DecodeDefaultData to directly decode the parameters
                CustomLogger.Log($"Decoding parameter data: {inputData}");
                for (int i = 0; i < functionAbi.InputParameters.Length; i++)
                {
                    CustomLogger.Log($"Parameter {i}: {functionAbi.InputParameters[i].Name} ({functionAbi.InputParameters[i].Type})");
                }
                
                var parameterValues = decoder.DecodeDefaultData(
                    inputData,
                    functionAbi.InputParameters
                );
                
                CustomLogger.Log($"Decoded {parameterValues?.Count ?? 0} parameter values");
                
                if (parameterValues == null || parameterValues.Count == 0)
                {
                    return "No parameters decoded";
                }

                List<string> formattedParams = new List<string>();
                for (int i = 0; i < functionAbi.InputParameters.Length && i < parameterValues.Count; i++)
                {
                    var param = functionAbi.InputParameters[i];
                    
                    // Ensure we access the actual value result, not just the parameter output object
                    object value = parameterValues[i]?.Result; 
                    
                    // Log the actual type and value for debugging
                    if (value != null)
                    {
                        CustomLogger.Log($"Parameter value type: {value.GetType().Name}, Value: {value}");
                    }
                    
                    formattedParams.Add($"{param.Name} ({param.Type}): {value}");
                }

                return string.Join("\n", formattedParams);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error decoding data for '{functionName}': {ex.Message}", ex);
            }
        }
        
        public static DecodedCallData DecodeFunctionInput(string abiJson, string functionName, string data)
        {
            var result = new DecodedCallData();
            result.FunctionName = functionName;
            
            try
            {
                if (string.IsNullOrEmpty(abiJson) || string.IsNullOrEmpty(functionName) || string.IsNullOrEmpty(data))
                {
                    return CreateErrorResult("ABI JSON, function name, and data are required.");
                }
                
                FunctionABI functionAbi = GetFunctionAbiDefinition(abiJson, functionName);
                if (functionAbi == null)
                {
                    return CreateErrorResult($"Function '{functionName}' not found in ABI.");
                }
                
                if (functionAbi.InputParameters == null || functionAbi.InputParameters.Length == 0)
                {
                    result.IsSuccessful = true;
                    result.Parameters = new Dictionary<string, object>();
                    return result;
                }

                var decoder = new FunctionCallDecoder();
                
                // Extract just the parameter data (remove function selector)
                string inputData = data;
                if (data.StartsWith("0x") && data.Length >= 10)
                {
                    inputData = "0x" + data.Substring(10); // Remove function selector but keep 0x prefix
                }
                
                CustomLogger.Log($"Decoding parameter data: {inputData}");
                var parameterValues = decoder.DecodeDefaultData(
                    inputData,
                    functionAbi.InputParameters
                );
                
                if (parameterValues == null || parameterValues.Count == 0)
                {
                    return CreateErrorResult("No parameters decoded");
                }

                // Store decoded parameters
                for (int i = 0; i < functionAbi.InputParameters.Length && i < parameterValues.Count; i++)
                {
                    var param = functionAbi.InputParameters[i];
                    object value = parameterValues[i]?.Result;
                    
                    // Log the actual value for debugging
                    if (value != null)
                    {
                        CustomLogger.Log($"Parameter {param.Name} value: {value} (Type: {value.GetType().Name})");
                    }
                    else
                    {
                        CustomLogger.Log($"Parameter {param.Name} value is null");
                    }
                    
                    result.Parameters.Add(param.Name, value);
                }
                
                result.IsSuccessful = true;
                return result;
            }
            catch (Exception ex)
            {
                return CreateErrorResult($"Error decoding data for '{functionName}': {ex.Message}");
            }
        }

        public static DecodedCallData DecodeFunctionData(string abiJson, string data)
        {
            var result = new DecodedCallData();
            
            try
            {
                if (string.IsNullOrEmpty(abiJson) || string.IsNullOrEmpty(data))
                {
                    result.ErrorMessage = "ABI JSON and data are required";
                    return result;
                }
                
                // Ensure data is properly formatted
                if (!data.StartsWith("0x") || data.Length < 10)
                {
                    result.ErrorMessage = "Invalid function data format, must start with 0x and be at least 10 characters";
                    return result;
                }
                
                // Extract the function selector (first 4 bytes/8 hex chars)
                string functionSelector = data.Substring(0, 10);
                
                // Parse ABI to find the matching function
                var abiArray = JArray.Parse(abiJson);
                JToken matchingFunction = null;
                
                foreach (var func in abiArray)
                {
                    if (func["type"]?.ToString() == "function")
                    {
                        string name = func["name"]?.ToString();
                        if (string.IsNullOrEmpty(name)) continue;
                        
                        // Build input parameter array for signature calculation
                        var inputParams = new List<Parameter>();
                        foreach (var input in func["inputs"])
                        {
                            inputParams.Add(new Parameter(
                                input["type"].ToString(),
                                input["name"]?.ToString() ?? string.Empty));
                        }
                        
                        // Create function ABI object for signature calculation
                        var functionAbi = new FunctionABI(name, false, false) { InputParameters = inputParams.ToArray() };
                        
                        // Calculate the signature hash and compare with input selector
                        string signature = "0x" + functionAbi.Sha3Signature;
                        
                        if (functionSelector.Equals(signature, StringComparison.OrdinalIgnoreCase))
                        {
                            matchingFunction = func;
                            break;
                        }
                    }
                }
                
                if (matchingFunction == null)
                {
                    result.ErrorMessage = "No matching function found in ABI";
                    return result;
                }
                
                // Extract function name
                string functionName = matchingFunction["name"].ToString();
                result.FunctionName = functionName;
                CustomLogger.Log($"Detected function: {functionName}");
                
                // Prepare parameter array for decoding
                var parameters = new List<Parameter>();
                foreach (var input in matchingFunction["inputs"])
                {
                    parameters.Add(new Parameter(
                        input["type"].ToString(),
                        input["name"]?.ToString() ?? string.Empty));
                }
                
                // Extract parameter data (remove function selector)
                string paramData = "0x" + data.Substring(10);
                
                // Decode parameters
                var decoder = new FunctionCallDecoder();
                CustomLogger.Log($"Decoding parameters with data: {paramData}");
                var decodedValues = decoder.DecodeDefaultData(paramData, parameters.ToArray());
                
                // Populate result dictionary
                if (decodedValues != null)
                {
                    for (int i = 0; i < parameters.Count && i < decodedValues.Count; i++)
                    {
                        // Access the Result property to get the actual value
                        object value = decodedValues[i]?.Result;
                        string paramName = parameters[i].Name;
                        
                        // Log the type and value for debugging
                        if (value != null)
                        {
                            string typeName = value.GetType().Name;
                            CustomLogger.Log($"Parameter {i}: {paramName} = {value} (Type: {typeName})");
                        }
                        else
                        {
                            CustomLogger.Log($"Parameter {i}: {paramName} = null");
                        }
                        
                        result.Parameters.Add(paramName, value);
                    }
                }
                
                result.IsSuccessful = true;
                return result;
            }
            catch (Exception ex)
            {
                result.ErrorMessage = $"Error decoding function data: {ex.Message}";
                CustomLogger.LogError($"Decoding error: {ex.Message}");
                if (ex.InnerException != null)
                {
                    CustomLogger.LogError($"Inner exception: {ex.InnerException.Message}");
                }
                return result;
            }
        }

        private static FunctionABI GetFunctionAbiDefinition(string abiJson, string functionName)
        {
            try
            {
                var abiArray = JArray.Parse(abiJson);
                JToken functionAbiJson = null;
                
                // Manually find the first matching element instead of using FirstOrDefault
                foreach (var item in abiArray)
                {
                    if (item["name"]?.ToString() == functionName && 
                        item["type"]?.ToString() == "function")
                    {
                        functionAbiJson = item;
                        break;
                    }
                }

                if (functionAbiJson == null)
                    throw new Exception($"Function '{functionName}' not found in ABI.");

                var functionInputs = functionAbiJson["inputs"]
                    .Select(input => new Parameter(
                        input["type"].ToString(),
                        input["name"]?.ToString() ?? string.Empty))
                    .ToArray();

                bool constant = (bool)(functionAbiJson["constant"] ?? false);
                return new FunctionABI(functionName, constant, false) { InputParameters = functionInputs };
            }
            catch (Exception ex)
            {
                throw new Exception($"Error parsing ABI JSON for function '{functionName}': {ex.Message}", ex);
            }
        }

        private static DecodedCallData CreateErrorResult(string errorMessage)
        {
            return new DecodedCallData
            {
                IsSuccessful = false,
                ErrorMessage = errorMessage,
                FunctionName = string.Empty,
                Parameters = new Dictionary<string, object>()
            };
        }

        public static string CleanInput(string input)
        {
            return Regex.Replace(input, @"[\n\r\\\"" ]", "").Trim();
        }
    }
} 