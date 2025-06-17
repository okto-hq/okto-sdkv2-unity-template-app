using System;
using UnityEngine;
using Newtonsoft.Json;

namespace OktoSDK
{
    /// <summary>
    /// MonoBehaviour implementation of OnboardingTheme
    /// Use this component when you need a MonoBehaviour that references a theme ScriptableObject
    /// </summary>
    public class OnboardingThemeComponent : MonoBehaviour
    {
        [Header("Theme Configuration")]
        [SerializeField]
        [Tooltip("Reference to a theme ScriptableObject")]
        private OnboardingTheme themeScriptableObject;

        /// <summary>
        /// Returns the AppearanceConfig from the referenced ScriptableObject
        /// </summary>
        public AppearanceConfig Config
        {
            get
            {
                if (themeScriptableObject == null)
                {
                    CustomLogger.LogWarning("No theme ScriptableObject assigned to OnboardingThemeComponent. Using default config.");
                    return new AppearanceConfig
                    {
                        version = "1.0.0",
                        appearance = new AppearanceSettings(),
                        vendor = new VendorSettings(),
                        loginOptions = new LoginOptions()
                    };
                }
                
                return new AppearanceConfig
                {
                    version = themeScriptableObject.version,
                    appearance = themeScriptableObject.appearance,
                    vendor = themeScriptableObject.vendor,
                    loginOptions = themeScriptableObject.loginOptions
                };
            }
        }

        /// <summary>
        /// Gets a formatted configuration object suitable for the web UI
        /// </summary>
        public object GetWebUIConfig()
        {
            if (themeScriptableObject != null)
            {
                return themeScriptableObject.GetWebUIConfig();
            }

            // Fallback to a minimal default config if no theme is assigned
            var config = Config;
            return new
            {
                version = config.version,
                appearance = new
                {
                    themeName = config.appearance.themeName,
                    theme = config.appearance.theme.GetCssProperties()
                },
                vendor = new
                {
                    name = config.vendor.name,
                    logo = config.vendor.logo
                },
                loginOptions = new
                {
                    socialLogins = config.loginOptions.socialLogins,
                    otpLoginOptions = config.loginOptions.otpLoginOptions,
                    externalWallets = config.loginOptions.externalWallets
                }
            };
        }
    }
} 