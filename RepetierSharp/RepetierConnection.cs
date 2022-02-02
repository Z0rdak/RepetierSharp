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

        #region Common EventHandler
        public event LogEventReceived OnLogReceived;
        public delegate void LogEventReceived(Log logEvent);

        public event JobFinishedReceivedHandler OnJobFinished;
        public delegate void JobFinishedReceivedHandler(string printer, JobState jobFinished);

        public event JobStartedReceivedHandler OnJobStarted;
        public delegate void JobStartedReceivedHandler(string printer, JobStarted jobStarted);

        public event JobStartFailedHandler OnRESTCallFailed;
        public delegate void JobStartFailedHandler(string printer, IRestResponse response);

        public event JobKilledReceivedHandler OnJobKilled;
        public delegate void JobKilledReceivedHandler(string printer, JobState jobKilled);

        public event JobDeactivatedReceivedHandler OnJobDeactivated;
        public delegate void JobDeactivatedReceivedHandler(string printer, JobState jobKilled);

        public event JobsChangedReceivedHandler OnJobsChanged;
        public delegate void JobsChangedReceivedHandler(string printer);

        public event PrinterStateReceivedHandler OnPrinterState;
        public delegate void PrinterStateReceivedHandler(string printer, PrinterState printerState);

        public event TemperatureChangeReceivedHandler OnTempChange;
        public delegate void TemperatureChangeReceivedHandler(string printer, Temp tempChange);

        public event PrinterListChangedReceivedHandler OnPrinterListChanged;
        public delegate void PrinterListChangedReceivedHandler(List<Printer> printerList);

        public event PrinterSettingChangedReceivedHandler OnPrinterSettingChanged;
        public delegate void PrinterSettingChangedReceivedHandler(SettingChanged printerSetting, string printer);

        public event UserCredentialsReceivedHandler OnUserCredentialsReceived;
        public delegate void UserCredentialsReceivedHandler(UserCredentials userCredentials);

        public event LoginRequiredReceivedHandler OnLoginRequired;
        public delegate void LoginRequiredReceivedHandler();

        public event MessagesReceivedHandler OnMessagesReceived;
        public delegate void MessagesReceivedHandler(List<Message> messages);

        public event LoginResultReceivedHandler OnLoginResult;
        public delegate void LoginResultReceivedHandler(LoginMessage loginResult);

        public event PermissionDeniedEvent OnPermissionDenied;
        public delegate void PermissionDeniedEvent(int command);

        public event PrinterStatesReceivedHandler OnPrinterStates;
        public delegate void PrinterStatesReceivedHandler(StateListMessage printerStates);

#endregion

        public event RepetierEventReceived OnEvent;
        public delegate void RepetierEventReceived(string eventName, string printer, IRepetierEvent repetierEvent);

        public event CommandResponseReceived OnResponse;
        public delegate void CommandResponseReceived(int callbackID, string command, IRepetierMessage message);

        /// <summary>
        /// Fired whenever a event from the repetier server is received. 
        /// The payload is the raw event itself (content of the data field of the json from the documentation).
        /// </summary>
        /// <param name="eventName"> Name of the received event </param>
        /// <param name="printer"> Printer associated with the event or empty if global </param>
        /// <param name="payload"> Event payload </param>
        public delegate void RawRepetierEventReceived(string eventName, string printer, byte[] payload);
        public event RawRepetierEventReceived OnRawEvent;

        /// <summary>
        /// Fired whenever a command response from the repetier server is received. 
        /// The payload is the raw response itself (content of the data field of the json from the documentation).
        /// </summary>
        /// <param name="callbackID"> CallBackId to identify the received command response </param>
        /// <param name="command"> Name of the command associated with the received response </param>
        /// <param name="response"> Command response payload </param>
        public delegate void RawCommandResponseReceived(int callbackID, string command, byte[] response);
        public event RawCommandResponseReceived OnRawResponse;

        /// <summary>
        /// Gets called when the connection with the server is established successfully for the first time.
        /// </summary>
        public delegate void RepetierServerConnected();
        public event RepetierServerConnected OnRepetierConnected;

        #region Properties
        /// <summary>
        /// TODO: add possibility to change Ping interval
        /// </summary>
        private Timer PeriodicPingTimer { get; set; }
        private uint PingInterval { get; set; } = 10000;
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
        public string ActivePrinter { get; set; } = "";
        #endregion

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
        /// Set up event handlers for WebSocket events and responses.
        /// </summary>
        private void InitWebSocket()
        {
            WebSocketClient.ReconnectTimeout = TimeSpan.FromSeconds(30);
            WebSocketClient.ReconnectionHappened.Subscribe(info =>
            {
                if (info.Type == ReconnectionType.Initial)
                {
                    OnRepetierConnected?.Invoke();
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
                            Task.Run(() =>
                            {
                                OnRawEvent?.Invoke(repEvent.Event, repEvent.Printer, Encoding.UTF8.GetBytes(eventData.GetRawText()));
                                HandleEvent(repEvent, eventData.GetRawText());
                            });
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
                            Task.Run(() =>
                            {
                                OnRawResponse?.Invoke(message.CallBackId, CommandManager.CommandIdentifierFor(message.CallBackId), Encoding.UTF8.GetBytes(data.GetRawText()));
                                HandleMessage(message, data);
                            });
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

        private void SendPing()
        {
            SendCommand(PingCommand.Instance, typeof(PingCommand));
        }

        /// <summary>
        /// Closes the WebSocket connection
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
            var request = new RestRequest($"/printer/job/{printerName}", Method.POST)
                .AddFile("gcode", gcodeFilePath)
                .AddHeader("Content-Type", "multipart/form-data")
                .AddParameter("a", "upload")
                .AddParameter("sess", SessionId)
                .AddParameter("name", GCODEFileName);
            return WithApiKeyHeader(request);
        }

        private IRestRequest WithApiKeyHeader(IRestRequest request)
        {
            if (AuthType == AuthenticationType.ApiKey)
            {
                request = request.AddHeader("x-api-key", ApiKey);
            }
            return request;
        }

        /// <summary>
        /// Create a REST request for uploading a gcode file
        /// </summary>
        /// <param name="gcodeFilePath"> The path of the file to upload (/path/file.gcode) </param>
        /// <param name="printer"> Printer slug to upload to </param>
        /// <param name="group"> Group to add gcode to </param>
        /// <param name="overwrite"> Flag to overwrite existing file with the same name </param>
        /// <returns></returns>
        private IRestRequest UploadModel(string gcodeFilePath, string printer, string group, bool overwrite = false)
        {
            var GCODEFileName = Path.GetFileNameWithoutExtension(gcodeFilePath);
            var request = new RestRequest($"/printer/model/{printer}", Method.POST)
                .AddFile("gcode", gcodeFilePath)
                .AddHeader("Content-Type", "multipart/form-data")
                .AddParameter("a", "upload")
                .AddParameter("sess", SessionId)
                .AddParameter("group", group)
                .AddParameter("overwrite", overwrite)
                .AddParameter("name", GCODEFileName);
            return WithApiKeyHeader(request);
        }

        /// <summary>
        /// Upload a gcode file via REST API
        /// </summary>
        /// <param name="gcodeFilePath"> The path of the file to upload (/path/file.gcode) </param>
        /// <param name="printer"> Printer slug to upload to </param>
        /// <param name="group"> Group to add gcode to </param>
        /// <param name="overwrite"> Flag to overwrite existing file with the same name </param>
        public void UploadGCode(string gcodeFilePath, string group, string printer, bool overwrite = false)
        {
            // TODO: Event
            /*
             * [gcodeInfoUpdated@Delta]: {"data":{"list":"models","modelId":18,"modelPath":"/home/demorepetier/storage/7/printer/Delta/models/00000018_Fuss_0.2mm_ABS_EL-11_2h24m.gin","slug":"Delta"},"event":"gcodeInfoUpdated","printer":"Delta"}
             */
            try
            {
                var request = UploadModel(gcodeFilePath, printer, group, overwrite);
                var Response = RestClient.Execute(request);
                HandleRestResponse(Response, printer);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"{ex}");
            }
        }
        
        private void HandleRestResponse(IRestResponse response, string printer)
        {
            if (response.StatusCode != HttpStatusCode.OK)
            {
                OnRESTCallFailed?.Invoke(printer, response);
                return;
            }
            if (response.ErrorException != null)
            {
                throw new Exception($"Exception executing RestRequest: {response.ErrorException.Message}");
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
                HandleRestResponse(Response, printer);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"{ex}");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="commandDataObject"></param>
        private void HandleMessage(RepetierBaseMessage message, JsonElement commandDataObject)
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
                    OnLoginResult?.Invoke(loginMessage);
                    OnResponse?.Invoke(message.CallBackId, commandStr, loginMessage);
                    break;
                case CommandConstants.LOGOUT:
                    OnResponse?.Invoke(message.CallBackId, commandStr, null);
                    break;
                case CommandConstants.LIST_PRINTER:
                    var ListprintersMessage = JsonSerializer.Deserialize<List<Printer>>(cmdData);
                    var printerMsg = new ListPrinterMessage() { Printers = ListprintersMessage };
                    OnResponse?.Invoke(message.CallBackId, commandStr, printerMsg);
                    OnPrinterListChanged?.Invoke(ListprintersMessage);
                    break;
                case CommandConstants.STATE_LIST:
                    var stateListMessage = JsonSerializer.Deserialize<Dictionary<string, PrinterState>>(cmdData);
                    var stateMsg = new StateListMessage() { PrinterStates = stateListMessage };
                    OnResponse?.Invoke(message.CallBackId, commandStr, stateMsg);
                    OnPrinterStates?.Invoke(stateMsg);
                    break;
                case CommandConstants.RESPONSE:
                    var responseMessage = JsonSerializer.Deserialize<Models.Messages.ResponseMessage>(cmdData);
                    OnResponse?.Invoke(message.CallBackId, commandStr, responseMessage);
                    break;
                case CommandConstants.MESSAGES:
                    var messagesMessage = JsonSerializer.Deserialize<List<Message>>(cmdData);
                    OnMessagesReceived?.Invoke(messagesMessage);
                    break;
                case CommandConstants.LIST_MODELS:
                    var modelList = JsonSerializer.Deserialize<List<Model>>(cmdData);
                    // OnModelListReceived?.Invoke(this, modelList);
                    break;
                case CommandConstants.LIST_JOBS:
                    // fixme
                    var jobList = JsonSerializer.Deserialize<List<Model>>(cmdData);
                    // OnJobListReceived?.Invoke(this, jobList);
                    break;
                case CommandConstants.MODEL_INFO:
                    var modelInfo = JsonSerializer.Deserialize<Model>(cmdData);
                    // OnModelInfoReceived?.Invoke(this, modelInfo);
                    break;
                case CommandConstants.JOB_INFO:
                    var jobInfo = JsonSerializer.Deserialize<Model>(cmdData);
                    // OnJobInfoReceived?.Invoke(this, jobInfo);
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
        private void HandleEvent(RepetierBaseEvent repetierEvent, string eventAsJson)
        {
            var json = JsonSerializer.Deserialize<JsonDocument>(eventAsJson);
            var dataJsonObject = json.RootElement.GetProperty("data");
            var eventData = dataJsonObject.GetRawText();
            repetierEvent.Data = Encoding.UTF8.GetBytes(dataJsonObject.GetRawText());


            // TODO: event updatePrinterState
            #region event comment updatePrinterState
            /* 
             * {
    "callback_id": -1,
    "data": [
        {
            "data": {
                "activeExtruder": 0,
                "autostartNextPrint": false,
                "debugLevel": 6,
                "doorOpen": false,
                "extruder": [
                    {
                        "error": 0,
                        "output": 39.2156867980957,
                        "tempRead": 200.0,
                        "tempSet": 200.0
                    },
                    {
                        "error": 0,
                        "output": 0.0,
                        "tempRead": 20.0,
                        "tempSet": 0.0
                    }
                ],
                "fans": [
                    {
                        "on": false,
                        "voltage": 0
                    }
                ],
                "filterFan": false,
                "firmware": "Repetier_0.92.6",
                "firmwareURL": "https://github.com/repetier/Repetier-Firmware/",
                "flowMultiply": 100,
                "hasXHome": true,
                "hasYHome": true,
                "hasZHome": true,
                "heatedBeds": [
                    {
                        "error": 0,
                        "output": 0.0,
                        "tempRead": 20.0,
                        "tempSet": 0.0
                    }
                ],
                "heatedChambers": [],
                "layer": 0,
                "lights": 0,
                "notification": "",
                "numExtruder": 2,
                "powerOn": true,
                "rec": false,
                "sdcardMounted": true,
                "shutdownAfterPrint": false,
                "speedMultiply": 100,
                "volumetric": false,
                "x": 126.20200347900391,
                "y": 106.04100036621089,
                "z": 5.1999998092651367
            },
            "event": "updatePrinterState",
            "printer": "Cartesian"
        },
        {
            "data": {
                "activeExtruder": 0,
                "autostartNextPrint": false,
                "debugLevel": 6,
                "doorOpen": false,
                "extruder": [
                    {
                        "error": 0,
                        "output": 39.2156867980957,
                        "tempRead": 200.0,
                        "tempSet": 200.0
                    },
                    {
                        "error": 0,
                        "output": 0.0,
                        "tempRead": 20.0,
                        "tempSet": 0.0
                    }
                ],
                "fans": [
                    {
                        "on": false,
                        "voltage": 0
                    }
                ],
                "filterFan": false,
                "firmware": "Repetier_0.92.6",
                "firmwareURL": "https://github.com/repetier/Repetier-Firmware/",
                "flowMultiply": 100,
                "hasXHome": true,
                "hasYHome": true,
                "hasZHome": true,
                "heatedBeds": [
                    {
                        "error": 0,
                        "output": 0.0,
                        "tempRead": 20.0,
                        "tempSet": 0.0
                    }
                ],
                "heatedChambers": [],
                "layer": 0,
                "lights": 0,
                "notification": "",
                "numExtruder": 2,
                "powerOn": true,
                "rec": false,
                "sdcardMounted": true,
                "shutdownAfterPrint": false,
                "speedMultiply": 100,
                "volumetric": false,
                "x": 126.20200347900391,
                "y": 106.04100036621089,
                "z": 5.1999998092651367
            },
            "event": "updatePrinterState",
            "printer": "Cartesian"
        },
        {
            "data": {
                "activeExtruder": 0,
                "autostartNextPrint": false,
                "debugLevel": 6,
                "doorOpen": false,
                "extruder": [
                    {
                        "error": 0,
                        "output": 39.2156867980957,
                        "tempRead": 200.0,
                        "tempSet": 200.0
                    },
                    {
                        "error": 0,
                        "output": 0.0,
                        "tempRead": 20.0,
                        "tempSet": 0.0
                    }
                ],
                "fans": [
                    {
                        "on": false,
                        "voltage": 0
                    }
                ],
                "filterFan": false,
                "firmware": "Repetier_0.92.6",
                "firmwareURL": "https://github.com/repetier/Repetier-Firmware/",
                "flowMultiply": 100,
                "hasXHome": true,
                "hasYHome": true,
                "hasZHome": true,
                "heatedBeds": [
                    {
                        "error": 0,
                        "output": 0.0,
                        "tempRead": 20.0,
                        "tempSet": 0.0
                    }
                ],
                "heatedChambers": [],
                "layer": 0,
                "lights": 0,
                "notification": "",
                "numExtruder": 2,
                "powerOn": true,
                "rec": false,
                "sdcardMounted": true,
                "shutdownAfterPrint": false,
                "speedMultiply": 100,
                "volumetric": false,
                "x": 0.0,
                "y": 0.0,
                "z": 20.0
            },
            "event": "updatePrinterState",
            "printer": "Cartesian"
        }
    ],
    "eventList": true
}
             */
            #endregion

            switch (repetierEvent.Event)
            {
                case EventConstants.JOBS_CHANGED:
                    OnEvent?.Invoke(repetierEvent.Event, repetierEvent.Printer, null);
                    this.ActivePrinter = repetierEvent.Printer;
                    break;
                case EventConstants.TIMER_30:
                case EventConstants.TIMER_60:
                case EventConstants.TIMER_300:
                case EventConstants.TIMER_1800:
                case EventConstants.TIMER_3600:
                    RepetierTimer timer = (RepetierTimer)int.Parse(repetierEvent.Event[5..]);
                    if (QueryIntervals.ContainsKey(timer))
                    {
                        QueryIntervals[timer].ForEach(command =>
                        {
                            SendCommand(command, command.GetType());
                        });
                    }
                    OnEvent?.Invoke(repetierEvent.Event, repetierEvent.Printer, null);
                    break;
                case EventConstants.LOGIN_REQUIRED:
                    if (AuthType == AuthenticationType.Credentials)
                    {
                        var loginRequiredEvent = JsonSerializer.Deserialize<LoginRequired>(eventData);
                        OnEvent?.Invoke(repetierEvent.Event, repetierEvent.Printer, loginRequiredEvent);
                        SessionId = loginRequiredEvent.SessionId;
                        Login();
                    }
                    else
                    {
                        throw new InvalidOperationException("Credentials not supplied.");
                    }
                    OnLoginRequired?.Invoke();

                    break;
                case EventConstants.USER_CREDENTIALS:
                    var userCredentialsEvent = JsonSerializer.Deserialize<UserCredentials>(eventData);
                    OnUserCredentialsReceived?.Invoke(userCredentialsEvent);
                    OnEvent?.Invoke(repetierEvent.Event, repetierEvent.Printer, userCredentialsEvent);
                    break;
                case EventConstants.PRINTER_LIST_CHANGED:
                    var printerListChangedEvent = JsonSerializer.Deserialize<PrinterListChanged>(eventAsJson);
                    OnPrinterListChanged?.Invoke(printerListChangedEvent.Printers);
                    OnEvent?.Invoke(repetierEvent.Event, repetierEvent.Printer, printerListChangedEvent);
                    /* 
                    var printerList = JsonSerializer.Deserialize<Printer[]>(eventAsJson);
                    var printerListChangedEvent = new PrinterListChanged() { Printers = new List<Printer>(printerList) };
                    OnPrinterListChanged?.Invoke(new List<Printer>(printerList));
                    OnRepetierEvent?.Invoke(repetierEvent.Event, repetierEvent.Printer, printerListChangedEvent);
                    */
                    break;
                case EventConstants.MESSAGES_CHANGED:
                    OnEvent?.Invoke(repetierEvent.Event, repetierEvent.Printer, null);
                    SendCommand(MessagesCommand.Instance, repetierEvent.Printer);
                    break;
                case EventConstants.MOVE:
                    var moveEvent = JsonSerializer.Deserialize<Move>(eventData);
                    OnEvent?.Invoke(repetierEvent.Event, repetierEvent.Printer, moveEvent);
                    break;
                case EventConstants.LOG:
                    var logEvent = JsonSerializer.Deserialize<Log>(eventData);
                    OnLogReceived?.Invoke(logEvent);
                    OnEvent?.Invoke(repetierEvent.Event, repetierEvent.Printer, logEvent);
                    break;
                case EventConstants.JOB_FINISHED:
                    var jobFinishedEvent = JsonSerializer.Deserialize<JobState>(eventData);
                    OnJobFinished?.Invoke(repetierEvent.Printer, jobFinishedEvent);
                    OnEvent?.Invoke(repetierEvent.Event, repetierEvent.Printer, jobFinishedEvent);
                    break;
                case EventConstants.JOB_KILLED:
                    var jobKilledEvent = JsonSerializer.Deserialize<JobState>(eventData);
                    OnJobKilled?.Invoke(repetierEvent.Printer, jobKilledEvent);
                    OnEvent?.Invoke(repetierEvent.Event, repetierEvent.Printer, jobKilledEvent);
                    break;
                case EventConstants.JOB_STARTED:
                    var jobStartedEvent = JsonSerializer.Deserialize<JobStarted>(eventData);
                    OnJobStarted?.Invoke(repetierEvent.Printer, jobStartedEvent);
                    OnEvent?.Invoke(repetierEvent.Event, repetierEvent.Printer, jobStartedEvent);
                    break;
                case EventConstants.EEPROM_DATA:
                    var eepromDataEvents = JsonSerializer.Deserialize<List<EepromData>>(eventData);
                    eepromDataEvents.ForEach(eepromData => OnEvent?.Invoke(repetierEvent.Event, repetierEvent.Printer, eepromData));
                    break;
                case EventConstants.STATE:
                    var printerStateChangedEvent = JsonSerializer.Deserialize<PrinterStateChange>(eventData);
                    OnPrinterState?.Invoke(repetierEvent.Printer, printerStateChangedEvent.PrinterState);
                    OnEvent?.Invoke(repetierEvent.Event, repetierEvent.Printer, printerStateChangedEvent);
                    break;
                case EventConstants.CONFIG:
                    var printerConfigEvent = JsonSerializer.Deserialize<PrinterConfig>(eventData); 
                    OnEvent?.Invoke(repetierEvent.Event, repetierEvent.Printer, printerConfigEvent);
                    break;
                case EventConstants.FIRMWARE_CHANGED:
                    var firmwareData = JsonSerializer.Deserialize<FirmwareData>(eventData);
                    OnEvent?.Invoke(repetierEvent.Event, repetierEvent.Printer, firmwareData);
                    break;
                case EventConstants.TEMP:
                    var tempChangeEvent = JsonSerializer.Deserialize<Temp>(eventData);
                    OnEvent?.Invoke(repetierEvent.Event, repetierEvent.Printer, tempChangeEvent);
                    OnTempChange?.Invoke(repetierEvent.Printer, tempChangeEvent);
                    break;
                case EventConstants.SETTING_CHANGED:
                    //var settings = JsonSerializer.Deserialize<List<SettingChangedEvent>>(eventData);
                    //OnRepetierEvent?.Invoke(repetierEvent.Event, repetierEvent.Printer, settings);
                    break;
                case EventConstants.PRINTER_SETTING_CHANGED:
                    var printerSetting = JsonSerializer.Deserialize<SettingChanged>(eventData);
                    OnPrinterSettingChanged?.Invoke(printerSetting, repetierEvent.Printer);
                    OnEvent?.Invoke(repetierEvent.Event, repetierEvent.Printer, printerSetting);
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
                    OnEvent?.Invoke(repetierEvent.Event, repetierEvent.Printer, null);
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
            if (!(baseCommand.Command.CommandIdentifier == CommandConstants.PING))
            {
                Console.WriteLine($"Send [{baseCommand.CallbackId}]: {baseCommand}");
            }
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