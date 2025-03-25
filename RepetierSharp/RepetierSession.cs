using System;
using System.Text;

namespace RepetierSharp
{
    public class RepetierSession(IRepetierAuthentication? defaultLogin = null, int keepAliveInS = 10, 
        string? sessionId = null, bool longLivedSession = true)
    { 
        public string? SessionId { get; set; } = sessionId;

        /// <summary>
        ///     If supplied, will be used when login is required.
        /// </summary>
        public IRepetierAuthentication? DefaultLogin { get; set; } = defaultLogin;

        public TimeSpan KeepAlivePing { get; set; } = TimeSpan.FromSeconds(keepAliveInS);

        public bool LongLivedSession { get; set; } = longLivedSession;

    }

    public interface IRepetierAuthentication
    {
        public AuthenticationType AuthType { get;  }
    }

    public class ApiKeyAuth(string apiKey) : IRepetierAuthentication
    {
        public AuthenticationType AuthType { get => AuthenticationType.ApiKey; }
        public string ApiKey { get; set; } = apiKey;
    }

    public class CredentialAuth(string loginName, string password) : IRepetierAuthentication
    {
        public AuthenticationType AuthType { get => AuthenticationType.Credentials; }
        public string LoginName { get; set; } = loginName;
        public string Password { get; set; } = password;
        
        /// <summary>
        ///     See: https://stackoverflow.com/questions/11454004/calculate-a-md5-hash-from-a-string
        /// </summary>
        /// <param name="input"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        private static string MD5(string input, Encoding? encoding = null)
        {
            if (encoding == null)
                encoding = Encoding.ASCII;
            // Use input string to calculate MD5 hash
            var inputBytes = encoding.GetBytes(input);
            var hashBytes = System.Security.Cryptography.MD5.HashData(inputBytes);

            // Convert the byte array to hexadecimal string
            var sb = new StringBuilder();
            for ( var i = 0; i < hashBytes.Length; i++ )
            {
                sb.Append(hashBytes[i].ToString("x2"));
            }
            return sb.ToString();
        }

        public static string HashPassword(string sessionKey, string login, string password)
        {
            return MD5(sessionKey + MD5(login + password, Encoding.ASCII), Encoding.ASCII);
        }
    }
    
    public enum AuthenticationType
    {
        None = 0,
        Credentials = 1,
        ApiKey = 2
    }
}
