using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace DotNetUtils.Utils.EncriptAndCompress
{
    public static class AES
    {

        /// <summary>
        /// AES Encrypt
        /// </summary>
        /// <param name="src">source string</param>
        /// <param name="password">password</param>
        /// <param name="iv">Encrypt key</param>
        /// <returns></returns>
        public static string AESEncrypt(string src, string iv, string password = null)
        {
            var rijndaelCipher = new RijndaelManaged
            {
                Mode = CipherMode.CBC,
                Padding = PaddingMode.PKCS7,
                KeySize = 128,
                BlockSize = 128
            };

            var keyBytes = new byte[16];
            if (!password.IsNullOrEmpty())
            {
                byte[] pwdBytes = System.Text.Encoding.UTF8.GetBytes(password);
                int len = pwdBytes.Length;
                if (len > keyBytes.Length) len = keyBytes.Length;
                System.Array.Copy(pwdBytes, keyBytes, len);
            }
            rijndaelCipher.Key = keyBytes;

            var ivBytes = System.Text.Encoding.UTF8.GetBytes(iv);
            rijndaelCipher.IV = ivBytes;
            var transform = rijndaelCipher.CreateEncryptor();
            byte[] plainText = Encoding.UTF8.GetBytes(src);
            byte[] cipherBytes = transform.TransformFinalBlock(plainText, 0, plainText.Length);

            return Convert.ToBase64String(cipherBytes);

        }

        /// <summary>
        /// AES Decrypt
        /// </summary>
        /// <param name="src"></param>
        /// <param name="password"></param>
        /// <param name="iv"></param>
        /// <returns></returns>
        public static string AESDecrypt(string src, string iv, string password = null)
        {
            var rijndaelCipher = new RijndaelManaged
            {
                Mode = CipherMode.CBC,
                Padding = PaddingMode.PKCS7,
                KeySize = 128,
                BlockSize = 128
            };
            var encryptedData = Convert.FromBase64String(src);

            var keyBytes = new byte[16];
            if (!password.IsNullOrEmpty())
            {
                byte[] pwdBytes = System.Text.Encoding.UTF8.GetBytes(password);
                int len = pwdBytes.Length;
                if (len > keyBytes.Length) len = keyBytes.Length;
                System.Array.Copy(pwdBytes, keyBytes, len);
            }
            rijndaelCipher.Key = keyBytes;

            var ivBytes = System.Text.Encoding.UTF8.GetBytes(iv);
            rijndaelCipher.IV = ivBytes;
            var transform = rijndaelCipher.CreateDecryptor();
            var plainText = transform.TransformFinalBlock(encryptedData, 0, encryptedData.Length);

            return Encoding.UTF8.GetString(plainText);
        }
    }
}
