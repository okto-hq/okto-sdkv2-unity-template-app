using UnityEngine;
using TMPro;
using System;
using System.Numerics;
using Nethereum.ABI.FunctionEncoding;
using System.Linq;
using Nethereum.ABI.Model;
using Newtonsoft.Json.Linq;
using UnityEngine.UI;
using System.Text;
using System.Globalization;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Text.RegularExpressions;

//utility to abi encode

namespace OktoSDK
{
    public class AbiEncoding : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private TMP_InputField abiJsonInput;
        [SerializeField] private TMP_InputField functionNameInput;
        [SerializeField] private TMP_InputField parametersInput;
        [SerializeField] private Button encodeButton;
        [SerializeField] private Button closeButton;
        [SerializeField] private GameObject encodingPanel;
        [SerializeField] private Button encodePanelBtn;


        private readonly HashSet<string> supportedTypes = new()
        {
            "address",
            "uint256", "uint", "uint8", "uint16", "uint32", "uint64", "uint128", "uint192", "uint224",
            "int256", "int", "int8", "int16", "int32", "int64", "int128", "int192", "int224",
            "bool",
            "string",
            "bytes", "bytes1", "bytes2", "bytes3", "bytes4", "bytes8", "bytes16", "bytes20", "bytes32"
        };

        void OpenPanel()
        {
            ClearFields();
            encodingPanel.SetActive(true);
        }

        void ClearFields()
        {
            abiJsonInput.text = string.Empty;
            functionNameInput.text = string.Empty;
            parametersInput.text = string.Empty;
        }

        void ClosePanel()
        {
            ClearFields();
            encodingPanel.SetActive(false);
        }

        private void OnEnable()
        {
             encodeButton.onClick.AddListener(OnEncodeButtonClick);
             closeButton.onClick.AddListener(ClosePanel);
             encodePanelBtn.onClick.AddListener(OpenPanel);
        }

        private void OnDisable()
        {
            encodeButton.onClick.RemoveListener(OnEncodeButtonClick);
            closeButton.onClick.RemoveListener(ClosePanel);
            closeButton.onClick.RemoveListener(ClosePanel);
        }


        public void OnEncodeButtonClick()
        {
            try
            {
                string abi = abiJsonInput.text.Trim();
                string functionName = functionNameInput.text.Trim();
                string parametersText = parametersInput.text.Trim();

                if (string.IsNullOrEmpty(abi) || string.IsNullOrEmpty(functionName))
                {
                    CustomLogger.LogError("Error: ABI and function name must be provided.");
                    return;
                }

                abi = JsonConvert.SerializeObject(JsonConvert.DeserializeObject(abi), Formatting.None);
                functionName = CleanInput(functionName);
                parametersText = CleanInput(parametersText);

                object[] parameters = ParseInputParameters(parametersText);
                string encodedData = EncodeFunctionData(abi, functionName, parameters);
                CustomLogger.Log($"Encoded Data: {encodedData}");
                ResponsePanel.SetResponse($"Encoded Data: {encodedData}");
            }
            catch (Exception ex)
            {
                string errorMessage = $"Encoding error: {ex.Message}";
                CustomLogger.LogError(errorMessage);
                ResponsePanel.SetResponse(errorMessage);
            }
        }

        private string CleanInput(string input)
        {
            return Regex.Replace(input, @"[\n\r\\\"" ]", "").Trim();
        }

        private object[] ParseInputParameters(string parametersText)
        {
            if (string.IsNullOrEmpty(parametersText))
                return Array.Empty<object>();

            List<object> paramList = new List<object>();
            bool inArray = false;
            StringBuilder currentParam = new StringBuilder();
            List<string> arrayElements = new List<string>();

            for (int i = 0; i < parametersText.Length; i++)
            {
                char c = parametersText[i];

                switch (c)
                {
                    case '[':
                        if (!inArray)
                        {
                            inArray = true;
                            arrayElements.Clear();
                        }
                        break;

                    case ']':
                        if (inArray && currentParam.Length > 0)
                        {
                            arrayElements.Add(currentParam.ToString().Trim());
                            currentParam.Clear();
                        }
                        inArray = false;
                        paramList.Add(arrayElements.ToArray());
                        break;

                    case ',' when !inArray:
                        if (currentParam.Length > 0)
                        {
                            paramList.Add(currentParam.ToString().Trim());
                            currentParam.Clear();
                        }
                        break;

                    case '|' when inArray:
                        if (currentParam.Length > 0)
                        {
                            arrayElements.Add(currentParam.ToString().Trim());
                            currentParam.Clear();
                        }
                        break;

                    default:
                        currentParam.Append(c);
                        break;
                }
            }

            if (currentParam.Length > 0)
            {
                if (inArray)
                {
                    arrayElements.Add(currentParam.ToString().Trim());
                    paramList.Add(arrayElements.ToArray());
                }
                else
                {
                    paramList.Add(currentParam.ToString().Trim());
                }
            }

            return paramList.ToArray();
        }


        public string EncodeFunctionData(string abiJson, string functionName, object[] parameters)
        {
            try
            {
                FunctionABI functionAbi = GetFunctionAbiDefinition(abiJson, functionName);
                if (functionAbi?.InputParameters == null)
                    throw new Exception($"Function '{functionName}' or its inputs not found in ABI.");

                // Validate parameter count
                if (parameters.Length != functionAbi.InputParameters.Length)
                    throw new ArgumentException($"Parameter count mismatch. Expected {functionAbi.InputParameters.Length}, got {parameters.Length}");

                // Validate parameter types
                foreach (var param in functionAbi.InputParameters)
                {
                    string baseType = param.Type.Replace("[]", "");
                    if (!supportedTypes.Contains(baseType))
                        throw new ArgumentException($"Unsupported parameter type: {param.Type}. This type is not supported by the encoder.");
                }

                object[] convertedParams = new object[parameters.Length];
                for (int i = 0; i < parameters.Length; i++)
                {
                    convertedParams[i] = parameters[i] switch
                    {
                        string strValue => ParseParameter(functionAbi.InputParameters[i], strValue),
                        string[] strArray => ParseArrayParameter(functionAbi.InputParameters[i], string.Join("|", strArray)),
                        _ => parameters[i]
                    };
                }

                var encoder = new FunctionCallEncoder();
                return encoder.EncodeRequest(functionAbi.Sha3Signature, functionAbi.InputParameters, convertedParams);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error encoding function data for '{functionName}': {ex.Message}", ex);
            }
        }

        private FunctionABI GetFunctionAbiDefinition(string abiJson, string functionName)
        {
            try
            {
                var abiArray = JArray.Parse(abiJson);
                var functionAbiJson = abiArray
                    .FirstOrDefault(x => x["name"]?.ToString() == functionName &&
                                       x["type"]?.ToString() == "function");

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

        private object ParseParameter(Parameter parameter, string value)
        {
            try
            {
                if (string.IsNullOrEmpty(value) && !parameter.Type.EndsWith("[]"))
                    throw new ArgumentException($"Empty value provided for parameter '{parameter.Name}' ({parameter.Type})");

                if (parameter.Type.EndsWith("[]"))
                    return ParseArrayParameter(parameter, value);

                // Validate type is supported
                string baseType = parameter.Type.Replace("[]", "");
                if (!supportedTypes.Contains(baseType))
                    throw new ArgumentException($"Unsupported parameter type: {parameter.Type}");

                return parameter.Type switch
                {
                    // Address
                    "address" => ParseAddress(value),

                    // Unsigned integers
                    "uint256" or "uint" => ParseBigInteger(value, false),
                    "uint8" => byte.Parse(value),
                    "uint16" => ushort.Parse(value),
                    "uint32" => uint.Parse(value),
                    "uint64" => ulong.Parse(value),
                    "uint128" or "uint192" or "uint224" => ParseBigInteger(value, false),

                    // Signed integers
                    "int256" or "int" => ParseBigInteger(value, true),
                    "int8" => sbyte.Parse(value),
                    "int16" => short.Parse(value),
                    "int32" => int.Parse(value),
                    "int64" => long.Parse(value),
                    "int128" or "int192" or "int224" => ParseBigInteger(value, true),

                    // Boolean
                    "bool" => ParseBool(value),

                    // String
                    "string" => value,

                    // Bytes
                    "bytes" => ParseDynamicBytes(value),
                    "bytes1" => ParseFixedBytes(value, 1),
                    "bytes2" => ParseFixedBytes(value, 2),
                    "bytes3" => ParseFixedBytes(value, 3),
                    "bytes4" => ParseFixedBytes(value, 4),
                    "bytes8" => ParseFixedBytes(value, 8),
                    "bytes16" => ParseFixedBytes(value, 16),
                    "bytes20" => ParseFixedBytes(value, 20),
                    "bytes32" => ParseFixedBytes(value, 32),

                    _ => throw new ArgumentException($"Unsupported parameter type: {parameter.Type}")
                };
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to parse value '{value}' for parameter '{parameter.Name}' of type '{parameter.Type}': {ex.Message}", ex);
            }
        }

        private object ParseArrayParameter(Parameter parameter, string value)
        {
            string elementType = parameter.Type[..^2];
            var elementParam = new Parameter(elementType, parameter.Name);

            if (string.IsNullOrEmpty(value))
                return Array.CreateInstance(GetSystemType(elementType), 0);

            string[] elements = value.Split('|');
            Array typedArray = Array.CreateInstance(GetSystemType(elementType), elements.Length);

            for (int i = 0; i < elements.Length; i++)
            {
                object parsedValue = ParseParameter(elementParam, elements[i].Trim());
                typedArray.SetValue(parsedValue, i);
            }

            return typedArray;
        }

        private Type GetSystemType(string abiType)
        {
            if (!supportedTypes.Contains(abiType))
                throw new ArgumentException($"Unsupported ABI type: {abiType}");

            return abiType switch
            {
                "address" => typeof(string),
                "string" => typeof(string),
                "bool" => typeof(bool),
                "bytes" => typeof(byte[]),
                var t when t.StartsWith("bytes") => typeof(byte[]),
                var t when t.StartsWith("uint") => typeof(BigInteger),
                var t when t.StartsWith("int") => typeof(BigInteger),
                _ => throw new ArgumentException($"Unsupported ABI type: {abiType}")
            };
        }

        private BigInteger ParseBigInteger(string value, bool signed)
        {
            value = value.Trim();
            if (value.StartsWith("0x"))
                return BigInteger.Parse(value[2..], NumberStyles.HexNumber);
            return BigInteger.Parse(value, NumberStyles.Any);
        }

        private byte[] ParseDynamicBytes(string value)
        {
            value = value.Trim();
            if (!value.StartsWith("0x"))
                value = "0x" + value;
            return value.HexToByteArray();
        }

        private byte[] ParseFixedBytes(string value, int length)
        {
            byte[] bytes = ParseDynamicBytes(value);
            if (bytes.Length > length)
                throw new ArgumentException($"Byte array too long for bytes{length}");

            byte[] result = new byte[length];
            Array.Copy(bytes, 0, result, length - bytes.Length, bytes.Length);
            return result;
        }

        private string ParseAddress(string value)
        {
            value = value.Trim();
            if (!value.StartsWith("0x"))
                value = "0x" + value;

            if (value.Length != 42 || !value[2..].All(c => Uri.IsHexDigit(c)))
                throw new FormatException($"Invalid Ethereum address format: {value}");

            return value.ToLower();
        }

        private bool ParseBool(string value)
        {
            return value.Trim().ToLower() switch
            {
                "true" or "1" => true,
                "false" or "0" => false,
                _ => throw new FormatException($"Cannot parse '{value}' as boolean")
            };
        }
    }
}