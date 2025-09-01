using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using CodeBase.Services.Encryption;
using Logger = ToolBox.Utils.Logger;

namespace ToolBox.Services.Encryption
{
    public class EncryptionService : IEncryptionService
    {
        private const string EncryptionPrefix = "AES_ENCRYPTED:";
        private const string InitVector = "HR$2pIjHR$2pIj12";
        private const string AesKeyStr = "AphFCTDElbdqlogro1MY5A==";

        public string Encrypt(string plainText) => EncryptX(plainText);
        
        private static string EncryptX(string plainText)
        {
            if (string.IsNullOrEmpty(plainText)) return string.Empty;

            using Aes aes = Aes.Create();
            try
            {
                aes.Key = Encoding.ASCII.GetBytes(AesKeyStr);
                aes.IV = Encoding.ASCII.GetBytes(InitVector);

                // Create an encryptor to perform the stream transform
                var encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

                // Create the streams used for encryption
                using var msEncrypt = new MemoryStream();

                using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                {
                    using (var swEncrypt = new StreamWriter(csEncrypt))
                    {
                        // Write all data to the stream
                        swEncrypt.Write(plainText);
                    }
                }

                // Return the encrypted bytes from the memory stream
                return EncryptionPrefix + Convert.ToBase64String(msEncrypt.ToArray());
            }
            catch (CryptographicException ex)
            {
                // Handle cryptographic exceptions here
                Logger.Log($"Encryption failed: {ex.Message}");
                return string.Empty; // Return empty string on failure
            }
        }

        public string Decrypt(string cipherText) => DecryptX(cipherText);
        
        private static string DecryptX(string cipherText)
        {
            if (string.IsNullOrEmpty(cipherText)) return string.Empty;

            using Aes aes = Aes.Create();
            try
            {
                if (!IsEncrypted(cipherText))
                {
                    // If not encrypted, return the original text
                    return cipherText;
                }

                cipherText = cipherText.Replace(EncryptionPrefix, "");

                var savedKey = Encoding.ASCII.GetBytes(AesKeyStr);
                var inputIv = Encoding.ASCII.GetBytes(InitVector);

                // Create a decryptor to perform the stream transform
                var decryptor = aes.CreateDecryptor(savedKey, inputIv);

                using (var msDecrypt = new MemoryStream(Convert.FromBase64String(cipherText)))
                using (var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                using (var srDecrypt = new StreamReader(csDecrypt))
                {
                    // Read the decrypted bytes from the decrypting stream and place them in a string
                    return srDecrypt.ReadToEnd();
                }
            }
            catch (CryptographicException ex)
            {
                Logger.Log($"Decryption failed: {ex.Message}");
                return string.Empty; // Return empty string on failure
            }
        }
        
        private static bool IsEncrypted(string content)
        {
            // Check for a signature or marker specific to the AES encryption process
            return content.StartsWith(EncryptionPrefix);
        }
    }
}

