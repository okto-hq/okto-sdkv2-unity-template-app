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
        private WebCamTexture webCamTexture;
        private bool isFrontFacing = true; // Always use front camera

        /// <summary>
        /// Handles camera permission request and invokes onResult with true/false.
        /// </summary>
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
#else
            context.StartCoroutine(WaitForGenericWebcamPermission(onResult));
#endif
        }

#if UNITY_ANDROID && !UNITY_EDITOR
        private static IEnumerator WaitForAndroidPermission(Action<bool> callback)
        {
            // Wait until the user responds to the permission dialog
            while (!Permission.HasUserAuthorizedPermission(Permission.Camera))
            {
                yield return null;
            }
            yield return new WaitForSeconds(0.1f);
            callback(Permission.HasUserAuthorizedPermission(Permission.Camera));
        }
#endif

#if (UNITY_IOS || UNITY_STANDALONE) && !UNITY_EDITOR
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
        private static IEnumerator WaitForGenericWebcamPermission(Action<bool> callback)
        {
            if (!Application.HasUserAuthorization(UserAuthorization.WebCam))
            {
                yield return Application.RequestUserAuthorization(UserAuthorization.WebCam);
            }
            callback(Application.HasUserAuthorization(UserAuthorization.WebCam));
        }
#endif

        /// <summary>
        /// Starts the camera and displays it on the RawImage.
        /// </summary>
        public void StartCamera()
        {
            if (webCamTexture != null && webCamTexture.isPlaying)
            {
                webCamTexture.Stop();
            }

#if UNITY_IOS && !UNITY_EDITOR
            // Force portrait mode for iOS
            Screen.orientation = ScreenOrientation.Portrait;
            
            // Find front camera on iOS
            WebCamDevice frontCamera = new WebCamDevice();
            bool foundFrontCamera = false;
            
            foreach (WebCamDevice device in WebCamTexture.devices)
            {
                if (device.isFrontFacing)
                {
                    frontCamera = device;
                    foundFrontCamera = true;
                    break;
                }
            }
            
            // Create camera texture with fixed resolution to avoid zoom issues
            if (foundFrontCamera)
            {
                webCamTexture = new WebCamTexture(frontCamera.name, 1280, 720, 30);
                Debug.Log($"Using front camera: {frontCamera.name}");
            }
            else
            {
                webCamTexture = new WebCamTexture(1280, 720, 30);
                Debug.Log("No front camera found, using default camera");
            }
#else
            // For non-iOS platforms
            webCamTexture = new WebCamTexture(1280, 720);
#endif

            rawImage.texture = webCamTexture;
            rawImage.material.mainTexture = webCamTexture;
            webCamTexture.Play();

            StartCoroutine(WaitUntilCameraReady());
        }

        private IEnumerator WaitUntilCameraReady()
        {
            // Wait until the camera is initialized with timeout
            float timeoutSeconds = 5.0f;
            float startTime = Time.time;

            while (webCamTexture.width <= 16)
            {
                if (Time.time - startTime > timeoutSeconds)
                {
                    Debug.LogWarning("Camera initialization timed out");
                    break;
                }
                yield return null;
            }

#if UNITY_IOS && !UNITY_EDITOR
            // Ensure we're in portrait mode
            if (Screen.orientation != ScreenOrientation.Portrait)
            {
                Screen.orientation = ScreenOrientation.Portrait;
                yield return new WaitForSeconds(0.2f);
            }

            // Set fixed rotation and mirroring for iOS portrait mode front camera
            rawImage.rectTransform.localEulerAngles = new Vector3(0, 0, -270);
            rawImage.uvRect = new Rect(1, 0, -1, 1); // Mirror horizontally for selfie view
            
            // Fix aspect ratio to prevent zoom issues
            float videoRatio = (float)webCamTexture.width / (float)webCamTexture.height;
            float screenRatio = (float)Screen.width / (float)Screen.height;
            
            if (videoRatio > screenRatio)
            {
                // Video is wider than screen - fit to width
                float height = Screen.width / videoRatio;
                rawImage.rectTransform.sizeDelta = new Vector2(Screen.width, height);
            }
            else
            {
                // Video is taller than screen - fit to height
                float width = Screen.height * videoRatio;
                rawImage.rectTransform.sizeDelta = new Vector2(width, Screen.height);
            }
            
            // Center the camera view
            rawImage.rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
            rawImage.rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
            rawImage.rectTransform.pivot = new Vector2(0.5f, 0.5f);
            rawImage.rectTransform.anchoredPosition = Vector2.zero;
            
            Debug.Log($"iOS camera configured: {webCamTexture.width}x{webCamTexture.height}");
#else
            // For other platforms, use standard settings
            int rotationAngle = webCamTexture.videoRotationAngle;
            rawImage.rectTransform.localEulerAngles = new Vector3(0, 0, -rotationAngle);

            // Mirror for front camera or use standard uvRect
            if (isFrontFacing)
            {
                rawImage.uvRect = new Rect(1, 0, -1, 1); // Mirror horizontally
            }
            else
            {
                rawImage.uvRect = webCamTexture.videoVerticallyMirrored
                    ? new Rect(1, 0, -1, 1)
                    : new Rect(0, 0, 1, 1);
            }

            // Fix aspect ratio
            float aspect = (float)webCamTexture.width / (float)webCamTexture.height;
            rawImage.rectTransform.sizeDelta = new Vector2(
                rawImage.rectTransform.sizeDelta.x,
                rawImage.rectTransform.sizeDelta.x / aspect);
#endif

            Debug.Log($"Camera ready: {webCamTexture.width}x{webCamTexture.height}, " +
                    $"Orientation: {Screen.orientation}");
        }

        private void OnDisable()
        {
            if (webCamTexture != null && webCamTexture.isPlaying)
            {
                webCamTexture.Stop();
            }
        }
    }
}