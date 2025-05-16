using UnityEngine;
using OktoSDK.Auth;

/// <summary>
/// Monitors clipboard for deeplink paste (e.g., Google Auth URL) and triggers extraction of the ID token.
/// This works reliably in Unity Editor even above WebView.
/// </summary>
public class DeeplinkPasteHandler : MonoBehaviour
{
    public GoogleAuthManager googleAuthManager;
    private string lastClipboard = "";

    private void Update()
    {
        string clipboardText = GUIUtility.systemCopyBuffer;

        // Clipboard changed?
        if (!string.IsNullOrEmpty(clipboardText) && clipboardText != lastClipboard)
        {
            lastClipboard = clipboardText;

            Debug.Log("📋 DeeplinkPasteHandler: New clipboard detected:\n" + clipboardText);

            // Optional: check for a specific pattern or host
            if (clipboardText.Contains("okto://auth"))
            {
                if (googleAuthManager != null)
                {
                    googleAuthManager.HandleDeepLink(clipboardText);
                    Debug.Log(" DeeplinkPasteHandler: Triggered HandleDeeplinkUrl.");
                }
                else
                {
                    Debug.LogError("DeeplinkPasteHandler: GoogleAuthManager.Instance is null.");
                }
            }
        }
    }
}
