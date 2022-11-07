using System;
using System.Collections.Generic;
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

            public RepetierConnection Build()
            {
                List<string> urlParams = new List<string>();
                switch (_repetierConnection.Session.AuthType)
                {
                    case AuthenticationType.None:
                    // nothing here
                    // using global as login with permission 65535
                    case AuthenticationType.Credentials:
                        // loginRequired event is fired after connecting and attempts to login with given credentials
                        break;
                    case AuthenticationType.ApiKey:
                        urlParams.Add($"apikey={_repetierConnection.Session.ApiKey}");
                        break;
                }

                if (!string.IsNullOrEmpty(_repetierConnection.Session.LangKey))
                {
                    urlParams.Add($"lang={_repetierConnection.Session.LangKey}");
                }

                var httpProtocol = _repetierConnection.Session.UseTls ? "https" : "http";
                var wsProtocol = _repetierConnection.Session.UseTls ? "wss" : "ws";
                var socketParams = urlParams.Count > 0 ? $"?{string.Join('&', urlParams)}" : "";
                _repetierConnection.RestClient = new RestClient($"{httpProtocol}://{_repetierConnection.BaseURL}");
                _repetierConnection.WebSocketClient = new WebsocketClient(new Uri($"{wsProtocol}://{_repetierConnection.BaseURL}/socket/{socketParams}"));
                return _repetierConnection;
            }

            public RepetierConnectionBuilder UseLang(string lang = "en")
            {
                _repetierConnection.Session.LangKey = lang;
                return this;
            }

            // TODO: seems to not be supported
            private RepetierConnectionBuilder WithTls(bool useTls = true)
            {
                _repetierConnection.Session.UseTls = useTls;
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

            public RepetierConnectionBuilder PingInterval(uint interval = 3000)
            {
                this._repetierConnection.PingInterval = interval;
                return this;
            }

            public RepetierConnectionBuilder WithApiKey(string apiKey)
            {
                _repetierConnection.Session.ApiKey = apiKey;
                _repetierConnection.Session.AuthType = AuthenticationType.ApiKey;
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

            private RepetierConnectionBuilder WithPrehasedCredentials(string login, string passwordMD5)
            {
                throw new NotImplementedException();
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
