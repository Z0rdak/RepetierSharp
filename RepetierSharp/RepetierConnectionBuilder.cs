using System;
using System.Net;
using RepetierSharp.Models.Events;
using RepetierSharp.Models.Commands;
using RestSharp;
using Websocket.Client;

namespace RepetierSharp
{
    public partial class RepetierConnection
    {
        public class RepetierConnectionBuilder
        {
            private readonly RepetierConnection _repetierConnection = new() { Session = new RepetierSession() };

            private string _host;
            private Uri? _hostUri;

            private IRestClient? _restClient;
            private RestClientOptions _restOptions = new();
            private IWebsocketClient? _websocketClient;
            private Uri? _websocketUri;

            public RepetierConnection Build()
            {
                if ( _restClient != null )
                {
                    _repetierConnection.RestClient = _restClient;
                }
                else
                {
                    if ( _restOptions == null )
                    {
                        throw new ArgumentNullException(nameof(_restOptions),
                            "No options for http connection supplied!");
                    }

                    _repetierConnection.RestClient = new RestClient(_restOptions);
                }

                if ( _websocketClient != null )
                {
                    _repetierConnection.WebSocketClient = _websocketClient;
                }
                else
                {
                    if ( _websocketUri == null )
                    {
                        throw new ArgumentNullException(nameof(_websocketUri),
                            "No Uri for websocket connection supplied!");
                    }

                    _repetierConnection.WebSocketClient = new WebsocketClient(_websocketUri);
                }

                return _repetierConnection;
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
                _restOptions = options;
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

            /// <summary>
            /// </summary>
            /// <param name="apiKey"></param>
            /// <returns></returns>
            public RepetierConnectionBuilder WithApiKey(string apiKey)
            {
                _repetierConnection.Session.ApiKey = apiKey;
                _repetierConnection.Session.AuthType = AuthenticationType.ApiKey;
                return this;
            }
            
            public RepetierConnectionBuilder WithSession(string sessionId)
            {
                _repetierConnection.Session.SessionId = sessionId;
                return this;
            }
            
            public RepetierConnectionBuilder ExcludePing(bool exclude = true)
            {
                _repetierConnection._excludePing = exclude;
                return this;
            }
            
            public RepetierConnectionBuilder SelectPrinter(string printerSlug)
            {
                _repetierConnection.SelectedPrinter = printerSlug;
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

            public RepetierConnectionBuilder WithCredentials(string login, string password,
                bool rememberSession = false)
            {
                _repetierConnection.Session.LongLivedSession = rememberSession;
                _repetierConnection.Session.LoginName = login;
                _repetierConnection.Session.Password = password;
                _repetierConnection.Session.AuthType = AuthenticationType.Credentials;
                _restOptions.Credentials = new NetworkCredential(login, password);
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
