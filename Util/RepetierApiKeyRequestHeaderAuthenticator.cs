using System.Threading.Tasks;
using RestSharp;
using RestSharp.Authenticators;

namespace RepetierSharp.Util
{
    /// <summary>
    /// The Api-Token authenticator for the Repetier-Server using the 'x-api-key' request header field.
    /// </summary>
    public class RepetierApiKeyRequestHeaderAuthenticator : AuthenticatorBase
    {
        public const string XApiKey = "x-api-key";

        /// <summary>
        /// Initializes a new instance of the <see cref="RepetierApiKeyRequestHeaderAuthenticator" /> class.
        /// </summary>
        /// <param name="apiKey">The access token, i.e. the api key.</param>
        public RepetierApiKeyRequestHeaderAuthenticator(string apiKey) :
            base(apiKey){}

        protected override ValueTask<Parameter> GetAuthenticationParameter(string accessToken)
            => new(new HeaderParameter(XApiKey, accessToken));
    }
}
