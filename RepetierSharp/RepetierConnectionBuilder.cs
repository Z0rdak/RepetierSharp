using System;
using System.Collections.Generic;
using System.Web;
using Microsoft.Extensions.Logging;
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
            private IWebsocketClient? _websocketClient;
            private IRestClient? _restClient;
            private string? _webSocketHost;
            private string? _restHost;
            private string? _selectedPrinter;
            private ILogger<RepetierConnection>? _logger;
            private RepetierSession _session = new();
            private RestClientOptions? _restClientOptions = new();
            private readonly List<Predicate<string>> _commandFilters = new();
            private readonly List<Predicate<string>> _eventFilters = new();
            private readonly CommandDispatcher _commandDispatcher = new();

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
                    var uri = new Uri(_webSocketHost);
                    var uriBuilder = new UriBuilder(uri);
                    var queryParameters = HttpUtility.ParseQueryString(uri.Query);
                    if ( _session.ApiKey != null )
                    {
                        queryParameters["apiKey"] = _session.ApiKey;
                    }

                    if ( _session.SessionId != null )
                    {
                        queryParameters["sess"] = _session.SessionId;
                    }

                    uriBuilder.Query = queryParameters.ToString();
                    _websocketClient = new WebsocketClient(uriBuilder.Uri);
                }

                if ( _restClient == null && !string.IsNullOrEmpty(_restHost) )
                {
                    if ( _restClientOptions != null )
                    {
                        _restClientOptions.BaseUrl = new Uri(_restHost);
                        if ( _session.ApiKey != null )
                        {
                            _restClientOptions.Authenticator =
                                new RepetierApiKeyRequestHeaderAuthenticator(_session.ApiKey);
                        }

                        _restClient = new RestClient(_restClientOptions);
                    }
                    else
                    {
                        _restClientOptions = new RestClientOptions(_restHost);
                        if ( _session.ApiKey != null )
                        {
                            _restClientOptions.Authenticator =
                                new RepetierApiKeyRequestHeaderAuthenticator(_session.ApiKey);
                        }

                        _restClient = new RestClient(_restClientOptions);
                    }
                }

                if ( _restClient == null || _websocketClient == null )
                {
                    throw new InvalidOperationException("Rest client and websocket client must be set.");
                }

                var con = new RepetierConnection(_restClient, _websocketClient, _session, _logger);
                // TODO: move to constructor?
                con._eventFilters = _eventFilters;
                con._commandFilters = _commandFilters;
                con.SelectedPrinter = _selectedPrinter ?? string.Empty;
                con._commandDispatcher = _commandDispatcher;
                return con;
            }

            public RepetierConnectionBuilder WithLogger(ILogger<RepetierConnection> logger)
            {
                _logger = logger;
                return this;
            }
            
            public RepetierConnectionBuilder WithDefaultLogger()
            {
                using var factory = LoggerFactory.Create(builder =>
                {
                    builder.AddConsole();
                });
                _logger = factory.CreateLogger<RepetierConnection>();
                return this;
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

            public RepetierConnectionBuilder WithHttpHost(string host)
            {
                _restHost = host;
                return this;
            }

            public RepetierConnectionBuilder WithHttpHost(Uri host)
            {
                _restHost = host.ToString();
                return this;
            }

            #region Session

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
                _session.SessionId = sessionId;
                return this;
            }

            public RepetierConnectionBuilder WithSession(RepetierSession session)
            {
                _session = session;
                return this;
            }

            /// <summary>
            ///     Keep alive interval for the websocket connection.
            /// </summary>
            /// <param name="seconds"></param>
            /// <returns></returns>
            public RepetierConnectionBuilder WithTimeout(int seconds = 5)
            {
                _session.KeepAlivePing = TimeSpan.FromSeconds(seconds);
                return this;
            }

            /// <summary>
            ///     Keep alive interval for the websocket connection.
            /// </summary>
            /// <param name="timeout"></param>
            /// <returns></returns>
            public RepetierConnectionBuilder WithTimeout(TimeSpan timeout)
            {
                _session.KeepAlivePing = timeout;
                return this;
            }

            #endregion

            public RepetierConnectionBuilder WithWebsocketAuth(string login, string password,
                bool rememberSession = false)
            {
                _session.DefaultLogin = new RepetierAuthentication(login, password, rememberSession);
                _session.AuthType = AuthenticationType.Credentials;
                return this;
            }

            public RepetierConnectionBuilder WithWebsocketAuth(RepetierAuthentication repAuth)
            {
                _session.DefaultLogin = repAuth;
                _session.AuthType = AuthenticationType.Credentials;
                return this;
            }

            public RepetierConnectionBuilder WithApiKey(string apiKey)
            {
                _session.ApiKey = apiKey;
                _session.AuthType = AuthenticationType.ApiKey;
                if ( _restClientOptions == null )
                {
                    _restClientOptions = new RestClientOptions();
                }

                _restClientOptions.Authenticator = new RepetierApiKeyRequestHeaderAuthenticator(apiKey);
                return this;
            }

            #region Commands and Events

            public RepetierConnectionBuilder ExcludePing(bool exclude = true)
            {
                if ( exclude )
                {
                    _commandFilters.Add(eventId => eventId == "ping");
                }

                return this;
            }

            public RepetierConnectionBuilder SelectPrinter(string printerSlug)
            {
                _selectedPrinter = printerSlug;
                return this;
            }

            public RepetierConnectionBuilder WithEventFilter(Predicate<string> eventFilter)
            {
                _eventFilters.Add(eventFilter);
                return this;
            }

            public RepetierConnectionBuilder WithEventFilter(string eventToFilter)
            {
                _eventFilters.Add(eventId => eventId == eventToFilter);
                return this;
            }

            public RepetierConnectionBuilder WithCommandFilter(Predicate<string> commandFilter)
            {
                _commandFilters.Add(commandFilter);
                return this;
            }

            public RepetierConnectionBuilder WithCommandFilter(string commandToFilter)
            {
                _commandFilters.Add(eventId => eventId == commandToFilter);
                return this;
            }

            public RepetierConnectionBuilder ScheduleServerCommand(RepetierTimer timer, ICommandData command)
            {
                _commandDispatcher.AddServerCommand(timer, command);
                return this;
            }
            
            public RepetierConnectionBuilder SchedulePrinterCommand(RepetierTimer timer, ICommandData command, string printer)
            {
                _commandDispatcher.AddPrinterCommand(timer, command, printer);
                return this;
            }

            #endregion
        }
    }
}
