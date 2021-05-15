using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Threading;
using RepetierMqtt.Event;
using RepetierMqtt.Messages;
using RepetierMqtt.Models;
using RepetierMqtt.Models.Commands;
using RepetierMqtt.Models.Events;
using RepetierMqtt.Models.Messages;
using RepetierMqtt.Util;
using RestSharp;
using WebSocketSharp;

namespace RepetierMqtt
{
    /// <summary>
    /// Represents a WebSocket connection to the Repetier Server.
    /// </summary>
    public class RepetierConnection
    {

        #region event handler for repetier events

        /// <summary>
        /// Event: log
        /// When the proper log level is set, you get events for each new log line you wish to see. Type indicates one of the following:
        /// 1 : Commands
        /// 2 : ACK responses like ok, wait, temperature
        /// 4 : Other responses
        /// 8 : Non maskable messages
        /// </summary>
        public event LogEventReceived OnLogReceived;
        public delegate void LogEventReceived(LogEvent logEvent);

        /// <summary>
        /// Event: jobFinished 
        /// Gets send after a normal job has finished.
        /// </summary>
        public event JobFinishedReceivedHandler OnJobFinishedReceived;
        public delegate void JobFinishedReceivedHandler(string printer, JobFinishedEvent jobFinished, long timestamp);

        /// <summary>
        /// Event: jobStarted
        /// Gets send after a normal job has been started.
        /// </summary>
        public event JobStartedReceivedHandler OnJobStartedReceived;
        public delegate void JobStartedReceivedHandler(string printer, JobStartedEvent jobStarted, long timestamp);

        /// <summary>
        /// Triggered when REST call to start print is not successfull
        /// </summary>
        public event JobStartedFailedHandler OnJobStartedFailed;
        public delegate void JobStartedFailedHandler(string printer, IRestResponse response, long timestamp);

        /// <summary>
        /// Event: jobKilled 
        /// Gets send after a normal job has been killed.
        /// </summary>
        public event JobKilledReceivedHandler OnJobKilledReceived;
        public delegate void JobKilledReceivedHandler(string printer, JobKilledEvent jobKilled, long timestamp);

        /// <summary>
        /// Event: jobsChanged
        /// Gets triggered if a printer has a change in it's stored g-file list.
        /// </summary>
        public event JobsChangedReceivedHandler OnJobsChanged;
        public delegate void JobsChangedReceivedHandler(string printer, long timestamp);

        /// <summary>
        /// Event: state
        /// Gets triggered when a state changes.
        /// </summary>
        public event PrinterStateReceivedHandler OnPrinterStateReceived;
        public delegate void PrinterStateReceivedHandler(string printer, PrinterState printerState, long timestamp);

        /// <summary>
        /// Event: temp
        /// New temperature entry. O = Output, S = Set temperature, T = Measued temperature, id = Extruder number, t = Timestamp 
        /// </summary>
        public event TemperatureChangeReceivedHandler OnTempChangeReceived;
        public delegate void TemperatureChangeReceivedHandler(string printer, TempChangeEvent tempChange, long timestamp);

        /// <summary>
        /// Event: changeFilamentRequested 
        /// Firmware requested a filament change on server side.
        /// </summary>
        public event ChangeFilamentReceivedHandler OnChangeFilamentReceived;
        public delegate void ChangeFilamentReceivedHandler(string printer, long timestamp);

        /// <summary>
        /// Event: printerListChanged
        /// Gets triggered when a printer was added, deleted or modified.
        /// </summary>
        public event PrinterListChangedReceivedHandler OnPrinterListChanged;
        public delegate void PrinterListChangedReceivedHandler(List<Printer> printerList, long timestamp);

        /// <summary>
        /// Event: userCredentials
        /// When you reconnect a still existing session no login is required. As a first event you get the credentials of
        /// </summary>
        public event UserCredentialsReceivedHandler OnUserCredentialsReceived;
        public delegate void UserCredentialsReceivedHandler(UserCredentialsEvent userCredentials, long timestamp);

        /// <summary>
        /// Event: loginRequired
        /// A authorized user was required. 
        /// Login with user credentials. You get this message only once when you establish the socket connection.
        /// Payload: None
        /// </summary>
        public event LoginRequiredReceivedHandler OnLoginRequiredReceived;
        public delegate void LoginRequiredReceivedHandler(long timestamp);

        /// <summary>
        /// Event: logout
        /// Send as response on a logout request. The event is send to all instances sharing the same session.
        /// Payload: None
        /// </summary>
        public event LogoutReceivedHandler OnLogoutReceived;
        public delegate void LogoutReceivedHandler(long timestamp);

        /// <summary>
        /// Event: messagesChanged  
        /// Gets triggered when a new message gets available. That way you do not need to poll for new messages, only once at the connection start.
        /// Payload: None
        /// </summary>
        public event MessagesChangedReceivedHandler OnMessagesChanged;
        public delegate void MessagesChangedReceivedHandler(long timestamp);
        #endregion

        public event MessagesReceivedHandler OnMessagesReceived;
        public delegate void MessagesReceivedHandler(List<Message> messages);

        public event ResponseReceivedHandler OnResponseReceived;
        public delegate void ResponseReceivedHandler(IRepetierMessage response);

        // TODO: Move implement move, printqueueChanged, foldersChanged, eepromClear, eepromChanged, 
        // config, firewareChanged, settingsChanged, printerSettingChanged, modelGroupListChanged,
        // prepareJob, prepareJobFinished, remoteServersChanged and getExternalLinks events

        // TODO: extract literals into class with constants

        /// <summary>
        /// Command -> PeriodicTask
        /// e.g. "stateList" -> () -> {}
        /// </summary>
        private Dictionary<string, PeriodicTask> PeriodicTaskMap { get; set; } = new Dictionary<string, PeriodicTask>();

        private PeriodicTask PeriodicPing { get; set; }

        private WebSocket WebSocket { get; set; }
        private RestClient RestClient { get; set; }

        public string BaseURL { get; }                 // server baseUrl (IP-Address + port, e.g.: "127.0.0.1:3344")
        public string ApiKey { get; set; }             // Authentification for Repetier-Server
        private bool ApiKeyProvided { get; set; }
        private string SessionKey { get; set; }


        /// <summary>
        /// BaseURL must be provided by the user. 
        /// TODO: Provide constructors with other authentication
        /// </summary>
        /// <param name="baseURL"></param>
        /// <param name="ApiKey"></param>
        /// <param name="lang"> can be omitted </param>
        /// <param name="session"> can be omitted </param>
        public RepetierConnection(string baseURL, string ApiKey = "", string lang = "US", string session = "")
        {
            this.BaseURL = baseURL;
            this.ApiKey = ApiKey;
            ApiKeyProvided = !string.IsNullOrEmpty(ApiKey.Trim());
            InitDefaultHandlers();
            RestClient = new RestClient($"http://{this.BaseURL}");
            WebSocket = new WebSocket($"ws://{this.BaseURL}/socket/?lang={lang}&sess={session}&apikey={this.ApiKey}");
        }

        /// <summary>
        /// Initializes fields, properties and default event handlers.
        /// </summary>
        private void InitDefaultHandlers()
        {
            // TODO: Rework default handlers
        }

        /// <summary>
        /// Open WebSocket connection to repetier server and start communication.
        /// </summary>
        public void Connect()
        {
            // TODO: Login with Username/Password or ApiKey/Session
            InitWebSocket();
            WebSocket.Connect();
            PeriodicPing = new PeriodicTask(() => SendPing(), 1000);
        }

        /// <summary>
        /// Closes WebSocket connection and disposes timers
        /// </summary>
        /// <param name="closeStatusCode"></param>
        /// <param name="reason"></param>
        public void Close(CloseStatusCode closeStatusCode = CloseStatusCode.Normal, string reason = "")
        {
            foreach (var entry in PeriodicTaskMap)
            {
                CancelPeriodicTask(entry.Key);
            }
            this.PeriodicPing.Cancel();
            this.WebSocket.Close(closeStatusCode, reason);
        }

        /// <summary>
        /// Upload given G-Code and start the printing process via REST-API.
        /// </summary>
        /// <param name="GCODEFilePath"></param>
        public void UploadAndStartPrint(string GCODEFilePath, string printer)
        {
            // TODO: check connection / authorization
            try
            {
                var request = ApiConstants.StartPrintRequest(GCODEFilePath, printer, this.ApiKey);
                var Response = RestClient.Execute(request);
                if (Response.StatusCode != System.Net.HttpStatusCode.OK)
                {
                    OnJobStartedFailed?.Invoke(printer, Response, DateTimeOffset.Now.ToUnixTimeSeconds());
                    return;
                }
                if (Response.ErrorException != null)
                {
                    throw new Exception($"Exception in UploadAndStartPrinting: {Response.ErrorException.Message}");
                }
                StartWebSocketMessageTimers();
            } catch (Exception ex)
            {
                Console.Error.WriteLine($"{ex}");
            }
      
      
        }



        /// <summary>
        /// Set up event handlers for WebSocket events and timers for cyclic websocket calls.
        /// </summary>
        private void InitWebSocket()
        {
            WebSocket.OnMessage += (sender, e) =>
            {
                // TODO: add try catch in case msg is not deserializeable 
                // TODO: test serialization
                var message = JsonSerializer.Deserialize<RepetierBaseMessage>(e.Data, DeserialisationHelper.IngoreNullableFieldsOptions);
                long timestamp = DateTimeOffset.Now.ToUnixTimeSeconds();
                var containsEvents = message.HasEvents != null && message.HasEvents == true;

                if (message.CallBackId == -1 || containsEvents)
                {
                    var events = JsonSerializer.Deserialize<List<RepetierBaseEvent>>(message.Data, DeserialisationHelper.IngoreNullableFieldsOptions);
                    events.ForEach(repetierEvent => HandleEvent(repetierEvent, timestamp));
                }
                else
                {
                    HandleMessage(message);
                }
            };

            WebSocket.OnOpen += (sender, e) => Console.WriteLine("Connection established");

            WebSocket.OnClose += (sender, e) =>
            {
                Console.WriteLine($"Connection closed: {e.Reason}");
            };

            WebSocket.OnError += (sender, e) =>
            {
                WebSocket.Close(CloseStatusCode.Abnormal);
                throw new Exception($"Exception in WebSocket.OnError: {e.Message}");
            };
        }

        private void HandleMessage(RepetierBaseMessage message)
        {
            var commandStr = CommandManager.CommandIdentifierFor(message.CallBackId);

            switch (commandStr)
            {
                case CommandConstants.LOGIN:
                    var loginMessage = JsonSerializer.Deserialize<LoginMessage>(message.Data);
                    // TODO: 
                    OnResponseReceived?.Invoke(loginMessage);
                    break;
                case CommandConstants.LOGOUT:
                    // TODO: 
                    // No payload
                    break;
                case CommandConstants.LIST_PRINTER:
                    {
                        var ListprintersMessage = JsonSerializer.Deserialize<ListPrinterMessage>(message.Data);
                        OnResponseReceived?.Invoke(ListprintersMessage);
                        var printers = ListprintersMessage.Printers;
                        // TODO: 
                    }
                    break;
                case CommandConstants.STATE_LIST:
                    {
                        var stateListMessage = JsonSerializer.Deserialize<StateListMessage>(message.Data);
                        var printers = stateListMessage.PrinterStates;
                        OnResponseReceived?.Invoke(stateListMessage);
                        foreach (var entry in printers)
                        {
                            // TODO: event for each printer?
                        }
                    }
                    break;
                case CommandConstants.RESPONSE:
                    {
                        var responseMessage = JsonSerializer.Deserialize<ResponseMessage>(message.Data);
                        OnResponseReceived?.Invoke(responseMessage);
                    }
                    break;
                case CommandConstants.MESSAGES:
                    {
                        var messagesMessage = JsonSerializer.Deserialize<List<Message>>(message.Data);
                        OnMessagesReceived?.Invoke(messagesMessage);
                    }
                    break;
                case CommandConstants.LIST_MODELS:
                    {
                        var modelList = JsonSerializer.Deserialize<List<Model>>(message.Data);
                    }
                    break;
                case CommandConstants.LIST_JOBS:
                    {
                        var modelsInJobQueue = JsonSerializer.Deserialize<List<Model>>(message.Data);
                    }
                    break;
                default:
                    break;
            }

        }

        private void HandleEvent(RepetierBaseEvent repetierBaseEvent, long timestamp)
        {
            var printer = repetierBaseEvent.Printer;
            var eventData = repetierBaseEvent.Data;

            switch (repetierBaseEvent.Event)
            {
                case EventConstants.LOGOUT:
                    OnLogoutReceived?.Invoke(timestamp);
                    break;
                case EventConstants.LOGIN_REQUIRED:
                    OnLoginRequiredReceived?.Invoke(timestamp);
                    break;
                case EventConstants.USER_CREDENTIALS:
                    var userCredentialsEvent = JsonSerializer.Deserialize<UserCredentialsEvent>(eventData);
                    OnUserCredentialsReceived?.Invoke(userCredentialsEvent, timestamp);
                    break;
                case EventConstants.PRINTER_LIST_CHANGED:
                    var printerListChangedEvent = JsonSerializer.Deserialize<PrinterListChangedEvent>(eventData);
                    OnPrinterListChanged?.Invoke(printerListChangedEvent.Printers, timestamp);
                    break;
                case EventConstants.MESSAGES_CHANGED:
                    OnMessagesChanged?.Invoke(timestamp);
                    SendCommand(MessagesCommand.Instance, printer);
                    break;
                case EventConstants.LOG:
                    var logEvent = JsonSerializer.Deserialize<LogEvent>(eventData);
                    OnLogReceived?.Invoke(logEvent);
                    break;
                case EventConstants.JOBS_CHANGED:
                    OnJobsChanged?.Invoke(printer, timestamp);
                    break;
                case EventConstants.JOB_FINISHED:
                    var jobFinishedEvent = JsonSerializer.Deserialize<JobFinishedEvent>(eventData);
                    OnJobFinishedReceived?.Invoke(printer, jobFinishedEvent, timestamp);
                    break;
                case EventConstants.JOB_KILLED:
                    var jobKilledEvent = JsonSerializer.Deserialize<JobKilledEvent>(eventData);
                    OnJobKilledReceived?.Invoke(printer, jobKilledEvent, timestamp);
                    break;
                case EventConstants.JOB_STARTED:
                    var jobStartedEvent = JsonSerializer.Deserialize<JobStartedEvent>(eventData);
                    OnJobStartedReceived?.Invoke(printer, jobStartedEvent, timestamp);
                    break;
                case EventConstants.STATE:
                    var printerStateChangedEvent = JsonSerializer.Deserialize<PrinterStateChangeEvent>(eventData);
                    OnPrinterStateReceived?.Invoke(printer, printerStateChangedEvent.PrinterState, timestamp);
                    break;
                case EventConstants.TEMP:
                    var tempChangeEvent = JsonSerializer.Deserialize<TempChangeEvent>(eventData);
                    OnTempChangeReceived?.Invoke(printer, tempChangeEvent, timestamp);
                    break;
                case EventConstants.CHANGE_FILAMENT:
                    OnChangeFilamentReceived?.Invoke(printer, timestamp);
                    break;
                default:
                    break;
            }
        }

        public void SendCommand(ICommandData command, string printer = "printer")
        {
            // TODO: send active printer or empty?
            var baseCommand = CommandManager.CommandWithId(command, printer);
            WebSocket.Send(baseCommand.ToBytes());
        }

        public void CancelPeriodicTask(string commandName)
        {
            if (PeriodicTaskMap.TryGetValue(commandName, out var periodicTask))
            {
                periodicTask.Cancel();
            }
        }

        /// <summary>
        /// TODO: print time is not available without prusa slicer - remove it?
        /// Initialize and start timers for cyclic WebSocket calls to repetier server.
        /// </summary>
        private void StartWebSocketMessageTimers(int estimatedPrintTime = 5000)
        {
            var estimationExists = estimatedPrintTime != -1;
            var defaultPollRate = 10000;
            var pingDelay = int.Parse(ConfigurationManager.AppSettings["ping-delay"]);
            var progressRate = estimationExists ? CalculatePollingInterval(estimatedPrintTime, 0.005) : defaultPollRate;
            var stateRate = estimationExists ? CalculatePollingInterval(estimatedPrintTime, 0.01) : defaultPollRate;

            PeriodicTaskMap.Add(CommandConstants.LIST_PRINTER, new PeriodicTask(() => SendListPrinters()));
            PeriodicTaskMap.Add(CommandConstants.STATE_LIST, new PeriodicTask(() => SendStateList()));
        }

        /// <summary>
        /// Retrieve printer name or API-key (or both) via REST-API
        /// If ApiKey or PrinterSlug are not empty, they will not be overwritten by the retrieved information. 
        /// </summary>
        public RepetierServerInformation GetRepetierServerInfo()
        {
            var response = RestClient.Execute(ApiConstants.PRINTER_INFO_REQUEST);
            return JsonSerializer.Deserialize<RepetierServerInformation>(response.Content);
        }

        /// <summary>
        /// TODO: is this still needed?
        /// Calculates polling intervals for the WebSocket calls based on the estimated print time and a given factor.
        /// </summary>
        /// <param name="estimatedPrintingTime"></param>
        /// <param name="factor"></param>
        /// <returns></returns>
        private int CalculatePollingInterval(int estimatedPrintingTime, double factor)
        {
            int pollinginterval = (int)(estimatedPrintingTime * 1000 * factor);
            return pollinginterval < 3000 ? 3000 : pollinginterval;
        }

        // TODO: add extension class for this?
        #region command convenience methods

        /// <summary>
        ///  Send a single "ping" message to the repetier server.
        /// </summary>
        private void SendPing()
        {
            SendCommand(PingCommand.Instance);
        }

        /// <summary>
        /// Send a single "listPrinters" message to the repetier rerver.
        /// The response to a "listPrinters" command contains the current print progress.
        /// </summary>
        public void SendListPrinters()
        {
            SendCommand(ListPrinterCommand.Instance);
        }

        /// <summary>
        /// Send a single "stateList" message to the repetier server.
        /// The response to a "stateList" command contains information regarding the printer state.
        /// </summary>
        public void SendStateList(bool includeHistory = false)
        {
            SendCommand(new StateListCommand(includeHistory));
        }

        /// <summary>
        /// Send a single "stopJob" meassage to repetier server.
        /// The printer will stop the current print and triggers a "jobKilled" event
        /// </summary>
        public void SendStopJob()
        {
            SendCommand(StopJobCommand.Instance);
        }

        public void Login(string user, string password)
        {
            SendCommand(new LoginCommand(user, password));
        }

        public void Logout()
        {
            SendCommand(LogoutCommand.Instance);
        }

        #endregion
    }
}