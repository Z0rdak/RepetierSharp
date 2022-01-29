using System;
using System.Collections.Generic;
using System.Text.Json;
using RepetierSharp.Models.Events;
using RepetierSharp.Models;
using RepetierSharp.Models.Commands;
using RepetierSharp.Models.Messages;
using RepetierSharp.Util;
using RestSharp;
using RepetierSharp.Config;
using RepetierSharp.Models.Config;
using System.Text;
using System.Threading;
using System.Net;
using Websocket.Client;
using System.Threading.Tasks;
using RepetierSharp.Extentions;
using System.IO;

namespace RepetierSharp
{
    public partial class RepetierConnection
    {
        #region oldeventhandler
        /*
        
        public event ModelGroupListChangedReceivedHandler OnModelGroupListChangedReceived;
        public delegate void ModelGroupListChangedReceivedHandler(string printer, long timestamp);
 
        public event ChangeFilamentReceivedHandler OnChangeFilamentReceived;
        public delegate void ChangeFilamentReceivedHandler(string printer, long timestamp);

        public event OnPrepareJobReceivedHandler OnPrepareJobReceived;
        public delegate void OnPrepareJobReceivedHandler(string printer, long timestamp);

        public event OnPrepareJobFinishedReceivedHandler OnPrepareJobFinishedReceived;
        public delegate void OnPrepareJobFinishedReceivedHandler(string printer, long timestamp);

        public event ExternalLinksReceivedHandler OnExternalLinksReceived;
        public delegate void ExternalLinksReceivedHandler(long timestamp);

        public event RemoteServersChangedReceivedHandler OnRemoteServersChangedReceived;
        public delegate void RemoteServersChangedReceivedHandler(long timestamp);
        
        public event EepromLoadStartedReceived OnEepromLoadStarted;
        public delegate void EepromLoadStartedReceived();

        public event EepromDataReceived OnEepromEntryReceived;
        public delegate void EepromDataReceived(EepromDataEvent eepromData);

        public event FoldersChangedReceived OnFoldersChanged;
        public delegate void FoldersChangedReceived();
        
        public event PrinterConfigReceived OnPrinterConfigReceived;
        public delegate void PrinterConfigReceived(string printer, PrinterConfig printerConfig);

        public event FirmwareDataReceived OnFirmwareDataReceived;
        public delegate void FirmwareDataReceived(FirmwareInfo firmwareInfo);
        
        public event PrintQueueChangedReceived OnPrintQueueChanged;
        public delegate void PrintQueueChangedReceived(string printer);

        public event MoveEventReceived OnMoveReceived;
        public delegate void MoveEventReceived(string printer, MoveEvent moveEvent);

        public event MessagesAvailableHandler OnMessagesAvailable;
        public delegate void MessagesAvailableHandler(long timestamp);

        public event LogoutReceivedHandler OnLogoutReceived;
        public delegate void LogoutReceivedHandler(long timestamp);

        */
        #endregion

        public event LogEventReceived OnLogReceived;
        public delegate void LogEventReceived(LogEvent logEvent);

        public event JobFinishedReceivedHandler OnJobFinishedReceived;
        public delegate void JobFinishedReceivedHandler(string printer, JobFinishedEvent jobFinished, long timestamp);

        public event JobStartedReceivedHandler OnJobStartedReceived;
        public delegate void JobStartedReceivedHandler(string printer, JobStartedEvent jobStarted, long timestamp);

        public event JobStartedFailedHandler OnJobStartedFailed;
        public delegate void JobStartedFailedHandler(string printer, IRestResponse response, long timestamp);

        public event JobKilledReceivedHandler OnJobKilledReceived;
        public delegate void JobKilledReceivedHandler(string printer, JobKilledEvent jobKilled, long timestamp);

        public event JobsChangedReceivedHandler OnJobsChanged;
        public delegate void JobsChangedReceivedHandler(string printer, long timestamp);

        public event PrinterStateReceivedHandler OnPrinterStateReceived;
        public delegate void PrinterStateReceivedHandler(string printer, PrinterState printerState, long timestamp);

        public event TemperatureChangeReceivedHandler OnTempChangeReceived;
        public delegate void TemperatureChangeReceivedHandler(string printer, TempChangeEvent tempChange, long timestamp);

        // same as OnPrinterListReceived ?
        public event PrinterListChangedReceivedHandler OnPrinterListChanged;
        public delegate void PrinterListChangedReceivedHandler(List<Printer> printerList, long timestamp);

        public event PrinterSettingChangedReceivedHandler OnPrinterSettingChangedReceived;
        public delegate void PrinterSettingChangedReceivedHandler(SettingChangedEvent printerSetting, string printer, long timestamp);

        public event UserCredentialsReceivedHandler OnUserCredentialsReceived;
        public delegate void UserCredentialsReceivedHandler(UserCredentialsEvent userCredentials, long timestamp);

        public event LoginRequiredReceivedHandler OnLoginRequiredReceived;
        public delegate void LoginRequiredReceivedHandler(long timestamp);
        // TODO: replace eventhandler with delegates
        public event EventHandler<List<Message>> OnMessagesReceived;
        public event EventHandler<LoginMessage> OnLoginMessageReceived;
        public event EventHandler<List<Printer>> OnPrinterListReceived;
        public event EventHandler<Dictionary<string, PrinterState>> OnStateListReceived;
        public event EventHandler<List<Model>> OnModelListReceived;
        public event EventHandler<Model> OnModelInfoReceived;
        public event EventHandler<Model> OnJobInfoReceived;
        public event EventHandler<List<Model>> OnJobListReceived;

        /* ################################################################## */
        // TODO: Add events to all messages / events
        public event RepetierEventReceived OnRepetierEvent;
        public delegate void RepetierEventReceived(string eventName, string printer, IRepetierEvent repetierEvent);

        public event CommandResponseReceived OnResponse;
        public delegate void CommandResponseReceived(int callbackID, string command, IRepetierMessage message);

        public event PermissionDeniedEvent OnPermissionDenied;
        public delegate void PermissionDeniedEvent(int command);
        /* ################################################################## */

        /// <summary>
        /// TODO: add possibility to change Ping interval
        /// </summary>
        private Timer PeriodicPingTimer { get; set; }
        private uint PingInterval { get; set; } = 3000;
        private Dictionary<RepetierTimer, List<ICommandData>> QueryIntervals { get; set; } = new Dictionary<RepetierTimer, List<ICommandData>>();
        private CommandManager CommandManager { get; set; } = new CommandManager();

        private WebsocketClient WebSocketClient { get; set; }
        private RestClient RestClient { get; set; }

        private bool UseTls { get; set; }
        public string BaseURL { get; private set; }
        private string ApiKey { get; set; }
        private AuthenticationType AuthType { get; set; } = AuthenticationType.None;
        private string SessionId { get; set; }
        private bool LongLivedSession { get; set; }
        private string LoginName { get; set; }
        private string Password { get; set; }
        private string LangKey { get; set; }

        public string ActivePrinter { get; private set; } = "";

        public RepetierConnection() { }

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
        /// Open WebSocket connection to repetier server and start communication.
        /// </summary>
        public void Connect()
        {
            InitWebSocket();
            WebSocketClient.StartOrFail();
            PeriodicPingTimer = CyclicCallHelper.CreateCyclicCall(() => this.SendPing(), PingInterval);
        }

        /// <summary>
        /// Set up event handlers for WebSocket events and timers for cyclic websocket calls.
        /// </summary>
        private void InitWebSocket()
        {
            WebSocketClient.ReconnectTimeout = TimeSpan.FromSeconds(30);
            WebSocketClient.ReconnectionHappened.Subscribe(info =>
            {
                if (info.Type == ReconnectionType.Initial)
                {
                    Console.WriteLine($"[WebSocket] Connection established ({info.Type})");
                    // Only query messages at this point when using a api-key or no auth
                    if (this.AuthType != AuthenticationType.Credentials)
                    {
                        this.QueryOpenMessages();
                    }
                }
            });

            WebSocketClient.DisconnectionHappened.Subscribe(info =>
            {
                Console.WriteLine($"[WebSocket] Connection closed: {info.Type} | {info.CloseStatus} | {info.CloseStatusDescription}");
            });

            WebSocketClient.MessageReceived.Subscribe(msg =>
            {
                if (msg.MessageType != System.Net.WebSockets.WebSocketMessageType.Text || msg.Text == null && msg.Text.Length == 0)
                {
                    return;
                }
                try
                {
                    long timestamp = DateTimeOffset.Now.ToUnixTimeSeconds();
                    byte[] msgBytes = Encoding.UTF8.GetBytes(msg.Text);
                    var message = JsonSerializer.Deserialize<RepetierBaseMessage>(msgBytes);
                    var containsEvents = message.HasEvents != null && message.HasEvents == true;

                    if (string.IsNullOrEmpty(SessionId) && !string.IsNullOrEmpty(message.SessionId))
                    {
                        SessionId = message.SessionId;
                    }
                    
                    var json = JsonSerializer.Deserialize<JsonDocument>(msgBytes);
                    var data = json.RootElement.GetProperty("data");
                    if (message.CallBackId == -1 || containsEvents)
                    {
                        // TODO: Custom JsonConverter
                        foreach (var eventData in data.EnumerateArray())
                        {
                            var repEvent = JsonSerializer.Deserialize<RepetierBaseEvent>(eventData.GetRawText());
                            Task.Run(() => HandleEvent(repEvent, eventData.GetRawText(), timestamp)); ;
                        }
                    }
                    else
                    {
                        if (msg.Text.Contains("permissionDenied"))
                        {
                            OnPermissionDenied?.Invoke(message.CallBackId);
                        }
                        else
                        {
                            Task.Run(() => HandleMessage(message, data, timestamp));
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.Error.WriteLine("[WebSocket] Error processing message from repetier server:");
                    Console.Error.WriteLine($"[WebSocket] {msg.Text}");
                    Console.Error.WriteLine($"{ex.Message}");
                }
            });
        }

        /// <summary>
        ///  Send a single "ping" message to the repetier server.
        /// </summary>
        public void SendPing()
        {
            SendCommand(PingCommand.Instance, typeof(PingCommand));
        }

        /// <summary>
        /// Closes WebSocket connection
        /// </summary>
        public void Close()
        {
            this.PeriodicPingTimer.Dispose();
            this.PeriodicPingTimer = null;
            this.WebSocketClient.Dispose();
            this.WebSocketClient = null;
        }

        private IRestRequest StartPrintRequest(string gcodeFilePath, string printerName)
        {
            var GCODEFileName = Path.GetFileNameWithoutExtension(gcodeFilePath);
            var request = new RestRequest($"/printer/{printerName}", Method.POST)
                .AddFile("gcode", GCODEFileName)
                .AddHeader("Content-Type", "multipart/form-data")
                .AddParameter("a", "upload")
                .AddParameter("sess", SessionId)
                .AddParameter("name", GCODEFileName);
            if (AuthType == AuthenticationType.ApiKey)
            {
                request = request.AddHeader("x-api-key", ApiKey);
            }
            return request;
        }

        // TODO: Test
        private IRestRequest UploadModel(string gcodeFilePath, string printer, string group, bool overwrite = false)
        {
            var GCODEFileName = Path.GetFileNameWithoutExtension(gcodeFilePath);
            var request = new RestRequest($"/printer/model/{printer}", Method.POST)
                .AddFile("gcode", GCODEFileName)
                .AddHeader("Content-Type", "multipart/form-data")
                .AddParameter("a", "upload")
                .AddParameter("sess", SessionId)
                .AddParameter("group", group)
                .AddParameter("overwrite", overwrite)
                .AddParameter("name", GCODEFileName);
            if (AuthType == AuthenticationType.ApiKey)
            {
                request = request.AddHeader("x-api-key", ApiKey);
            }
            return request;
        }

        private IRestRequest StartPrintRequest(string gcodeFilePath)
        {
            return StartPrintRequest(gcodeFilePath, ActivePrinter);
        }

        public void UploadGCode(string gcodeFilePath, string group, string printer, bool overwrite = false)
        {
            try
            {
                var request = UploadModel(gcodeFilePath, printer, group, overwrite);
                var Response = RestClient.Execute(request);
                if (Response.StatusCode != HttpStatusCode.OK)
                {
                    // TODO event 
                    return;
                }
                if (Response.ErrorException != null)
                {
                    throw new Exception($"Exception in UploadGCode: {Response.ErrorException.Message}");
                }
            }
            catch (Exception ex)
            {
                // TODO: event?
                Console.Error.WriteLine($"{ex}");
            }
        }

        /// <summary>
        /// Upload given G-Code and start the printing process via REST-API.
        /// </summary>
        /// <param name="GCODEFilePath"></param>
        public void UploadAndStartPrint(string GCODEFilePath, string printer)
        {
            try
            {
                var request = StartPrintRequest(GCODEFilePath, printer);
                var Response = RestClient.Execute(request);
                if (Response.StatusCode != HttpStatusCode.OK)
                {
                    OnJobStartedFailed?.Invoke(printer, Response, DateTimeOffset.Now.ToUnixTimeSeconds());
                    return;
                }
                if (Response.ErrorException != null)
                {
                    throw new Exception($"Exception in UploadAndStartPrinting: {Response.ErrorException.Message}");
                }
            }
            catch (Exception ex)
            {
                // TODO: event?
                Console.Error.WriteLine($"{ex}");
            }
        }

        private void HandleMessage(RepetierBaseMessage message, JsonElement commandDataObject, long timestamp)
        {
            var commandStr = CommandManager.CommandIdentifierFor(message.CallBackId);
            var cmdData = commandDataObject.GetRawText();
            message.Data = Encoding.UTF8.GetBytes(commandDataObject.GetRawText());

            switch (commandStr)
            {
                case CommandConstants.LOGIN:
                    var loginMessage = JsonSerializer.Deserialize<LoginMessage>(cmdData);
                    if (string.IsNullOrEmpty(loginMessage.Error))
                    {
                        this.QueryOpenMessages();
                    }
                    OnLoginMessageReceived?.Invoke(this, loginMessage);
                    OnResponse?.Invoke(message.CallBackId, commandStr, loginMessage);
                    break;
                case CommandConstants.LOGOUT:
                    OnResponse?.Invoke(message.CallBackId, commandStr, null);
                    break;
                case CommandConstants.LIST_PRINTER:
                    var ListprintersMessage = JsonSerializer.Deserialize<List<Printer>>(cmdData);
                    var printerMsg = new ListPrinterMessage() { Printers = ListprintersMessage };
                    OnResponse?.Invoke(message.CallBackId, commandStr, printerMsg);
                    OnPrinterListReceived?.Invoke(this, ListprintersMessage);
                    break;
                case CommandConstants.STATE_LIST:
                    var stateListMessage = JsonSerializer.Deserialize<Dictionary<string, PrinterState>>(cmdData);
                    var stateMsg = new StateListMessage() { PrinterStates = stateListMessage };
                    OnResponse?.Invoke(message.CallBackId, commandStr, stateMsg);
                    OnStateListReceived?.Invoke(this, stateListMessage);
                    break;
                case CommandConstants.RESPONSE:
                    var responseMessage = JsonSerializer.Deserialize<Models.Messages.ResponseMessage>(cmdData);
                    OnResponse?.Invoke(message.CallBackId, commandStr, responseMessage);
                    break;
                case CommandConstants.MESSAGES:
                    var messagesMessage = JsonSerializer.Deserialize<List<Message>>(cmdData);
                    OnMessagesReceived?.Invoke(this, messagesMessage);
                    break;
                case CommandConstants.LIST_MODELS:
                    var modelList = JsonSerializer.Deserialize<List<Model>>(cmdData);
                    OnModelListReceived?.Invoke(this, modelList);
                    break;
                case CommandConstants.LIST_JOBS:
                    var jobList = JsonSerializer.Deserialize<List<Model>>(cmdData);
                    OnJobListReceived?.Invoke(this, jobList);
                    break;
                case CommandConstants.MODEL_INFO:
                    var modelInfo = JsonSerializer.Deserialize<Model>(cmdData);
                    OnModelInfoReceived?.Invoke(this, modelInfo);
                    break;
                case CommandConstants.JOB_INFO:
                    var jobInfo = JsonSerializer.Deserialize<Model>(cmdData);
                    OnJobInfoReceived?.Invoke(this, jobInfo);
                    break;
                case CommandConstants.REMOVE_JOB:
                case CommandConstants.SEND:
                case CommandConstants.COPY_MODEL:
                    OnResponse?.Invoke(message.CallBackId, commandStr, null);
                    break;
                case CommandConstants.EMERGENCY_STOP:
                    // TODO: event
                    // no payload
                    OnResponse?.Invoke(message.CallBackId, commandStr, null);
                    break;
                case CommandConstants.ACTIVATE:
                    // TODO: event
                    // no payload
                    break;
                case CommandConstants.DEACTIVATE:
                    // TODO: event
                    // no payload
                    break;
                case CommandConstants.CREATE_USER:
                    var createStatusMessage = JsonSerializer.Deserialize<StatusMessage>(cmdData);
                    OnResponse?.Invoke(message.CallBackId, commandStr, createStatusMessage);
                    break;
                case CommandConstants.UPDATE_USER:
                    // TODO: event
                    // no payload 
                    break;
                case CommandConstants.DELETE_USER:
                    var deleteStatusMessage = JsonSerializer.Deserialize<StatusMessage>(cmdData);
                    OnResponse?.Invoke(message.CallBackId, commandStr, deleteStatusMessage);
                    break;
                case CommandConstants.USER_LIST:
                    // TODO: rework/check message/deserialization
                    var userList = JsonSerializer.Deserialize<UserListMessage>(cmdData);
                    OnResponse?.Invoke(message.CallBackId, commandStr, userList);
                    // payload: { "loginRequired": true, "users": [ { "id": 1, "login": "repetier", "permissions": 15 } ] }
                    break;
                case CommandConstants.START_JOB:
                    /* no payload */
                    break;
                case CommandConstants.STOP_JOB:
                    /* no payload */
                    break;
                case CommandConstants.CONTINUE_JOB:
                    /* no payload */
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Handles an incoming repetier event.
        /// The event data is then forwarded by calling their corresponding event handlers.
        /// </summary>
        private void HandleEvent(RepetierBaseEvent repetierEvent, string eventAsJson, long timestamp)
        {
            var json = JsonSerializer.Deserialize<JsonDocument>(eventAsJson);
            var dataJsonObject = json.RootElement.GetProperty("data");
            var eventData = dataJsonObject.GetRawText();
            repetierEvent.Data = Encoding.UTF8.GetBytes(dataJsonObject.GetRawText());

            switch (repetierEvent.Event)
            {
                case EventConstants.JOBS_CHANGED:
                    OnRepetierEvent?.Invoke(repetierEvent.Event, repetierEvent.Printer, null);
                    this.ActivePrinter = repetierEvent.Printer;
                    break;

                /*
                 * {"callback_id":-1,"data":[{"data":{"activeExtruder":0,"autostartNextPrint":false,"debugLevel":6,"doorOpen":false,"extruder":[{"error":0,"output":39.2156867980957,"tempRead":200.0,"tempSet":200.0},{"error":0,"output":0.0,"tempRead":20.0,"tempSet":0.0}],"fans":[{"on":false,"voltage":0}],"filterFan":false,"firmware":"Repetier_0.92.6","firmwareURL":"https://github.com/repetier/Repetier-Firmware/","flowMultiply":100,"hasXHome":true,"hasYHome":true,"hasZHome":true,"heatedBeds":[{"error":0,"output":0.0,"tempRead":20.0,"tempSet":0.0}],"heatedChambers":[],"layer":0,"lights":0,"notification":"","numExtruder":2,"powerOn":true,"rec":false,"sdcardMounted":true,"shutdownAfterPrint":false,"speedMultiply":100,"volumetric":false,"x":126.2020034790039,"y":106.0410003662109,"z":5.199999809265137},"event":"updatePrinterState","printer":"Cartesian"},{"data":{"activeExtruder":0,"autostartNextPrint":false,"debugLevel":6,"doorOpen":false,"extruder":[{"error":0,"output":39.2156867980957,"tempRead":200.0,"tempSet":200.0},{"error":0,"output":0.0,"tempRead":20.0,"tempSet":0.0}],"fans":[{"on":false,"voltage":0}],"filterFan":false,"firmware":"Repetier_0.92.6","firmwareURL":"https://github.com/repetier/Repetier-Firmware/","flowMultiply":100,"hasXHome":true,"hasYHome":true,"hasZHome":true,"heatedBeds":[{"error":0,"output":0.0,"tempRead":20.0,"tempSet":0.0}],"heatedChambers":[],"layer":0,"lights":0,"notification":"","numExtruder":2,"powerOn":true,"rec":false,"sdcardMounted":true,"shutdownAfterPrint":false,"speedMultiply":100,"volumetric":false,"x":126.2020034790039,"y":106.0410003662109,"z":5.199999809265137},"event":"updatePrinterState","printer":"Cartesian"},{"data":{"activeExtruder":0,"autostartNextPrint":false,"debugLevel":6,"doorOpen":false,"extruder":[{"error":0,"output":39.2156867980957,"tempRead":200.0,"tempSet":200.0},{"error":0,"output":0.0,"tempRead":20.0,"tempSet":0.0}],"fans":[{"on":false,"voltage":0}],"filterFan":false,"firmware":"Repetier_0.92.6","firmwareURL":"https://github.com/repetier/Repetier-Firmware/","flowMultiply":100,"hasXHome":true,"hasYHome":true,"hasZHome":true,"heatedBeds":[{"error":0,"output":0.0,"tempRead":20.0,"tempSet":0.0}],"heatedChambers":[],"layer":0,"lights":0,"notification":"","numExtruder":2,"powerOn":true,"rec":false,"sdcardMounted":true,"shutdownAfterPrint":false,"speedMultiply":100,"volumetric":false,"x":0.0,"y":0.0,"z":20.0},"event":"updatePrinterState","printer":"Cartesian"}],"eventList":true}
                 * 
                 * dispatcherCount
                 * 
                 */

                case EventConstants.TIMER_30:
                case EventConstants.TIMER_60:
                case EventConstants.TIMER_300:
                case EventConstants.TIMER_1800:
                case EventConstants.TIMER_3600:
                    RepetierTimer timer = (RepetierTimer) int.Parse(repetierEvent.Event[5..]);
                    if (QueryIntervals.ContainsKey(timer))
                    {
                        QueryIntervals[timer].ForEach(command =>
                        {
                            SendCommand(command, command.GetType());
                        });
                    }
                    OnRepetierEvent?.Invoke(repetierEvent.Event, repetierEvent.Printer, null);
                    break;
                case EventConstants.LOGIN_REQUIRED:
                    if (AuthType == AuthenticationType.Credentials)
                    {
                        var loginRequiredEvent = JsonSerializer.Deserialize<LoginRequiredEvent>(eventData);
                        OnRepetierEvent?.Invoke(repetierEvent.Event, repetierEvent.Printer, loginRequiredEvent);
                        SessionId = loginRequiredEvent.SessionId;
                        Login();
                    }
                    else
                    {
                        throw new InvalidOperationException("Credentials not supplied.");
                    }
                    OnLoginRequiredReceived?.Invoke(timestamp);
               
                    break;
                case EventConstants.USER_CREDENTIALS:
                    var userCredentialsEvent = JsonSerializer.Deserialize<UserCredentialsEvent>(eventData);
                    OnUserCredentialsReceived?.Invoke(userCredentialsEvent, timestamp);
                    OnRepetierEvent?.Invoke(repetierEvent.Event, repetierEvent.Printer, userCredentialsEvent);
                    break;
                case EventConstants.PRINTER_LIST_CHANGED:
                    var printerListChangedEvent = JsonSerializer.Deserialize<PrinterListChangedEvent>(eventAsJson);
                    OnPrinterListChanged?.Invoke(printerListChangedEvent.Printers, timestamp);
                    OnRepetierEvent?.Invoke(repetierEvent.Event, repetierEvent.Printer, printerListChangedEvent);
                    break;
                case EventConstants.MESSAGES_CHANGED:
                    OnRepetierEvent?.Invoke(repetierEvent.Event, repetierEvent.Printer, null);
                    SendCommand(MessagesCommand.Instance, typeof(MessagesCommand), repetierEvent.Printer);
                    break;
                case EventConstants.MOVE:
                    var moveEvent = JsonSerializer.Deserialize<MoveEvent>(eventData);
                    OnRepetierEvent?.Invoke(repetierEvent.Event, repetierEvent.Printer, moveEvent);
                    break;
                case EventConstants.LOG:
                    var logEvent = JsonSerializer.Deserialize<LogEvent>(eventData);
                    OnLogReceived?.Invoke(logEvent);
                    OnRepetierEvent?.Invoke(repetierEvent.Event, repetierEvent.Printer, logEvent);
                    break;
                case EventConstants.JOB_FINISHED:
                    var jobFinishedEvent = JsonSerializer.Deserialize<JobFinishedEvent>(eventData);
                    OnJobFinishedReceived?.Invoke(repetierEvent.Printer, jobFinishedEvent, timestamp);
                    OnRepetierEvent?.Invoke(repetierEvent.Event, repetierEvent.Printer, jobFinishedEvent);
                    break;
                case EventConstants.JOB_KILLED:
                    var jobKilledEvent = JsonSerializer.Deserialize<JobKilledEvent>(eventData);
                    OnJobKilledReceived?.Invoke(repetierEvent.Printer, jobKilledEvent, timestamp);
                    OnRepetierEvent?.Invoke(repetierEvent.Event, repetierEvent.Printer, jobKilledEvent);
                    break;
                case EventConstants.JOB_STARTED:
                    var jobStartedEvent = JsonSerializer.Deserialize<JobStartedEvent>(eventData);
                    OnJobStartedReceived?.Invoke(repetierEvent.Printer, jobStartedEvent, timestamp);
                    OnRepetierEvent?.Invoke(repetierEvent.Event, repetierEvent.Printer, jobStartedEvent);
                    break;            
                case EventConstants.EEPROM_DATA:
                    var eepromDataEvent = JsonSerializer.Deserialize<EepromDataEvent>(eventData);
                    OnRepetierEvent?.Invoke(repetierEvent.Event, repetierEvent.Printer, eepromDataEvent);
                    break;
                case EventConstants.STATE:
                    var printerStateChangedEvent = JsonSerializer.Deserialize<PrinterStateChangeEvent>(eventData);
                    OnPrinterStateReceived?.Invoke(repetierEvent.Printer, printerStateChangedEvent.PrinterState, timestamp);
                    OnRepetierEvent?.Invoke(repetierEvent.Event, repetierEvent.Printer, printerStateChangedEvent);
                    break;
                case EventConstants.CONFIG:
                    var printerConfigEvent = JsonSerializer.Deserialize<PrinterConfig>(eventData);
                    OnRepetierEvent?.Invoke(repetierEvent.Event, repetierEvent.Printer, printerConfigEvent);
                    break;
                case EventConstants.FIRMWARE_CHANGED:
                    var firmwareData = JsonSerializer.Deserialize<FirmwareData>(eventData);
                    OnRepetierEvent?.Invoke(repetierEvent.Event, repetierEvent.Printer, firmwareData);
                    break;
                case EventConstants.TEMP:
                    var tempChangeEvent = JsonSerializer.Deserialize<TempChangeEvent>(eventData);
                    OnRepetierEvent?.Invoke(repetierEvent.Event, repetierEvent.Printer, tempChangeEvent);
                    OnTempChangeReceived?.Invoke(repetierEvent.Printer, tempChangeEvent, timestamp);
                    break;
                case EventConstants.SETTING_CHANGED:
                    //var settings = JsonSerializer.Deserialize<List<SettingChangedEvent>>(eventData);
                    //OnRepetierEvent?.Invoke(repetierEvent.Event, repetierEvent.Printer, settings);
                    break;
                case EventConstants.PRINTER_SETTING_CHANGED:
                    var printerSetting = JsonSerializer.Deserialize<SettingChangedEvent>(eventData);
                    OnPrinterSettingChangedReceived?.Invoke(printerSetting, repetierEvent.Printer, timestamp);
                    OnRepetierEvent?.Invoke(repetierEvent.Event, repetierEvent.Printer, printerSetting);
                    break;
                //case EventConstants.JOBS_CHANGED:
                case EventConstants.LOGOUT:
                case EventConstants.PRINT_QUEUE_CHANGED:
                case EventConstants.FOLDERS_CHANGED:
                case EventConstants.EEPROM_CLEAR:
                case EventConstants.MODEL_GROUPLIST_CHANGED:
                case EventConstants.PREPARE_JOB:
                case EventConstants.PREPARE_JOB_FINIHSED:
                case EventConstants.CHANGE_FILAMENT_REQUESTED:
                case EventConstants.REMOTE_SERVERS_CHANGED:
                case EventConstants.GET_EXTERNAL_LINKS:
                    OnRepetierEvent?.Invoke(repetierEvent.Event, repetierEvent.Printer, null);
                    break;
                default:
                    break;
            }
        }

        public void SendCommand(ICommandData command)
        {
            SendCommand(command, command.GetType(), ActivePrinter);
        }


        public void SendCommand(ICommandData command, string printer)
        {
            SendCommand(command, command.GetType(), printer);
        }

        protected void SendCommand(ICommandData command, Type commandType)
        {
            SendCommand(command, commandType, ActivePrinter);
        }

        protected void SendCommand(ICommandData command, Type commandType, string printer)
        {
            var baseCommand = CommandManager.CommandWithId(command, commandType, printer);
            //Console.WriteLine($"\n[Sending]: {baseCommand.Command.GetType().Name}({baseCommand.CallbackId})\n");
            Console.WriteLine(baseCommand.ToString()+"\n");
            Task.Run(() => WebSocketClient.Send(baseCommand.ToBytes()));
        }

        /// <summary>
        /// Attempt login with the previously set credentials
        /// </summary>
        public void Login()
        {
            if (!string.IsNullOrEmpty(LoginName) && !string.IsNullOrEmpty(Password))
            {
                Login(LoginName, Password);
            }
        }

        /// <summary>
        /// Attempt login with the given user and password
        /// </summary>
        /// <param name="user"></param>
        /// <param name="password"></param>
        public void Login(string user, string password)
        {
            if (!string.IsNullOrEmpty(SessionId))
            {
                var pw = CommandHelper.HashPassword(SessionId, user, password);
                this.SendCommand(new LoginCommand(user, pw, LongLivedSession), typeof(LoginCommand));
            }
        }


    }
}