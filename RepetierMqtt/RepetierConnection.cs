using System;
using System.Collections.Generic;
using System.Configuration;
using System.Text.Json;
using RepetierMqtt.Models.Events;
using RepetierMqtt.Models;
using RepetierMqtt.Models.Commands;
using RepetierMqtt.Models.Messages;
using RepetierMqtt.Util;
using RestSharp;
using WebSocketSharp;
using RepetierMqtt.Config;
using RepetierMqtt.Models.Config;

namespace RepetierMqtt
{
    /// <summary>
    /// Represents a WebSocket connection to the Repetier Server.
    /// </summary>
    public partial class RepetierConnection
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

        public event PrintQueueChangedReceived OnPrintQueueChanged;
        public delegate void PrintQueueChangedReceived(string printer);

        public event FoldersChangedReceived OnFoldersChanged;
        public delegate void FoldersChangedReceived();

        public event EepromLoadStartedReceived OnEepromLoadStarted;
        public delegate void EepromLoadStartedReceived();

        public event EepromDataReceived OnEepromEntryReceived;
        public delegate void EepromDataReceived(EepromDataEvent eepromData);

        public event PrinterConfigReceived OnPrinterConfigReceived;
        public delegate void PrinterConfigReceived(string printer, PrinterConfig printerConfig);

        public event FirmwareDataReceived OnFirmwareDataReceived;
        public delegate void FirmwareDataReceived(FirmwareInfo firmwareInfo);

        public event MoveEventReceived OnMoveReceived;
        public delegate void MoveEventReceived(string printer, MoveEvent moveEvent);
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
        public event EventHandler<StatusMessage> OnUserCreateResponseReceived;
        public event EventHandler<StatusMessage> OnUserDeleteResponseReceived;
        public event EventHandler<UserListMessage> OnUserListReceived;
        #endregion

        /// <summary>
        /// Command -> PeriodicTask
        /// e.g. "stateList" -> () -> {}
        /// </summary>
        private Dictionary<string, PeriodicTask> PeriodicTaskMap { get; set; } = new Dictionary<string, PeriodicTask>();

        private PeriodicTask PeriodicPing { get; set; }

        private WebSocket WebSocket { get; set; }
        private RestClient RestClient { get; set; }

        public string BaseURL { get; }                 // server baseUrl (IP-Address + port, e.g.: "127.0.0.1:3344")
        private string ApiKey { get; set; }             // Authentification for Repetier-Server
        protected bool ApiKeyProvided { get; set; }
        private string SessionKey { get; set; }
        private string LoginName { get; set; }
        private string Password { get; set; }
        protected string ActivePrinter { get; private set; }


        /// <summary>
        /// BaseURL must be provided by the user. 
        /// TODO: Provide constructors with other authentication
        /// </summary>
        /// <param name="baseURL"></param>
        /// <param name="ApiKey"></param>
        /// <param name="lang"> can be omitted </param>
        /// <param name="session"> can be omitted </param>
        public RepetierConnection(string baseURL, string ApiKey, string lang = "US")
        {
            this.BaseURL = baseURL;
            this.ApiKey = ApiKey;
            ApiKeyProvided = !string.IsNullOrEmpty(ApiKey.Trim());
            InitDefaultHandlers();
            RestClient = new RestClient($"http://{this.BaseURL}");
            WebSocket = new WebSocket($"ws://{this.BaseURL}/socket/?lang={lang}&apikey={this.ApiKey}");
        }

        public RepetierConnection(string baseURL, string login, string password, string lang = "US")
        {
            this.BaseURL = baseURL;
            this.ApiKey = ApiKey;
            this.LoginName = login;
            this.Password = password;
            ApiKeyProvided = !string.IsNullOrEmpty(ApiKey.Trim());
            InitDefaultHandlers();
            RestClient = new RestClient($"http://{this.BaseURL}");
            WebSocket = new WebSocket($"ws://{this.BaseURL}/socket/?lang={lang}");
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
            InitWebSocket();
            WebSocket.Connect();
            PeriodicPing = new PeriodicTask(() => this.SendPing(), 1000);
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
                        SessionKey = message.SessionId;
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
                        var statusMessage = JsonSerializer.Deserialize<StatusMessage>(message.Data);
                        OnUserCreateResponseReceived?.Invoke(this, statusMessage);
                    }
                    break;
                case CommandConstants.UPDATE_USER:
                    {
                        // no payload 
                    }
                    break;
                case CommandConstants.DELETE_USER:
                    {
                        var statusMessage = JsonSerializer.Deserialize<StatusMessage>(message.Data);
                        OnUserDeleteResponseReceived?.Invoke(this, statusMessage);
                    }
                    break;
                case CommandConstants.USER_LIST:
                    {
                        // TODO: rework/check message/deserialization
                        var userList = JsonSerializer.Deserialize<UserListMessage>(message.Data);
                        OnUserListReceived?.Invoke(this, userList);
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
                case EventConstants.LOGIN_REQUIRED:
                    OnLoginRequiredReceived?.Invoke(timestamp);
                    break;
                case EventConstants.LOGOUT:
                    OnLogoutReceived?.Invoke(timestamp);
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
                case EventConstants.MOVE:
                    var moveEvent = JsonSerializer.Deserialize<MoveEvent>(eventData);
                    OnMoveReceived?.Invoke(printer, moveEvent);
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
                case EventConstants.PRINT_QUEUE_CHANGED:
                    OnPrintQueueChanged?.Invoke(printer);
                    break;
                case EventConstants.FOLDERS_CHANGED:
                    OnFoldersChanged?.Invoke();
                    break;
                case EventConstants.EEPROM_CLEAR:
                    OnEepromLoadStarted?.Invoke();
                    break;
                case EventConstants.EEPROM_DATA:
                    var eepromDataEvent = JsonSerializer.Deserialize<EepromDataEvent>(eventData);
                    OnEepromEntryReceived?.Invoke(eepromDataEvent);
                    break;
                case EventConstants.STATE:
                    var printerStateChangedEvent = JsonSerializer.Deserialize<PrinterStateChangeEvent>(eventData);
                    OnPrinterStateReceived?.Invoke(printer, printerStateChangedEvent.PrinterState, timestamp);
                    break;
                case EventConstants.CONFIG:
                    var printerConfigEvent = JsonSerializer.Deserialize<PrinterConfig>(eventData);
                    OnPrinterConfigReceived?.Invoke(printer, printerConfigEvent);
                    break;
                case EventConstants.FIRMWARE_CHANGED:
                    var firmwareData = JsonSerializer.Deserialize<FirmwareData>(eventData);
                    OnFirmwareDataReceived?.Invoke(firmwareData.Firmware);
                    break;
                case EventConstants.TEMP:
                    var tempChangeEvent = JsonSerializer.Deserialize<TempChangeEvent>(eventData);
                    OnTempChangeReceived?.Invoke(printer, tempChangeEvent, timestamp);
                    break;
                case EventConstants.SETTING_CHANGED:
                    /* Payload: List of new settings.
                     * Gets triggered when a global setting variable got changed.
                     */
                    // TODO: poco
                    // TODO: event
                    break;
                case EventConstants.PRINTER_SETTING_CHANGED:
                    /* Payload: {key:"key",value:"new value"}
                     * Gets triggered everytime a printer setting gets changed.
                     */
                    var printerSettings = JsonSerializer.Deserialize<KeyValuePair<string, string>>(eventData);
                    // TODO: event
                    break;
                case EventConstants.MODEL_GROUPLIST_CHANGED:
                    /* Payload: None
                     * Gets triggered if a printer changes the list of groups for g-codes stored.
                     * Clients should use this to update their lists.
                     */
                    break;
                case EventConstants.PREPARE_JOB:
                    /* Payload: None
                     * Gets triggered if a model gets copied to job directory.
                     * Since feedback is normally instant and large files can take some time this allows giving some feedback.
                     */
                    break;
                case EventConstants.PREPARE_JOB_FINIHSED:
                    /* Payload: None
                     * Gets triggered at the end of copy model to job.
                     * Used to hide messages from prepareJob.
                     */
                    break;
                case EventConstants.CHANGE_FILAMENT_REQUESTED:
                    OnChangeFilamentReceived?.Invoke(printer, timestamp);
                    break;
                case EventConstants.REMOTE_SERVERS_CHANGED:
                    /* Payload: None
                     * Gets triggered when the content of remote lists has changed.
                     */
                    break;
                case EventConstants.GET_EXTERNAL_LINKS:
                    /* Payload: None
                     * Returns a list of external links associated with the server.
                     * Can be extended in the repetier-server.xml files.
                     */
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
        /// Initialize and start timers for cyclic WebSocket calls to repetier server.
        /// </summary>
        private void StartWebSocketMessageTimers()
        {
            PeriodicTaskMap.Add(CommandConstants.LIST_PRINTER, new PeriodicTask(() => this.SendListPrinters()));
            PeriodicTaskMap.Add(CommandConstants.STATE_LIST, new PeriodicTask(() => this.SendStateList()));
        }

        /// <summary>
        /// Retrieve printer name or API-key (or both) via REST-API
        /// If ApiKey or PrinterSlug are not empty, they will not be overwritten by the retrieved information. 
        /// </summary>
        public RepetierServerInformation GetRepetierServerInfo()
        {
            var response = this.RestClient.Execute(ApiConstants.PRINTER_INFO_REQUEST);
            return JsonSerializer.Deserialize<RepetierServerInformation>(response.Content);
        }

        /// <summary>
        /// Attempt login with the previously set credentials
        /// </summary>
        public void Login()
        {
            if (!String.IsNullOrEmpty(LoginName) && !String.IsNullOrEmpty(Password))
            {
                Password = this.HashPassword(SessionKey, LoginName, Password);
                this.SendCommand(new LoginCommand(LoginName, Password));
            }
        }

        /// <summary>
        /// Attempt login with the given user and password
        /// </summary>
        /// <param name="user"></param>
        /// <param name="password"></param>
        public void Login(string user, string password)
        {
            if (!String.IsNullOrEmpty(SessionKey))
            {
                var pw = this.HashPassword(SessionKey, user, password);
                this.SendCommand(new LoginCommand(user, pw));
            }
        }
        private string HashPassword(string sessionKey, string login, string password)
        {
            return CommandManager.MD5(sessionKey + CommandManager.MD5(login + password));
        }


    }
}