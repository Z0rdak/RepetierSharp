using System.Collections.Generic;
using System.Text.Json;
using RepetierSharp.Models;
using RepetierSharp.Models.Events;
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
    public static class RepetierJsonSerializer
    {
        public static IRepetierResponse? DeserializeResponse(string commandIdentifier, byte[] msgBytes,
            JsonSerializerOptions options)
        {
            switch ( commandIdentifier )
            {
                case CommandConstants.PING:
                    return JsonSerializer.Deserialize(msgBytes, typeof(PingResponse), options) as IRepetierResponse;
                case CommandConstants.LOGIN:
                    return JsonSerializer.Deserialize(msgBytes, typeof(LoginResponse), options) as IRepetierResponse;
                case CommandConstants.LIST_PRINTER:
                    var listPrintersMessage = JsonSerializer.Deserialize<List<Printer>>(msgBytes, options);
                    return new ListPrinterResponse { Printers = listPrintersMessage ?? new List<Printer>() };
                case CommandConstants.STATE_LIST:
                    var stateListMessage =
                        JsonSerializer.Deserialize<Dictionary<string, PrinterState>>(msgBytes, options);
                    return new StateListResponse
                    {
                        PrinterStates = stateListMessage ?? new Dictionary<string, PrinterState>()
                    };
                case CommandConstants.MESSAGES:
                    var messagesMessage = JsonSerializer.Deserialize<List<Message>>(msgBytes, options);
                    return new MessageList { Messages = messagesMessage ?? new List<Message>() };
                case CommandConstants.LIST_MODELS:
                    var modelList = JsonSerializer.Deserialize<List<ModelResponse>>(msgBytes, options);
                    return new ModelResponseList { Models = modelList ?? new List<ModelResponse>() };
                case CommandConstants.LIST_JOBS:
                    var jobList = JsonSerializer.Deserialize<List<ModelResponse>>(msgBytes, options);
                    return new ModelResponseList { Models = jobList ?? new List<ModelResponse>() };
                case CommandConstants.MODEL_INFO:
                    return JsonSerializer.Deserialize(msgBytes, typeof(ModelResponse), options) as IRepetierResponse;
                case CommandConstants.JOB_INFO:
                    return JsonSerializer.Deserialize(msgBytes, typeof(ModelResponse), options) as IRepetierResponse;
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
    }
}
