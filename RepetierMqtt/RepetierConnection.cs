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

        #region Event handler for received messages
        public event EventHandler<IRepetierMessage> OnRepetierMessageReceived;
        public event EventHandler<List<Message>> OnMessagesReceived;
        public event EventHandler<LoginMessage> OnLoginMessageReceived;
        public event EventHandler<List<Printer>> OnPrinterListReceived;
        public event EventHandler<Dictionary<string, PrinterState>> OnStateListReceived;
        public event EventHandler<List<Model>> OnModelListReceived;
        public event EventHandler<Model> OnModelInfoReceived;
        public event EventHandler<Model> OnJobInfoReceived;
        public event EventHandler<List<Model>> OnJobListReceived;
        #endregion

        // TODO: Move implement move, printqueueChanged, foldersChanged, eepromClear, eepromChanged, 
        // config, firewareChanged, settingsChanged, printerSettingChanged, modelGroupListChanged,
        // prepareJob, prepareJobFinished, remoteServersChanged and getExternalLinks events

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

        public string ActivePrinter { get; private set; }


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
            }
            catch (Exception ex)
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
                try
                {
                    var message = JsonSerializer.Deserialize<RepetierBaseMessage>(e.Data, DeserialisationHelper.IngoreNullableFieldsOptions);
                    long timestamp = DateTimeOffset.Now.ToUnixTimeSeconds();
                    var containsEvents = message.HasEvents != null && message.HasEvents == true;

                    if (message.CallBackId == -1 || containsEvents)
                    {
                        var eventList = JsonSerializer.Deserialize<List<RepetierBaseEvent>>(message.Data, DeserialisationHelper.IngoreNullableFieldsOptions);
                        eventList.ForEach(repetierEvent => HandleEvent(repetierEvent, timestamp));
                    }
                    else
                    {
                        HandleMessage(message);
                    }
                }
                catch (Exception ex)
                {
                    Console.Error.WriteLine("Error processing message from repetier server");
                    Console.Error.WriteLine($"{ex.StackTrace}");
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

        /// <summary>
        /// Handles an incoming repetier message.
        /// Depeding on content of message a corresponding event with the data of the message is fired.
        /// </summary>
        /// <param name="message"></param>
        private void HandleMessage(RepetierBaseMessage message)
        {
            var commandStr = CommandManager.CommandIdentifierFor(message.CallBackId);

            switch (commandStr)
            {
                case CommandConstants.LOGIN:
                    var loginMessage = JsonSerializer.Deserialize<LoginMessage>(message.Data);
                    OnLoginMessageReceived?.Invoke(this, loginMessage);
                    OnRepetierMessageReceived?.Invoke(this, loginMessage);
                    break;
                case CommandConstants.LOGOUT:
                    OnRepetierMessageReceived?.Invoke(this, null);
                    // No payload
                    break;
                case CommandConstants.LIST_PRINTER:
                    {
                        var ListprintersMessage = JsonSerializer.Deserialize<ListPrinterMessage>(message.Data);
                        OnRepetierMessageReceived?.Invoke(this, ListprintersMessage);
                        OnPrinterListReceived?.Invoke(this, ListprintersMessage.Printers);
                    }
                    break;
                case CommandConstants.STATE_LIST:
                    {
                        var stateListMessage = JsonSerializer.Deserialize<StateListMessage>(message.Data);
                        OnRepetierMessageReceived?.Invoke(this, stateListMessage);
                        OnStateListReceived?.Invoke(this, stateListMessage.PrinterStates);
                    }
                    break;
                case CommandConstants.RESPONSE:
                    {
                        var responseMessage = JsonSerializer.Deserialize<ResponseMessage>(message.Data);
                        OnRepetierMessageReceived?.Invoke(this, responseMessage);
                    }
                    break;
                case CommandConstants.MESSAGES:
                    {
                        var messagesMessage = JsonSerializer.Deserialize<List<Message>>(message.Data);
                        OnMessagesReceived?.Invoke(this, messagesMessage);
                    }
                    break;
                case CommandConstants.LIST_MODELS:
                    {
                        var modelList = JsonSerializer.Deserialize<List<Model>>(message.Data);
                        OnModelListReceived?.Invoke(this, modelList);
                    }
                    break;
                case CommandConstants.LIST_JOBS:
                    {
                        var jobList = JsonSerializer.Deserialize<List<Model>>(message.Data);
                        OnJobListReceived?.Invoke(this, jobList);
                    }
                    break;
                case CommandConstants.MODEL_INFO:
                    {
                        var modelInfo = JsonSerializer.Deserialize<Model>(message.Data);
                        OnModelInfoReceived?.Invoke(this, modelInfo);
                    }
                    break;
                case CommandConstants.JOB_INFO:
                    {
                        var jobInfo = JsonSerializer.Deserialize<Model>(message.Data);
                        OnJobInfoReceived?.Invoke(this, jobInfo);
                    }
                    break;
                case CommandConstants.REMOVE_JOB:
                    {
                        // no payload
                    }
                    break;
                case CommandConstants.SEND:
                    {
                        // no payload
                    }
                    break;
                case CommandConstants.COPY_MODEL:
                    {
                        // TODO: event?
                        // no payload
                    }
                    break;
                case CommandConstants.EMERGENCY_STOP:
                    {
                        // TODO: event
                        // no payload
                    }
                    break;
                case CommandConstants.ACTIVATE:
                    {
                        // no payload
                    }
                    break;
                case CommandConstants.DEACTIVATE:
                    {
                        // no payload
                    }
                    break;
                case CommandConstants.CREATE_USER:
                    {
                        // payload: {"success":true}
                    }
                    break;
                case CommandConstants.UPDATE_USER:
                    {
                        // no payload 
                    }
                    break;
                case CommandConstants.DELETE_USER:
                    {
                        // payload: {"success":true}
                    }
                    break;
                case CommandConstants.USER_LIST:
                    {
                        // payload: { "loginRequired": true, "users": [ { "id": 1, "login": "repetier", "permissions": 15 } ] }
                    }
                    break;
                case CommandConstants.START_JOB:
                    {
                        // no payload
                    }
                    break;
                case CommandConstants.STOP_JOB:
                    {
                        // no payload
                    }
                    break;
                case CommandConstants.CONTINUE_JOB:
                    {
                        // no payload
                    }
                    break;
                default:
                    break;
            }

        }

        /// <summary>
        /// Handles an incoming repetier event.
        /// The event data is then forwarded by calling their corresponding event handlers.
        /// </summary>
        /// <param name="repetierBaseEvent"> Information of corresponding printer and event data</param>
        /// <param name="timestamp"> Unix timestamp of the event</param>
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

        public void SendCommand(ICommandData command)
        {
            var baseCommand = CommandManager.CommandWithId(command, this.ActivePrinter);
            WebSocket.Send(baseCommand.ToBytes());
        }

        private void SendCommand(ICommandData command, string printer)
        {
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

        public void EnqueueJob(int modelId, bool autostart = true)
        {
            SendCommand(new CopyModelCommand(modelId, autostart));
        }

        public void GetModelInfo(int modelId)
        {
            SendCommand(new ModelInfoCommand(modelId));
        }

        public void GetJobInfo(int jobId)
        {
            SendCommand(new JobInfoCommand(jobId));
        }

        public void StartJob(int jobId)
        {
            SendCommand(new StartJobCommand(jobId));
        }

        public void ContinueJob()
        {
            SendCommand(ContinueJobCommand.Instance);
        }

        public void RemoveJob(int jobId)
        {
            SendCommand(new RemoveJobCommand(jobId));
        }

        public void ActivatePrinter(string printerSlug)
        {
            SendCommand(new ActivateCommand(printerSlug));
        }

        public void DeactivatePrinter(string printerSlug)
        {
            SendCommand(new DeactivateCommand(printerSlug));
        }

        #endregion
    }
}