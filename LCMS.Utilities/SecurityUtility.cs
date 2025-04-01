using System.Security.Cryptography;
using System.Text;

namespace LCMS.Utilities
{
    public static class SecurityUtility
    {
        /// <summary>
        /// Generate a random temporary password for users
        /// </summary>
        /// <param name="length">Length of password to generate</param>
        /// <returns>A string of random alpha numeric characters</returns>
        public static string GeneratePassword(int length = 7)
        {
            char[] arrPossibleChars = "abcdefghijkmnpqrstuvwxyzABCDEFGHJKLMNPQRSTUVWXYZ23456789".ToCharArray();
            int intPasswordLength = length;
            string stringPassword = string.Empty;
            var random = new Random();
            for (int i = 0; i < intPasswordLength; i++)
            {
                int intRandom = random.Next(arrPossibleChars.Length);
                stringPassword += arrPossibleChars[intRandom].ToString(System.Globalization.CultureInfo.InvariantCulture);
            }
            return stringPassword;
        }

        /// <summary>
        /// Generate a random salt for password hashing.
        /// </summary>
        /// <param name="input"></param>
        /// <returns>Hashed string.</returns>
        public static string MD5Hash(string input)
        {
            var sb = new StringBuilder();
            using (var md5 = MD5.Create())
            {
                // Step 1, Calculate MD5 hash from input
                var inputBytes = Encoding.ASCII.GetBytes(input);
                var hash = md5.ComputeHash(inputBytes);

                // Step 2, Convert byte array to hex string
                for (int i = 0; i < hash.Length; i++)
                {
                    sb.Append(hash[i].ToString("X2", System.Globalization.CultureInfo.InvariantCulture));
                }
            }

            return sb.ToString();
        }

        /// <summary>
        /// Takes a cleartext password, returns a Hash. Implements base Bcrypt salt length 24 bytes, work/cost factor of default 10 = 1024 iterations. This method generates a salt included in the hash
        /// </summary>
        /// <param name="passwordToHash">string value to hash</param>
        /// <returns>Hashed Password including salt</returns>
        public static string PasswordHash(string passwordToHash)
        {
            return BCrypt.Net.BCrypt.HashPassword(passwordToHash);
        }

        /// <summary>
        /// Verifies a string against a stored hash string. Uses Bcrypt.net included salt, work/cost factor default of 10.
        /// </summary>
        /// <param name="text"></param>
        /// <param name="hash"></param>
        /// <returns>True/False</returns>
        public static bool PasswordHashVerify(string text, string hash)
        {
            try
            {
                if (BCrypt.Net.BCrypt.Verify(text, hash))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception)
            {
                // log ex?
                return false;
                //return false;
            }
        }
    }
}
