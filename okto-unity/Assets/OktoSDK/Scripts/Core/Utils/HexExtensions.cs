using System;

//helper class
namespace OktoSDK
{
    public static class HexExtensions
    {

        public static string ToHex(this byte[] bytes, bool prefix = false)
        {
            string hex = BitConverter.ToString(bytes).Replace("-", "");
            return prefix ? "0x" + hex : hex;
        }

        public static byte[] HexToByteArray(this string hex)
        {
            hex = hex.RemoveHexPrefix();
            
            if (hex.Length % 2 == 1)
                hex = "0" + hex;

            byte[] arr = new byte[hex.Length >> 1];
            for (int i = 0; i < hex.Length >> 1; ++i)
            {
                arr[i] = (byte)((GetHexVal(hex[i << 1]) << 4) + GetHexVal(hex[(i << 1) + 1]));
            }

            return arr;
        }

        private static int GetHexVal(char hex)
        {
            int val = (int)hex;
            return val - (val < 58 ? 48 : (val < 97 ? 55 : 87));
        }
    }
}