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
            readonly RepetierConnection _repetierConnection = new RepetierConnection();

            public RepetierConnection Build()
            {
                List<string> urlParams = new List<string>();
                switch (_repetierConnection.AuthType)
                {
                    case AuthenticationType.None:
                    // nothing here
                    // using global as login with permission 65535
                    case AuthenticationType.Credentials:
                        // loginRequired event is fired after connecting and attempts to login with given credentials
                        break;
                    case AuthenticationType.ApiKey:
                        urlParams.Add($"apikey={_repetierConnection.ApiKey}");
                        break;
                }

                if (!string.IsNullOrEmpty(_repetierConnection.LangKey))
                {
                    urlParams.Add($"lang={_repetierConnection.LangKey}");
                }

                var httpProtocol = _repetierConnection.UseTls ? "https" : "http";
                var wsProtocol = _repetierConnection.UseTls ? "wss" : "ws";
                var socketParams = urlParams.Count > 0 ? $"?{string.Join('&', urlParams)}" : "";
                _repetierConnection.RestClient = new RestClient($"{httpProtocol}://{_repetierConnection.BaseURL}");
                _repetierConnection.WebSocketClient = new WebsocketClient(new Uri($"{wsProtocol}://{_repetierConnection.BaseURL}/socket/{socketParams}"));
                return _repetierConnection;
            }

            public RepetierConnectionBuilder WithCyclicRequest(uint seconds, string command)
            {
                return this;
            }

            public RepetierConnectionBuilder UseLang(string lang = "US")
            {
                _repetierConnection.LangKey = lang;
                return this;
            }

            public RepetierConnectionBuilder WithTls(bool useTls = true)
            {
                _repetierConnection.UseTls = useTls;
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

            public RepetierConnectionBuilder ActivatePrinter(string printerSlug)
            {
                throw new NotImplementedException();
            }

            public RepetierConnectionBuilder PingInterval(uint interval = 3000)
            {
                this._repetierConnection.PingInterval = interval;
                return this;
            }

            public RepetierConnectionBuilder WithApiKey(string apiKey)
            {
                _repetierConnection.ApiKey = apiKey;
                _repetierConnection.AuthType = AuthenticationType.ApiKey;
                return this;
            }

            public RepetierConnectionBuilder WithCredentials(string login, string password, bool rememberSession = false)
            {
                _repetierConnection.LongLivedSession = rememberSession;
                _repetierConnection.LoginName = login;
                _repetierConnection.Password = password;
                _repetierConnection.AuthType = AuthenticationType.Credentials;
                return this;
            }

            public RepetierConnectionBuilder WithPrehasedCredentials(string login, string passwordMD5)
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