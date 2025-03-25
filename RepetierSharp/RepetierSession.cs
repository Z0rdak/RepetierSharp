using System;

namespace RepetierSharp
{
    public class RepetierSession(IRepetierAuthentication? defaultLogin = null, int keepAliveInS = 5, 
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
    }
    
    public enum AuthenticationType
    {
        None = 0,
        Credentials = 1,
        ApiKey = 2
    }
}
