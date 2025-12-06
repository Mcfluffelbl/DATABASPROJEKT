using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DATABASPROJEKT
{
    // Klartext -> krypterar -> lagra -> läsa -> dekryptera - klartext
    public class EncryptionHelper
    {
        private const byte Key = 0x42; // 66 bytes

        public static string Encrypt(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return text;
            }

            // 1. konvertera texten till bytes
            // Varför? Texten är Unicode (char/strings)
            // XOR för att kunna förvränga vår sträng och då behöver vi omvandla texten till en byte array
            var bytes = System.Text.Encoding.UTF8.GetBytes(text);

            // 2. 
            // En logisk operation
            // 0 ^ 0 = 0
            // 0 ^ 1 = 1
            // 1 ^ 0 = 1
            // 1 ^ 1 = 0
            // olika = 1, lika = 0
            //
            // Varför just XOR?
            // - Enkelt att förstå
            // - Symmetriskt: (A ^ K) ^ K = A
            //
            // Varför (byte)(bytes[i] ^ Key)
            // -
            // -
            // -
            //
            for (int i = 0; i < bytes.Length; i++)
            {
                bytes[i] = (byte)(bytes[i] ^ Key);
            }

            // 3 För att kunna spara resultatet som text. Kodar vi bytes till Base64.
            // Varför Base64?
            // Efter vi har gjort XOR kan bytes innehålla obegripliga eller ogiltiga tecken för text/JSON.
            // Lättare att lagra i filerm JSon, Databaser osv.
            return Convert.ToBase64String(bytes);
        }

        public static string Decrypt(string krypteradText)
        {
            // 1.
            if (string.IsNullOrEmpty(krypteradText))
            {
                return krypteradText;
            }

            // 2.
            // Gör om Base64-strängen till bytes igen
            // XOR tillbaka med samma nyckel
            // Här utnyttjar 
            //
            //
            //
            var bytes = Convert.FromBase64String(krypteradText);

            for (int i = 0; i <= bytes.Length; i++)
            {
                bytes[i] = (byte)(bytes[i] ^ Key);
            }

            // 3. Konverterar tillbaka från bytes -> klartext med UTF8
            return System.Text.Encoding.UTF8.GetString(bytes);
        }
    }
}
