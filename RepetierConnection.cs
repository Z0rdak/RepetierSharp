using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using RepetierSharp.Extentions;
using RepetierSharp.Internal;
using RepetierSharp.Models;
using RepetierSharp.Models.Events;
using RepetierSharp.Models.Messages;
using RepetierSharp.Models.Requests;
using RepetierSharp.Util;
using RestSharp;
using Websocket.Client;
using ResponseMessage = Websocket.Client.ResponseMessage;

namespace RepetierSharp
{
    public partial class RepetierConnection : Disposable
    {
        private const string FilenameParam = "filename";
        private const string NameParam = "name";
        private const string SessionParam = "sess";
        private const string ActionParam = "a";
        private const string AutostartParam = "autostart";
        private const string UploadAction = "upload";
        private const ContentType MultiPartFormData = "multipart/form-data";

        private static readonly JsonSerializerOptions DefaultOptions = new()
        {
            AllowTrailingCommas = true,
            UnknownTypeHandling = JsonUnknownTypeHandling.JsonElement,
            WriteIndented = true,
            Converters =
            {
                new RepetierEventConverter(), new RepetierMessageConverter(), new RepetierBaseEventConverter()
            }
        };

        private RepetierConnection(ILogger<RepetierConnection>? logger = null)
        {
            _logger = logger ?? NullLogger<RepetierConnection>.Instance;
            SessionIdReceivedAsync += async sessionIdArgs =>
            {
                await _clientEvents.ConnectedEvent.InvokeAsync(new RepetierConnectedEventArgs(sessionIdArgs.SessionId));
            };
        }

        /// <summary>
        ///     Retrieve printer name or API-key (or both) via REST-API
        ///     If ApiKey or PrinterSlug are not empty, they will not be overwritten by the retrieved information.
        /// </summary>
        public async Task<RepetierServerInformation?> GetRepetierServerInfoAsync()
        {
            var response = await RestClient.ExecuteAsync(new RestRequest("/printer/info"));
            if ( response is { StatusCode: HttpStatusCode.OK, Content: not null } )
            {
                return JsonSerializer.Deserialize<RepetierServerInformation>(response.Content);
            }
            await _clientEvents.HttpRequestFailedEvent.InvokeAsync(new HttpContextEventArgs(response.Request, response));
            return null;
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
                    .ContinueWith(_ => SendPing());
            }
            catch ( Exception e )
            {
                _logger.LogError(e, "Error while starting websocket connection: {Error}", e.Message);
            }
        }

        private void OnMsgReceived(ResponseMessage msg)
        {
            // each message send to and from the Repetier Server is a valid JSON message
            if ( msg.MessageType != WebSocketMessageType.Text || string.IsNullOrEmpty(msg.Text) )
            {
                return;
            }

            try
            {
                // Send ping if interval is elapsed
                var timestamp = DateTimeOffset.Now.ToUnixTimeSeconds();
                if ( _lastPingTimestamp + PingInterval < DateTimeOffset.Now.ToUnixTimeSeconds() )
                {
                    _lastPingTimestamp = timestamp;
                    Task.Run(async () => await SendPing());
                }

                // handle command response or event
                var msgBytes = Encoding.UTF8.GetBytes(msg.Text);
                var repetierMessage = JsonSerializer.Deserialize<RepetierBaseMessage>(msgBytes);
                if ( repetierMessage == null )
                {
                    _logger.LogWarning(
                        "Unable to serialize received message. It is not a valid Repetier message and won't be processed: '{Msg}'",
                        msg.Text);
                    return;
                }

                // ensures setting session ID after first ping reply back from the server
                // when no login is required this is the first instance to require a session ID
                if ( string.IsNullOrEmpty(Session.SessionId) && !string.IsNullOrEmpty(repetierMessage.SessionId) )
                {
                    Session.SessionId = repetierMessage.SessionId;
                    Task.Run(async () =>
                    {
                        var sessionIdArgs = new SessionIdReceivedEventArgs(Session.SessionId);
                        await _clientEvents.SessionIdReceivedEvent.InvokeAsync(sessionIdArgs);
                    });
                }

                var json = JsonSerializer.Deserialize<JsonDocument>(msgBytes);
                if ( json == null )
                {
                    _logger.LogWarning("Received message is not a valid JSON and won't be processed: '{Msg}'",
                        msg.Text);
                    return;
                }

                var dataElement = json.RootElement.GetProperty("data");

                if ( repetierMessage.HasEvents is true || repetierMessage.CallBackId == -1 )
                {
                    PublishRawEventInfo(dataElement);
                    // process events
                    var repetierBaseEvents =
                        JsonSerializer.Deserialize<List<RepetierBaseEvent>>(msgBytes, DefaultOptions);
                    if ( repetierBaseEvents == null )
                    {
                        _logger.LogWarning("Unable to deserialize events: '{Event}'", msg.Text);
                        return;
                    }

                    repetierBaseEvents.ForEach(repetierEvent =>
                    {
                        Task.Run(async () =>
                        {
                            await HandleEvent(repetierEvent);
                        });
                    });
                }
                else
                {
                    if ( msg.Text.Contains("permissionDenied") )
                    {
                        var permissionDeniedArgs = new PermissionDeniedEventArgs(repetierMessage.CallBackId);
                        Task.Run(async () => await _clientEvents.PermissionDeniedEvent.InvokeAsync(permissionDeniedArgs));
                    }
                    else
                    {
                        Task.Run(async () =>
                        {
                            var commandIdentifier = CommandManager.CommandIdentifierFor(repetierMessage.CallBackId);
                            if ( commandIdentifier == string.Empty )
                            {
                                _logger.LogWarning(
                                    "Received message callbackId '{CallbackId}' could not be found in cache. Not serializing message: '{Response}'",
                                    repetierMessage.CallBackId, dataElement.GetRawText());
                                return;
                            }

                            await HandleResponse(repetierMessage, commandIdentifier, dataElement);
                        });
                    }
                }
            }
            catch ( Exception ex )
            {
                var errorMsg = "[WebSocket] Error processing message from repetier server: '{Msg}'. Error: {Error}";
                _logger.LogError(ex, errorMsg, msg.Text, ex.Message);
            }
        }

        private async Task HandleResponse(RepetierBaseMessage message, string commandIdentifier,
            JsonElement dataElement)
        {
            var commandData = Encoding.UTF8.GetBytes(dataElement.GetRawText());
            await _clientEvents.RawRepetierResponseReceivedEvent.InvokeAsync(
                new RawRepetierResponseReceivedEventArgs(message.CallBackId, commandIdentifier, commandData));
            var repetierResponse =
                RepetierJsonSerializer.DeserializeResponse(commandIdentifier, commandData, DefaultOptions);
            if ( repetierResponse == null )
            {
                _logger.LogWarning(
                    "Unable to deserialize response for CommandIdentifier='{Command}' with Id='{CallbackId}': '{Response}'",
                    commandIdentifier, message.CallBackId, dataElement.GetRawText());
                return;
            }

            await ProcessResponse(repetierResponse, message.CallBackId);
        }

        private void PublishRawEventInfo(JsonElement dataElement)
        {
            foreach ( var eventData in dataElement.EnumerateArray() )
            {
                var rawText = eventData.GetRawText();
                var repEventInfo = JsonSerializer.Deserialize<RepetierBaseEventInfo>(rawText);
                if ( repEventInfo == null )
                {
                    _logger.LogWarning("Unable to deserialize event: '{Event}'", rawText);
                    continue;
                }

                Task.Run(async () =>
                {
                    var bytes = Encoding.UTF8.GetBytes(eventData.GetRawText());
                    await _clientEvents.RawRepetierEventReceivedEvent.InvokeAsync(
                        new RawRepetierEventReceivedEventArgs(repEventInfo.Event, repEventInfo.Printer, bytes));
                });
            }
        }

        private void OnDisconnect(DisconnectionInfo info)
        {
            _logger.LogInformation("[WebSocket] Connection closed: Reason={Reason}, Status={Status}, Desc={Desc}",
                info.Type, info.CloseStatus, info.CloseStatusDescription);
        }

        private void OnReconnect(ReconnectionInfo info)
        {
            if ( info.Type == ReconnectionType.Initial && Session.AuthType != AuthenticationType.Credentials )
            {
                // Only query messages at this point when using an api-key or no auth
                Task.Run(async () => await this.QueryOpenMessages());
            }

            Task.Run(async () => await SendPing());
        }

        private async Task<bool> SendPing()
        {
            return await SendCommand(PingRequest.Instance, typeof(PingRequest));
        }

        public async Task<bool> SendExtendPing(uint timeout)
        {
            return await SendCommand(new ExtendPingRequest(timeout), typeof(ExtendPingRequest));
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

            if ( !string.IsNullOrEmpty(Session.SessionId) )
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

            if ( !string.IsNullOrEmpty(Session.SessionId) )
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

            if ( !string.IsNullOrEmpty(Session.SessionId) )
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

            if ( !string.IsNullOrEmpty(Session.SessionId) )
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
                if ( response.StatusCode != HttpStatusCode.OK )
                {
                    await _clientEvents.HttpRequestFailedEvent.InvokeAsync(new HttpContextEventArgs(request, response));
                    return false;
                }

                if ( response.ErrorException != null )
                {
                    throw response.ErrorException;
                }
            }
            catch ( Exception ex )
            {
                _logger.LogError(ex, "Error while uploading gcode file: {Error}", ex.Message);
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
                if ( response.StatusCode != HttpStatusCode.OK )
                {
                    await _clientEvents.HttpRequestFailedEvent.InvokeAsync(new HttpContextEventArgs(request, response));
                    return false;
                }

                if ( response.ErrorException != null )
                {
                    throw response.ErrorException;
                }
            }
            catch ( Exception ex )
            {
                _logger.LogError(ex, "Error while uploading gcode file: {Error}", ex.Message);
            }

            return await Task.FromResult(true);
        }

        /// <summary>
        ///     Upload given gcode and start the printing process via REST-API.
        /// </summary>
        /// <param name="gcodeFilePath"> The path of the file to upload (/path/file.gcode) </param>
        /// <param name="printer"> Printer slug to upload to </param>
        /// <param name="autostart"> Flag to indicate the start behavior when uploading gcode. Defaults to autostart </param>
        public async Task<bool> UploadAndStartPrint(string gcodeFilePath, string printer,
            StartBehavior autostart = StartBehavior.Autostart)
        {
            try
            {
                var request = StartPrintRequest(gcodeFilePath, printer, autostart);
                var response = await RestClient.ExecuteAsync(request);
                if ( response.StatusCode != HttpStatusCode.OK )
                {
                    await _clientEvents.HttpRequestFailedEvent.InvokeAsync(new HttpContextEventArgs(request, response));
                    return false;
                }

                if ( response.ErrorException != null )
                {
                    throw response.ErrorException;
                }
            }
            catch ( Exception ex )
            {
                _logger.LogError(ex, "Error while uploading and starting print: {Error}", ex.Message);
            }

            return await Task.FromResult(true);
        }

        /// <summary>
        ///     Upload given gcode and start the printing process via REST-API.
        /// </summary>
        /// <param name="fileName"> The name of the file to upload (file.gcode) </param>
        /// <param name="file"> The content of the actual gcode file </param>
        /// <param name="printer"> Printer slug to upload to </param>
        /// <param name="autostart"> Flag to indicate the start behavior when uploading gcode. Defaults to autostart </param>
        public async Task<bool> UploadAndStartPrint(string fileName, byte[] file, string printer,
            StartBehavior autostart = StartBehavior.Autostart)
        {
            try
            {
                var request = StartPrintRequest(fileName, file, printer, autostart);
                var response = await RestClient.ExecuteAsync(request);
                if ( response.StatusCode != HttpStatusCode.OK )
                {
                    await _clientEvents.HttpRequestFailedEvent.InvokeAsync(new HttpContextEventArgs(request, response));
                    return false;
                }

                if ( response.ErrorException != null )
                {
                    throw response.ErrorException;
                }
            }
            catch ( Exception ex )
            {
                _logger.LogError(ex, "Error while uploading and starting print: {Error}", ex.Message);
            }

            return await Task.FromResult(true);
        }

        /// <summary>
        ///     Activate printer with given printerSlug by sending the corresponding command to the server.
        /// </summary>
        /// <param name="printerSlug"> Printer to activate </param>
        public async Task<bool> ActivatePrinter(string printerSlug)
        {
            return await SendCommand(new ActivateRequest(printerSlug));
        }

        /// <summary>
        ///     Deactivate printer with given printerSlug by sending the corresponding command to the server.
        /// </summary>
        /// <param name="printerSlug"> Printer to deactivate </param>
        public async Task<bool> DeactivatePrinter(string printerSlug)
        {
            return await SendCommand(new DeactivateRequest(printerSlug));
        }

        private async Task ProcessResponse(IRepetierResponse response, int callbackId)
        {
            var commandStr = CommandManager.CommandIdentifierFor(callbackId);
            await _clientEvents.RepetierResponseReceivedEvent.InvokeAsync(
                new RepetierResponseReceivedEventArgs(callbackId, commandStr, response));

            switch ( commandStr )
            {
                case CommandConstants.PING:
                    await Task.Delay(TimeSpan.FromSeconds(Math.Min(5, PingInterval / 2)))
                        .ContinueWith(async _ =>
                        {
                            await SendPing();
                        });
                    break;
                case CommandConstants.LOGIN:
                    {
                        var loginMessage = (LoginResponse)response;
                        if ( string.IsNullOrEmpty(loginMessage.Error) )
                        {
                            await this.QueryOpenMessages();
                        }

                        await _clientEvents.LoginResultEvent.InvokeAsync(new LoginResultEventArgs(loginMessage));
                        if ( loginMessage.Authenticated )
                        {
                            await _clientEvents.SessionIdReceivedEvent.InvokeAsync(
                                new SessionIdReceivedEventArgs(Session.SessionId));
                        }
                    }
                    break;
                case CommandConstants.LIST_PRINTER:
                    // TODO:
                    //var printerMsg = (ListPrinterResponse)response;
                    //await _serverEvents.PrinterListChangedEvent.InvokeAsync(new PrinterListChangedEventArgs(printerMsg));
                    break;
                case CommandConstants.STATE_LIST:
                    var stateMsg = (StateListResponse)response;
                    break;
                case CommandConstants.MESSAGES:
                    var messagesMessage = (MessageList)response;
                    break;
                case CommandConstants.REMOVE_JOB:
                case CommandConstants.SEND:
                case CommandConstants.COPY_MODEL:
                case CommandConstants.EMERGENCY_STOP:
                case CommandConstants.ACTIVATE:
                case CommandConstants.DEACTIVATE:
                case CommandConstants.UPDATE_USER:
                case CommandConstants.START_JOB:
                case CommandConstants.STOP_JOB:
                case CommandConstants.CONTINUE_JOB:
                case CommandConstants.LOGOUT:
                    /* no payload */
                    break;
                /* vvv not yet implemented vvv */
                case CommandConstants.LIST_MODELS:
                    var modelList = (ModelResponseList)response;
                    break;
                case CommandConstants.LIST_JOBS:
                    var jobList = (ModelResponseList)response;
                    break;
                case CommandConstants.MODEL_INFO:
                    var modelInfo = (ModelResponse)response;
                    break;
                case CommandConstants.JOB_INFO:
                    var jobInfo = (ModelResponse)response;
                    break;
                case CommandConstants.CREATE_USER:
                    var createStatusMessage = (StatusResponse)response;
                    break;
                case CommandConstants.DELETE_USER:
                    var deleteStatusMessage = (StatusResponse)response;
                    break;
                case CommandConstants.USER_LIST:
                    var userList = (UserListResponse)response;
                    break;
            }
        }

        private async Task HandleEvent(RepetierBaseEvent repetierEvent)
        {
            var repetierEventArgs = new RepetierEventReceivedEventArgs(repetierEvent.Event, repetierEvent.Printer,
                repetierEvent.RepetierEvent);
            await _clientEvents.RepetierEventReceivedEvent.InvokeAsync(repetierEventArgs);
            switch ( repetierEvent.Event )
            {
                case EventConstants.JOBS_CHANGED:
                    // TODO: WTH?
                    ActivePrinter = repetierEvent.Printer;
                    break;
                case EventConstants.TIMER_30:
                case EventConstants.TIMER_60:
                case EventConstants.TIMER_300:
                case EventConstants.TIMER_1800:
                case EventConstants.TIMER_3600:
                    if ( int.TryParse(repetierEvent.Event[5..], out var timerInt) )
                    {
                        var timer = (RepetierTimer)timerInt;
                        var requests = _commandDispatcher.GetRequests(timer);
                        requests.ForEach(async request =>
                        {
                            await SendCommand(request, request.GetType());
                        });
                    }

                    break;
                case EventConstants.LOGIN_REQUIRED:
                    // TODO: check
                    if ( Session.AuthType == AuthenticationType.Credentials )
                    {
                        var loginRequiredEvent = (LoginRequired)repetierEvent.RepetierEvent;
                        Session.SessionId = loginRequiredEvent.SessionId;
                        await Login();
                    }
                    else
                    {
                        throw new InvalidOperationException("Credentials not supplied.");
                    }

                    await _clientEvents.LoginRequiredEvent.InvokeAsync(new LoginRequiredEventArgs());
                    break;
                case EventConstants.USER_CREDENTIALS:
                    var userCredentialsEvent = (UserCredentials)repetierEvent.RepetierEvent;
                    var userCredentialsArgs = new UserCredentialsReceivedEventArgs(userCredentialsEvent);
                    await _clientEvents.SessionReconnectedEvent.InvokeAsync(userCredentialsArgs);
                    break;
                case EventConstants.PRINTER_LIST_CHANGED:
                    var printerListChangedEvent = (PrinterListChanged)repetierEvent.RepetierEvent;
                    var printerListChangedArgs = new PrinterListChangedEventArgs(printerListChangedEvent);
                    await _serverEvents.PrinterListChangedEvent.InvokeAsync(printerListChangedArgs);
                    break;
                case EventConstants.MESSAGES_CHANGED:
                    await SendCommand(MessagesRequest.Instance, repetierEvent.Printer);
                    break;
                case EventConstants.MOVE:
                    var moveEntry = (MoveEntry)repetierEvent.RepetierEvent;
                    var moveEntryArgs = new MovedEventArgs(repetierEvent.Printer, moveEntry);
                    await _printerEvents.MovedEvent.InvokeAsync(moveEntryArgs);
                    break;
                case EventConstants.LOG:
                    var logEntry = (LogEntry)repetierEvent.RepetierEvent;
                    var logEntryArgs = new LogEntryEventArgs(logEntry);
                    await _serverEvents.LogEntryEvent.InvokeAsync(logEntryArgs);
                    break;
                case EventConstants.JOB_FINISHED:
                    var jobFinishedState = (JobState)repetierEvent.RepetierEvent;
                    var jobFinishedStateArgs = new PrintJobFinishedEventArgs(repetierEvent.Printer, jobFinishedState);
                    await _printJobEvents.PrintFinishedEvent.InvokeAsync(jobFinishedStateArgs);
                    break;
                case EventConstants.JOB_KILLED:
                    var jobKilledState = (JobState)repetierEvent.RepetierEvent;
                    var jobKilledStateArgs = new PrintJobKilledEventArgs(repetierEvent.Printer, jobKilledState);
                    await _printJobEvents.PrintKilledEvent.InvokeAsync(jobKilledStateArgs);
                    break;
                case EventConstants.JOB_STARTED:
                    var jobStartedInfo = (JobStarted)repetierEvent.RepetierEvent;
                    var jobStartedArgs = new PrintJobStartedEventArgs(repetierEvent.Printer, jobStartedInfo);
                    await _printJobEvents.PrintStartedEvent.InvokeAsync(jobStartedArgs);
                    break;
                case EventConstants.STATE:
                    var printerStateChange = (PrinterStateChange)repetierEvent.RepetierEvent;
                    var printerStateChangedArgs =
                        new StateChangedEventArgs(repetierEvent.Printer, printerStateChange.PrinterState);
                    await _printerEvents.StateChangedEvent.InvokeAsync(printerStateChangedArgs);
                    break;
                case EventConstants.TEMP:
                    var tempEntry = (TempEntry)repetierEvent.RepetierEvent;
                    var tempChangeArgs = new TemperatureChangedEventArgs(repetierEvent.Printer, tempEntry);
                    await _printerEvents.TemperatureChangedEvent.InvokeAsync(tempChangeArgs);
                    break;
                case EventConstants.PRINTER_SETTING_CHANGED:
                    var printerSetting = (PrinterSetting)repetierEvent.RepetierEvent;
                    var settingChangedArgs = new SettingChangedEventArgs(repetierEvent.Printer, printerSetting);
                    await _printerEvents.SettingChangedEvent.InvokeAsync(settingChangedArgs);
                    break;
                case EventConstants.PRINTER_CONDITION_CHANGED:
                    var printerConditionChange = (PrinterConditionChanged)repetierEvent.RepetierEvent;
                    var conditionChangedArgs =
                        new ConditionChangedEventArgs(repetierEvent.Printer, printerConditionChange);
                    await _printerEvents.ConditionChangedEvent.InvokeAsync(conditionChangedArgs);
                    break;
                case EventConstants.EEPROM_DATA:
                case EventConstants.SETTING_CHANGED:
                case EventConstants.CONFIG:
                case EventConstants.FIRMWARE_CHANGED:
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
                    break;
            }
        }

        /// <summary>
        ///     Send the given command to the server with the current active printer as argument.
        /// </summary>
        /// <param name="command"> The command to send to the server </param>
        /// <returns></returns>
        public async Task<bool> SendCommand(IRepetierRequest command)
        {
            return await SendCommand(command, command.GetType(), ActivePrinter);
        }

        private async Task<bool> SendCommand(IRepetierRequest command, string printer)
        {
            return await SendCommand(command, command.GetType(), printer);
        }

        protected async Task<bool> SendCommand(IRepetierRequest command, Type commandType)
        {
            return await SendCommand(command, commandType, ActivePrinter);
        }

        protected async Task<bool> SendCommand(IRepetierRequest command, Type commandType, string printer)
        {
            var baseCommand = CommandManager.CommandWithId(command, commandType, printer);
            return await Task.Run(async () =>
            {
                var isInQueue = WebSocketClient.Send(baseCommand.ToBytes());
                if ( isInQueue )
                {
                    await _clientEvents.RepetierRequestSendEvent.InvokeAsync(new RepetierRequestEventArgs(baseCommand));
                }
                else
                {
                    await _clientEvents.RepetierRequestFailedEvent.InvokeAsync(
                        new RepetierRequestEventArgs(baseCommand));
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
            if ( !string.IsNullOrEmpty(Session.LoginName) && !string.IsNullOrEmpty(Session.Password) )
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
            if ( !string.IsNullOrEmpty(Session.SessionId) )
            {
                var pw = CommandHelper.HashPassword(Session.SessionId, user, password);
                await SendCommand(new LoginRequest(user, pw, Session.LongLivedSession), typeof(LoginRequest));
            }
        }

        #region Client Events

        private readonly RepetierClientEvents _clientEvents = new();

        /// <summary>
        ///     Fired when the connection with the server is established successfully for the first time. <br></br>
        ///     At this point, the sessionId should already be assigned to this RepetierConnection.
        /// </summary>
        public event Func<RepetierConnectedEventArgs, Task> ConnectedAsync
        {
            add => _clientEvents.ConnectedEvent.AddHandler(value);
            remove => _clientEvents.ConnectedEvent.RemoveHandler(value);
        }

        /// <summary>
        ///     Fired after establishment of the connection to the server if the server requires a login. <br></br>
        ///     This might be the case if no API-Key is supplied in the URI and the server has at least one user account.
        /// </summary>
        public event Func<LoginRequiredEventArgs, Task> LoginRequiredAsync
        {
            add => _clientEvents.LoginRequiredEvent.AddHandler(value);
            remove => _clientEvents.LoginRequiredEvent.RemoveHandler(value);
        }

        /// <summary>
        ///     Fired when the login result response is received from the server after sending the login request.
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
        ///     Event which is fired when a command is not permitted for the current sessionId.
        /// </summary>
        public event Func<SessionIdReceivedEventArgs, Task> SessionIdReceivedAsync
        {
            add => _clientEvents.SessionIdReceivedEvent.AddHandler(value);
            remove => _clientEvents.SessionIdReceivedEvent.RemoveHandler(value);
        }

        /// <summary>
        ///     Fired when reconnecting to a still existing session. There is no login is required with an existing session.
        ///     <br></br>
        ///     This is the first event you will receive. It contains the permission flags, login name of the user and user only
        ///     settings.
        /// </summary>
        public event Func<UserCredentialsReceivedEventArgs, Task> SessionReconnectedAsync
        {
            add => _clientEvents.SessionReconnectedEvent.AddHandler(value);
            remove => _clientEvents.SessionReconnectedEvent.RemoveHandler(value);
        }

        /// <summary>
        ///     Fired for received events from the repetier server. Note that temp, move and log events are not included here.
        ///     <br></br>
        ///     They can be enabled by setting the appropriate properties.
        /// </summary>
        public event Func<RepetierEventReceivedEventArgs, Task> RepetierEventReceivedAsync
        {
            add => _clientEvents.RepetierEventReceivedEvent.AddHandler(value);
            remove => _clientEvents.RepetierEventReceivedEvent.RemoveHandler(value);
        }

        /// <summary>
        ///     Fired whenever an event from the repetier server is received. Unlike the RepetierEventReceivedAsync event,
        ///     <br></br>
        ///     this event includes the raw event itself (content of the data field).  <br></br>
        ///     This is mainly a fallback event for backwards compatibility and support events in newer repetier versions,
        ///     <br></br>
        ///     which are not yet implemented by RepetierSharp.
        /// </summary>
        public event Func<RawRepetierEventReceivedEventArgs, Task> RawRepetierEventReceivedAsync
        {
            add => _clientEvents.RawRepetierEventReceivedEvent.AddHandler(value);
            remove => _clientEvents.RawRepetierEventReceivedEvent.RemoveHandler(value);
        }

        /// <summary>
        ///     Fired when a response from the server is received. This does not include the ping response.
        /// </summary>
        public event Func<RepetierResponseReceivedEventArgs, Task> RepetierResponseReceivedAsync
        {
            add => _clientEvents.RepetierResponseReceivedEvent.AddHandler(value);
            remove => _clientEvents.RepetierResponseReceivedEvent.RemoveHandler(value);
        }

        /// <summary>
        ///     Fired when a raw response from the server is received. Unlike the RepetierResponseReceivedAsync event, <br></br>
        ///     this event includes the raw response itself (content of the data field). <br></br>
        ///     This is mainly a fallback event for backwards compatibility and support events in newer repetier versions,
        ///     <br></br>
        ///     which are not yet implemented by RepetierSharp.
        /// </summary>
        public event Func<RawRepetierResponseReceivedEventArgs, Task> RawRepetierResponseReceivedAsync
        {
            add => _clientEvents.RawRepetierResponseReceivedEvent.AddHandler(value);
            remove => _clientEvents.RawRepetierResponseReceivedEvent.RemoveHandler(value);
        }

        #endregion

        #region PrintJob Events

        private readonly RepetierPrintJobEvents _printJobEvents = new();

        public event Func<PrintJobStartedEventArgs, Task> PrintStartedAsync
        {
            add => _printJobEvents.PrintStartedEvent.AddHandler(value);
            remove => _printJobEvents.PrintStartedEvent.RemoveHandler(value);
        }

        /// <summary>
        ///     Fired after a print job has been finished.
        /// </summary>
        public event Func<PrintJobFinishedEventArgs, Task> PrintFinishedAsync
        {
            add => _printJobEvents.PrintFinishedEvent.AddHandler(value);
            remove => _printJobEvents.PrintFinishedEvent.RemoveHandler(value);
        }

        /// <summary>
        ///     Fired after a normal job has been finished or killed. Shortcut if you do not care why a job is not active anymore.
        /// </summary>
        public event Func<PrintJobDeactivatedEventArgs, Task> PrintDeactivatedAsync
        {
            add => _printJobEvents.PrintDeactivatedEvent.AddHandler(value);
            remove => _printJobEvents.PrintDeactivatedEvent.RemoveHandler(value);
        }

        /// <summary>
        ///     Triggered after a print job has been killed.
        /// </summary>
        public event Func<PrintJobKilledEventArgs, Task> PrintKilledAsync
        {
            add => _printJobEvents.PrintKilledEvent.AddHandler(value);
            remove => _printJobEvents.PrintKilledEvent.RemoveHandler(value);
        }

        /// <summary>
        ///     Triggered when a failure upon starting a print job was detected. This usually happens when the printer is not ready
        ///     or on connection issues.
        /// </summary>
        public event Func<PrintJobStartFailedEventArgs, Task> PrintStartFailedAsync
        {
            add => _printJobEvents.PrintStartFailedEvent.AddHandler(value);
            remove => _printJobEvents.PrintStartFailedEvent.RemoveHandler(value);
        }

        /// <summary>
        ///     Fired after a job got added. It might already be started for printing and info might be in state of evaluation at
        ///     that point.
        /// </summary>
        public event Func<PrintJobAddedEventArgs, Task> PrintJobChangedAsync
        {
            add => _printJobEvents.PrintJobAddedEvent.AddHandler(value);
            remove => _printJobEvents.PrintJobAddedEvent.RemoveHandler(value);
        }

        #endregion

        #region Printer Events

        private readonly RepetierPrinterEvents _printerEvents = new();

        /// <summary>
        ///     Fired when the state of a printer changes.
        /// </summary>
        public event Func<StateChangedEventArgs, Task> PrinterStateReceivedAsync
        {
            add => _printerEvents.StateChangedEvent.AddHandler(value);
            remove => _printerEvents.StateChangedEvent.RemoveHandler(value);
        }

        /// <summary>
        ///     Fired when the condition of a printer changes
        /// </summary>
        public event Func<ConditionChangedEventArgs, Task> PrinterConditionChangedAsync
        {
            add => _printerEvents.ConditionChangedEvent.AddHandler(value);
            remove => _printerEvents.ConditionChangedEvent.RemoveHandler(value);
        }

        /// <summary>
        ///     Fired everytime settings of a printer change
        /// </summary>
        public event Func<SettingChangedEventArgs, Task> PrinterSettingChangedAsync
        {
            add => _printerEvents.SettingChangedEvent.AddHandler(value);
            remove => _printerEvents.SettingChangedEvent.RemoveHandler(value);
        }

        /// <summary>
        ///     Fired when a printer has a change in it's stored g-file list.
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
        ///     Fired after the response for the emergency stop request from the printer was received.
        /// </summary>
        public event Func<EmergencyStopTriggeredEventArgs, Task> PrinterEmergencyStopTriggeredAsync
        {
            add => _printerEvents.EmergencyStopTriggeredEvent.AddHandler(value);
            remove => _printerEvents.EmergencyStopTriggeredEvent.RemoveHandler(value);
        }

        /// <summary>
        ///     Fired after a new temperature entry for the printer is available. Depending on the printer configuration this can
        ///     be a very frequently occurring event.
        /// </summary>
        public event Func<TemperatureChangedEventArgs, Task> PrinterTempChangedAsync
        {
            add => _printerEvents.TemperatureChangedEvent.AddHandler(value);
            remove => _printerEvents.TemperatureChangedEvent.RemoveHandler(value);
        }

        /// <summary>
        ///     Triggered for every move the printer does. This can be used to provide a live preview of what the printer does.
        ///     This is only fired when moves are enabled (sendMoves action).
        /// </summary>
        public event Func<MovedEventArgs, Task> PrinterMovedAsync
        {
            add => _printerEvents.MovedEvent.AddHandler(value);
            remove => _printerEvents.MovedEvent.RemoveHandler(value);
        }

        #endregion

        #region Server Events

        private readonly RepetierServerEvents _serverEvents = new();

        /// <summary>
        ///     Fired for each new log line. You received logs depends on the log level set at the server.
        /// </summary>
        public event Func<LogEntryEventArgs, Task> LogEntryAsync
        {
            add => _serverEvents.LogEntryEvent.AddHandler(value);
            remove => _serverEvents.LogEntryEvent.RemoveHandler(value);
        }

        /// <summary>
        ///     Fired whenever a printer was added, deleted or modified.
        /// </summary>
        public event Func<PrinterListChangedEventArgs, Task> PrinterListChangedAsync
        {
            add => _serverEvents.PrinterListChangedEvent.AddHandler(value);
            remove => _serverEvents.PrinterListChangedEvent.RemoveHandler(value);
        }

        /// <summary>
        ///     Fired when a new message gets available. <br></br>
        ///     Contains the new added message as payload or null when a message got removed.
        /// </summary>
        public event Func<MessagesChangedEventArgs, Task> MessagesChangedAsync
        {
            add => _serverEvents.MessagesChangedEvent.AddHandler(value);
            remove => _serverEvents.MessagesChangedEvent.RemoveHandler(value);
        }

        /// <summary>
        ///     Fired for every command/request send to the repetier server. <br></br>
        ///     This is mainly useful for debugging purposes.
        /// </summary>
        public event Func<RepetierRequestEventArgs, Task> RepetierRequestSendAsync
        {
            add => _clientEvents.RepetierRequestSendEvent.AddHandler(value);
            remove => _clientEvents.RepetierRequestSendEvent.RemoveHandler(value);
        }

        /// <summary>
        ///     Fired when a request to the repetier server failed. <br></br>
        ///     This happens when the websocket client is unable to queue the request internally. <br></br>
        ///     To check if the server received the response either listen for the corresponding response.
        /// </summary>
        public event Func<RepetierRequestEventArgs, Task> RepetierRequestFailedAsync
        {
            add => _clientEvents.RepetierRequestFailedEvent.AddHandler(value);
            remove => _clientEvents.RepetierRequestFailedEvent.RemoveHandler(value);
        }

        public event Func<HttpContextEventArgs, Task> HttpRequestFailedAsync
        {
            add => _clientEvents.HttpRequestFailedEvent.AddHandler(value);
            remove => _clientEvents.HttpRequestFailedEvent.RemoveHandler(value);
        }

        #endregion

        public uint PingInterval
        {
            get => _pingInterval;
            set
            {
                _pingInterval = value;
                Task.Run(async () => await SendExtendPing(_pingInterval));
            }
        }

        private long _lastPingTimestamp;
        private uint _pingInterval = 10000;
        private TimeSpan _pingTimeSpan = TimeSpan.FromMilliseconds(10000);

        private readonly ILogger<RepetierConnection> _logger;

        private readonly CommandDispatcher _commandDispatcher = new();
        private CommandManager CommandManager { get; } = new();
        private IWebsocketClient WebSocketClient { get; set; }
        private IRestClient RestClient { get; set; }
        private RepetierSession Session { get; init; }
        private string ActivePrinter { get; set; } = "";
    }

    public sealed class CommandDispatcher
    {
        public Dictionary<RepetierTimer, List<IRepetierRequest>> QueryIntervals { get; } = new();

        public List<IRepetierRequest> GetRequests(RepetierTimer timer)
        {
            return QueryIntervals.TryGetValue(timer, out var commandDataForInterval)
                ? commandDataForInterval
                : new List<IRepetierRequest>();
        }
    }
}
