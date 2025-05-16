using UnityEngine;
using OktoSDK.OnRamp;

namespace OktoSDK.OnRamp
{
    /// <summary>
    /// Simple utility to test the OktoRemoteConfig loading at runtime.
    /// Attach this to a GameObject in your scene to verify config loading.
    /// </summary>
    public class ConfigTester : MonoBehaviour
    {
        [SerializeField] private bool logAllConfigValues = false;

        void Start()
        {
            CustomLogger.Log("ConfigTester: Testing OktoRemoteConfig loading...");
            
            try
            {
                var config = OktoRemoteConfig.Instance;
                
                // Test for the required on_ramp_enabled config
                var onRampEnabled = config.GetValue("on_ramp_enabled");
                CustomLogger.Log($"ConfigTester: on_ramp_enabled = {onRampEnabled.BooleanValue} (StringValue: {onRampEnabled.StringValue})");
                
                if (logAllConfigValues)
                {
                    // Log more info if needed for debugging
                    LogConfigValues();
                }
                
                CustomLogger.Log("ConfigTester: OktoRemoteConfig test completed successfully");
            }
            catch (System.Exception ex)
            {
                CustomLogger.LogError($"ConfigTester ERROR: {ex.Message}\n{ex.StackTrace}");
            }
        }
        
        private void LogConfigValues()
        {
            CustomLogger.Log("ConfigTester: Dumping all known config keys...");
            
            // Test some common keys that should be available
            string[] keysToTest = {
                "on_ramp_enabled",
                "pay_partner_supported_countries_meta_data",
                "currency_mapping_meta_data",
                "add_funds_meta_data",
                "partner_permission_meta_data",
                "payment_methods_meta_data"
            };
            
            var config = OktoRemoteConfig.Instance;
            foreach (string key in keysToTest)
            {
                try
                {
                    var value = config.GetValue(key);
                    CustomLogger.Log($"ConfigTester: {key} = {value.StringValue.Substring(0, Mathf.Min(50, value.StringValue.Length))}...");
                }
                catch (System.Exception ex)
                {
                    CustomLogger.LogWarning($"ConfigTester: Failed to get config for key '{key}': {ex.Message}");
                }
            }
        }
    }
} 