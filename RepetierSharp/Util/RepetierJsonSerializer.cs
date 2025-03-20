using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using RepetierSharp.Models;
using RepetierSharp.Models.Messages;

namespace RepetierSharp.Util
{
    /// <summary>
    ///     A custom JSON converter for repetier command responses.
    ///     This can't be covered by a custom JSON converter, inheriting
    ///     <see cref="System.Text.Json.Serialization.JsonConverter" /> ,
    ///     because the response type is determined by the command identifier.
    ///     It would be possible, if the command identifier would be retrievable from a static context,
    ///     but currently it is managed by the command manager instance of each repetier connection.
    /// </summary>
    public class RepetierJsonSerializer
    {
        private static readonly ILogger<RepetierJsonSerializer> _logger = LoggerFactory.Create(builder =>
        {
            builder.AddConsole();
        }).CreateLogger<RepetierJsonSerializer>();

        public static IRepetierResponse? DeserializeResponse(int callbackId, string commandIdentifier, 
            byte[] msgBytes, JsonSerializerOptions options)
        {
            try
            {
                switch ( commandIdentifier )
                {
                    case CommandConstants.PING:
                        return JsonSerializer.Deserialize(msgBytes, typeof(PingResponse), options) as IRepetierResponse;
                    case CommandConstants.LOGIN:
                        return JsonSerializer.Deserialize(msgBytes, typeof(LoginResponse), options) as IRepetierResponse;
                    case CommandConstants.LIST_PRINTER:
                        var listPrintersMessage = JsonSerializer.Deserialize<List<Printer>>(msgBytes, options);
                        return new ListPrinterResponse
                        {
                            Printers = listPrintersMessage ?? new List<Printer>()
                        };
                    case CommandConstants.STATE_LIST:
                        var stateListMessage =
                            JsonSerializer.Deserialize<Dictionary<string, PrinterState>>(msgBytes, options);
                        return new StateListResponse
                        {
                            PrinterStates = stateListMessage ?? new Dictionary<string, PrinterState>()
                        };
                    case CommandConstants.MESSAGES:
                        var messagesMessage = JsonSerializer.Deserialize<List<Message>>(msgBytes, options);
                        return new MessageList
                        {
                            Messages = messagesMessage ?? new List<Message>()
                        };
                    case CommandConstants.LIST_MODELS:
                        var modelList = JsonSerializer.Deserialize<ModelInfoList>(msgBytes);
                        return modelList;
                    case CommandConstants.LIST_JOBS:
                        var jobList = JsonSerializer.Deserialize<ModelInfoList>(msgBytes);
                        return jobList;
                    case CommandConstants.MODEL_INFO:
                        return JsonSerializer.Deserialize(msgBytes, typeof(ModelInfo), options) as IRepetierResponse;
                    case CommandConstants.JOB_INFO:
                        return JsonSerializer.Deserialize(msgBytes, typeof(ModelInfo), options) as IRepetierResponse;
                    case CommandConstants.COPY_MODEL:
                        return JsonSerializer.Deserialize(msgBytes, typeof(OkErrorResponse), options) as IRepetierResponse;
                    case CommandConstants.CREATE_USER:
                        return JsonSerializer.Deserialize(msgBytes, typeof(StatusResponse), options) as IRepetierResponse;
                    case CommandConstants.DELETE_USER:
                        return JsonSerializer.Deserialize(msgBytes, typeof(StatusResponse), options) as IRepetierResponse;
                    case CommandConstants.USER_LIST:
                        var userList = JsonSerializer.Deserialize<UserListResponse>(msgBytes, options);
                        // payload: { "loginRequired": true, "users": [ { "id": 1, "login": "repetier", "permissions": 15 } ] }
                        return JsonSerializer.Deserialize(msgBytes, typeof(UserListResponse), options) as IRepetierResponse;
                    case CommandConstants.LOGOUT:
                    case CommandConstants.REMOVE_JOB:
                    case CommandConstants.SEND:
                    case CommandConstants.EMERGENCY_STOP:
                    case CommandConstants.ACTIVATE:
                    case CommandConstants.DEACTIVATE:
                    case CommandConstants.UPDATE_USER:
                    case CommandConstants.START_JOB:
                    case CommandConstants.STOP_JOB:
                    case CommandConstants.CONTINUE_JOB:
                        return JsonSerializer.Deserialize(msgBytes, typeof(EmptyResponse), options) as IRepetierResponse;
                    default:
                        return null;
                }
            }
            catch ( Exception e )
            {
                _logger.LogError(e, "Error deserializing command '{}' with id {}", commandIdentifier, callbackId);
                throw;
            }
        }
    }
}
