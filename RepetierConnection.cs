using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using RepetierSharp.Config;
using RepetierSharp.Extentions;
using RepetierSharp.Internal;
using RepetierSharp.Models;
using RepetierSharp.Models.Requests;
using RepetierSharp.Models.Config;
using RepetierSharp.Models.Events;
using RepetierSharp.Models.Messages;
using RepetierSharp.Util;
using RestSharp;
using Websocket.Client;
using static RepetierSharp.Internal.RepetierClientEvents;
using ResponseMessage = Websocket.Client.ResponseMessage;

namespace RepetierSharp
{

    public partial class RepetierConnection
    {
        private const string FilenameParam = "filename";
        private const string NameParam = "name";
        private const string SessionParam = "sess";
        private const string ActionParam = "a";
        private const string AutostartParam = "autostart";
        private const string UploadAction = "upload";
        public static readonly ContentType MultiPartFormData = "multipart/form-data";

        #region Client Events
        
        readonly RepetierClientEvents _clientEvents = new RepetierClientEvents();
        /// <summary>
        /// Fired when the connection with the server is established successfully for the first time.
        /// At this point, the sessionId should already be assigned to this RepetierConnection. 
        /// </summary>
        public event Func<RepetierConnectedEventArgs, Task> ConnectedAsync
        {
            add => _clientEvents.ConnectedEvent.AddHandler(value);
            remove => _clientEvents.ConnectedEvent.RemoveHandler(value);
        }
        
        /// <summary>
        /// Fired after establishment of the connection to the server if the server requires a login.
        /// This might be the case if no API-Key is supplied in the URI and the server has at least one user account.
        /// </summary>
        public event Func<LoginRequiredEventArgs, Task> LoginRequiredAsync
        {
            add => _clientEvents.LoginRequiredEvent.AddHandler(value);
            remove => _clientEvents.LoginRequiredEvent.RemoveHandler(value);
        }

        /// <summary>
        /// Fired when the login result response is received from the server after sending the login request.
        /// </summary>
        public event Func<LoginResultEventArgs, Task> LoginResultAsync
        {
            add => _clientEvents.LoginResultEvent.AddHandler(value);
            remove => _clientEvents.LoginResultEvent.RemoveHandler(value);
        }
        
        /// <summary>
        ///     Event which is fired when a command is not permitted for the current sessionId.
        /// </summary>
        public event Func<PermissionDeniedEventArgs, Task> PermissionDeniedAsync
        {
            add => _clientEvents.PermissionDeniedEvent.AddHandler(value);
            remove => _clientEvents.PermissionDeniedEvent.RemoveHandler(value);
        }
        
        /// <summary>
        ///  Event which is fired when a command is not permitted for the current sessionId.
        /// </summary>
        public event Func<SessionIdReceivedEventArgs, Task> SessionIdReceivedAsync
        {
            add => _clientEvents.SessionIdReceivedEvent.AddHandler(value);
            remove => _clientEvents.SessionIdReceivedEvent.RemoveHandler(value);
        }
        
        /// <summary>
        ///  Fired for received events from the repetier server. Note that temp, move and log events are not included here.
        ///  They can be enabled by setting the appropriate properties.
        /// </summary>
        public event Func<RepetierEventReceivedEventArgs, Task> RepetierEventReceivedAsync
        {
            add => _clientEvents.RepetierEventReceivedEvent.AddHandler(value);
            remove => _clientEvents.RepetierEventReceivedEvent.RemoveHandler(value);
        }
        
        /// <summary>
        /// Fired whenever an event from the repetier server is received. Unlike the RepetierEventReceivedAsync event,
        /// this event includes the raw event itself (content of the data field).
        /// This is mainly a fallback event for backwards compatibility and support events in newer repetier versions,
        /// which are not yet implemented by RepetierSharp.
        /// </summary>
        public event Func<RawRepetierEventReceivedEventArgs, Task> RawRepetierEventReceivedAsync
        {
            add => _clientEvents.RawRepetierEventReceivedEvent.AddHandler(value);
            remove => _clientEvents.RawRepetierEventReceivedEvent.RemoveHandler(value);
        }
        
        /// <summary>
        ///  Fired when a response from the server is received. This does not include the ping response.
        /// </summary>
        public event Func<RepetierResponseReceivedEventArgs, Task> RepetierResponseReceivedAsync
        {
            add => _clientEvents.RepetierResponseReceivedEvent.AddHandler(value);
            remove => _clientEvents.RepetierResponseReceivedEvent.RemoveHandler(value);
        }
        
        /// <summary>
        /// Fired when a raw response from the server is received. Unlike the RepetierResponseReceivedAsync event,
        /// this event includes the raw response itself (content of the data field).
        /// This is mainly a fallback event for backwards compatibility and support events in newer repetier versions,
        /// which are not yet implemented by RepetierSharp.
        /// </summary>
        public event Func<RawRepetierResponseReceivedEventArgs, Task> RawRepetierResponseReceivedAsync
        {
            add => _clientEvents.RawRepetierResponseReceivedEvent.AddHandler(value);
            remove => _clientEvents.RawRepetierResponseReceivedEvent.RemoveHandler(value);
        }
        #endregion
        
        #region PrintJob Events
        readonly RepetierPrintJobEvents _printJobEvents = new();
        public event Func<PrintJobStartedEventArgs, Task> PrintStartedAsync
        {
            add => _printJobEvents.PrintStartedEvent.AddHandler(value);
            remove => _printJobEvents.PrintStartedEvent.RemoveHandler(value);
        }
        
        /// <summary>
        /// Fired after a print job has been finished.
        /// </summary>
        public event Func<PrintJobFinishedEventArgs, Task> PrintFinishedAsync
        {
            add => _printJobEvents.PrintFinishedEvent.AddHandler(value);
            remove => _printJobEvents.PrintFinishedEvent.RemoveHandler(value);
        }
        
        /// <summary>
        /// Fired after a normal job has been finished or killed. Shortcut if you do not care why a job is not active anymore.
        /// </summary>
        public event Func<PrintJobDeactivatedEventArgs, Task> PrintDeactivatedAsync
        {
            add => _printJobEvents.PrintDeactivatedEvent.AddHandler(value);
            remove => _printJobEvents.PrintDeactivatedEvent.RemoveHandler(value);
        }
        /// <summary>
        /// Triggered after a print job has been killed.
        /// </summary>
        public event Func<PrintJobKilledEventArgs, Task> PrintKilledAsync
        {
            add => _printJobEvents.PrintKilledEvent.AddHandler(value);
            remove => _printJobEvents.PrintKilledEvent.RemoveHandler(value);
        }
        
        /// <summary>
        /// Triggered when a failure upon starting a print job was detected. This usually happens when the printer is not ready or on connection issues.
        /// </summary>
        public event Func<PrintJobStartFailedEventArgs, Task> PrintStartFailedAsync
        {
            add => _printJobEvents.PrintStartFailedEvent.AddHandler(value);
            remove => _printJobEvents.PrintStartFailedEvent.RemoveHandler(value);
        }
        
        /// <summary>
        /// Fired after a job got added. It might already be started for printing and info might be in state of evaluation at that point.
        /// </summary>
        public event Func<PrintJobAddedEventArgs, Task> PrintJobChangedAsync
        {
            add => _printJobEvents.PrintJobAddedEvent.AddHandler(value);
            remove => _printJobEvents.PrintJobAddedEvent.RemoveHandler(value);
        }
        #endregion
        
        #region PrintJob Events
        readonly RepetierPrinterEvents _printerEvents = new();

        /// <summary>
        /// Fired when the state of a printer changes.
        /// </summary>
        public event Func<StateChangedEventArgs, Task> PrinterStateReceivedAsync
        {
            add => _printerEvents.StateChangedEvent.AddHandler(value);
            remove => _printerEvents.StateChangedEvent.RemoveHandler(value);
        }

        /// <summary>
        /// Fired when the condition of a printer changes
        /// </summary>
        public event Func<ConditionChangedEventArgs, Task> PrinterConditionChangedAsync
        {
            add => _printerEvents.ConditionChangedEvent.AddHandler(value);
            remove => _printerEvents.ConditionChangedEvent.RemoveHandler(value);
        }

        /// <summary>
        /// Fired everytime settings of a printer change   
        /// </summary>
        public event Func<SettingChangedEventArgs, Task> PrinterSettingChangedAsync
        {
            add => _printerEvents.SettingChangedEvent.AddHandler(value);
            remove => _printerEvents.SettingChangedEvent.RemoveHandler(value);
        }

        /// <summary>
        ///   Fired when a printer has a change in it's stored g-file list.
        /// </summary>
        public event Func<JobsChangedEventArgs, Task> PrinterJobsChangedAsync
        {
            add => _printerEvents.JobsChangedEvent.AddHandler(value);
            remove => _printerEvents.JobsChangedEvent.RemoveHandler(value);
        }
        
        /// <summary>
        ///     Fired after the response for the printer activation request from was received.
        /// </summary>
        public event Func<ActivatedEventArgs, Task> PrinterActivatedAsync
        {
            add => _printerEvents.PrinterActivatedEvent.AddHandler(value);
            remove => _printerEvents.PrinterActivatedEvent.RemoveHandler(value);
        }
        /// <summary>
        ///     Fired after the response for the printer deactivation request from was received.
        /// </summary>
        public event Func<DeactivatedEventArgs, Task> PrinterDeactivatedAsync
        {
            add => _printerEvents.PrinterDeactivatedEvent.AddHandler(value);
            remove => _printerEvents.PrinterDeactivatedEvent.RemoveHandler(value);
        }    
        /// <summary>
        ///    Fired after the response for the emergency stop request from the printer was received.
        /// </summary>
        public event Func<EmergencyStopTriggeredEventArgs, Task> PrinterEmergencyStopTriggeredAsync
        {
            add => _printerEvents.EmergencyStopTriggeredEvent.AddHandler(value);
            remove => _printerEvents.EmergencyStopTriggeredEvent.RemoveHandler(value);
        }

        #endregion
    
    
        #region Common EventHandler
        public event LogEventReceived OnLogReceived;
        public delegate void LogEventReceived(Log logEvent);

        public event JobFinishedReceivedHandler OnJobFinished;
        public delegate void JobFinishedReceivedHandler(string printer, JobState jobFinished);

        public event JobStartedReceivedHandler OnJobStarted;
        public delegate void JobStartedReceivedHandler(string printer, JobStarted jobStarted);

        public event JobStartFailedHandler OnRestRequestFailed;
        public delegate void JobStartFailedHandler(string printer, RestResponse response);

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

        public event PrinterConditionChangedHandler OnPrinterConditionChanged;
        public delegate void PrinterConditionChangedHandler(PrinterConditionChanged printerConditionChange, string printer);
        
        public event PrinterSettingChangedReceivedHandler OnPrinterSettingChanged;
        public delegate void PrinterSettingChangedReceivedHandler(SettingChanged printerSetting, string printer);

        public event UserCredentialsReceivedHandler OnUserCredentialsReceived;
        public delegate void UserCredentialsReceivedHandler(UserCredentials userCredentials);

        /// <summary>
        /// Event which is fired when the server requires an authentication with user credentials.
        /// </summary>
        public event LoginRequiredReceivedHandler OnLoginRequired;
        public delegate void LoginRequiredReceivedHandler();

        public event MessagesReceivedHandler OnMessagesReceived;
        public delegate void MessagesReceivedHandler(List<Message> messages);

        /// <summary>
        /// Event which is fired after a login attempt. The LoginMessage holds information about the result.
        /// </summary>
        public event LoginResultReceivedHandler OnLoginResult;
        public delegate void LoginResultReceivedHandler(LoginMessage loginResult);

        /// <summary>
        /// Event which is fired when a command is not permitted for the current sessionId.
        /// </summary>
        public event PermissionDeniedEvent OnPermissionDenied;
        public delegate void PermissionDeniedEvent(int commandId);

        public event PrinterStatesReceivedHandler OnPrinterStates;
        public delegate void PrinterStatesReceivedHandler(StateListMessage printerStates);
        #endregion

        public event RepetierRequestSend? OnRequestSend;
        public delegate void RepetierRequestSend(RepetierBaseRequest request);
        
        public event RepetierRequestFailed? OnFailedRequest;
        public delegate void RepetierRequestFailed(RepetierBaseRequest request);
        

        
    
 

        #region Properties
        public uint PingInterval
        {
            get
            {
                return _pingInterval;
            }
            set
            {
                _pingInterval = value;
                Task.Run(async () => await SendExtendPing(_pingInterval));
            }
        }
        private long _lastPingTimestamp = 0;
        private uint _pingInterval = 10000;

        private readonly ILogger<RepetierConnection> _logger;

        private Dictionary<RepetierTimer, List<ICommandData>> QueryIntervals { get; } = new();
        private CommandManager CommandManager { get; } = new();
        private IWebsocketClient WebSocketClient { get; set; }
        private IRestClient RestClient { get; set; }
        private RepetierSession Session { get; init; }
        private string ActivePrinter { get; set; } = "";

        #endregion

        private RepetierConnection(ILogger<RepetierConnection>? logger = null)
        {
            _logger = logger ?? NullLogger<RepetierConnection>.Instance;
            OnSessionEstablished += sessionId =>
            {
                OnRepetierConnected?.Invoke(sessionId);
            };
        }

        /// <summary>
        ///     Retrieve printer name or API-key (or both) via REST-API
        ///     If ApiKey or PrinterSlug are not empty, they will not be overwritten by the retrieved information.
        /// </summary>
        public async Task<RepetierServerInformation> GetRepetierServerInfoAsync()
        {
            var response = await RestClient.ExecuteAsync(new RestRequest("/printer/info"));
            return JsonSerializer.Deserialize<RepetierServerInformation>(response.Content);
        }

        /// <summary>
        ///     Open WebSocket connection to repetier server and start communication.
        /// </summary>
        public async Task Connect()
        {
            WebSocketClient.ReconnectTimeout = TimeSpan.FromSeconds(15);
            WebSocketClient.ReconnectionHappened.Subscribe(OnReconnect);
            WebSocketClient.DisconnectionHappened.Subscribe(OnDisconnect);
            WebSocketClient.MessageReceived.Subscribe(OnMsgReceived);
            try
            {
                await WebSocketClient.StartOrFail()
                    .ContinueWith(t => SendPing());
            }
            catch (Exception e)
            {
                _logger?.LogError(e, "Error while starting websocket connection: {Error}", e.Message);
            }
        }

        private void OnMsgReceived(ResponseMessage msg)
        {
            // each message send to and from the Repetier Server is a valid JSON message
            if (msg.MessageType != WebSocketMessageType.Text || string.IsNullOrEmpty(msg.Text))
            {
                return;
            }

            try
            {
                // Send ping interval is elapsed
                var timestamp = DateTimeOffset.Now.ToUnixTimeSeconds();
                if (_lastPingTimestamp + PingInterval < DateTimeOffset.Now.ToUnixTimeSeconds())
                {
                    _lastPingTimestamp = timestamp;
                    Task.Run(async () => await SendPing());
                }

                // handle command response or event
                var msgBytes = Encoding.UTF8.GetBytes(msg.Text);
                var message = JsonSerializer.Deserialize<RepetierBaseMessage>(msgBytes);
                var containsEvents = message.HasEvents != null && message.HasEvents == true;

                // ensures setting session ID after first ping reply back from the server
                // when no login is required this is the first instance to require a session ID
                if (string.IsNullOrEmpty(Session.SessionId) && !string.IsNullOrEmpty(message.SessionId))
                {
                    Session.SessionId = message.SessionId;
                    OnSessionEstablished?.Invoke(Session.SessionId);
                }

                var json = JsonSerializer.Deserialize<JsonDocument>(msgBytes);
                var data = json.RootElement.GetProperty("data");
                if (message.CallBackId == -1 || containsEvents)
                {
                    // TODO: Custom JsonConverter
                    foreach (var eventData in data.EnumerateArray())
                    {
                        var rawText = eventData.GetRawText();
                        var repEvent = JsonSerializer.Deserialize<RepetierBaseEvent>(rawText);
                        Task.Run(async () =>
                        {
                            OnRawEvent?.Invoke(repEvent.Event, repEvent.Printer,
                                Encoding.UTF8.GetBytes(eventData.GetRawText()));
                            await HandleEvent(repEvent, rawText);
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
                        Task.Run(async () =>
                        {
                            OnRawResponse?.Invoke(message.CallBackId,
                                CommandManager.CommandIdentifierFor(message.CallBackId),
                                Encoding.UTF8.GetBytes(data.GetRawText()));
                            await HandleMessage(message, data);
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex,
                    "[WebSocket] Error processing message from repetier server: '{Msg}'. Error: {Error}", msg.Text,
                    ex.Message);
            }
        }

        private void OnDisconnect(DisconnectionInfo info)
        {
            _logger?.LogInformation("[WebSocket] Connection closed: Reason={Reason}, Status={Status}, Desc={Desc}",
                info.Type, info.CloseStatus, info.CloseStatusDescription);
        }

        private void OnReconnect(ReconnectionInfo info)
        {
            if (info.Type == ReconnectionType.Initial && Session.AuthType != AuthenticationType.Credentials)
            {
                // Only query messages at this point when using an api-key or no auth
                this.QueryOpenMessages();
            }

            Task.Run(async () => await SendPing());
        }

        private async Task<bool> SendPing()
        {
            return await SendCommand(PingCommand.Instance, typeof(PingCommand));
        }

        public async Task<bool> SendExtendPing(uint timeout)
        {
            return await SendCommand(new ExtendPingCommand(timeout), typeof(ExtendPingCommand));
        }


        /// <summary>
        ///     Closes the WebSocket connection
        /// </summary>
        public void Close()
        {
            WebSocketClient.Stop(WebSocketCloseStatus.Empty, "Closing initiated by user");
            WebSocketClient.Dispose();
        }

        private RestRequest StartPrintRequest(string gcodeFilePath, string printerName,
            StartBehavior autostart = StartBehavior.Autostart)
        {
            var gcodeFileName = Path.GetFileNameWithoutExtension(gcodeFilePath);
            var request = new RestRequest($"/printer/job/{printerName}", Method.Post)
                .AddFile(FilenameParam, gcodeFilePath)
                .AddHeader(KnownHeaders.ContentType, MultiPartFormData)
                .AddParameter(ActionParam, UploadAction)
                .AddParameter(AutostartParam, $"{(int)autostart}")
                .AddParameter(NameParam, gcodeFileName);

            if (!string.IsNullOrEmpty(Session.SessionId))
            {
                request = request.AddParameter(SessionParam, Session.SessionId);
            }

            return request;
        }


        private RestRequest StartPrintRequest(string fileName, byte[] data, string printerName,
            StartBehavior autostart = StartBehavior.Autostart)
        {
            var request = new RestRequest($"/printer/job/{printerName}", Method.Post)
                .AddFile(FilenameParam, data, fileName)
                .AddHeader(KnownHeaders.ContentType, MultiPartFormData)
                .AddParameter(ActionParam, UploadAction)
                .AddParameter(AutostartParam, $"{(int)autostart}")
                .AddParameter(NameParam, fileName);

            if (!string.IsNullOrEmpty(Session.SessionId))
            {
                request = request.AddParameter(SessionParam, Session.SessionId);
            }

            return request;
        }


        /// <summary>
        ///     Create a REST request for uploading a gcode file
        /// </summary>
        /// <param name="gcodeFilePath"> The path of the file to upload (/path/file.gcode) </param>
        /// <param name="printer"> Printer slug to upload to </param>
        /// <param name="group"> Group to add gcode to </param>
        /// <param name="overwrite"> Flag to overwrite existing file with the same name </param>
        /// <returns></returns>
        private RestRequest UploadModel(string gcodeFilePath, string printer, string group, bool overwrite = false)
        {
            var gcodeFileName = Path.GetFileNameWithoutExtension(gcodeFilePath);
            var request = new RestRequest($"/printer/model/{printer}", Method.Post)
                .AddFile(FilenameParam, gcodeFilePath)
                .AddHeader(KnownHeaders.ContentType, MultiPartFormData)
                .AddParameter(ActionParam, UploadAction)
                .AddParameter("group", group)
                .AddParameter("overwrite", $"{overwrite}")
                .AddParameter(NameParam, gcodeFileName);

            if (!string.IsNullOrEmpty(Session.SessionId))
            {
                request = request.AddParameter(SessionParam, Session.SessionId);
            }

            return request;
        }

        /// <summary>
        ///     Create a REST request for uploading a gcode file.
        /// </summary>
        /// <param name="fileName">  The name of the file to upload (file.gcode) </param>
        /// <param name="file"> The content of the actual gcode file </param>
        /// <param name="printer"> Printer slug to upload to </param>
        /// <param name="group"> Group to add gcode to </param>
        /// <param name="overwrite"> Flag to overwrite existing file with the same name </param>
        /// <returns></returns>
        private RestRequest UploadModel(string fileName, byte[] file, string printer, string group,
            bool overwrite = false)
        {
            var request = new RestRequest($"/printer/model/{printer}", Method.Post)
                .AddFile(FilenameParam, file, fileName)
                .AddHeader(KnownHeaders.ContentType, MultiPartFormData)
                .AddParameter(ActionParam, UploadAction)
                .AddParameter("group", group)
                .AddParameter("overwrite", $"{overwrite}")
                .AddParameter(NameParam, fileName);

            if (!string.IsNullOrEmpty(Session.SessionId))
            {
                request = request.AddParameter(SessionParam, Session.SessionId);
            }

            return request;
        }


        /// <summary>
        ///     Upload a gcode file via REST API
        /// </summary>
        /// <param name="gcodeFilePath"> The path of the file to upload (/path/file.gcode) </param>
        /// <param name="printer"> Printer slug to upload to </param>
        /// <param name="group"> Group to upload gcode to </param>
        /// <param name="overwrite"> Flag to overwrite existing file with the same name. Defaults to false </param>
        public async Task<bool> UploadGCode(string gcodeFilePath, string printer, string group, bool overwrite = false)
        {
            try
            {
                var request = UploadModel(gcodeFilePath, printer, group, overwrite);
                var response = await RestClient.ExecuteAsync(request);
                if (response.StatusCode != HttpStatusCode.OK)
                {
                    OnRestRequestFailed?.Invoke(printer, response);
                    return false;
                }

                if (response.ErrorException != null)
                {
                    throw response.ErrorException;
                }
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Error while uploading gcode file: {Error}", ex.Message);
            }

            return await Task.FromResult(true);
        }

        /// <summary>
        ///     Upload a gcode file via REST API
        /// </summary>
        /// <param name="fileName"> The name of the file to upload (file.gcode) </param>
        /// <param name="file"> The content of the actual gcode file </param>
        /// <param name="printer"> Printer slug to upload to </param>
        /// <param name="group"> Group to upload gcode to </param>
        /// <param name="overwrite"> Flag to overwrite existing file with the same name. Defaults to false </param>
        public async Task<bool> UploadGCode(string fileName, byte[] file, string printer, string group,
            bool overwrite = false)
        {
            try
            {
                var request = UploadModel(fileName, file, printer, group, overwrite);
                var response = await RestClient.ExecuteAsync(request);
                if (response.StatusCode != HttpStatusCode.OK)
                {
                    OnRestRequestFailed?.Invoke(printer, response);
                    return false;
                }

                if (response.ErrorException != null)
                {
                    throw response.ErrorException;
                }
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Error while uploading gcode file: {Error}", ex.Message);
            }

            return await Task.FromResult(true);
        }

        /// <summary>
        ///     Upload given gcode and start the printing process via REST-API.
        /// </summary>
        /// <param name="gcodeFilePath"> The path of the file to upload (/path/file.gcode) </param>
        /// <param name="printer"> Printer slug to upload to </param>
        /// <param name= AutostartParam> Flag to indicate the start behavior when uploading gcode. Defaults to autostart </param>
        public async Task<bool> UploadAndStartPrint(string gcodeFilePath, string printer,
            StartBehavior autostart = StartBehavior.Autostart)
        {
            try
            {
                var request = StartPrintRequest(gcodeFilePath, printer, autostart);
                var response = await RestClient.ExecuteAsync(request);
                if (response.StatusCode != HttpStatusCode.OK)
                {
                    OnRestRequestFailed?.Invoke(printer, response);
                    return false;
                }

                if (response.ErrorException != null)
                {
                    throw response.ErrorException;
                }
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Error while uploading and starting print: {Error}", ex.Message);
            }

            return await Task.FromResult(true);
        }

        /// <summary>
        ///     Upload given gcode and start the printing process via REST-API.
        /// </summary>
        /// <param name="fileName"> The name of the file to upload (file.gcode) </param>
        /// <param name="file"> The content of the actual gcode file </param>
        /// <param name="printer"> Printer slug to upload to </param>
        /// <param name= AutostartParam> Flag to indicate the start behavior when uploading gcode. Defaults to autostart </param>
        public async Task<bool> UploadAndStartPrint(string fileName, byte[] file, string printer,
            StartBehavior autostart = StartBehavior.Autostart)
        {
            try
            {
                var request = StartPrintRequest(fileName, file, printer, autostart);
                var response = await RestClient.ExecuteAsync(request);
                if (response.StatusCode != HttpStatusCode.OK)
                {
                    OnRestRequestFailed?.Invoke(printer, response);
                    return false;
                }

                if (response.ErrorException != null)
                {
                    throw response.ErrorException;
                }
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Error while uploading and starting print: {Error}", ex.Message);
            }

            return await Task.FromResult(true);
        }

        /// <summary>
        ///     Activate printer with given printerSlug by sending the corresponding command to the server.
        /// </summary>
        /// <param name="printerSlug"> Printer to activate </param>
        public async Task<bool> ActivatePrinter(string printerSlug)
        {
            ActivePrinter = printerSlug;
            return await SendCommand(new ActivateCommand(printerSlug));
        }

        /// <summary>
        ///     Deactivate printer with given printerSlug by sending the corresponding command to the server.
        /// </summary>
        /// <param name="printerSlug"> Printer to deactivate </param>
        public async Task<bool> DeactivatePrinter(string printerSlug)
        {
            ActivePrinter = "";
            return await SendCommand(new DeactivateCommand(printerSlug));
        }

        /// <summary>
        /// </summary>
        /// <param name="message"></param>
        /// <param name="commandDataObject"></param>
        private async Task HandleMessage(RepetierBaseMessage message, JsonElement commandDataObject)
        {
            var commandStr = CommandManager.CommandIdentifierFor(message.CallBackId);
            var cmdData = commandDataObject.GetRawText();
            message.Data = Encoding.UTF8.GetBytes(cmdData);
            if (commandStr == string.Empty)
            { 
                _logger.LogWarning("Received message callbackId '{CallbackId}' could not be found in cache. Not serializing message: '{Json}'", message.CallBackId, cmdData);
                return;
            }
            switch (commandStr)
            {
                case CommandConstants.PING:
                    await Task.Delay(TimeSpan.FromSeconds(Math.Min(5, PingInterval / 2)))
                        .ContinueWith(async t =>
                        {
                            await SendPing();
                        });
                    break;
                case CommandConstants.LOGIN:
                    var loginMessage = JsonSerializer.Deserialize<LoginMessage>(cmdData);
                    if (loginMessage != null)
                    {
                        if (string.IsNullOrEmpty(loginMessage.Error))
                        {
                            await this.QueryOpenMessages();
                        }
                        OnLoginResult?.Invoke(loginMessage);
                        if (loginMessage.Authenticated)
                        {
                            OnSessionEstablished?.Invoke(Session.SessionId);
                        }
                    }
                    OnResponse?.Invoke(message.CallBackId, commandStr, loginMessage);
                    break;
                case CommandConstants.LOGOUT:
                    OnResponse?.Invoke(message.CallBackId, commandStr, null);
                    break;
                case CommandConstants.LIST_PRINTER:
                    var listPrintersMessage = JsonSerializer.Deserialize<List<Printer>>(cmdData);
                    if (listPrintersMessage != null)
                    {
                        OnPrinterListChanged?.Invoke(listPrintersMessage);
                    }
                    var printerMsg = new ListPrinterMessage { Printers = listPrintersMessage ?? new List<Printer>() };
                    OnResponse?.Invoke(message.CallBackId, commandStr, printerMsg);
                    break;
                case CommandConstants.STATE_LIST:
                    var stateListMessage = JsonSerializer.Deserialize<Dictionary<string, PrinterState>>(cmdData);
                    var stateMsg = new StateListMessage { PrinterStates = stateListMessage };
                    OnResponse?.Invoke(message.CallBackId, commandStr, stateMsg);
                    OnPrinterStates?.Invoke(stateMsg);
                    break;
                case CommandConstants.RESPONSE:
                    var responseMessage = JsonSerializer.Deserialize<Models.Messages.ResponseMessage>(cmdData);
                    OnResponse?.Invoke(message.CallBackId, commandStr, responseMessage);
                    break;
                case CommandConstants.MESSAGES:
                    var messagesMessage = JsonSerializer.Deserialize<List<Message>>(cmdData);
                    if (messagesMessage != null)
                    {
                        OnMessagesReceived?.Invoke(messagesMessage);
                    }
                    // TODO: Define IRepetierMessage type for response
                    OnResponse?.Invoke(message.CallBackId, commandStr, null);
                    break;
                case CommandConstants.LIST_MODELS:
                    // TODO:
                    var modelList = JsonSerializer.Deserialize<List<Model>>(cmdData);
                    // OnModelListReceived?.Invoke(this, modelList);
                    break;
                case CommandConstants.LIST_JOBS:
                    // TODO:
                    var jobList = JsonSerializer.Deserialize<List<Model>>(cmdData);
                    // OnJobListReceived?.Invoke(this, jobList);
                    break;
                case CommandConstants.MODEL_INFO:
                    // TODO:
                    var modelInfo = JsonSerializer.Deserialize<Model>(cmdData);
                    // OnModelInfoReceived?.Invoke(this, modelInfo);
                    break;
                case CommandConstants.JOB_INFO:
                    // TODO:
                    var jobInfo = JsonSerializer.Deserialize<Model>(cmdData);
                    // OnJobInfoReceived?.Invoke(this, jobInfo);
                    break;
                case CommandConstants.REMOVE_JOB:
                case CommandConstants.SEND:
                case CommandConstants.COPY_MODEL:
                    OnResponse?.Invoke(message.CallBackId, commandStr, null);
                    break;
                case CommandConstants.EMERGENCY_STOP:
                    /* no payload */
                    OnResponse?.Invoke(message.CallBackId, commandStr, null);
                    break;
                case CommandConstants.ACTIVATE:
                    /* no payload */
                    OnResponse?.Invoke(message.CallBackId, commandStr, null);
                    break;
                case CommandConstants.DEACTIVATE:
                    /* no payload */
                    OnResponse?.Invoke(message.CallBackId, commandStr, null);
                    break;
                case CommandConstants.CREATE_USER:
                    var createStatusMessage = JsonSerializer.Deserialize<StatusMessage>(cmdData);
                    OnResponse?.Invoke(message.CallBackId, commandStr, createStatusMessage);
                    break;
                case CommandConstants.UPDATE_USER: 
                    /* no payload */
                    OnResponse?.Invoke(message.CallBackId, commandStr, null);
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
                    OnResponse?.Invoke(message.CallBackId, commandStr, null);
                    break;
                case CommandConstants.STOP_JOB:
                    /* no payload */
                    OnResponse?.Invoke(message.CallBackId, commandStr, null);
                    break;
                case CommandConstants.CONTINUE_JOB:
                    /* no payload */
                    OnResponse?.Invoke(message.CallBackId, commandStr, null);
                    break;
            }
        }

        /// <summary>
        ///     Handles an incoming repetier event.
        ///     The event data is then forwarded by calling their corresponding event handlers.
        /// </summary>
        private async Task HandleEvent(RepetierBaseEvent repetierEvent, string eventAsJson)
        {
            var json = JsonSerializer.Deserialize<JsonDocument>(eventAsJson);
            if (json == null)
            {
                _logger.LogWarning("Received event '{Event}' could not be deserialized. Not processing event: '{Json}'", eventAsJson, eventAsJson);
                return;
            }
            var dataJsonObject = json.RootElement.GetProperty("data");
            var eventData = dataJsonObject.GetRawText();
            repetierEvent.Data = Encoding.UTF8.GetBytes(dataJsonObject.GetRawText());

            switch (repetierEvent.Event)
            {
                case EventConstants.JOBS_CHANGED:
                    OnEvent?.Invoke(repetierEvent.Event, repetierEvent.Printer, null);
                    ActivePrinter = repetierEvent.Printer;
                    break;
                case EventConstants.TIMER_30:
                case EventConstants.TIMER_60:
                case EventConstants.TIMER_300:
                case EventConstants.TIMER_1800:
                case EventConstants.TIMER_3600:
                    var timer = (RepetierTimer)int.Parse(repetierEvent.Event[5..]);
                    if (QueryIntervals.TryGetValue(timer, out var commandDataForInterval))
                    {
                        commandDataForInterval.ForEach(command =>
                        {
                            Task.Run(async () => await SendCommand(command, command.GetType()));
                        });
                    }

                    OnEvent?.Invoke(repetierEvent.Event, repetierEvent.Printer, null);
                    break;
                case EventConstants.LOGIN_REQUIRED:
                    if (Session.AuthType == AuthenticationType.Credentials)
                    {
                        var loginRequiredEvent = JsonSerializer.Deserialize<LoginRequired>(eventData);

                        if (loginRequiredEvent != null)
                        {
                            Session.SessionId = loginRequiredEvent.SessionId;
                            await Login();
                        }
                        OnEvent?.Invoke(repetierEvent.Event, repetierEvent.Printer, loginRequiredEvent);
                    }
                    else
                    {
                        throw new InvalidOperationException("Credentials not supplied.");
                    }
                    OnLoginRequired?.Invoke();
                    break;
                case EventConstants.USER_CREDENTIALS:
                    var userCredentialsEvent = JsonSerializer.Deserialize<UserCredentials>(eventData);
                    if (userCredentialsEvent != null)
                    {
                        OnUserCredentialsReceived?.Invoke(userCredentialsEvent);
                    }
                    OnEvent?.Invoke(repetierEvent.Event, repetierEvent.Printer, userCredentialsEvent);
                    break;
                case EventConstants.PRINTER_LIST_CHANGED:
                    var printerListChangedEvent = JsonSerializer.Deserialize<PrinterListChanged>(eventAsJson);
                    if (printerListChangedEvent != null)
                    {
                        OnPrinterListChanged?.Invoke(printerListChangedEvent.Printers);
                    }
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
                    await SendCommand(MessagesCommand.Instance, repetierEvent.Printer);
                    break;
                case EventConstants.MOVE:
                    var moveEvent = JsonSerializer.Deserialize<Move>(eventData);
                    OnEvent?.Invoke(repetierEvent.Event, repetierEvent.Printer, moveEvent);
                    break;
                case EventConstants.LOG:
                    var logEvent = JsonSerializer.Deserialize<Log>(eventData);
                    if (logEvent != null)
                    {
                        OnLogReceived?.Invoke(logEvent);
                    }
                    OnEvent?.Invoke(repetierEvent.Event, repetierEvent.Printer, logEvent);
                    break;
                case EventConstants.JOB_FINISHED:
                    var jobFinishedEvent = JsonSerializer.Deserialize<JobState>(eventData);
                    if (jobFinishedEvent != null)
                    {
                        OnJobFinished?.Invoke(repetierEvent.Printer, jobFinishedEvent);
                    }
                    OnEvent?.Invoke(repetierEvent.Event, repetierEvent.Printer, jobFinishedEvent);
                    break;
                case EventConstants.JOB_KILLED:
                    var jobKilledEvent = JsonSerializer.Deserialize<JobState>(eventData);
                    if (jobKilledEvent != null)
                    {
                        OnJobKilled?.Invoke(repetierEvent.Printer, jobKilledEvent);
                    }
                    OnEvent?.Invoke(repetierEvent.Event, repetierEvent.Printer, jobKilledEvent);
                    break;
                case EventConstants.JOB_STARTED:
                    var jobStartedEvent = JsonSerializer.Deserialize<JobStarted>(eventData);
                    if (jobStartedEvent != null)
                    {
                        OnJobStarted?.Invoke(repetierEvent.Printer, jobStartedEvent);
                    }
                    OnEvent?.Invoke(repetierEvent.Event, repetierEvent.Printer, jobStartedEvent);
                    break;
                case EventConstants.EEPROM_DATA:
                    var eepromDataEvents = JsonSerializer.Deserialize<List<EepromData>>(eventData);
                    eepromDataEvents?.ForEach(eepromData =>
                        OnEvent?.Invoke(repetierEvent.Event, repetierEvent.Printer, eepromData));
                    break;
                case EventConstants.STATE:
                    var printerStateChangedEvent = JsonSerializer.Deserialize<PrinterStateChange>(eventData);
                    if (printerStateChangedEvent != null)
                    {
                        OnPrinterState?.Invoke(repetierEvent.Printer, printerStateChangedEvent.PrinterState);
                    }
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
                    if (tempChangeEvent != null)
                    {
                        OnTempChange?.Invoke(repetierEvent.Printer, tempChangeEvent);    
                    }
                    break;
                case EventConstants.SETTING_CHANGED:
                    //var settings = JsonSerializer.Deserialize<List<SettingChangedEvent>>(eventData);
                    //OnRepetierEvent?.Invoke(repetierEvent.Event, repetierEvent.Printer, settings);
                    break;
                case EventConstants.PRINTER_SETTING_CHANGED:
                    var printerSetting = JsonSerializer.Deserialize<SettingChanged>(eventData);
                    if (printerSetting != null)
                    {
                        OnPrinterSettingChanged?.Invoke(printerSetting, repetierEvent.Printer);
                    }
                    OnEvent?.Invoke(repetierEvent.Event, repetierEvent.Printer, printerSetting);
                    break;
                case EventConstants.PRINTER_CONDITION_CHANGED:
                    var conditionChange = JsonSerializer.Deserialize<PrinterConditionChanged>(eventData);
                    if (conditionChange != null)
                    {
                        OnPrinterConditionChanged?.Invoke(conditionChange, repetierEvent.Printer);
                    }
                    OnEvent?.Invoke(repetierEvent.Event, repetierEvent.Printer, conditionChange);
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
            }
        }

        /// <summary>
        ///     Send the given command to the server with the current active printer as argument.
        /// </summary>
        /// <param name="command"> The command to send to the server </param>
        /// <returns></returns>
        public async Task<bool> SendCommand(ICommandData command)
        {
            return await SendCommand(command, command.GetType(), ActivePrinter);
        }

        private async Task<bool> SendCommand(ICommandData command, string printer)
        {
            return await SendCommand(command, command.GetType(), printer);
        }

        protected async Task<bool> SendCommand(ICommandData command, Type commandType)
        {
            return await SendCommand(command, commandType, ActivePrinter);
        }

        protected async Task<bool> SendCommand(ICommandData command, Type commandType, string printer)
        {
            var baseCommand = CommandManager.CommandWithId(command, commandType, printer);
            return await Task.Run(() =>
            {
                var isInQueue = WebSocketClient.Send(baseCommand.ToBytes());
                if ( isInQueue )
                {
                    OnRequestSend?.Invoke(baseCommand);
                }
                else
                {
                    OnFailedRequest?.Invoke(baseCommand);
                }
                return isInQueue;
            });
        }

        /// <summary>
        ///     Send a raw command to the server/given printer.
        /// </summary>
        /// <param name="command"> The identifier of the command </param>
        /// <param name="printer"> The printer the command is issued to </param>
        /// <param name="data"> The raw data as key-value pairs </param>
        public async Task<bool> SendCommand(string command, string printer, Dictionary<string, object> data)
        {
            var baseCommand = CommandManager.CommandWithId(command, printer, data);
            return await Task.Run(() => WebSocketClient.Send(baseCommand.ToBytes()));
        }

        /// <summary>
        ///     Attempt login with the user and password already provided when building the RepetierConnection.
        ///     The password will be hashed. See:
        ///     https://prgdoc.repetier-server.com/v1/docs/index.html#/en/web-api/websocket/basicCommands?id=login
        /// </summary>
        public async Task Login()
        {
            if (!string.IsNullOrEmpty(Session.LoginName) && !string.IsNullOrEmpty(Session.Password))
            {
                await Login(Session.LoginName, Session.Password);
            }
        }

        /// <summary>
        ///     Attempt login with the given user and password.
        ///     The password will be hashed. See:
        ///     https://prgdoc.repetier-server.com/v1/docs/index.html#/en/web-api/websocket/basicCommands?id=login
        /// </summary>
        /// <param name="user"> The user name for login </param>
        /// <param name="password"> The password in plaintext </param>
        public async Task Login(string user, string password)
        {
            if (!string.IsNullOrEmpty(Session.SessionId))
            {
                var pw = CommandHelper.HashPassword(Session.SessionId, user, password);
                await SendCommand(new LoginCommand(user, pw, Session.LongLivedSession), typeof(LoginCommand));
            }
        }
    }
}
