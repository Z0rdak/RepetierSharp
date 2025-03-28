using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Logging;
using RepetierSharp.Models;
using RepetierSharp.Models.Communication;
using RepetierSharp.Models.Responses;
using RepetierSharp.Util;

namespace RepetierSharp.Serialization
{
    public class RepetierResponseConverter : JsonConverter<RepetierResponse>
    {
        private readonly ILogger<RepetierResponseConverter> _logger;
        private readonly string commandId;
        public RepetierResponseConverter(string commandId)
        {
            using var factory = LoggerFactory.Create(builder =>
            {
                builder.AddConsole();
            });
            _logger = factory.CreateLogger<RepetierResponseConverter>();
            this.commandId = commandId;
        }

        private static readonly ImmutableDictionary<string, Type> s_responseTypes = ImmutableDictionary.CreateRange
        (new[]
            {
                KeyValuePair.Create(CommandConstants.LOGIN, typeof(LoginResponse)),
                KeyValuePair.Create(CommandConstants.PING, typeof(PingResponse)),
                KeyValuePair.Create(CommandConstants.EXTEND_PING, typeof(OkResponse)),
                KeyValuePair.Create(CommandConstants.LIST_PRINTER, typeof(PrinterListResponse)),
                KeyValuePair.Create(CommandConstants.STATE_LIST, typeof(StateListResponse)),
                KeyValuePair.Create(CommandConstants.MESSAGES, typeof(MessageList)),
                KeyValuePair.Create(CommandConstants.LIST_MODELS, typeof(ModelInfoList)),
                KeyValuePair.Create(CommandConstants.LIST_JOBS, typeof(ModelInfoList)),
                KeyValuePair.Create(CommandConstants.MODEL_INFO, typeof(ModelInfo)),
                KeyValuePair.Create(CommandConstants.JOB_INFO, typeof(ModelInfo)),
                KeyValuePair.Create(CommandConstants.COPY_MODEL, typeof(OkErrorResponse)),
                KeyValuePair.Create(CommandConstants.CREATE_USER, typeof(StatusResponse)),
                KeyValuePair.Create(CommandConstants.DELETE_USER, typeof(StatusResponse)),
                KeyValuePair.Create(CommandConstants.USER_LIST, typeof(UserListResponse)),
                KeyValuePair.Create(CommandConstants.CONTINUE_JOB, typeof(EmptyResponse)),
                KeyValuePair.Create(CommandConstants.SET_MQTT_CONFIG, typeof(MqttConfigResponse)),
                KeyValuePair.Create(CommandConstants.GET_MQTT_CONFIG, typeof(MqttConfigResponse)),
                KeyValuePair.Create(CommandConstants.GET_MQTT_STATE, typeof(MqttStateResponse)),
            }
        );
        public override RepetierResponse? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            
            if ( reader.TokenType != JsonTokenType.StartObject )
            {
                throw new JsonException("Expected start of object.");
            }

            using var document = JsonDocument.ParseValue(ref reader);
            var jsonObject = document.RootElement;
            int callbackId = -1;
            string sessionId = null;
            IResponseData? responseData = null;

            if (jsonObject.TryGetProperty("callback_id", out var callbackIdJsonElement)) 
                callbackId = callbackIdJsonElement.GetInt32();
            
            if (jsonObject.TryGetProperty("session", out var sessionJsonElement)) 
                sessionId = sessionJsonElement.GetString();
            
            if ( callbackId == -1)
            {
                _logger.LogWarning("Missing commandId discriminator"); 
                throw new JsonException("Missing commandId discriminator.");
            }

            if (jsonObject.TryGetProperty("data", out var dataJsonElement))
            {
                var isInResponseTypes = s_responseTypes.TryGetValue(commandId, out var eventType);
                if (isInResponseTypes && eventType != null)
                {
                    switch ( commandId )
                    {
                        case CommandConstants.STATE_LIST:
                            var printerStates = JsonSerializer.Deserialize<Dictionary<string, PrinterState>>(dataJsonElement.GetRawText(), options);
                            if (printerStates == null) 
                                throw new JsonException($"Error deserializing printer states: {dataJsonElement.GetRawText()}");
                            responseData = new StateListResponse() { PrinterStates = printerStates };
                            break;
                        case CommandConstants.MESSAGES:
                            var messages = JsonSerializer.Deserialize<List<Message>>(dataJsonElement.GetRawText(), options);
                            responseData = new MessageList() {Messages = messages ?? new List<Message>() };
                            break;
                        default:
                            {
                                var res = JsonSerializer.Deserialize(dataJsonElement.GetRawText(), eventType, options);
                                if ( res == null )
                                {
                                    _logger.LogWarning("Unable to deserialize event with type: {}",callbackId); 
                                    throw new JsonException($"Unable to deserialize event with type: {callbackId}");
                                }
                                responseData = (IResponseData)res;
                            }
                            break;
                    }
                    
                   
                    if ( responseData == null )
                    {
                        _logger.LogWarning("Unable to deserialize response for: {cmd}, Id={id}", commandId, callbackId);
                        throw new JsonException($"Unable to deserialize response for: {commandId}, Id={callbackId}");
                    }
                }
                if (!isInResponseTypes)
                {
                    _logger.LogWarning("No type defined for command: {}", commandId); 
                    throw new JsonException($"No type defined for command: {commandId}.");
                }
            }
            
            if ( responseData == null )
            {
                _logger.LogWarning(
                    "Unable to deserialize response for CommandIdentifier='{Command}' with Id='{CallbackId}': '{Response}'",
                    commandId, callbackId, dataJsonElement.GetRawText());
                throw new JsonException("Unable to deserialize response.");
            }
            
            return new RepetierResponse()
            {
                CommandId = commandId,
                CallBackId = callbackId,
                Data = responseData,
                EventList = false,
                SessionId = sessionId
            };
        }

        public override void Write(Utf8JsonWriter writer, RepetierResponse value, JsonSerializerOptions options)
        {
            JsonSerializer.Serialize(writer, value, value.GetType(), options);
        }
    }
}
