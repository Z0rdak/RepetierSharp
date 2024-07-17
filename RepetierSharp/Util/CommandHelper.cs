using System.Text;

namespace RepetierSharp.Util
{
    internal class CommandHelper
    {
        /// <summary>
        ///     See: https://stackoverflow.com/questions/11454004/calculate-a-md5-hash-from-a-string
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        internal static string MD5(string input)
        {
            // Use input string to calculate MD5 hash
            using ( var md5 = System.Security.Cryptography.MD5.Create() )
            {
                var inputBytes = Encoding.ASCII.GetBytes(input);
                var hashBytes = md5.ComputeHash(inputBytes);

                // Convert the byte array to hexadecimal string
                var sb = new StringBuilder();
                for ( var i = 0; i < hashBytes.Length; i++ )
                {
                    sb.Append(hashBytes[i].ToString("x2"));
                }

                return sb.ToString();
            }
        }

        public static string HashPassword(string sessionKey, string login, string password)
        {
            return MD5(sessionKey + MD5(login + password));
        }
    }
}
