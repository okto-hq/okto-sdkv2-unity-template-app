using UnityEngine;
using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO;

namespace OktoSDK.OnRamp
{
    public class OktoRemoteConfig
    {
        private static OktoRemoteConfig _instance;
        public static OktoRemoteConfig Instance => _instance ?? (_instance = new OktoRemoteConfig());

        private Dictionary<string, ConfigValue> configValues = new Dictionary<string, ConfigValue>();

        public class ConfigValue
        {
            public string StringValue { get; private set; }
            public bool BooleanValue { get; private set; }
            public double DoubleValue { get; private set; }
            public long LongValue { get; private set; }
            public JToken JsonValue { get; private set; }

            public ConfigValue(object value, string valueType)
            {
                switch (valueType)
                {
                    case "STRING":
                        StringValue = value?.ToString() ?? string.Empty;
                        break;
                    case "BOOLEAN":
                        BooleanValue = value != null && Convert.ToBoolean(value.ToString());
                        StringValue = value?.ToString() ?? string.Empty;
                        break;
                    case "NUMBER":
                        if (value != null)
                        {
                            DoubleValue = Convert.ToDouble(value.ToString());
                            LongValue = Convert.ToInt64(value.ToString());
                        }
                        StringValue = value?.ToString() ?? "0";
                        break;
                    case "JSON":
                        StringValue = value?.ToString() ?? "{}";
                        try
                        {
                            if (value != null)
                            {
                                JsonValue = JToken.Parse(value.ToString());
                            }
                            else
                            {
                                JsonValue = JToken.Parse("{}");
                            }
                        }
                        catch (Exception ex)
                        {
                            CustomLogger.LogError($"Error parsing JSON value: {ex.Message}");
                            JsonValue = JToken.Parse("{}");
                        }
                        break;
                    default:
                        StringValue = value?.ToString() ?? string.Empty;
                        break;
                }
            }
        }

        private OktoRemoteConfig()
        {
            LoadConfigFromFile();
        }

        private void LoadConfigFromFile()
        {
            try
            {
                string filePath = Path.Combine(Application.streamingAssetsPath, "okto_remote_config.json");
                
                // For editor testing, also check the Resources folder
                if (!File.Exists(filePath))
                {
                    TextAsset textAsset = Resources.Load<TextAsset>("okto_remote_config");
                    if (textAsset != null)
                    {
                        ParseConfigJson(textAsset.text);
                        return;
                    }
                    
                    CustomLogger.LogWarning("Remote config file not found at: " + filePath);
                    CreateDefaultConfig();
                    return;
                }

                string jsonContent = File.ReadAllText(filePath);
                ParseConfigJson(jsonContent);
            }
            catch (Exception ex)
            {
                CustomLogger.LogError($"Error loading remote config: {ex.Message}");
                CreateDefaultConfig();
            }
        }

        private void CreateDefaultConfig()
        {
            CustomLogger.Log("Creating default config values");
            
            // Add minimum required configs with default values
            configValues["on_ramp_enabled"] = new ConfigValue("true", "BOOLEAN");
            
            // You can add more default values as needed
            CustomLogger.Log("Default config created with required values");
        }

        private void ParseConfigJson(string jsonContent)
        {
            try
            {
                if (string.IsNullOrEmpty(jsonContent))
                {
                    CustomLogger.LogError("JSON content is null or empty");
                    CreateDefaultConfig();
                    return;
                }

                // Parse using JToken for more robust handling
                JToken rootToken = JToken.Parse(jsonContent);
                JToken parametersToken = rootToken["parameters"];
                
                if (parametersToken == null || parametersToken.Type != JTokenType.Object)
                {
                    CustomLogger.LogError("No valid 'parameters' object found in config");
                    CreateDefaultConfig();
                    return;
                }
                
                JObject parametersObj = (JObject)parametersToken;
                foreach (var property in parametersObj.Properties())
                {
                    try
                    {
                        string key = property.Name;
                        JToken paramValue = property.Value;
                        
                        if (paramValue == null || paramValue.Type != JTokenType.Object)
                        {
                            CustomLogger.LogWarning($"Parameter '{key}' is not properly formatted");
                            continue;
                        }
                        
                        JToken valueTypeToken = paramValue["valueType"];
                        if (valueTypeToken == null || valueTypeToken.Type != JTokenType.String)
                        {
                            CustomLogger.LogWarning($"Parameter '{key}' has no valid valueType");
                            continue;
                        }
                        
                        string valueType = valueTypeToken.Value<string>();
                        
                        JToken defaultValueToken = paramValue["defaultValue"];
                        if (defaultValueToken == null || defaultValueToken.Type != JTokenType.Object)
                        {
                            CustomLogger.LogWarning($"No valid defaultValue found for parameter '{key}'");
                            continue;
                        }
                        
                        JToken valueToken = defaultValueToken["value"];
                        if (valueToken == null)
                        {
                            CustomLogger.LogWarning($"No value found for parameter '{key}'");
                            continue;
                        }
                        
                        // Create ConfigValue based on the token type
                        object value;
                        if (valueToken.Type == JTokenType.String)
                        {
                            value = valueToken.Value<string>();
                        }
                        else if (valueToken.Type == JTokenType.Boolean)
                        {
                            value = valueToken.Value<bool>();
                        }
                        else if (valueToken.Type == JTokenType.Integer)
                        {
                            value = valueToken.Value<long>();
                        }
                        else if (valueToken.Type == JTokenType.Float)
                        {
                            value = valueToken.Value<double>();
                        }
                        else if (valueToken.Type == JTokenType.Array || valueToken.Type == JTokenType.Object)
                        {
                            value = valueToken.ToString();
                        }
                        else
                        {
                            value = valueToken.ToString();
                        }
                        
                        configValues[key] = new ConfigValue(value, valueType);
                    }
                    catch (Exception ex)
                    {
                        CustomLogger.LogError($"Error processing parameter '{property.Name}': {ex.Message}");
                    }
                }
                
                CustomLogger.Log($"Successfully loaded {configValues.Count} config values");
            }
            catch (Exception ex)
            {
                CustomLogger.LogError($"Error parsing config JSON: {ex.Message}\n{ex.StackTrace}");
                CreateDefaultConfig();
            }
        }

        public ConfigValue GetValue(string key)
        {
            if (configValues.TryGetValue(key, out ConfigValue value))
            {
                return value;
            }
            
            CustomLogger.LogWarning($"Config key not found: {key}");
            return new ConfigValue("", "STRING");
        }
    }
} 