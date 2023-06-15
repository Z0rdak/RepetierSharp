using System;
using System.Collections.Generic;
using System.Linq;
using RepetierSharp.Models.Commands;
using RepetierSharp.Models.Events;
using RestSharp;
using Websocket.Client;

namespace RepetierSharp
{
    public partial class RepetierConnection
    {
        public class RepetierConnectionBuilder
        {
            readonly RepetierConnection _repetierConnection = new RepetierConnection()
            {
                Session = new RepetierSession()
            };

            private readonly Dictionary<string, string> _urlParameter = new Dictionary<string, string>();

            private string WebSocketProtocol => _repetierConnection.Session.UseTls ? "wss" : "ws";
            private string HttpProtocol => _repetierConnection.Session.UseTls ? "https" : "http";
            public string WebsocketUrl => $"{WebSocketProtocol}://{_repetierConnection.BaseURL}/socket/{BuildUrlParams()}";
            public string HttpUrl => $"{HttpProtocol}://{_repetierConnection.BaseURL}";

            public RepetierConnection Build()
            {
                var urlParams = new List<string>();

                switch (_repetierConnection.Session.AuthType)
                {
                    case AuthenticationType.None:
                        // Without an explicit authentication type, an anonymous authentication with the 'global' user profile is attempted
                        // Using WithCredentials("global", "") should result in the same outcome
                        // If at least one user is defined beside the global user, this will result in a permission denied message
                        break;
                    case AuthenticationType.Credentials:
                        // loginRequired event is fired after connecting and attempts to login with given credentials
                        break;
                    case AuthenticationType.ApiKey:
                        if (!_urlParameter.ContainsKey("apikey"))
                        {
                            _urlParameter.Add($"apikey", _repetierConnection.Session.ApiKey);
                        }
                        break;
                }

                if (!string.IsNullOrEmpty(_repetierConnection.Session.LangKey) && !_urlParameter.ContainsKey("lang"))
                {
                    _urlParameter.Add("lang", _repetierConnection.Session.LangKey);
                }
                var socketParams = BuildUrlParams();
                _repetierConnection.RestClient = new RestClient($"{HttpProtocol}://{_repetierConnection.BaseURL}");
                _repetierConnection.WebSocketClient = new WebsocketClient(new Uri($"{WebSocketProtocol}://{_repetierConnection.BaseURL}/socket/{socketParams}"));
                return _repetierConnection;
            }

            private string BuildUrlParams()
            {
                var parameterList = _urlParameter
                    .Select((pair) => $"{pair.Key}={pair.Value}")
                    .ToList(); ;
                return _urlParameter.Count > 0 ? $"?{string.Join('&', parameterList)}" : "";
            }

            public RepetierConnectionBuilder UseLang(string lang = "en")
            {
                _repetierConnection.Session.LangKey = lang;
                _urlParameter.Add("lang", lang);
                return this;
            }

            public RepetierConnectionBuilder WithHost(string baseUrl)
            {
                _repetierConnection.BaseURL = baseUrl;
                return this;
            }

            public RepetierConnectionBuilder WithHost(string baseUrl, uint port = 3344)
            {
                _repetierConnection.BaseURL = $"{baseUrl}:{port}";
                return this;
            }

            public RepetierConnectionBuilder WithTls(bool useTls = true)
            {
                _repetierConnection.Session.UseTls = useTls;
                return this;
            }

            public RepetierConnectionBuilder PingInterval(uint interval = 3000)
            {
                _repetierConnection.PingInterval = interval;
                return this;
            }

            public RepetierConnectionBuilder WithApiKey(string apiKey)
            {
                _repetierConnection.Session.ApiKey = apiKey;
                _repetierConnection.Session.AuthType = AuthenticationType.ApiKey;
                _urlParameter.Add("apikey", apiKey);
                return this;
            }

            public RepetierConnectionBuilder WithCredentials(string login, string password, bool rememberSession = false)
            {
                _repetierConnection.Session.LongLivedSession = rememberSession;
                _repetierConnection.Session.LoginName = login;
                _repetierConnection.Session.Password = password;
                _repetierConnection.Session.AuthType = AuthenticationType.Credentials;
                return this;
            }

            public RepetierConnectionBuilder QueryPrinterInterval(RepetierTimer timer = RepetierTimer.Timer30)
            {
                if (!_repetierConnection.QueryIntervals.ContainsKey(timer))
                {
                    _repetierConnection.QueryIntervals.Add(timer, new List<ICommandData>());
                }
                _repetierConnection.QueryIntervals[timer].Add(ListPrinterCommand.Instance);
                return this;
            }

            public RepetierConnectionBuilder QueryStateInterval(RepetierTimer timer = RepetierTimer.Timer30, bool withHistory = false)
            {
                if (!_repetierConnection.QueryIntervals.ContainsKey(timer))
                {
                    _repetierConnection.QueryIntervals.Add(timer, new List<ICommandData>());
                }
                _repetierConnection.QueryIntervals[timer].Add(new StateListCommand(withHistory));
                return this;
            }

            public RepetierConnectionBuilder WithCyclicCommand(RepetierTimer timer, ICommandData command)
            {
                if (!_repetierConnection.QueryIntervals.ContainsKey(timer))
                {
                    _repetierConnection.QueryIntervals.Add(timer, new List<ICommandData>());
                }
                _repetierConnection.QueryIntervals[timer].Add(command);
                return this;
            }
        }


    }
}
