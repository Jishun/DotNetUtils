using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace DotNetUtils
{
    public static class Password
    {
        /// <summary>
        /// For Creating Salt and password Hash
        /// </summary>
        /// <returns>Salt String</returns>
        public static string CreateSalt(int bytes)
        {
            // Generate a cryptographic random number using the Cryptographic service provider
            var rng = new RNGCryptoServiceProvider();
            var buff = new byte[bytes];
            rng.GetBytes(buff);

            // Return Base64 representation of Random Number
            return Convert.ToBase64String(buff);
        }

        /// <summary>
        ///  Creates Password Hash
        /// </summary>
        /// <param name="passwordAndSalt">The password and salt.</param>
        /// <returns>Password Hash String</returns>
        public static string CreatePasswordHash(string passwordAndSalt)
        {
            //// Convert into GetBytes
            var sourceStringToBytes = (new UnicodeEncoding()).GetBytes(passwordAndSalt);
            //// convert bytes using MD5CryptoServiceProvider
            var hashedBytes = new MD5CryptoServiceProvider().ComputeHash(sourceStringToBytes);
            //// convert into Bit
            var hashedPasswd = BitConverter.ToString(hashedBytes);
            return hashedPasswd;
        }

        public static int PBKDF2IterationCount = 1000;
        public static int PBKDF2Bytes = 16;
        /// <summary>
        /// Creates Password Hash by PBKDF2
        /// </summary>
        /// <param name="password">password</param>
        /// <param name="salt">salt</param>
        /// <returns>Password Hash String</returns>
        public static string CreatePasswordHash(string password, string salt)
        {
            var hashedBytes = new Rfc2898DeriveBytes(password, (new UnicodeEncoding()).GetBytes(salt), PBKDF2IterationCount).GetBytes(PBKDF2Bytes);
            return BitConverter.ToString(hashedBytes); ;
        }

    }
}
