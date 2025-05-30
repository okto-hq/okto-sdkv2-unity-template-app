using UnityEngine;
using UnityEditor;

namespace OktoSDK.Editor
{
    [CustomEditor(typeof(OnboardingTheme))]
    public class OnboardingThemeEditor : UnityEditor.Editor
    {
        private bool showAppearanceSection = true;
        private bool showVendorSection = true;
        private bool showLoginOptionsSection = true;
        
        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            
            OnboardingTheme theme = (OnboardingTheme)target;
            
            EditorGUILayout.Space(10);
            EditorGUILayout.LabelField("Okto Onboarding Theme Configuration", EditorStyles.boldLabel);
            EditorGUILayout.Space(5);
            
            // Version
            EditorGUILayout.PropertyField(serializedObject.FindProperty("version"));
            EditorGUILayout.Space(10);
            
            // Appearance settings
            showAppearanceSection = EditorGUILayout.Foldout(showAppearanceSection, "Appearance Settings", true, EditorStyles.foldoutHeader);
            if (showAppearanceSection)
            {
                EditorGUI.indentLevel++;
                
                var appearanceProp = serializedObject.FindProperty("appearance");
                EditorGUILayout.PropertyField(appearanceProp.FindPropertyRelative("themeName"));
                
                EditorGUILayout.Space(5);
                EditorGUILayout.LabelField("Theme Colors and Styles", EditorStyles.boldLabel);
                
                var themeProp = appearanceProp.FindPropertyRelative("theme");
                DrawColorProperty(themeProp.FindPropertyRelative("bodyBackground"), "Body Background");
                DrawColorProperty(themeProp.FindPropertyRelative("bodyColorTertiary"), "Tertiary Text Color");
                DrawColorProperty(themeProp.FindPropertyRelative("accentColor"), "Accent Color");
                DrawColorProperty(themeProp.FindPropertyRelative("borderColor"), "Border Color");
                DrawColorProperty(themeProp.FindPropertyRelative("strokeDivider"), "Divider Color");
                DrawColorProperty(themeProp.FindPropertyRelative("textPrimary"), "Primary Text");
                DrawColorProperty(themeProp.FindPropertyRelative("textSecondary"), "Secondary Text");
                DrawColorProperty(themeProp.FindPropertyRelative("backgroundSurface"), "Surface Background");
                DrawColorProperty(themeProp.FindPropertyRelative("successColor"), "Success Color");
                DrawColorProperty(themeProp.FindPropertyRelative("warningColor"), "Warning Color");
                DrawColorProperty(themeProp.FindPropertyRelative("errorColor"), "Error Color");
                
                EditorGUILayout.Space(5);
                EditorGUILayout.LabelField("Typography and Layout", EditorStyles.boldLabel);
                
                EditorGUILayout.PropertyField(themeProp.FindPropertyRelative("buttonFontWeight"), new GUIContent("Button Font Weight"));
                EditorGUILayout.PropertyField(themeProp.FindPropertyRelative("fontFamily"), new GUIContent("Font Family"));
                
                EditorGUILayout.Space(5);
                EditorGUILayout.LabelField("Border Radius Settings", EditorStyles.boldLabel);
                
                EditorGUILayout.PropertyField(themeProp.FindPropertyRelative("roundedSm"), new GUIContent("Small Radius"));
                EditorGUILayout.PropertyField(themeProp.FindPropertyRelative("roundedMd"), new GUIContent("Medium Radius"));
                EditorGUILayout.PropertyField(themeProp.FindPropertyRelative("roundedLg"), new GUIContent("Large Radius"));
                EditorGUILayout.PropertyField(themeProp.FindPropertyRelative("roundedXl"), new GUIContent("X-Large Radius"));
                EditorGUILayout.PropertyField(themeProp.FindPropertyRelative("roundedFull"), new GUIContent("Full/Circular Radius"));
                
                EditorGUI.indentLevel--;
            }
            
            EditorGUILayout.Space(10);
            
            // Vendor settings
            showVendorSection = EditorGUILayout.Foldout(showVendorSection, "Vendor Settings", true, EditorStyles.foldoutHeader);
            if (showVendorSection)
            {
                EditorGUI.indentLevel++;
                
                var vendorProp = serializedObject.FindProperty("vendor");
                EditorGUILayout.PropertyField(vendorProp.FindPropertyRelative("name"), new GUIContent("Vendor Name"));
                EditorGUILayout.PropertyField(vendorProp.FindPropertyRelative("logo"), new GUIContent("Logo URL/Path"));
                
                EditorGUI.indentLevel--;
            }
            
            EditorGUILayout.Space(10);
            
            // Login options
            showLoginOptionsSection = EditorGUILayout.Foldout(showLoginOptionsSection, "Login Options", true, EditorStyles.foldoutHeader);
            if (showLoginOptionsSection)
            {
                EditorGUI.indentLevel++;
                
                var loginOptionsProp = serializedObject.FindProperty("loginOptions");
                
                EditorGUILayout.LabelField("Social Logins", EditorStyles.boldLabel);
                EditorGUILayout.PropertyField(loginOptionsProp.FindPropertyRelative("socialLogins"), new GUIContent("Social Login Providers"), true);
                
                EditorGUILayout.Space(5);
                EditorGUILayout.LabelField("OTP Login Options", EditorStyles.boldLabel);
                EditorGUILayout.PropertyField(loginOptionsProp.FindPropertyRelative("otpLoginOptions"), new GUIContent("OTP Login Methods"), true);
                
                EditorGUILayout.Space(5);
                EditorGUILayout.LabelField("External Wallets", EditorStyles.boldLabel);
                EditorGUILayout.PropertyField(loginOptionsProp.FindPropertyRelative("externalWallets"), new GUIContent("External Wallet Options"), true);
                
                EditorGUI.indentLevel--;
            }
            
            serializedObject.ApplyModifiedProperties();
            
            EditorGUILayout.Space(15);
            
            // Add preview button
            if (GUILayout.Button("Preview JSON Configuration", GUILayout.Height(30)))
            {
                // Get the JSON representation
                string json = EditorJsonUtility.ToJson(theme, true);
                CustomLogger.Log("Theme Configuration JSON:\n" + json);
            }
        }
        
        private void DrawColorProperty(SerializedProperty property, string label)
        {
            string colorValue = property.stringValue;
            
            EditorGUILayout.BeginHorizontal();
            
            // Try to preview the color if it's a valid hex value
            Color previewColor = Color.white;
            if (ColorUtility.TryParseHtmlString(colorValue, out previewColor))
            {
                EditorGUI.DrawRect(GUILayoutUtility.GetRect(20, 20), previewColor);
            }
            else
            {
                // For rgba or other formats, show a placeholder
                EditorGUI.DrawRect(GUILayoutUtility.GetRect(20, 20), Color.gray);
            }
            
            EditorGUILayout.PropertyField(property, new GUIContent(" " + label));
            
            EditorGUILayout.EndHorizontal();
        }
    }
} 