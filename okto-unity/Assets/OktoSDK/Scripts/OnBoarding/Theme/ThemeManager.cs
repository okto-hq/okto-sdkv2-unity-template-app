using UnityEngine;

namespace OktoSDK
{
    /// <summary>
    /// Utility class for managing theme switching
    /// </summary>
    public static class ThemeManager
    {
        /// <summary>
        /// Set theme for an OnboardingTheme instance
        /// </summary>
        public static void SetTheme(OnboardingTheme theme, string themeName)
        {
            if (theme == null) return;
            
            theme.appearance.themeName = themeName;
            theme.appearance.ApplyThemeColors();
            
            #if UNITY_EDITOR
            UnityEditor.EditorUtility.SetDirty(theme);
            #endif
        }
        
        /// <summary>
        /// Switch to light theme
        /// </summary>
        public static void SetLightTheme(OnboardingTheme theme)
        {
            SetTheme(theme, "light");
        }
        
        /// <summary>
        /// Switch to dark theme
        /// </summary>
        public static void SetDarkTheme(OnboardingTheme theme)
        {
            SetTheme(theme, "dark");
        }
        
        /// <summary>
        /// Toggle between light and dark themes
        /// </summary>
        public static void ToggleTheme(OnboardingTheme theme)
        {
            if (theme == null) return;
            
            string newTheme = theme.appearance.themeName == "dark" ? "light" : "dark";
            SetTheme(theme, newTheme);
        }
    }
} 