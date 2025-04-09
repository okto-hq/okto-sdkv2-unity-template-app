using UnityEngine;

namespace OktoSDK
{
    public static class CustomLogger
    {
        public static void Log(string message)
        {
            if (Environment.IsLogEnabled())
            {
#if UNITY_EDITOR
                Debug.Log("<color=cyan>DEBUG: " + message + "</color>");
#else
                Debug.Log("DEBUG: " + message);
#endif
            }
        }

        public static void LogError(string message)
        {
            if (Environment.IsLogEnabled())
            {
#if UNITY_EDITOR
                Debug.LogError("<color=red>ERROR: " + message + "</color>");
#else
                Debug.LogError("ERROR: " + message);
#endif
            }
        }

        public static void LogWarning(string message)
        {
            if (Environment.IsLogEnabled())
            {
#if UNITY_EDITOR
                Debug.LogWarning("<color=yellow>WARNING: " + message + "</color>");
#else
                Debug.LogWarning("WARNING: " + message);
#endif
            }
        }
    }
}
