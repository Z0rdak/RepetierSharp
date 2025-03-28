using System;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using RepetierSharp.Control;
using RepetierSharp.Internal;
using RepetierSharp.Models;
using RepetierSharp.Models.Commands;
using RepetierSharp.Models.Communication;
using RepetierSharp.Models.Events;
using RepetierSharp.Models.Responses;
using RepetierSharp.Serialization;
using RepetierSharp.Util;
using RestSharp;
using Websocket.Client;

namespace RepetierSharp
{
    public partial class RepetierConnection : Disposable
    {

        private RepetierConnection(ILogger<RepetierConnection>? logger = null)
        {
            _logger = logger ?? NullLogger<RepetierConnection>.Instance;
            _commandDispatcher = new CommandDispatcher();
            _commandManager = new CommandManager(_logger);
        }

        private RepetierConnection(IRestClient restClient, IWebsocketClient websocket, RepetierSession? session = null, ILogger<RepetierConnection>? logger = null) : this(logger)
        {
            Session = session ?? new RepetierSession();
            RestClient = restClient;
            WebSocketClient = websocket;
            ConnectedAsync += async (connectedArgs) =>
            {
                if (!connectedArgs.Reconnect && Session.KeepAlivePing.Seconds > 10)
                    await SendExtendPing(Session.KeepAlivePing);
            };
        }

        /// <summary>
        ///     Open WebSocket connection to repetier server and start communication.
        /// </summary>
        public async Task Connect()
        {
            WebSocketClient.ReconnectTimeout = TimeSpan.FromSeconds(5);
            WebSocketClient.ReconnectionHappened.Subscribe(OnReconnect);
            WebSocketClient.DisconnectionHappened.Subscribe(OnDisconnect);
            WebSocketClient.MessageReceived.Subscribe(OnMsgReceived);
            try
            {
                await WebSocketClient.StartOrFail();
            }
            catch ( Exception e )
            {
                _logger.LogError(e, "Error while starting websocket connection: {Error}", e.Message);
            }
        }

        private void OnMsgReceived(ResponseMessage msg)
        {
            // Each message send to and from the Repetier Server is a valid JSON message
            if ( msg.MessageType != WebSocketMessageType.Text || string.IsNullOrEmpty(msg.Text))
                return;
            var msgJson = JsonSerializer.Deserialize<JsonDocument>(msg.Text);
            if ( msgJson == null )
            {
                _logger.LogWarning("Received message is not a valid JSON and won't be processed: '{Msg}'",
                    msg.Text);
                return;
            }
            
            try
            {
                Task.Run(async () => await HandlePing());
                var msgDataJson = msgJson.RootElement.GetProperty("data");               
                var repetierMsgHeader = msgJson.Deserialize<RepetierMessageHeader>(SerializationOptions.DefaultOptions);
                if ( repetierMsgHeader == null )
                {
                    _logger.LogWarning("Unable to serialize repetier message header: '{Msg}'", msg.Text);
                    return;
                }

                // Ensures setting session ID after first ping reply back from the server.
                // When no login is required this is the first instance to require a session ID
                if ( string.IsNullOrEmpty(Session.SessionId) && !string.IsNullOrEmpty(repetierMsgHeader.SessionId) )
                {
                    Task.Run(async () =>
                    {
                        Session.SessionId = repetierMsgHeader.SessionId;
                        var sessionIdArgs = new SessionIdReceivedEventArgs(Session.SessionId);
                        await _clientEvents.SessionIdReceivedEvent.InvokeAsync(sessionIdArgs);
                    });
                }
              
                if ( repetierMsgHeader.IsEventList ) 
                {
                    PublishRawEventInfo(msgDataJson);
                    var repetierEventList = JsonSerializer.Deserialize<RepetierEventList>(msg.Text, SerializationOptions.DefaultOptions);
                    if ( repetierEventList == null )
                    {
                        _logger.LogWarning("Unable to serialize repetier event list: '{Msg}'", msg.Text);
                        return;
                    }
                    repetierEventList.Data.ForEach(repEvent =>
                    {
                        Task.Run(async () => await HandleEvent(repEvent));
                    });
                }
                else // handle response
                {
                    var cmdIdentifier = _commandManager.CommandIdentifierFor(repetierMsgHeader.CallBackId);
                    var repetierResponse = msgJson.Deserialize<RepetierResponse>(SerializationOptions.ResponseConverter(cmdIdentifier));
                    if ( repetierResponse == null )
                    {
                        _logger.LogWarning("Unable to serialize repetier event list: '{Msg}'", msg.Text);
                        return;
                    }
                    if (msgDataJson.GetRawText().Contains("permissionDenied") )
                    {
                        var permissionDeniedArgs = new PermissionDeniedEventArgs(repetierResponse.CallBackId, repetierResponse.CommandId);
                        Task.Run(async () => await _clientEvents.PermissionDeniedEvent.InvokeAsync(permissionDeniedArgs));
                        return;
                    }
               
                    if ( cmdIdentifier == string.Empty )
                    {
                        _logger.LogWarning(
                            "Received message with Id='{CallbackId}' not found in cache. Not serializing message: '{Response}'",
                            repetierResponse.CallBackId, msgDataJson.GetRawText());
                        return;
                    }
                    Task.Run(async () => await HandleResponse(repetierResponse, msgDataJson));
                }
            }
            catch ( Exception ex )
            {
                _logger.LogError(ex, "[WebSocket] Error processing message from repetier server. Error: {Error}", ex.Message);
            }
        }
        
        private async Task HandlePing()
        {
            // Send ping if interval is elapsed
            var timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            var elapsedTime = timestamp - _lastPingTimestamp;
            if ( elapsedTime >= (Session.KeepAlivePing.Seconds) )
            {
                _lastPingTimestamp = timestamp;
                _logger.LogTrace("[Ping@{nowt}]\n\t- Now: \t{now}\n\t- Last:\t{last}\n\t- Interval: {interval}\n\t- Difference: {}", 
                    DateTimeOffset.UtcNow, timestamp, _lastPingTimestamp, Session.KeepAlivePing.Seconds, elapsedTime);
                await SendPing();
            }
        }

        public ScheduledPrinterCmd SchedulePrinterCommand(string printer, ICommandData commandData, RepetierTimer timer = RepetierTimer.Timer30)
        {
            return _commandDispatcher.AddPrinterCommand(printer, commandData, timer);
        }
        
        public ScheduledServerCmd ScheduleServerCommand(ICommandData commandData, RepetierTimer timer = RepetierTimer.Timer30)
        {
            return _commandDispatcher.AddServerCommand(commandData, timer);
        }

        private async Task HandleResponse(RepetierResponse response, JsonElement dataElement)
        { 
            if ( !IsFiltered(response.CommandId, _responseFilters) )
            {
                var rawResponsePayload = Encoding.UTF8.GetBytes(dataElement.GetRawText());
                var rawResponseArgs = new RawResponseReceivedEventArgs(response.CallBackId, response.CommandId, rawResponsePayload);
                await _clientEvents.RawResponseReceivedEvent.InvokeAsync(rawResponseArgs);
                var callbackInfo = _commandManager.CallbackInfoFor(response.CallBackId);
                if ( callbackInfo != null )
                {
                    if ( callbackInfo.CmdType == CommandType.Printer )
                        await _printerEvents.ResponseReceivedEvent.InvokeAsync(new PrinterResponseEventArgs(response, callbackInfo.Printer));
                    else
                        await _serverEvents.ResponseReceivedEvent.InvokeAsync(new ResponseEventArgs(response));
                    LogResponse(response, dataElement);
                }
            }
            await ProcessResponse(response);
        }
        
        private void LogResponse(RepetierResponse response, JsonElement dataElement)
        {
            var responseDataJson = JsonSerializer.Serialize(dataElement, SerializationOptions.WriteOptions);
            _logger.LogDebug("<=#=[{action}]=#= | #{callbackId}", response.CommandId, response.CallBackId);
            _logger.LogTrace("<=#=[{action}]=#= | #{callbackId}: Data={}", response.CommandId, response.CallBackId, responseDataJson);
        }

        private bool IsFiltered(string id, List<Predicate<string>> filterList)
        {
            return filterList.Exists(pre => pre.Invoke(id));
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
                    if ( !IsFiltered(repEventInfo.Event, _eventFilters) )
                    {
                        var eventArgs = new RawEventReceivedEventArgs(
                            repEventInfo.Event, repEventInfo.Printer, Encoding.UTF8.GetBytes(rawText));
                        await _clientEvents.RawEventReceivedEvent.InvokeAsync(eventArgs);
                    }
                });
            }
        }

        private void OnDisconnect(DisconnectionInfo info)
        {
            Task.Run(async () =>
            {
                var disconnectedArgs = new DisconnectedEventArgs(info);
                await _clientEvents.DisconnectedEvent.InvokeAsync(disconnectedArgs);
            });
            _logger.LogWarning("[WebSocket] Connection closed: Reason={Reason}, Status={Status}, Desc={Desc}",
                info.Type, info.CloseStatus, info.CloseStatusDescription);
        }

        private void OnReconnect(ReconnectionInfo info)
        {
            var isReconnect = info.Type != ReconnectionType.Initial;
            Task.Run(async () => await _clientEvents.ConnectedEvent.InvokeAsync(new ConnectedEventArgs(this.WebSocketClient.Url, isReconnect)));
        }

        private async Task<bool> SendPing()
        {
            return await SendServerCommand(PingCommand.Instance);
        }

        public async Task<bool> SendExtendPing(TimeSpan timeout)
        {
            return await SendServerCommand(new ExtendPingCommand((uint)timeout.Seconds));
        }

        /// <summary>
        ///     Closes the WebSocket connection
        /// </summary>
        public async Task Close()
        {
            await WebSocketClient.Stop(WebSocketCloseStatus.Empty, "Closing initiated by user");
            WebSocketClient.Dispose();
        }

        private async Task ProcessResponse(RepetierResponse response)
        {
            switch ( response.CommandId )
            {
                case CommandConstants.PING:
                    await Task.Delay(TimeSpan.FromSeconds(Session.KeepAlivePing.Seconds))
                        .ContinueWith(async _ =>
                        {
                            await SendPing();
                        });
                    break;
                case CommandConstants.LOGIN:
                    {
                        var loginMessage = (LoginResponse)response.Data;
                        if ( string.IsNullOrEmpty(loginMessage.Error) )
                        {
                            await SendServerCommand(MessagesCommand.Instance);
                        }
                        await _clientEvents.LoginResultEvent.InvokeAsync(new LoginResultEventArgs(loginMessage));
                        if ( loginMessage.Authenticated )
                        {
                            await _clientEvents.SessionIdReceivedEvent.InvokeAsync(
                                new SessionIdReceivedEventArgs(Session.SessionId));
                        }
                    }
                    break;
                case CommandConstants.STATE_LIST:
                    var stateMsg = (StateListResponse)response.Data;
                    foreach ( var printerState in stateMsg.PrinterStates )
                    {
                        var printerStateChange = new StateChangedEventArgs(printerState.Key, printerState.Value);
                        await _printerEvents.StateChangedEvent.InvokeAsync(printerStateChange);
                    }
                    break;
            }
            _commandManager.AcknowledgeCommand(response.CallBackId);
        }

        private async Task HandleEvent(IRepetierEvent repetierEvent)
        {
            if ( !IsFiltered(repetierEvent.Event, _eventFilters) )
            {

                if ( string.IsNullOrEmpty(repetierEvent.Printer) )
                {
                    var repetierEventArgs = new ServerEventEventArgs(repetierEvent.Event, repetierEvent.EventData);
                    await _serverEvents.EventReceivedEvent.InvokeAsync(repetierEventArgs);
                }
                else
                {
                    var repetierEventArgs = new PrinterEventEventArgs(repetierEvent.Event, repetierEvent.Printer, repetierEvent.EventData);
                    await _printerEvents.EventReceivedEvent.InvokeAsync(repetierEventArgs);
                } 
                LogEvent(repetierEvent);
            }

            switch ( repetierEvent.Event )
            {
                case EventConstants.JOBS_CHANGED:
                    var jobsChanged = new JobsChangedEventArgs(repetierEvent.Printer);
                    await _printerEvents.JobsChangedEvent.InvokeAsync(jobsChanged);
                    break;
                case EventConstants.PRINT_QUEUE_CHANGED:
                    var printQueueChanged = new JobsChangedEventArgs(repetierEvent.Printer);
                    await _printerEvents.JobsChangedEvent.InvokeAsync(printQueueChanged);
                    break;
                case EventConstants.GCODE_STORAGE_CHANGED: // TODO: Test when 1.5 is released
                    var gcodeStorageChange = (GcodeStorageChange)repetierEvent.EventData;
                    var gcodeChangeEvent = new GcodeStorageChangedEventArgs(repetierEvent.Printer, gcodeStorageChange);
                    await _printJobEvents.GcodeStorageChangedEvent.InvokeAsync(gcodeChangeEvent);
                    break;
                case EventConstants.PONG:
                    await SendPing();
                    break;
                case EventConstants.TIMER_30:
                case EventConstants.TIMER_60:
                case EventConstants.TIMER_300:
                case EventConstants.TIMER_1800:
                case EventConstants.TIMER_3600:
                    // Try to get the numbers from the string and parse it to a RepetierTimer 
                    if ( int.TryParse(repetierEvent.Event[5..], out var timerInt) )
                    {
                        var timer = (RepetierTimer)timerInt;
                        await _commandDispatcher.DispatchCommands(timer, this);
                    }

                    break;
                case EventConstants.LOGIN_REQUIRED:
                    var loginRequiredEvent = (LoginRequired)repetierEvent.EventData;
                    if ( !string.IsNullOrEmpty(Session.SessionId) )
                    {
                        Session.SessionId = loginRequiredEvent.SessionId;
                    } 
                    await _clientEvents.LoginRequiredEvent.InvokeAsync(new LoginRequiredEventArgs());
                    if ( Session.DefaultLogin != null )
                    { 
                        await Login();
                    }
                    break;
                case EventConstants.USER_CREDENTIALS:
                    var userCredentialsEvent = (UserCredentials)repetierEvent.EventData;
                    var userCredentialsArgs = new UserCredentialsReceivedEventArgs(userCredentialsEvent);
                    await _clientEvents.CredentialsReceivedEvent.InvokeAsync(userCredentialsArgs);
                    break;
                case EventConstants.PRINTER_LIST_CHANGED:
                    var printerListChanged = (PrinterListChanged)repetierEvent.EventData;
                    var printerListChangedArgs = new PrinterListChangedEventArgs(printerListChanged);
                    await _serverEvents.PrinterListChangedEvent.InvokeAsync(printerListChangedArgs);
                    break;
                case EventConstants.MESSAGES_CHANGED:
                    await SendServerCommand(MessagesCommand.Instance);
                    break;
                case EventConstants.MOVE:
                    var moveEntry = (MoveEntry)repetierEvent.EventData;
                    var moveEntryArgs = new MovedEventArgs(repetierEvent.Printer, moveEntry);
                    await _printerEvents.MovedEvent.InvokeAsync(moveEntryArgs);
                    break;
                case EventConstants.LOG:
                    var logEntry = (LogEntry)repetierEvent.EventData;
                    var logEntryArgs = new LogEntryEventArgs(logEntry);
                    await _serverEvents.LogEntryEvent.InvokeAsync(logEntryArgs);
                    break;
                case EventConstants.JOB_FINISHED:
                    var jobFinishedState = (JobState)repetierEvent.EventData;
                    var jobFinishedStateArgs = new PrintJobFinishedEventArgs(repetierEvent.Printer, jobFinishedState);
                    await _printJobEvents.PrintFinishedEvent.InvokeAsync(jobFinishedStateArgs);
                    break;
                case EventConstants.JOB_KILLED:
                    var jobKilledState = (JobState)repetierEvent.EventData;
                    var jobKilledStateArgs = new PrintJobKilledEventArgs(repetierEvent.Printer, jobKilledState);
                    await _printJobEvents.PrintKilledEvent.InvokeAsync(jobKilledStateArgs);
                    break;
                case EventConstants.JOB_STARTED:
                    var jobStartedInfo = (JobStarted)repetierEvent.EventData;
                    var jobStartedArgs = new PrintJobStartedEventArgs(repetierEvent.Printer, jobStartedInfo);
                    await _printJobEvents.PrintStartedEvent.InvokeAsync(jobStartedArgs);
                    break;
                case EventConstants.STATE:
                    var printerStateChangedArgs =
                        new StateChangedEventArgs(repetierEvent.Printer, (PrinterState)repetierEvent.EventData);
                    await _printerEvents.StateChangedEvent.InvokeAsync(printerStateChangedArgs);
                    break;
                case EventConstants.TEMP:
                    var tempEntry = (TempEntry)repetierEvent.EventData;
                    var tempChangeArgs = new TemperatureChangedEventArgs(repetierEvent.Printer, tempEntry);
                    await _printerEvents.TemperatureChangedEvent.InvokeAsync(tempChangeArgs);
                    break;
                case EventConstants.PRINTER_SETTING_CHANGED:
                    var printerSetting = (PrinterSettingChanged)repetierEvent.EventData;
                    var settingChangedArgs = new SettingChangedEventArgs(repetierEvent.Printer, printerSetting);
                    await _printerEvents.SettingChangedEvent.InvokeAsync(settingChangedArgs);
                    break;
                case EventConstants.PRINTER_CONDITION_CHANGED:
                    var printerConditionChange = (PrinterConditionChanged)repetierEvent.EventData;
                    var conditionChangedArgs =
                        new ConditionChangedEventArgs(repetierEvent.Printer, printerConditionChange);
                    await _printerEvents.ConditionChangedEvent.InvokeAsync(conditionChangedArgs);
                    break;
                case EventConstants.LAYER_CHANGED:
                    var layerChangedEventArgs = new LayerChangedEventArgs(repetierEvent.Printer, (LayerChanged)repetierEvent.EventData);
                    await _printerEvents.LayerChangedEvent.InvokeAsync(layerChangedEventArgs);
                    break;
                case EventConstants.CHANGE_FILAMENT_REQUESTED:
                    var changeFilamentRequestedEventArgs = new ChangeFilamentRequestedEventArgs(repetierEvent.Printer);
                    await _printerEvents.ChangeFilamentRequestedEvent.InvokeAsync(changeFilamentRequestedEventArgs);
                    break;
                case EventConstants.PREPARE_JOB:
                case EventConstants.PREPARE_JOB_FINIHSED:
                case EventConstants.SETTING_CHANGED:
                case EventConstants.CONFIG:
                case EventConstants.REMOTE_SERVERS_CHANGED:
                case EventConstants.EEPROM_DATA:
                case EventConstants.FIRMWARE_CHANGED:
                case EventConstants.LOGOUT:
                case EventConstants.FOLDERS_CHANGED:
                case EventConstants.EEPROM_CLEAR:
                case EventConstants.GET_EXTERNAL_LINKS:
                case EventConstants.MODEL_GROUPLIST_CHANGED:
                    break;
            }
        }
        private void LogEvent(IRepetierEvent repetierEvent)
        {var eventJson = JsonSerializer.Serialize(repetierEvent.EventData, SerializationOptions.WriteOptions);
            if ( string.IsNullOrEmpty(repetierEvent.Printer) )
            {
                _logger.LogDebug("<=!=[{action}]=!= | Server ", repetierEvent.Event);
                _logger.LogTrace("<=!=[{action}]=!= | Server | Data={dataJson}", repetierEvent.Event, eventJson);
            }
            else
            {
                _logger.LogDebug("<=!=[{action}]=!= | {printer}", repetierEvent.Event, repetierEvent.Printer);
                _logger.LogTrace("<=!=[{action}]=!= | {printer} |Data={dataJson}", repetierEvent.Event, repetierEvent.Printer, eventJson);
            }
        }

        /// <summary>
        /// Remote server gives access to methods to upload and start print jobs as well as get basic server infos
        /// Internally this is a wrapper around http requests to the server.
        /// </summary>
        /// <returns></returns>
        public IRemoteServer GetRemoteServer()
        {
            return new RemoteRepetierServer(Session, _printJobEvents, _clientEvents, RestClient);
        }
        
        /// <summary>
        /// Internally this bundles printer related websocket commands for the specified printer.
        /// Currently, there is no validation of the provided printer slug done.readonly
        /// Invalid printer slugs will result in wrong websocket commands.
        /// </summary>
        /// <param name="printer">The printer to get access to printer related commands</param>
        /// <returns></returns>
        public IRemotePrinter GetRemotePrinter(string printer)
        {
            return new RemoteRepetierPrinter(this, printer);
        }

        public async Task<bool> SendPrinterCommand(ICommandData command, string printer)
        {
            var printerCommand = _commandManager.PrinterCommandWithId(command, printer);
            return await SendCommand(printerCommand);
        }
        
        public async Task<bool> SendServerCommand(ICommandData command)
        {
            var serverCommand = _commandManager.ServerCommandWithId(command);
            return await SendCommand(serverCommand);
        }
        
        private async Task<bool> SendCommand(BaseCommand command)
        {
            var isFiltered = IsFiltered(command.Action, _commandFilters);
            if (!isFiltered)
            {
                LogCommand(command);
            }
            return await Task.Run(async () =>
            {
                var payload = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(command, SerializationOptions.WriteCommandOptions));
                var isInQueue = WebSocketClient.Send(payload);
                if ( !isInQueue ) // command send failed
                {
                    await _serverEvents.CommandFailedEvent.InvokeAsync(new CommandEventArgs(command));
                    return false;
                }
                if (isFiltered) return isInQueue;
                switch ( command )
                {
                    case ServerCommand serverCmd:
                        {
                            await _serverEvents.CommandSendEvent.InvokeAsync(new ServerCommandEventArgs(serverCmd));
                        }
                        break;
                    case PrinterCommand printerCmd:
                        {
                            await _printerEvents.CommandSendEvent.InvokeAsync(new PrinterCommandEventArgs(printerCmd));
                        }
                        break;
                }
                return isInQueue;
            });
        }
        
        private void LogCommand(BaseCommand command)
        {
            var cmdStr = JsonSerializer.Serialize(command, SerializationOptions.WriteCommandOptions);
            switch ( command )
            {
                case PrinterCommand printerCmd:
                    _logger.LogDebug("=?=[{action}]=?=> | {printer} | #{}", command.Action, printerCmd.Printer, command.CallbackId);
                    _logger.LogTrace("=?=[{action}]=?=> | {printer} | #{}: Data={cmd}", command.Action, printerCmd.Printer, command.CallbackId, cmdStr);
                    break;
                case ServerCommand _:
                    _logger.LogDebug("=?=[{action}]=?=> | Server | #{}", command.Action, command.CallbackId);
                    _logger.LogTrace("=?=[{action}]=?=> | Server | #{}: Data={cmd}", command.CallbackId, command.Action, cmdStr);
                    break;
            }
        }

        /// <summary>
        ///     Attempt login with the user and password already provided when building the RepetierConnection.
        ///     The password will be hashed. See:
        ///     https://prgdoc.repetier-server.com/v1/docs/index.html#/en/web-api/websocket/basicCommands?id=login
        /// </summary>
        private async Task Login()
        {
            if ( Session.DefaultLogin is CredentialAuth credentialAuth )
            {
                
                if ( !string.IsNullOrEmpty(credentialAuth.LoginName) && !string.IsNullOrEmpty(credentialAuth.Password) )
                {
                    await Login(credentialAuth);
                }
            }
        }

        /// <summary>
        ///     Attempt login with the given user and password.
        ///     The password will be hashed. See:
        ///     https://prgdoc.repetier-server.com/v1/docs/index.html#/en/web-api/websocket/basicCommands?id=login
        /// </summary>
        /// <param name="user"> The user name for login </param>
        /// <param name="password"> The password in plaintext </param>
        /// <param name="longLivedSession"> Flag to indicate if the session should be long lived. Defaults to true </param>
        public async Task<bool> Login(string user, string password, bool longLivedSession = true)
        {
            if ( string.IsNullOrEmpty(Session.SessionId) ) return false;
            // TODO: Set session stuff and auth
            var pw = CredentialAuth.HashPassword(Session.SessionId, user, password);
            var loginCommand = new LoginCommand(user, pw, longLivedSession);
            return await SendServerCommand(loginCommand);
        }
        
        public async Task<bool> Login(CredentialAuth repAuth)
        {
            return await this.Login(repAuth.LoginName, repAuth.Password, Session.LongLivedSession);
        }

        #region Events

        #region Client Events

        private readonly RepetierClientEvents _clientEvents = new();

        /// <summary>
        ///     Fired when the connection with the server is established successfully for the first time. <br></br>
        ///     At this point, the sessionId should already be assigned to this RepetierConnection.
        /// </summary>
        public event Func<ConnectedEventArgs, Task> ConnectedAsync
        {
            add => _clientEvents.ConnectedEvent.AddHandler(value);
            remove => _clientEvents.ConnectedEvent.RemoveHandler(value);
        }

        /// <summary>
        ///     Fired when the connection with the server was closed. Either willingly or by other means. <br></br>
        ///     In case the disconnected client had the last connection open for this particular session, the session may be discarded by the server.
        /// </summary>
        public event Func<DisconnectedEventArgs, Task> DisconnectedAsync
        {
            add => _clientEvents.DisconnectedEvent.AddHandler(value);
            remove => _clientEvents.DisconnectedEvent.RemoveHandler(value);
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
        public event Func<SessionIdReceivedEventArgs, Task> SessionEstablishedAsync
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
        public event Func<UserCredentialsReceivedEventArgs, Task> CredentialsReceivedAsync
        {
            add => _clientEvents.CredentialsReceivedEvent.AddHandler(value);
            remove => _clientEvents.CredentialsReceivedEvent.RemoveHandler(value);
        }

        /// <summary>
        ///     Fired whenever an event from the repetier server is received. Unlike the RepetierEventReceivedAsync event,
        ///     <br></br>
        ///     this event includes the raw event itself (content of the data field).  <br></br>
        ///     This is mainly a fallback event for backwards compatibility and support events in newer repetier versions,
        ///     <br></br>
        ///     which are not yet implemented by RepetierSharp.
        /// </summary>
        public event Func<RawEventReceivedEventArgs, Task> RawEventReceivedAsync
        {
            add => _clientEvents.RawEventReceivedEvent.AddHandler(value);
            remove => _clientEvents.RawEventReceivedEvent.RemoveHandler(value);
        }

        /// <summary>
        ///     Fired when a raw response from the server is received. Unlike the RepetierResponseReceivedAsync event, <br></br>
        ///     this event includes the raw response itself (content of the data field). <br></br>
        ///     This is mainly a fallback event for backwards compatibility and support events in newer repetier versions,
        ///     <br></br>
        ///     which are not yet implemented by RepetierSharp.
        /// </summary>
        public event Func<RawResponseReceivedEventArgs, Task> RawResponseReceivedAsync
        {
            add => _clientEvents.RawResponseReceivedEvent.AddHandler(value);
            remove => _clientEvents.RawResponseReceivedEvent.RemoveHandler(value);
        }

        #endregion

        #region PrintJob Events

        private readonly RepetierPrintJobEvents _printJobEvents = new();

        /// <summary>
        ///     Fired after a print job has been started.
        /// </summary>
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
        ///     This only triggers for print jobs started through this connection by calling UploadAndStartPrint.
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
        
        /// <summary>
        ///     Gets triggered if a change in one gcode storage was detected.
        ///     Replaces jobsChanged and printQueueChanged events starting with version 1.5
        /// </summary>
        public event Func<GcodeStorageChangedEventArgs, Task> GcodeStorageChangedAsync
        {
            add => _printJobEvents.GcodeStorageChangedEvent.AddHandler(value);
            remove => _printJobEvents.GcodeStorageChangedEvent.RemoveHandler(value);
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
        
        public event Func<ResponseEventArgs, Task> ServerResponseReceivedAsync
        {
            add => _serverEvents.ResponseReceivedEvent.AddHandler(value);
            remove => _serverEvents.ResponseReceivedEvent.RemoveHandler(value);
        }
        
        public event Func<PrinterResponseEventArgs, Task> PrinterResponseReceivedAsync
        {
            add => _printerEvents.ResponseReceivedEvent.AddHandler(value);
            remove => _printerEvents.ResponseReceivedEvent.RemoveHandler(value);
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
        
        public event Func<PrinterEventEventArgs, Task> PrinterEventReceivedAsync
        {
            add => _printerEvents.EventReceivedEvent.AddHandler(value);
            remove => _printerEvents.EventReceivedEvent.RemoveHandler(value);
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
        ///     Fired for every move the printer does. This can be used to provide a live preview of what the printer does.
        ///     This is only fired when moves are enabled (sendMoves action).
        /// </summary>
        public event Func<MovedEventArgs, Task> PrinterMovedAsync
        {
            add => _printerEvents.MovedEvent.AddHandler(value);
            remove => _printerEvents.MovedEvent.RemoveHandler(value);
        }

        /// <summary>
        ///     Fired when the print progresses to the next layer
        /// </summary>
        public event Func<LayerChangedEventArgs, Task> LayerChangedAsync
        {
            add => _printerEvents.LayerChangedEvent.AddHandler(value);
            remove => _printerEvents.LayerChangedEvent.RemoveHandler(value);
        }
        
        /// <summary>
        ///     Firmware requested a filament change on server side.
        /// </summary>
        public event Func<ChangeFilamentRequestedEventArgs, Task> ChangeFilamentRequestedAsync
        {
            add => _printerEvents.ChangeFilamentRequestedEvent.AddHandler(value);
            remove => _printerEvents.ChangeFilamentRequestedEvent.RemoveHandler(value);
        }
        
        /// <summary>
        ///     Fired for every printer command send to the repetier server. <br></br>
        ///     This is mainly useful to get the callback_id and printer name of commands and for debugging purposes.
        /// </summary>
        public event Func<PrinterCommandEventArgs, Task> PrinterCommandSendAsync
        {
            add => _printerEvents.CommandSendEvent.AddHandler(value);
            remove => _printerEvents.CommandSendEvent.RemoveHandler(value);
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
        ///     Fired for every server command send to the repetier server. <br></br>
        ///     This is mainly useful to get the callback_id of commands and for debugging purposes.
        /// </summary>
        public event Func<ServerCommandEventArgs, Task> ServerCommandSendAsync
        {
            add => _serverEvents.CommandSendEvent.AddHandler(value);
            remove => _serverEvents.CommandSendEvent.RemoveHandler(value);
        }

        /// <summary>
        ///     Fired when a command to the repetier server failed. <br></br>
        ///     This happens when the websocket client is unable to queue the command internally. <br></br>
        ///     To check if the server received the command either listen for the corresponding response.
        /// </summary>
        public event Func<CommandEventArgs, Task> CommandFailedAsync
        {
            add => _serverEvents.CommandFailedEvent.AddHandler(value);
            remove => _serverEvents.CommandFailedEvent.RemoveHandler(value);
        }

        /// <summary>
        ///    Fired when a request to the repetier server failed. <br></br>
        /// </summary>
        public event Func<HttpContextEventArgs, Task> HttpRequestFailedAsync
        {
            add => _clientEvents.HttpRequestFailedEvent.AddHandler(value);
            remove => _clientEvents.HttpRequestFailedEvent.RemoveHandler(value);
        }
        
        public event Func<ServerEventEventArgs, Task> ServerEventReceivedAsync
        {
            add => _serverEvents.EventReceivedEvent.AddHandler(value);
            remove => _serverEvents.EventReceivedEvent.RemoveHandler(value);
        }

        #endregion

        #endregion

        private readonly ILogger<RepetierConnection> _logger;
        private CommandDispatcher _commandDispatcher;
        private readonly CommandManager _commandManager;
        private IWebsocketClient WebSocketClient { get; set; }
        private IRestClient RestClient { get; set; }
        public RepetierSession Session { get; }
        private long _lastPingTimestamp;
        private List<Predicate<string>> _responseFilters = new();
        private List<Predicate<string>> _commandFilters = new();
        private List<Predicate<string>> _eventFilters = new();
    }
}
