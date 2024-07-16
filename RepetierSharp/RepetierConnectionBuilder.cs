using System;
using RepetierSharp.Models.Commands;
using RepetierSharp.Util;
using RestSharp;
using Websocket.Client;

namespace RepetierSharp
{
    public partial class RepetierConnection
    {
        public class RepetierConnectionBuilder
        {
            private readonly RepetierConnection _repetierConnection = new() { Session = new RepetierSession() };

            private IWebsocketClient? _websocketClient;
            private IRestClient? _restClient;
            private string? _webSocketHost;
            private string? _restHost;
            private RestClientOptions? _restClientOptions = new();

            public RepetierConnection Build()
            {
                if ( string.IsNullOrEmpty(_webSocketHost) && _websocketClient == null )
                {
                    throw new InvalidOperationException("Websocket host must be set.");
                }

                if ( string.IsNullOrEmpty(_restHost) && _restClient == null )
                {
                    throw new InvalidOperationException("Http host must be set.");
                }

                // Create default clients if not provided
                if ( _websocketClient == null && !string.IsNullOrEmpty(_webSocketHost) )
                {
                    _websocketClient = new WebsocketClient(new Uri(_webSocketHost));
                }

                if ( _restClient == null && !string.IsNullOrEmpty(_restHost) )
                {
                    if ( _restClientOptions != null )
                    {
                        _restClientOptions.BaseUrl = new Uri(_restHost);
                        _restClient = new RestClient();
                    }
                    else
                    {
                        _restClient = new RestClient(_restHost);
                    }
                }

                if ( _restClient == null || _websocketClient == null )
                {
                    throw new InvalidOperationException("Rest client and websocket client must be set.");
                }

                return new RepetierConnection(_restClient, _websocketClient);
            }

            public RepetierConnectionBuilder WithWebsocketHost(string host)
            {
                _webSocketHost = host;
                return this;
            }

            public RepetierConnectionBuilder WithWebsocketHost(Uri host)
            {
                _webSocketHost = host.ToString();
                return this;
            }

            public RepetierConnectionBuilder WithHttpRestHost(string host)
            {
                _restHost = host;
                return this;
            }

            public RepetierConnectionBuilder WithHttpHost(Uri host)
            {
                _restHost = host.ToString();
                return this;
            }


            /// <summary>
            ///     The rest client which should be used to query the http of the repetier server. <br></br>
            ///     Note that rest options supplied will not be used when supplying an own rest client instance.
            /// </summary>
            /// <param name="restClient"></param>
            /// <returns></returns>
            public RepetierConnectionBuilder UseRestClient(IRestClient restClient)
            {
                _restClient = restClient;
                return this;
            }

            /// <summary>
            ///     The rest client options which should be used to instantiate the rest client upon calling build.
            /// </summary>
            /// <param name="options"></param>
            /// <returns></returns>
            public RepetierConnectionBuilder UseRestOptions(RestClientOptions options)
            {
                _restClientOptions = options;
                return this;
            }

            /// <summary>
            ///     The websocket client which should be used to communicate with the repetier server.
            /// </summary>
            /// <param name="websocketClient"></param>
            /// <returns></returns>
            public RepetierConnectionBuilder UseWebSocketClient(IWebsocketClient websocketClient)
            {
                _websocketClient = websocketClient;
                return this;
            }

            public RepetierConnectionBuilder WithSession(string sessionId)
            {
                _repetierConnection.Session.SessionId = sessionId;
                return this;
            }

            public RepetierConnectionBuilder ExcludePing(bool exclude = true)
            {
                if ( exclude )
                {
                    _repetierConnection._eventFilters.Add(eventId => eventId == "ping");
                }

                return this;
            }

            public RepetierConnectionBuilder SelectPrinter(string printerSlug)
            {
                _repetierConnection.SelectedPrinter = printerSlug;
                return this;
            }

            public RepetierConnectionBuilder WithEventFilter(Predicate<string> eventFilter)
            {
                _repetierConnection._eventFilters.Add(eventFilter);
                return this;
            }

            public RepetierConnectionBuilder WithEventFilter(string eventToFilter)
            {
                _repetierConnection._eventFilters.Add(eventId => eventId == eventToFilter);
                return this;
            }

            public RepetierConnectionBuilder WithCommandFilter(Predicate<string> commandFilter)
            {
                _repetierConnection._eventFilters.Add(commandFilter);
                return this;
            }

            public RepetierConnectionBuilder WithCommandFilter(string commandToFilter)
            {
                _repetierConnection._eventFilters.Add(eventId => eventId == commandToFilter);
                return this;
            }

            /// <summary>
            ///     Keep alive interval for the websocket connection.
            /// </summary>
            /// <param name="seconds"></param>
            /// <returns></returns>
            public RepetierConnectionBuilder WithTimeout(int seconds = 10)
            {
                _repetierConnection.Session.KeepAlivePing = TimeSpan.FromSeconds(seconds);
                return this;
            }


            /// <summary>
            ///     Keep alive interval for the websocket connection.
            /// </summary>
            /// <param name="timeout"></param>
            /// <returns></returns>
            public RepetierConnectionBuilder WithTimeout(TimeSpan timeout)
            {
                _repetierConnection.Session.KeepAlivePing = timeout;
                return this;
            }

            public RepetierConnectionBuilder WithWebsocketAuth(string login, string password,
                bool rememberSession = false)
            {
                _repetierConnection.Session.LongLivedSession = rememberSession;
                _repetierConnection.Session.LoginName = login;
                _repetierConnection.Session.Password = password;
                _repetierConnection.Session.AuthType = AuthenticationType.Credentials;
                return this;
            }

            public RepetierConnectionBuilder WithApiKey(string apiKey, bool rememberSession = false)
            {
                _repetierConnection.Session.LongLivedSession = rememberSession;
                _repetierConnection.Session.ApiKey = apiKey;
                _repetierConnection.Session.AuthType = AuthenticationType.ApiKey;
                if ( _restClientOptions == null )
                {
                    _restClientOptions = new RestClientOptions();
                }

                _restClientOptions.Authenticator = new RepetierApiKeyRequestHeaderAuthenticator(apiKey);
                return this;
            }

            public RepetierConnectionBuilder ScheduleCommand(RepetierTimer timer, IRepetierCommand command)
            {
                _repetierConnection._commandDispatcher.AddCommand(timer, command);
                return this;
            }
        }
    }
}
