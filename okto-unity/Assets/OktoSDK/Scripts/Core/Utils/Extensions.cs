using System;
using System.Collections.Generic;
using UnityEngine.Networking;
using Org.BouncyCastle.Math;

//helper class
namespace OktoSDK
{
    public static class Extensions
    {
        public static string RemoveHexPrefix(this string value)
        {
            return value.StartsWith("0x", StringComparison.OrdinalIgnoreCase)
                ? value.Substring(2)
                : value;
        }

        public static Dictionary<string, string> GetRequestHeaders(this UnityWebRequest request)
        {
            var headers = new Dictionary<string, string>();
            foreach (string headerName in request.GetResponseHeaders().Keys)
            {
                if (request.GetRequestHeader(headerName) != null)
                {
                    headers[headerName] = request.GetRequestHeader(headerName);
                }
            }
            return headers;
        }

        public static byte[] ToByteArrayUnsigned(this BigInteger value)
        {
            byte[] bytes = value.ToByteArray();

            // If the last byte is 0, it was added to ensure the number is positive
            // We can remove it for unsigned representation
            if (bytes.Length > 0 && bytes[bytes.Length - 1] == 0)
            {
                byte[] temp = new byte[bytes.Length - 1];
                Array.Copy(bytes, 0, temp, 0, temp.Length);
                bytes = temp;
            }

            // Reverse the array since BigInteger uses little-endian
            Array.Reverse(bytes);
            return bytes;
        }

    }

}