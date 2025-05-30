using System;
using System.Numerics;
using System.Threading.Tasks;
using UnityEngine;
using Newtonsoft.Json;
using System.Linq;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Collections.Generic;
using System.Text;
using Org.BouncyCastle.Crypto.Digests;
using System.Text.RegularExpressions;

//utility to read contract for configured chains in ChainUrls
namespace OktoSDK.Features.SmartContract
{
    public class ReadContract : MonoBehaviour
    {
        public TMP_InputField abiInput;
        public TMP_InputField contractAddressInput;
        public TMP_InputField functionNameInput;
        public TMP_InputField functionParametersInput;
        public TMP_Dropdown networkDropdown;
        public Button closeBtn;
        public Button readContractBtn;
        public GameObject readContractPanel;

        public ChainUrls chainUrl;

        private void OnEnable()
        {
            if(closeBtn!= null)
                closeBtn.onClick.AddListener(ClosePanel);

            if (readContractBtn != null)
                readContractBtn.onClick.AddListener(OpenPanel);
        }

        private void OnDisable()
        {
            if (closeBtn != null)
                closeBtn.onClick.RemoveListener(ClosePanel);

            if (readContractBtn != null)
                readContractBtn.onClick.RemoveListener(OpenPanel);
        }

        void OpenPanel()
        {
            ClearFields();
            readContractPanel.SetActive(true);
        }

        void ClearFields()
        {
            abiInput.text = string.Empty;
            contractAddressInput.text = string.Empty;
            functionNameInput.text = string.Empty;
            functionParametersInput.text = string.Empty;
        }

        void ClosePanel()
        {
            ClearFields();
            readContractPanel.SetActive(false);
        }

        private byte[] Keccak256(byte[] value)
        {
            var digest = new KeccakDigest(256);
            var output = new byte[digest.GetDigestSize()];
            digest.BlockUpdate(value, 0, value.Length);
            digest.DoFinal(output, 0);
            return output;
        }

        private string CalculateFunctionSelector(string functionSignature)
        {
            byte[] functionBytes = Encoding.UTF8.GetBytes(functionSignature);
            byte[] hashBytes = Keccak256(functionBytes);
            return "0x" + BitConverter.ToString(hashBytes.Take(4).ToArray()).Replace("-", "").ToLower();
        }

        public async void OnReadSmartContract()
        {
            try
            {
                Loader.ShowLoader();
                string abi = abiInput.text.Trim();
                string functionName = functionNameInput.text.Trim();
                string contractAddress = contractAddressInput.text.Trim();
                string parameters = functionParametersInput.text.Trim();
                if (string.IsNullOrEmpty(abi) || string.IsNullOrEmpty(functionName) || string.IsNullOrEmpty(contractAddress))
                {
                    ResponsePanel.SetResponse("Error: ABI, function name, and contract address must be filled.");
                    return;
                }
                try
                {
                    // Attempt to parse the ABI to check its validity
                    abi = JsonConvert.SerializeObject(JsonConvert.DeserializeObject(abi), Formatting.None);
                    functionName = CleanInput(functionName);
                    contractAddress = CleanInput(contractAddress);
                    parameters = CleanInput(parameters);
                }
                catch (Exception ex)
                {
                    throw new Exception($"Error parsing ABI: {ex.Message}");
                    //return;
                }
                string result = await ReadSmartContractAsync(networkDropdown.options[networkDropdown.value].text,abi, functionName, contractAddress, parameters);
                UpdateResultText(result);
            }
            catch(Exception ex)
            {
                UpdateResultText(ex.Message);

            }

        }

        private string CleanInput(string input)
        {
            return Regex.Replace(input, @"[\n\r\\\"" ]", "").Trim();
        }

        public async Task<string> ReadSmartContractAsync(string selectedNetwork,string abi, string functionName, string contractAddress, string parameters = "")
        {
            try
            {
                string network = selectedNetwork;
                CustomLogger.Log($"Network: {network}");

                // Check if URL is configured before using
                if (chainUrl.IsChainUrlConfigured(network))
                {
                    string url = chainUrl.GetChainUrl(network);
                    var abiArray = JsonConvert.DeserializeObject<AbiItem[]>(abi);
                    var functionAbi = abiArray.FirstOrDefault(x => x.name == functionName);

                    if (functionAbi == null)
                    {
                        throw new Exception($"Error: Function {functionName} not found in ABI");
                        //return;
                    }

                    string functionSignature = $"{functionName}({string.Join(",", functionAbi.inputs.Select(x => x.type))})";
                    CustomLogger.Log($"Function Signature: {functionSignature}");

                    string functionSelector = CalculateFunctionSelector(functionSignature);
                    CustomLogger.Log($"Function Selector: {functionSelector}");

                    object[] parameterValues = ParseParameters(parameters, functionAbi);
                    string encodedParams = EncodeParameters(functionAbi.inputs, parameterValues);
                    string data = functionSelector + encodedParams;
                    CustomLogger.Log($"Encoded Data: {data}");

                    var requestData = new
                    {
                        jsonrpc = "2.0",
                        method = "eth_call",
                        @params = new object[]
                        {
                        new
                        {
                            to = contractAddress,
                            data = data,
                            gas = "0x500000",
                            value = "0x0"
                        },
                        "latest"
                        },
                        id = 1
                    };

                    string jsonRequest = JsonConvert.SerializeObject(requestData);
                    CustomLogger.Log($"Request JSON: {jsonRequest}");

                    using (UnityWebRequest request = new UnityWebRequest(url, "POST"))
                    {
                        byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonRequest);
                        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
                        request.downloadHandler = new DownloadHandlerBuffer();
                        request.SetRequestHeader("Content-Type", "application/json");

                        var operation = request.SendWebRequest();
                        while (!operation.isDone)
                        {
                            await Task.Yield();
                        }

                        if (request.result == UnityWebRequest.Result.Success)
                        {
                            string response = request.downloadHandler.text;
                            CustomLogger.Log($"Raw Response: {response}");

                            var responseObj = JsonConvert.DeserializeObject<JsonRpcResponse>(response);

                            if (responseObj.error != null)
                            {
                                CustomLogger.Log($"Error Details: {JsonConvert.SerializeObject(responseObj.error)}");
                                throw new Exception($"RPC Error: {responseObj.error.message}");
                                CustomLogger.Log($"Error Details: {JsonConvert.SerializeObject(responseObj.error)}");
                                //return;
                            }

                            if (string.IsNullOrEmpty(responseObj.result))
                            {
                                throw new Exception("Error: Empty response from contract");
                                //return;
                            }

                            try
                            {
                                string decodedResult = DecodeResponse(responseObj.result, functionAbi.outputs[0].type);
                                string result = ($"{functionName} : {decodedResult}");
                                return result;
                            }
                            catch (Exception decodeEx)
                            {
                                CustomLogger.Log($"Raw Result: {responseObj.result}");
                                CustomLogger.Log($"Decode Error: {decodeEx}");
                                throw new Exception($"Error decoding response: {decodeEx.Message}");
                            }
                        }
                        else
                        {
                            CustomLogger.Log($"Request Error Details: {request.error}");
                            throw new Exception($"Web Request Error: {request.error}");
                        }
                    }
                }
                else
                {
                    throw new Exception("Please configure Ethereum RPC URL in the Inspector");
                }
            }
            catch (Exception e)
            {
                CustomLogger.Log($"Exception Details: {e}");
                throw new Exception($"Error: {e.Message}");
            }
        }

        private object[] ParseParameters(string parametersString, AbiItem functionAbi)
        {
            if (string.IsNullOrWhiteSpace(parametersString))
            {
                if (functionAbi.inputs.Count > 0)
                {
                    throw new Exception($"Error: Function requires {functionAbi.inputs.Count} parameters, but none were provided");
                    //return new object[0];
                }
                return new object[0];
            }

            string[] paramParts = SplitParameterString(parametersString);

            if (paramParts.Length != functionAbi.inputs.Count)
            {
                throw new Exception($"Error: Function requires {functionAbi.inputs.Count} parameters, but got {paramParts.Length}");
                //return new object[0];
            }

            object[] result = new object[paramParts.Length];

            for (int i = 0; i < paramParts.Length; i++)
            {
                string paramValue = paramParts[i].Trim();
                string paramType = functionAbi.inputs[i].type;

                try
                {
                    result[i] = ConvertParameter(paramValue, paramType);
                }
                catch (Exception ex)
                {
                    throw new Exception($"Error parsing parameter {i}: {ex.Message}");
                    //return new object[0];
                }
            }

            return result;
        }

        private string[] SplitParameterString(string parametersString)
        {
            if (string.IsNullOrEmpty(parametersString))
                return new string[0];

            var result = new List<string>();
            bool inQuotes = false;
            int startIndex = 0;

            for (int i = 0; i < parametersString.Length; i++)
            {
                if (parametersString[i] == '"')
                    inQuotes = !inQuotes;
                else if (parametersString[i] == ',' && !inQuotes)
                {
                    result.Add(parametersString.Substring(startIndex, i - startIndex));
                    startIndex = i + 1;
                }
            }

            if (startIndex < parametersString.Length)
                result.Add(parametersString.Substring(startIndex));

            return result.Select(s => s.Trim()).ToArray();
        }

        private string EncodeParameters(List<AbiParameter> parameters, object[] values)
        {
            StringBuilder encoded = new StringBuilder();

            for (int i = 0; i < parameters.Count && i < values.Length; i++)
            {
                string value = values[i].ToString();
                string type = parameters[i].type;

                switch (type)
                {
                    case "address":
                        value = value.Replace("0x", "").PadLeft(64, '0');
                        encoded.Append(value);
                        break;
                    case "uint256":
                    case "uint8":
                    case "uint":
                        BigInteger bigInt;
                        if (BigInteger.TryParse(value, out bigInt))
                        {
                            encoded.Append(bigInt.ToString("X").PadLeft(64, '0'));
                        }
                        break;
                    case "bool":
                        encoded.Append((value.ToLower() == "true" ? "1" : "0").PadLeft(64, '0'));
                        break;
                    case "string":
                        byte[] stringBytes = Encoding.UTF8.GetBytes(value);
                        encoded.Append(stringBytes.Length.ToString("X").PadLeft(64, '0'));
                        string hexString = BitConverter.ToString(stringBytes).Replace("-", "");
                        encoded.Append(hexString.PadRight(((hexString.Length + 63) / 64) * 64, '0'));
                        break;
                    case "bytes32":
                        if (value.StartsWith("0x")) value = value.Substring(2);
                        encoded.Append(value.PadRight(64, '0'));
                        break;
                    default:
                       throw new ArgumentException($"Unsupported parameter type!");

                }
            }

            return encoded.ToString();
        }

        private string DecodeResponse(string hexResult, string outputType)
        {
            if (string.IsNullOrEmpty(hexResult) || hexResult == "0x")
                return "null";

            hexResult = hexResult.StartsWith("0x") ? hexResult.Substring(2) : hexResult;

            try
            {
                switch (outputType)
                {
                    case "uint256":
                    case "uint8":
                    case "uint":
                        return BigInteger.Parse(hexResult, System.Globalization.NumberStyles.HexNumber).ToString();

                    case "address":
                        return "0x" + hexResult.Substring(24);

                    case "bool":
                        return (hexResult.Substring(63) == "1").ToString().ToLower();

                    case "string":
                        // First 32 bytes contain the offset
                        int offset = Convert.ToInt32(hexResult.Substring(0, 64), 16);
                        // Next 32 bytes at the offset contain the string length
                        int strLength = Convert.ToInt32(hexResult.Substring(offset * 2, 64), 16);
                        // The actual string data follows
                        string strHex = hexResult.Substring(offset * 2 + 64, strLength * 2);
                        byte[] bytes = StringToByteArray(strHex);
                        return Encoding.UTF8.GetString(bytes);

                    case "bytes32":
                        return "0x" + hexResult;

                    default:
                        throw new ArgumentException($"Unsupported output type!");
                }
            }
            catch (Exception ex)
            {
                CustomLogger.Log($"Raw hex: {hexResult}");
                throw new Exception($"Error decoding {outputType}: {ex.Message}");
                //CustomLogger.Log($"Raw hex: {hexResult}");
                //return hexResult;
            }
        }

        private byte[] StringToByteArray(string hex)
        {
            int numberChars = hex.Length;
            byte[] bytes = new byte[numberChars / 2];
            for (int i = 0; i < numberChars; i += 2)
            {
                bytes[i / 2] = Convert.ToByte(hex.Substring(i, 2), 16);
            }
            return bytes;
        }

        private object ConvertParameter(string paramValue, string paramType)
        {
            if (paramValue.StartsWith("\"") && paramValue.EndsWith("\""))
                paramValue = paramValue.Substring(1, paramValue.Length - 2);

            try
            {
                switch (paramType)
                {
                    case "address":
                        if (!paramValue.StartsWith("0x") || paramValue.Length != 42)
                        {
                            throw new Exception($"Invalid Ethereum address format: {paramValue}");
                        }
                        return paramValue;

                    case "uint256":
                    case "uint8":
                    case "uint16":
                    case "uint32":
                    case "uint64":
                    case "uint128":
                    case "int256":
                    case "int8":
                    case "int16":
                    case "int32":
                    case "int64":
                    case "int128":
                    case "uint":
                    case "int":
                        return BigInteger.Parse(paramValue);

                    case "bool":
                        return bool.Parse(paramValue.ToLower());

                    case "string":
                        return paramValue;

                    case "bytes32":
                        if (paramValue.StartsWith("0x"))
                        {
                            paramValue = paramValue.Substring(2);
                            byte[] bytes = new byte[paramValue.Length / 2];
                            for (int i = 0; i < bytes.Length; i++)
                            {
                                bytes[i] = Convert.ToByte(paramValue.Substring(i * 2, 2), 16);
                            }
                            return bytes;
                        }
                        return paramValue;

                    default:
                        return paramValue;
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error encoding abi value. Type: '{paramType}', Value: '{paramValue}'. Details: {ex.Message}");
            }
        }

        private void UpdateResultText(string message)
        {
            ResponsePanel.SetResponse(message);
            CustomLogger.Log(message);
        }

        private class AbiItem
        {
            public string name { get; set; }
            public string type { get; set; }
            public List<AbiParameter> inputs { get; set; } = new List<AbiParameter>();
            public List<AbiParameter> outputs { get; set; } = new List<AbiParameter>();
            public bool constant { get; set; }
            public bool payable { get; set; }
            public string stateMutability { get; set; }
        }

        private class AbiParameter
        {
            public string name { get; set; }
            public string type { get; set; }
        }

        private class JsonRpcResponse
        {
            public string jsonrpc { get; set; }
            public string result { get; set; }
            public JsonRpcError error { get; set; }
            public int id { get; set; }
        }

        private class JsonRpcError
        {
            public int code { get; set; }
            public string message { get; set; }
            public string data { get; set; }
        }
    }
}


