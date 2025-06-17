using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace OktoSDK
{
    public class DefaultThemeGenerator : MonoBehaviour
    {
        [Header("Default Theme Generation")]
        private const string THEME_ASSET_PATH = "Assets/OktoSDK/Resources/DefaultOnboardingTheme.asset";

        [ContextMenu("Generate Default Theme")]
        public void GenerateDefaultTheme()
        {
#if UNITY_EDITOR
            CreateDefaultThemeAsset();
#else
    CustomLogger.LogWarning("GenerateDefaultTheme is only supported in the Unity Editor.");
#endif
        }


#if UNITY_EDITOR
        [MenuItem("OktoSDK/Generate Default Theme")]
        public static void GenerateDefaultThemeMenuItem()
        {
            CreateDefaultThemeAsset();
        }

        private static void CreateDefaultThemeAsset()
        {
            // Create the default theme with exact values from UI config
            OnboardingTheme defaultTheme = ScriptableObject.CreateInstance<OnboardingTheme>();
            
            // Set version
            defaultTheme.version = "1.0.0";
            
            // Configure appearance
            defaultTheme.appearance = new AppearanceSettings
            {
                themeName = "light",
                theme = new ThemeSettings
                {
                    buttonFontWeight = "500",
                    fontFamily = "\"Inter\", sans-serif, \"Segoe UI\", Roboto, Helvetica, Arial, sans-serif",
                    roundedSm = "0.25rem",
                    roundedMd = "0.5rem",
                    roundedLg = "0.75rem",
                    roundedXl = "1rem",
                    roundedFull = "9999px"
                }
            };
            
            // Apply theme-specific colors
            defaultTheme.appearance.ApplyThemeColors();
            
            // Configure vendor
            defaultTheme.vendor = new VendorSettings
            {
                name = "Okto wallet",
                logo = "/okto.svg"
            };
            
            // Configure login options
            defaultTheme.loginOptions = new LoginOptions
            {
                socialLogins = new System.Collections.Generic.List<LoginOption>
                {
                    new LoginOption { type = "google", position = 1 },
                    new LoginOption { type = "steam", position = 2 },
                    new LoginOption { type = "twitter", position = 3 },
                    new LoginOption { type = "apple", position = 4 },
                    new LoginOption { type = "epic_games", position = 5 },
                    new LoginOption { type = "telegram", position = 6 }
                },
                
                otpLoginOptions = new System.Collections.Generic.List<LoginOption>
                {
                    new LoginOption { type = "email", position = 1 },
                    new LoginOption { type = "whatsapp", position = 2 }
                },
                
                externalWallets = new System.Collections.Generic.List<ExternalWallet>
                {
                    new ExternalWallet 
                    { 
                        type = "metamask", 
                        position = 1,
                        metadata = new WalletMetadata
                        {
                            iconUrl = "https://coindcx.s3.amazonaws.com/static/images/metamask.png",
                            isInstalled = true,
                            deepLink = ""
                        }
                    },
                    new ExternalWallet 
                    { 
                        type = "walletconnect", 
                        position = 2,
                        metadata = new WalletMetadata
                        {
                            iconUrl = "https://coindcx.s3.amazonaws.com/static/images/metamask.png",
                            isInstalled = false,
                            deepLink = ""
                        }
                    }
                }
            };
            
            // Ensure directory exists
            string directory = System.IO.Path.GetDirectoryName(THEME_ASSET_PATH);
            if (!System.IO.Directory.Exists(directory))
            {
                System.IO.Directory.CreateDirectory(directory);
            }
            
            // Create or update the asset
            AssetDatabase.CreateAsset(defaultTheme, THEME_ASSET_PATH);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            
            // Select the created asset
            Selection.activeObject = defaultTheme;
            EditorGUIUtility.PingObject(defaultTheme);
            
            CustomLogger.Log($"Default OnboardingTheme created at: {THEME_ASSET_PATH}");
        }
#endif

        [ContextMenu("Apply Default Values to Existing Theme")]
        public void ApplyDefaultValuesToExistingTheme()
        {
#if UNITY_EDITOR
            // Find existing OnboardingTheme assets
            string[] guids = AssetDatabase.FindAssets("t:OnboardingTheme");
            
            if (guids.Length == 0)
            {
                CustomLogger.LogWarning("No OnboardingTheme assets found. Creating new default theme.");
                CreateDefaultThemeAsset();
                return;
            }
            
            // Use the first found theme or let user select
            string path = AssetDatabase.GUIDToAssetPath(guids[0]);
            OnboardingTheme existingTheme = AssetDatabase.LoadAssetAtPath<OnboardingTheme>(path);
            
            if (existingTheme != null)
            {
                ApplyDefaultValues(existingTheme);
                EditorUtility.SetDirty(existingTheme);
                AssetDatabase.SaveAssets();
                CustomLogger.Log($"Default values applied to existing theme: {path}");
            }
#endif
        }

        private static void ApplyDefaultValues(OnboardingTheme theme)
        {
            // Apply the same default values as in CreateDefaultThemeAsset
            theme.version = "1.0.0";
            
            if (theme.appearance == null) theme.appearance = new AppearanceSettings();
            theme.appearance.themeName = "light";
            
            if (theme.appearance.theme == null) theme.appearance.theme = new ThemeSettings();
            var themeSettings = theme.appearance.theme;
            
            // Set common properties that don't change between themes
            themeSettings.buttonFontWeight = "500";
            themeSettings.fontFamily = "\"Inter\", sans-serif, \"Segoe UI\", Roboto, Helvetica, Arial, sans-serif";
            themeSettings.roundedSm = "0.25rem";
            themeSettings.roundedMd = "0.5rem";
            themeSettings.roundedLg = "0.75rem";
            themeSettings.roundedXl = "1rem";
            themeSettings.roundedFull = "9999px";
            
            // Apply theme-specific colors
            theme.appearance.ApplyThemeColors();
            
            if (theme.vendor == null) theme.vendor = new VendorSettings();
            theme.vendor.name = "Okto wallet";
            theme.vendor.logo = "/okto.svg";
            
            if (theme.loginOptions == null) theme.loginOptions = new LoginOptions();
            
            // Social logins
            if (theme.loginOptions.socialLogins == null) 
                theme.loginOptions.socialLogins = new System.Collections.Generic.List<LoginOption>();
            
            theme.loginOptions.socialLogins.Clear();
            theme.loginOptions.socialLogins.AddRange(new[]
            {
                new LoginOption { type = "google", position = 1 },
                new LoginOption { type = "steam", position = 2 },
                new LoginOption { type = "twitter", position = 3 },
                new LoginOption { type = "apple", position = 4 },
                new LoginOption { type = "epic_games", position = 5 },
                new LoginOption { type = "telegram", position = 6 }
            });
            
            // OTP logins
            if (theme.loginOptions.otpLoginOptions == null) 
                theme.loginOptions.otpLoginOptions = new System.Collections.Generic.List<LoginOption>();
            
            theme.loginOptions.otpLoginOptions.Clear();
            theme.loginOptions.otpLoginOptions.AddRange(new[]
            {
                new LoginOption { type = "email", position = 1 },
                new LoginOption { type = "whatsapp", position = 2 }
            });
            
            // External wallets
            if (theme.loginOptions.externalWallets == null) 
                theme.loginOptions.externalWallets = new System.Collections.Generic.List<ExternalWallet>();
            
            theme.loginOptions.externalWallets.Clear();
            theme.loginOptions.externalWallets.AddRange(new[]
            {
                new ExternalWallet 
                { 
                    type = "metamask", 
                    position = 1,
                    metadata = new WalletMetadata
                    {
                        iconUrl = "https://coindcx.s3.amazonaws.com/static/images/metamask.png",
                        isInstalled = true,
                        deepLink = ""
                    }
                },
                new ExternalWallet 
                { 
                    type = "walletconnect", 
                    position = 2,
                    metadata = new WalletMetadata
                    {
                        iconUrl = "https://coindcx.s3.amazonaws.com/static/images/metamask.png",
                        isInstalled = false,
                        deepLink = ""
                    }
                }
            });
        }
    }
} 