using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Linq;

namespace OktoSDK
{
    [System.Serializable]
    public class UIThemeSettings
    {
        [Header("Colors")]
        public string bodyBackground = "#ffffff";
        public string bodyColorTertiary = "#adb5bd";
        public string accentColor = "#5166ee";
        public string borderColor = "rgba(22, 22, 22, 0.12)";
        public string strokeDivider = "rgba(22, 22, 22, 0.06)";
        public string successColor = "#28a745";
        public string warningColor = "#ffc107";
        public string errorColor = "#f75757";
        public string textPrimary = "#161616";
        public string textSecondary = "#707070";
        public string backgroundSurface = "#f8f8f8";

        [Header("Typography")]
        public string buttonFontWeight = "500";
        public string fontFamily = "'Inter', sans-serif, 'Segoe UI', Roboto, Helvetica, Arial, sans-serif";

        [Header("Border Radius")]
        public string roundedSm = "0.25rem";
        public string roundedMd = "0.5rem";
        public string roundedLg = "0.75rem";
        public string roundedXl = "1rem";
        public string roundedFull = "9999px";
    }

    [System.Serializable]
    public class UIVendorSettings
    {
        public string vendorName = "Okto wallet";
        public string logoUrl = "/okto.svg";
    }

    [System.Serializable]
    public class UILoginOption
    {
        public string type = "";
        public int position = 1;
        public bool enabled = true;
    }

    [System.Serializable]
    public class UIWalletOption
    {
        public string type = "";
        public int position = 1;
        public bool enabled = true;
        public string iconUrl = "";
        public bool isInstalled = false;
        public string deepLink = "";
    }

    [System.Serializable]
    public class ThemeUIFields
    {
        [Header("General Settings")]
        public TMP_Dropdown themeNameDropdown;

        [Header("Color Fields")]
        public TMP_InputField bodyBackgroundField;
        public TMP_InputField bodyColorTertiaryField;
        public TMP_InputField accentColorField;
        public TMP_InputField borderColorField;
        public TMP_InputField strokeDividerField;
        public TMP_InputField successColorField;
        public TMP_InputField warningColorField;
        public TMP_InputField errorColorField;
        public TMP_InputField textPrimaryField;
        public TMP_InputField textSecondaryField;
        public TMP_InputField backgroundSurfaceField;

        [Header("Typography Fields")]
        public TMP_Dropdown buttonFontWeightDropdown;
        public TMP_InputField fontFamilyField;

        [Header("Border Radius Fields")]
        public TMP_Dropdown roundedSmDropdown;
        public TMP_Dropdown roundedMdDropdown;
        public TMP_Dropdown roundedLgDropdown;
        public TMP_Dropdown roundedXlDropdown;
        public TMP_Dropdown roundedFullDropdown;

        [Header("Vendor Fields")]
        public TMP_InputField vendorNameField;
        public TMP_InputField logoUrlField;
    }

    [System.Serializable]
    public class LoginOptionUI
    {
        public TMP_Dropdown typeDropdown;
        public Toggle enabledToggle;
    }

    [System.Serializable]
    public class WalletOptionUI
    {
        public TMP_Dropdown typeDropdown;
        public Toggle enabledToggle;
        public TMP_InputField iconUrlField;
        public Toggle isInstalledToggle;
        public TMP_InputField deepLinkField;
    }

    [System.Serializable]
    public class SocialLoginFields
    {
        [Header("Google")]
        public Toggle googleToggle;
        
        [Header("Steam")]
        public Toggle steamToggle;
        
        [Header("Twitter")]
        public Toggle twitterToggle;
        
        [Header("Apple")]
        public Toggle appleToggle;
        
        [Header("Epic Games")]
        public Toggle epicGamesToggle;
        
        [Header("Telegram")]
        public Toggle telegramToggle;
    }

    [System.Serializable]
    public class OtpLoginFields
    {
        [Header("Email")]
        public Toggle emailToggle;
        
        [Header("WhatsApp")]
        public Toggle whatsappToggle;
    }

    [System.Serializable]
    public class ExternalWalletFields
    {
        [Header("Metamask")]
        public Toggle metamaskToggle;
        
        [Header("WalletConnect")]
        public Toggle walletconnectToggle;
    }

    public class UIConfigManager : MonoBehaviour
    {
        [Header("Target Theme")]
        [SerializeField] private OnboardingTheme targetTheme;
        
        [Header("Theme UI Fields")]
        [SerializeField] private ThemeUIFields themeFields;
        
        [Header("Social Login UI")]
        [SerializeField] private SocialLoginFields socialLoginFields;
        
        [Header("OTP Login UI")]
        [SerializeField] private OtpLoginFields otpLoginFields;
        
        [Header("External Wallet UI")]
        [SerializeField] private ExternalWalletFields externalWalletFields;
        
        [Header("Control Buttons")]
        [SerializeField] private Button applyButton;
        [SerializeField] private Button resetButton;
        
        private void Start()
        {
            SetupButtonCallbacks();
            InitializeDropdownOptions();
        }

        private void SetupButtonCallbacks()
        {
            if (applyButton != null)
                applyButton.onClick.AddListener(ApplyUIToScriptableObject);
                
            if (resetButton != null)
                resetButton.onClick.AddListener(ResetUIToDefaults);
        }

        private void InitializeDropdownOptions()
        {
            InitializeThemeNameDropdown();
            InitializeFontWeightDropdown();
            InitializeBorderRadiusDropdowns();
        }

        private void InitializeThemeNameDropdown()
        {
            if (themeFields.themeNameDropdown != null)
            {
                themeFields.themeNameDropdown.ClearOptions();
                
                var themeNameOptions = new List<string>
                {
                    "light", "dark"
                };
                
                themeFields.themeNameDropdown.AddOptions(themeNameOptions);
                themeFields.themeNameDropdown.value = 0; // Default to "light"
            }
        }

        private void InitializeFontWeightDropdown()
        {
            if (themeFields.buttonFontWeightDropdown != null)
            {
                themeFields.buttonFontWeightDropdown.ClearOptions();
                
                var fontWeightOptions = new List<string>
                {
                    "100", "200", "300", "400", "500", "600", "700", "800", "900"
                };
                
                themeFields.buttonFontWeightDropdown.AddOptions(fontWeightOptions);
                themeFields.buttonFontWeightDropdown.value = 4; // Default to "500"
            }
        }

        private void InitializeBorderRadiusDropdowns()
        {
            var radiusOptions = new List<string>
            {
                "0", "0.125rem", "0.25rem", "0.375rem", "0.5rem", "0.75rem", "1rem", "1.25rem", "1.5rem", "2rem", "9999px"
            };

            if (themeFields.roundedSmDropdown != null)
            {
                themeFields.roundedSmDropdown.ClearOptions();
                themeFields.roundedSmDropdown.AddOptions(radiusOptions);
                themeFields.roundedSmDropdown.value = 2; // Default to "0.25rem"
            }

            if (themeFields.roundedMdDropdown != null)
            {
                themeFields.roundedMdDropdown.ClearOptions();
                themeFields.roundedMdDropdown.AddOptions(radiusOptions);
                themeFields.roundedMdDropdown.value = 4; // Default to "0.5rem"
            }

            if (themeFields.roundedLgDropdown != null)
            {
                themeFields.roundedLgDropdown.ClearOptions();
                themeFields.roundedLgDropdown.AddOptions(radiusOptions);
                themeFields.roundedLgDropdown.value = 5; // Default to "0.75rem"
            }

            if (themeFields.roundedXlDropdown != null)
            {
                themeFields.roundedXlDropdown.ClearOptions();
                themeFields.roundedXlDropdown.AddOptions(radiusOptions);
                themeFields.roundedXlDropdown.value = 6; // Default to "1rem"
            }

            if (themeFields.roundedFullDropdown != null)
            {
                themeFields.roundedFullDropdown.ClearOptions();
                themeFields.roundedFullDropdown.AddOptions(radiusOptions);
                themeFields.roundedFullDropdown.value = 10; // Default to "9999px"
            }
        }

        [ContextMenu("Apply UI to ScriptableObject")]
        public void ApplyUIToScriptableObject()
        {
            if (targetTheme == null)
            {
                CustomLogger.LogError("No target theme assigned!");
                return;
            }

            ApplyGeneralSettings();
            ApplyThemeSettings();
            ApplyVendorSettings();
            ApplyLoginSettings();

            #if UNITY_EDITOR
            UnityEditor.EditorUtility.SetDirty(targetTheme);
            #endif

            CustomLogger.Log("UI settings applied to ScriptableObject successfully!");
        }

        [ContextMenu("Reset UI to Defaults")]
        public void ResetUIToDefaults()
        {
            SetDefaultValuesToUI();
            CustomLogger.Log("UI reset to defaults!");
        }

        private void ApplyGeneralSettings()
        {
            if (themeFields.themeNameDropdown != null)
                targetTheme.appearance.themeName = themeFields.themeNameDropdown.options[themeFields.themeNameDropdown.value].text;
        }

        private void ApplyThemeSettings()
        {
            var theme = targetTheme.appearance.theme;

            if (themeFields.bodyBackgroundField != null)
                theme.bodyBackground = themeFields.bodyBackgroundField.text;
            if (themeFields.bodyColorTertiaryField != null)
                theme.bodyColorTertiary = themeFields.bodyColorTertiaryField.text;
            if (themeFields.accentColorField != null)
                theme.accentColor = themeFields.accentColorField.text;
            if (themeFields.borderColorField != null)
                theme.borderColor = themeFields.borderColorField.text;
            if (themeFields.strokeDividerField != null)
                theme.strokeDivider = themeFields.strokeDividerField.text;
            if (themeFields.successColorField != null)
                theme.successColor = themeFields.successColorField.text;
            if (themeFields.warningColorField != null)
                theme.warningColor = themeFields.warningColorField.text;
            if (themeFields.errorColorField != null)
                theme.errorColor = themeFields.errorColorField.text;
            if (themeFields.textPrimaryField != null)
                theme.textPrimary = themeFields.textPrimaryField.text;
            if (themeFields.textSecondaryField != null)
                theme.textSecondary = themeFields.textSecondaryField.text;
            if (themeFields.backgroundSurfaceField != null)
                theme.backgroundSurface = themeFields.backgroundSurfaceField.text;

            if (themeFields.buttonFontWeightDropdown != null)
                theme.buttonFontWeight = themeFields.buttonFontWeightDropdown.options[themeFields.buttonFontWeightDropdown.value].text;
            if (themeFields.fontFamilyField != null)
                theme.fontFamily = themeFields.fontFamilyField.text;

            if (themeFields.roundedSmDropdown != null)
                theme.roundedSm = themeFields.roundedSmDropdown.options[themeFields.roundedSmDropdown.value].text;
            if (themeFields.roundedMdDropdown != null)
                theme.roundedMd = themeFields.roundedMdDropdown.options[themeFields.roundedMdDropdown.value].text;
            if (themeFields.roundedLgDropdown != null)
                theme.roundedLg = themeFields.roundedLgDropdown.options[themeFields.roundedLgDropdown.value].text;
            if (themeFields.roundedXlDropdown != null)
                theme.roundedXl = themeFields.roundedXlDropdown.options[themeFields.roundedXlDropdown.value].text;
            if (themeFields.roundedFullDropdown != null)
                theme.roundedFull = themeFields.roundedFullDropdown.options[themeFields.roundedFullDropdown.value].text;
        }

        private void ApplyVendorSettings()
        {
            if (themeFields.vendorNameField != null)
                targetTheme.vendor.name = themeFields.vendorNameField.text;
            if (themeFields.logoUrlField != null)
                targetTheme.vendor.logo = themeFields.logoUrlField.text;
        }

        private void ApplyLoginSettings()
        {
            // Apply social logins
            targetTheme.loginOptions.socialLogins.Clear();
            if (socialLoginFields.googleToggle != null && socialLoginFields.googleToggle.isOn)
            {
                targetTheme.loginOptions.socialLogins.Add(new LoginOption
                {
                    type = "google",
                    position = 1
                });
            }
            if (socialLoginFields.steamToggle != null && socialLoginFields.steamToggle.isOn)
            {
                targetTheme.loginOptions.socialLogins.Add(new LoginOption
                {
                    type = "steam",
                    position = 2
                });
            }
            if (socialLoginFields.twitterToggle != null && socialLoginFields.twitterToggle.isOn)
            {
                targetTheme.loginOptions.socialLogins.Add(new LoginOption
                {
                    type = "twitter",
                    position = 3
                });
            }
            if (socialLoginFields.appleToggle != null && socialLoginFields.appleToggle.isOn)
            {
                targetTheme.loginOptions.socialLogins.Add(new LoginOption
                {
                    type = "apple",
                    position = 4
                });
            }
            if (socialLoginFields.epicGamesToggle != null && socialLoginFields.epicGamesToggle.isOn)
            {
                targetTheme.loginOptions.socialLogins.Add(new LoginOption
                {
                    type = "epic_games",
                    position = 5
                });
            }
            if (socialLoginFields.telegramToggle != null && socialLoginFields.telegramToggle.isOn)
            {
                targetTheme.loginOptions.socialLogins.Add(new LoginOption
                {
                    type = "telegram",
                    position = 6
                });
            }

            // Apply OTP logins
            targetTheme.loginOptions.otpLoginOptions.Clear();
            if (otpLoginFields.emailToggle != null && otpLoginFields.emailToggle.isOn)
            {
                targetTheme.loginOptions.otpLoginOptions.Add(new LoginOption
                {
                    type = "email",
                    position = 1
                });
            }
            if (otpLoginFields.whatsappToggle != null && otpLoginFields.whatsappToggle.isOn)
            {
                targetTheme.loginOptions.otpLoginOptions.Add(new LoginOption
                {
                    type = "whatsapp",
                    position = 2
                });
            }

            // Apply external wallets
            targetTheme.loginOptions.externalWallets.Clear();
            if (externalWalletFields.metamaskToggle != null && externalWalletFields.metamaskToggle.isOn)
            {
                targetTheme.loginOptions.externalWallets.Add(new ExternalWallet
                {
                    type = "metamask",
                    position = 1,
                    metadata = new WalletMetadata
                    {
                        iconUrl = "",
                        isInstalled = false,
                        deepLink = ""
                    }
                });
            }
            if (externalWalletFields.walletconnectToggle != null && externalWalletFields.walletconnectToggle.isOn)
            {
                targetTheme.loginOptions.externalWallets.Add(new ExternalWallet
                {
                    type = "walletconnect",
                    position = 2,
                    metadata = new WalletMetadata
                    {
                        iconUrl = "",
                        isInstalled = false,
                        deepLink = ""
                    }
                });
            }

            // Sort all lists by position
            targetTheme.loginOptions.socialLogins = targetTheme.loginOptions.socialLogins.OrderBy(x => x.position).ToList();
            targetTheme.loginOptions.otpLoginOptions = targetTheme.loginOptions.otpLoginOptions.OrderBy(x => x.position).ToList();
            targetTheme.loginOptions.externalWallets = targetTheme.loginOptions.externalWallets.OrderBy(x => x.position).ToList();
        }

        private void SetDefaultValuesToUI()
        {
            // Set default general settings
            if (themeFields.themeNameDropdown != null)
            {
                int themeNameIndex = themeFields.themeNameDropdown.options.FindIndex(option => option.text == "light");
                themeFields.themeNameDropdown.value = themeNameIndex >= 0 ? themeNameIndex : 0;
            }

            // Set default theme settings
            if (themeFields.bodyBackgroundField != null)
                themeFields.bodyBackgroundField.text = "#ffffff";
            if (themeFields.bodyColorTertiaryField != null)
                themeFields.bodyColorTertiaryField.text = "#adb5bd";
            if (themeFields.accentColorField != null)
                themeFields.accentColorField.text = "#5166ee";
            if (themeFields.borderColorField != null)
                themeFields.borderColorField.text = "rgba(22, 22, 22, 0.12)";
            if (themeFields.strokeDividerField != null)
                themeFields.strokeDividerField.text = "rgba(22, 22, 22, 0.06)";
            if (themeFields.successColorField != null)
                themeFields.successColorField.text = "#28a745";
            if (themeFields.warningColorField != null)
                themeFields.warningColorField.text = "#ffc107";
            if (themeFields.errorColorField != null)
                themeFields.errorColorField.text = "#f75757";
            if (themeFields.textPrimaryField != null)
                themeFields.textPrimaryField.text = "#161616";
            if (themeFields.textSecondaryField != null)
                themeFields.textSecondaryField.text = "#707070";
            if (themeFields.backgroundSurfaceField != null)
                themeFields.backgroundSurfaceField.text = "#f8f8f8";

            if (themeFields.buttonFontWeightDropdown != null)
            {
                int fontWeightIndex = themeFields.buttonFontWeightDropdown.options.FindIndex(option => option.text == "500");
                themeFields.buttonFontWeightDropdown.value = fontWeightIndex >= 0 ? fontWeightIndex : 4;
            }
            if (themeFields.fontFamilyField != null)
                themeFields.fontFamilyField.text = "'Inter', sans-serif, 'Segoe UI', Roboto, Helvetica, Arial, sans-serif";

            if (themeFields.roundedSmDropdown != null)
            {
                int roundedSmIndex = themeFields.roundedSmDropdown.options.FindIndex(option => option.text == "0.25rem");
                themeFields.roundedSmDropdown.value = roundedSmIndex >= 0 ? roundedSmIndex : 2;
            }
            if (themeFields.roundedMdDropdown != null)
            {
                int roundedMdIndex = themeFields.roundedMdDropdown.options.FindIndex(option => option.text == "0.5rem");
                themeFields.roundedMdDropdown.value = roundedMdIndex >= 0 ? roundedMdIndex : 4;
            }
            if (themeFields.roundedLgDropdown != null)
            {
                int roundedLgIndex = themeFields.roundedLgDropdown.options.FindIndex(option => option.text == "0.75rem");
                themeFields.roundedLgDropdown.value = roundedLgIndex >= 0 ? roundedLgIndex : 5;
            }
            if (themeFields.roundedXlDropdown != null)
            {
                int roundedXlIndex = themeFields.roundedXlDropdown.options.FindIndex(option => option.text == "1rem");
                themeFields.roundedXlDropdown.value = roundedXlIndex >= 0 ? roundedXlIndex : 6;
            }
            if (themeFields.roundedFullDropdown != null)
            {
                int roundedFullIndex = themeFields.roundedFullDropdown.options.FindIndex(option => option.text == "9999px");
                themeFields.roundedFullDropdown.value = roundedFullIndex >= 0 ? roundedFullIndex : 10;
            }

            // Set default vendor settings
            if (themeFields.vendorNameField != null)
                themeFields.vendorNameField.text = "Okto wallet";
            if (themeFields.logoUrlField != null)
                themeFields.logoUrlField.text = "/okto.svg";

            // Set default social logins
            if (socialLoginFields.googleToggle != null)
                socialLoginFields.googleToggle.isOn = true;
            if (socialLoginFields.steamToggle != null)
                socialLoginFields.steamToggle.isOn = true;
            if (socialLoginFields.twitterToggle != null)
                socialLoginFields.twitterToggle.isOn = true;
            if (socialLoginFields.appleToggle != null)
                socialLoginFields.appleToggle.isOn = true;
            if (socialLoginFields.epicGamesToggle != null)
                socialLoginFields.epicGamesToggle.isOn = true;
            if (socialLoginFields.telegramToggle != null)
                socialLoginFields.telegramToggle.isOn = true;

            // Set default OTP logins
            if (otpLoginFields.emailToggle != null)
                otpLoginFields.emailToggle.isOn = true;
            if (otpLoginFields.whatsappToggle != null)
                otpLoginFields.whatsappToggle.isOn = true;

            // Set default external wallets
            if (externalWalletFields.metamaskToggle != null)
            {
                externalWalletFields.metamaskToggle.isOn = true;
            }
            if (externalWalletFields.walletconnectToggle != null)
            {
                externalWalletFields.walletconnectToggle.isOn = true;
            }
        }

        public void SetTargetTheme(OnboardingTheme theme)
        {
            targetTheme = theme;
        }

        public OnboardingTheme GetTargetTheme()
        {
            return targetTheme;
        }
    }
} 
