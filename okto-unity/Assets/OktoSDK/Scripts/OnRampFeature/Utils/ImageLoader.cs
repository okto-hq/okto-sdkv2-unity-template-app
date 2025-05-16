using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Threading.Tasks;

namespace OktoSDK.OnRamp
{
    public static class ImageLoader
    {
        public static async Task LoadImageFromURL(string url, Image targetImage)
        {
            if (targetImage == null)
            {
                CustomLogger.LogError("Target image is null.");
                return;
            }

            Texture2D texture = await DownloadImageAsync(url);
            if (texture != null)
            {
                targetImage.sprite = SpriteFromTexture(texture);
            }
        }

        private static async Task<Texture2D> DownloadImageAsync(string url)
        {
            using (UnityWebRequest request = UnityWebRequestTexture.GetTexture(url))
            {
                var asyncOperation = request.SendWebRequest();
                while (!asyncOperation.isDone)
                {
                    await Task.Yield(); // Yield control to avoid blocking main thread
                }

                if (request.result == UnityWebRequest.Result.Success)
                {
                    return DownloadHandlerTexture.GetContent(request);
                }
                else
                {
                    CustomLogger.LogError("Failed to load image: " + request.error);
                    return null;
                }
            }
        }

        private static Sprite SpriteFromTexture(Texture2D texture)
        {
            return Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
        }
    }
}