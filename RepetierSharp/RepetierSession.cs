using System;

namespace RepetierSharp
{
    public class RepetierSession
    {
        public string? ApiKey { get; set; }
        public AuthenticationType AuthType { get; set; } = AuthenticationType.None;
        public string? SessionId { get; set; }

        /// <summary>
        ///     If supplied, will be used when login is required.
        /// </summary>
        public RepetierAuthentication? DefaultLogin { get; set; }

        public TimeSpan KeepAlivePing { get; set; } = TimeSpan.FromSeconds(5);
    }

    public class RepetierAuthentication
    {
        public bool LongLivedSession { get; set; } = true;
        public string LoginName { get; set; }
        public string Password { get; set; }

        public RepetierAuthentication(string loginName, string password, bool longLivedSession = true)
        {
            this.LoginName = loginName;
            this.Password = password;
            this.LongLivedSession = longLivedSession;
        }
    }
}
