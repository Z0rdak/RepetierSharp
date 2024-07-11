using System;
using System.Collections.Generic;
using System.Net;
using RepetierSharp.Models.Events;
using RepetierSharp.Models.Requests;
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
            ///     Keep alive interval for the websocket connection.
            /// </summary>
            /// <param name="interval"></param>
            /// <returns></returns>
            public RepetierConnectionBuilder PingInterval(uint interval = 3000)
            {
                _repetierConnection.PingInterval = interval;
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

            public RepetierConnectionBuilder QueryPrinterInterval(RepetierTimer timer = RepetierTimer.Timer30)
            {
                if ( !_repetierConnection.QueryIntervals.ContainsKey(timer) )
                {
                    _repetierConnection.QueryIntervals.Add(timer, new List<IRepetierRequest>());
                }

                _repetierConnection.QueryIntervals[timer].Add(ListPrinterRequest.Instance);
                return this;
            }

            public RepetierConnectionBuilder QueryStateInterval(RepetierTimer timer = RepetierTimer.Timer30,
                bool withHistory = false)
            {
                if ( !_repetierConnection.QueryIntervals.ContainsKey(timer) )
                {
                    _repetierConnection.QueryIntervals.Add(timer, new List<IRepetierRequest>());
                }

                _repetierConnection.QueryIntervals[timer].Add(new StateListRequest(withHistory));
                return this;
            }

            public RepetierConnectionBuilder WithCyclicCommand(RepetierTimer timer, IRepetierRequest command)
            {
                if ( !_repetierConnection.QueryIntervals.ContainsKey(timer) )
                {
                    _repetierConnection.QueryIntervals.Add(timer, new List<IRepetierRequest>());
                }

                _repetierConnection.QueryIntervals[timer].Add(command);
                return this;
            }
        }
    }
}
