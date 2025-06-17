using System;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

namespace OktoSDK
{
    [Serializable]
    public class ThemeSettings 
    {
        [Tooltip("Body background color")]
        public string bodyBackground = "#ffffff";
        
        [Tooltip("Tertiary body color")]
        public string bodyColorTertiary = "#adb5bd";
        
        [Tooltip("Accent color for buttons and highlights")]
        public string accentColor = "#5166ee";
        
        [Tooltip("Button font weight")]
        public string buttonFontWeight = "500";
        
        [Tooltip("Border color")]
        public string borderColor = "rgba(22, 22, 22, 0.12)";
        
        [Tooltip("Divider stroke color")]
        public string strokeDivider = "rgba(22, 22, 22, 0.06)";
        
        [Tooltip("Font family")]
        public string fontFamily = "\"Inter\", sans-serif, \"Segoe UI\", Roboto, Helvetica, Arial, sans-serif";
        
        [Tooltip("Small border radius")]
        public string roundedSm = "0.25rem";
        
        [Tooltip("Medium border radius")]
        public string roundedMd = "0.5rem";
        
        [Tooltip("Large border radius")]
        public string roundedLg = "0.75rem";
        
        [Tooltip("Extra large border radius")]
        public string roundedXl = "1rem";
        
        [Tooltip("Full/circular border radius")]
        public string roundedFull = "9999px";
        
        [Tooltip("Success color for confirmations")]
        public string successColor = "#28a745";
        
        [Tooltip("Warning color for alerts")]
        public string warningColor = "#ffc107";
        
        [Tooltip("Error color for failures")]
        public string errorColor = "#f75757";
        
        [Tooltip("Primary text color")]
        public string textPrimary = "#161616";
        
        [Tooltip("Secondary text color")]
        public string textSecondary = "#707070";
        
        [Tooltip("Surface background color")]
        public string backgroundSurface = "#f8f8f8";
        
        /// <summary>
        /// Apply colors based on theme type (light/dark)
        /// </summary>
        public void ApplyThemeColors(string themeName)
        {
            if (themeName == "dark")
            {
                ApplyDarkTheme();
            }
            else
            {
                ApplyLightTheme();
            }
        }
        
        private void ApplyLightTheme()
        {
            accentColor = "#5166ee";
            backgroundSurface = "#f8f8f8";
            bodyBackground = "#ffffff";
            bodyColorTertiary = "#adb5bd";
            borderColor = "rgba(22, 22, 22, 0.12)";
            strokeDivider = "rgba(22, 22, 22, 0.06)";
            successColor = "#28a745";
            textPrimary = "#161616";
            textSecondary = "#707070";
            warningColor = "#ffc107";
            errorColor = "#f75757";
        }
        
        private void ApplyDarkTheme()
        {
            accentColor = "#6c7aff";
            backgroundSurface = "#1e1e1e";
            bodyBackground = "#121212";
            bodyColorTertiary = "#6c757d";
            borderColor = "rgba(255, 255, 255, 0.12)";
            strokeDivider = "rgba(255, 255, 255, 0.06)";
            successColor = "#2dd160";
            textPrimary = "#ffffff";
            textSecondary = "#b0b0b0";
            warningColor = "#ffcc33";
            errorColor = "#ff6b6b";
        }
        
        // Get CSS style properties with proper naming
        public Dictionary<string, string> GetCssProperties()
        {
            return new Dictionary<string, string>
            {
                {"--okto-body-background", bodyBackground},
                {"--okto-body-color-tertiary", bodyColorTertiary},
                {"--okto-accent-color", accentColor},
                {"--okto-button-font-weight", buttonFontWeight},
                {"--okto-border-color", borderColor},
                {"--okto-stroke-divider", strokeDivider},
                {"--okto-font-family", fontFamily},
                {"--okto-rounded-sm", roundedSm},
                {"--okto-rounded-md", roundedMd},
                {"--okto-rounded-lg", roundedLg},
                {"--okto-rounded-xl", roundedXl},
                {"--okto-rounded-full", roundedFull},
                {"--okto-success-color", successColor},
                {"--okto-warning-color", warningColor},
                {"--okto-error-color", errorColor},
                {"--okto-text-primary", textPrimary},
                {"--okto-text-secondary", textSecondary},
                {"--okto-background-surface", backgroundSurface}
            };
        }
    }

    [Serializable]
    public class AppearanceSettings
    {
        [Tooltip("Theme name (light/dark)")]
        public string themeName = "light";
        
        [Tooltip("Theme settings")]
        public ThemeSettings theme = new ThemeSettings();
        
        /// <summary>
        /// Apply theme colors based on current themeName
        /// </summary>
        public void ApplyThemeColors()
        {
            if (theme != null)
            {
                theme.ApplyThemeColors(themeName);
            }
        }
    }

    [Serializable]
    public class VendorSettings
    {
        [Tooltip("Vendor/App name")]
        public string name = "Okto wallet";
        
        [Tooltip("URL to vendor logo")]
        public string logo = "/okto.svg";
    }

    [Serializable]
    public class LoginOption
    {
        [Tooltip("Login provider type")]
        public string type;
        
        [Tooltip("Display position")]
        public int position;
    }

    [Serializable]
    public class WalletMetadata
    {
        [Tooltip("Icon URL")]
        public string iconUrl;
        
        [Tooltip("Is wallet installed")]
        public bool isInstalled;
        
        [Tooltip("Deep link URL")]
        public string deepLink;
    }

    [Serializable]
    public class ExternalWallet
    {
        [Tooltip("Wallet type")]
        public string type;
        
        [Tooltip("Display position")]
        public int position;
        
        [Tooltip("Additional wallet metadata")]
        public WalletMetadata metadata = new WalletMetadata();
    }

    [Serializable]
    public class LoginOptions
    {
        [Tooltip("Available social login options")]
        public List<LoginOption> socialLogins = new List<LoginOption>();
        
        [Tooltip("Available OTP login options")]
        public List<LoginOption> otpLoginOptions = new List<LoginOption>();
        
        [Tooltip("Available external wallet options")]
        public List<ExternalWallet> externalWallets = new List<ExternalWallet>();
    }

    [Serializable]
    public class AppearanceConfig
    {
        public string version = "1.0.0";
        public AppearanceSettings appearance = new AppearanceSettings();
        public VendorSettings vendor = new VendorSettings { name = "Okto wallet", logo = "/okto.svg" };
        public LoginOptions loginOptions = new LoginOptions();
    }

    [CreateAssetMenu(fileName = "OnboardingTheme", menuName = "OktoSDK/Onboarding Theme")]
    public class OnboardingTheme : ScriptableObject
    {
        [Header("Theme Configuration")]
        [Tooltip("Configuration version")]
        public string version = "1.0.0";
        
        [Tooltip("Appearance settings")]
        public AppearanceSettings appearance = new AppearanceSettings();
        
        [Tooltip("Vendor settings")]
        public VendorSettings vendor = new VendorSettings { name = "Okto wallet", logo = "/okto.svg" };
        
        [Tooltip("Login options")]
        public LoginOptions loginOptions = new LoginOptions();

        // Initialize with default values
        private void OnEnable()
        {
            if (loginOptions.socialLogins.Count == 0)
            {
                InitializeDefaultOptions();
            }
        }
        
        // Called when the script is loaded or a value is changed in the Inspector
        public void OnValidate()
        {
            // Initialize default options if needed
            if (loginOptions.socialLogins.Count == 0)
            {
                InitializeDefaultOptions();
            }
        }

        private void InitializeDefaultOptions()
        {
            // Set default appearance based on config
            appearance.themeName = "light";
            appearance.theme = new ThemeSettings(); // Default values are already set in the class
            
            // Set default vendor settings
            vendor.name = "Okto wallet";
            vendor.logo = "/okto.svg";
            
            // Clear any existing options
            loginOptions.socialLogins.Clear();
            loginOptions.otpLoginOptions.Clear();
            loginOptions.externalWallets.Clear();
            
            // Social logins
            loginOptions.socialLogins.Add(new LoginOption { type = "google", position = 1 });
            loginOptions.socialLogins.Add(new LoginOption { type = "steam", position = 2 });
            loginOptions.socialLogins.Add(new LoginOption { type = "twitter", position = 3 });
            loginOptions.socialLogins.Add(new LoginOption { type = "apple", position = 4 });
            loginOptions.socialLogins.Add(new LoginOption { type = "epic_games", position = 5 });
            loginOptions.socialLogins.Add(new LoginOption { type = "telegram", position = 6 });
            
            // OTP login options
            loginOptions.otpLoginOptions.Add(new LoginOption { type = "email", position = 1 });
            loginOptions.otpLoginOptions.Add(new LoginOption { type = "whatsapp", position = 2 });
            
            // External wallets
            var metamaskWallet = new ExternalWallet { 
                type = "metamask", 
                position = 1,
                metadata = new WalletMetadata { 
                    iconUrl = "https://coindcx.s3.amazonaws.com/static/images/metamask.png",
                    isInstalled = true
                }
            };
            
            var walletConnectWallet = new ExternalWallet { 
                type = "walletconnect", 
                position = 2,
                metadata = new WalletMetadata {
                    iconUrl = "https://coindcx.s3.amazonaws.com/static/images/metamask.png",
                    isInstalled = false
                }
            };
            
            loginOptions.externalWallets.Add(metamaskWallet);
            loginOptions.externalWallets.Add(walletConnectWallet);
        }

        /// <summary>
        /// Gets a formatted configuration object suitable for the web UI
        /// </summary>
        public object GetWebUIConfig()
        {
            // Apply theme colors before returning the config
            appearance.ApplyThemeColors();
            
            return new
            {
                version = version,
                appearance = new
                {
                    themeName = appearance.themeName,
                    theme = appearance.theme.GetCssProperties()
                },
                vendor = new
                {
                    name = vendor.name,
                    logo = vendor.logo
                },
                loginOptions = new
                {
                    socialLogins = loginOptions.socialLogins,
                    otpLoginOptions = loginOptions.otpLoginOptions,
                    externalWallets = loginOptions.externalWallets
                }
            };
        }
    }
} 