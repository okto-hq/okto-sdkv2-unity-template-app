using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
#if UNITY_ANDROID
using UnityEngine.Android;
#endif

namespace OktoSDK.OnRamp
{
    public class CameraPermissionManager : MonoBehaviour
    {
        public RawImage rawImage; // Assign in Inspector

        public static void HandleCameraPermission(MonoBehaviour context, Action<bool> onResult)
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            if (!Permission.HasUserAuthorizedPermission(Permission.Camera))
            {
                Permission.RequestUserPermission(Permission.Camera);
                context.StartCoroutine(WaitForAndroidPermission(onResult));
            }
            else
            {
                onResult(true);
            }
#elif UNITY_IOS && !UNITY_EDITOR
            context.StartCoroutine(WaitForIOSPermission(onResult));
#else // Unity Editor, Standalone, WebGL, etc.
            context.StartCoroutine(WaitForEditorOrGenericPermission(onResult));
#endif
        }

#if UNITY_ANDROID && !UNITY_EDITOR
        private static IEnumerator WaitForAndroidPermission(Action<bool> callback)
        {
            while (!Permission.HasUserAuthorizedPermission(Permission.Camera))
            {
                yield return null;
            }
            yield return new WaitForSeconds(0.1f);
            callback(Permission.HasUserAuthorizedPermission(Permission.Camera));
        }
#endif

#if UNITY_IOS && !UNITY_EDITOR
        private static IEnumerator WaitForIOSPermission(Action<bool> callback)
        {
            if (!Application.HasUserAuthorization(UserAuthorization.WebCam))
            {
                yield return Application.RequestUserAuthorization(UserAuthorization.WebCam);
            }
            callback(Application.HasUserAuthorization(UserAuthorization.WebCam));
        }
#endif

#if UNITY_EDITOR || (!UNITY_ANDROID && !UNITY_IOS)
        private static IEnumerator WaitForEditorOrGenericPermission(Action<bool> callback)
        {
            if (!Application.HasUserAuthorization(UserAuthorization.WebCam))
            {
                yield return Application.RequestUserAuthorization(UserAuthorization.WebCam);
            }
            callback(Application.HasUserAuthorization(UserAuthorization.WebCam));
        }
#endif
    }
}
