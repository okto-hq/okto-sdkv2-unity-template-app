using UnityEngine;
using OktoSDK.Auth;
using System;

/// <summary>
/// Monitors clipboard for deeplink paste (e.g., Google Auth URL) and triggers extraction of the ID token.
/// This works reliably in Unity Editor even above WebView.
/// </summary>
/// 
namespace OktoSDK.Auth
{
    public class DeeplinkPasteHandler : MonoBehaviour
    {
        public GoogleAuthManager googleAuthManager;
        private string lastClipboard = "";

        [Header("Control")]
        [SerializeField] private bool isGoogleLoginActive = false;

        public bool IsGoogleLoginActive
        {
            get => isGoogleLoginActive;
            set => isGoogleLoginActive = value;
        }

        private void Update()
        {
            if (!isGoogleLoginActive)
                return;

            string clipboardText = GUIUtility.systemCopyBuffer;

            if (!string.IsNullOrEmpty(clipboardText) && clipboardText != lastClipboard)
            {
                lastClipboard = clipboardText;
                CustomLogger.Log(" DeeplinkPasteHandler: New clipboard detected:\n" + clipboardText);

                // Try extracting and decoding the "state" param if present
                if (clipboardText.Contains("#state="))
                {
                    try
                    {
                        // Extract fragment after "#state="
                        string fragment = clipboardText.Split(new[] { "#state=" }, StringSplitOptions.None)[1];
                        string decodedFragment = Uri.UnescapeDataString(fragment);

                        // Look for oktosdk://auth inside the decoded state JSON
                        if (decodedFragment.Contains("oktosdk://auth"))
                        {
                            if (googleAuthManager != null)
                            {
                                googleAuthManager.HandleDeepLink(clipboardText);
                                CustomLogger.Log("DeeplinkPasteHandler: Triggered HandleDeepLink.");
                                isGoogleLoginActive = false;
                            }
                            else
                            {
                                CustomLogger.LogError(" DeeplinkPasteHandler: GoogleAuthManager is null.");
                            }
                        }
                        else
                        {
                            CustomLogger.Log("DeeplinkPasteHandler: 'oktosdk://auth' not found in decoded fragment.");
                        }
                    }
                    catch (Exception e)
                    {
                        CustomLogger.LogError($"DeeplinkPasteHandler: Failed to parse deep link state - {e.Message}");
                    }
                }
                else
                {
                    CustomLogger.Log("DeeplinkPasteHandler: '#state=' not found in clipboard text.");
                }
            }
        }


        /// <summary>
        /// Enable Google login monitoring
        /// </summary>
        public void EnableGoogleLogin()
        {
            isGoogleLoginActive = true;
            CustomLogger.Log("DeeplinkPasteHandler: Google login monitoring enabled");
        }

        /// <summary>
        /// Disable Google login monitoring
        /// </summary>
        public void DisableGoogleLogin()
        {
            isGoogleLoginActive = false;
            CustomLogger.Log("DeeplinkPasteHandler: Google login monitoring disabled");
        }
    }
}