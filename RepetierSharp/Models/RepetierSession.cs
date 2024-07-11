namespace RepetierSharp
{
    public class RepetierSession
    {
        public string ApiKey { get; set; }
        public AuthenticationType AuthType { get; set; } = AuthenticationType.None;
        public string SessionId { get; set; }
        public bool LongLivedSession { get; set; }
        public string LoginName { get; set; }
        public string Password { get; set; }
    }
}
