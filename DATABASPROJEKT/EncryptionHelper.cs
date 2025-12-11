using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DATABASPROJEKT.Helpers;
using DATABASPROJEKT.Models;

namespace DATABASPROJEKT
{
    // Cleartext -> encrypt -> store -> read -> decrypt
    public class EncryptionHelper
    {
        private const byte Key = 0x42; 

        public static string Encrypt(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return text;
            }

            var bytes = System.Text.Encoding.UTF8.GetBytes(text);

            for (int i = 0; i < bytes.Length; i++)
            {
                bytes[i] = (byte)(bytes[i] ^ Key);
            }

            return Convert.ToBase64String(bytes);
        }

        public static string Decrypt(string krypteradText)
        {

            if (string.IsNullOrEmpty(krypteradText))
            {
                return krypteradText;
            }

            var bytes = Convert.FromBase64String(krypteradText);

            for (int i = 0; i < bytes.Length; i++)
            {
                bytes[i] = (byte)(bytes[i] ^ Key);
            }

            return System.Text.Encoding.UTF8.GetString(bytes);
        }
    }
}
