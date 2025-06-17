#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace OktoSDK.Editor
{
    public static class EnvironmentConfigCreator
    {
        [MenuItem("Assets/Create/OktoSDK/Environment Config With Defaults")]
        public static void CreateEnvironmentConfig()
        {
            var asset = ScriptableObject.CreateInstance<EnvironmentConfig>();

            asset.clientDetails = new Config[]
            {
                new Config { env = OktoEnv.SANDBOX, clientSwa = "", clientPrivateKey = "" },
                new Config { env = OktoEnv.PRODUCTION, clientSwa = "", clientPrivateKey = "" }
            };

            string path = AssetDatabase.GenerateUniqueAssetPath("Assets/OktoSDK/Resources/EnvironmentConfig.asset");
            AssetDatabase.CreateAsset(asset, path);
            AssetDatabase.SaveAssets();
            EditorUtility.FocusProjectWindow();
            Selection.activeObject = asset;
        }
    }
}
#endif
