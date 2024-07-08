using System.Security.Cryptography;
using System.Text;

namespace CORNCafePOSAPICommon
{
    public static class MyUtilityMethod
    {
        private const string _keyString = "pokjmngf76174g59";
        private const string _ivString = "3254rv72tyv22ygh";

        public static string Encrypt(string plainText)
        {
            plainText = string.IsNullOrEmpty(plainText) ? "" : plainText;

            if (plainText.Length == 0)
                return plainText;

            byte[] cipherData;
            Aes aes = Aes.Create();

            aes.Key = Encoding.UTF8.GetBytes(_keyString);
            aes.IV = Encoding.UTF8.GetBytes(_ivString);
            aes.Mode = CipherMode.CBC;

            ICryptoTransform cipher = aes.CreateEncryptor(aes.Key, aes.IV);

            using (MemoryStream ms = new MemoryStream())
            {
                using (CryptoStream cs = new CryptoStream(ms, cipher, CryptoStreamMode.Write))
                {
                    using (StreamWriter sw = new StreamWriter(cs))
                    {
                        sw.Write(plainText);
                    }
                }

                cipherData = ms.ToArray();
            }

            byte[] combinedData = new byte[aes.IV.Length + cipherData.Length];

            Array.Copy(aes.IV, 0, combinedData, 0, aes.IV.Length);
            Array.Copy(cipherData, 0, combinedData, aes.IV.Length, cipherData.Length);

            return Convert.ToBase64String(combinedData);
        }
        public static string Decrypt(string combinedString)
        {
            combinedString = string.IsNullOrEmpty(combinedString) ? "" : combinedString;

            if (combinedString.Length == 0)
                return combinedString;

            string plainText = "";

            try
            {
                byte[] combinedData = Convert.FromBase64String(combinedString);
                Aes aes = Aes.Create();

                aes.Key = Encoding.UTF8.GetBytes(_keyString);
                aes.IV = Encoding.UTF8.GetBytes(_ivString);

                byte[] iv = aes.IV;
                byte[] cipherText = new byte[combinedData.Length - iv.Length];

                Array.Copy(combinedData, iv, iv.Length);
                Array.Copy(combinedData, iv.Length, cipherText, 0, cipherText.Length);

                aes.Mode = CipherMode.CBC;

                ICryptoTransform decipher = aes.CreateDecryptor(aes.Key, aes.IV);

                using (MemoryStream ms = new MemoryStream(cipherText))
                {
                    using (CryptoStream cs = new CryptoStream(ms, decipher, CryptoStreamMode.Read))
                    {
                        using (StreamReader sr = new StreamReader(cs))
                        {
                            plainText = sr.ReadToEnd();
                        }
                    }
                }
            }
            catch (Exception) { }

            return plainText;
        }
    }
}