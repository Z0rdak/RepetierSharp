using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Logging;
using RepetierSharp.Config;
using RepetierSharp.Models;
using RepetierSharp.Models.Communication;
using RepetierSharp.Models.Config;
using RepetierSharp.Models.Events;
using static RepetierSharp.Util.EventConstants;

namespace RepetierSharp.Serialization
{
    public class RepetierBaseEventConverter : JsonConverter<IRepetierEvent>
    {
        private static readonly Dictionary<string, Type> s_extendableEventTypes = new();
        private static readonly ILogger<RepetierBaseEventConverter> _logger = LoggerFactory.Create(builder =>
        {
            builder.AddConsole();
        }).CreateLogger<RepetierBaseEventConverter>();

        public static ImmutableDictionary<string, Type> GetExtendableEventTypes()
        {
            return s_extendableEventTypes.ToImmutableDictionary();
        }

        private static readonly ImmutableDictionary<string, Type> s_eventTypes = ImmutableDictionary.CreateRange
        (new[]
            {
                /* core event types */
       
                // TODO: Check for own events
                KeyValuePair.Create(PRINTER_LIST_CHANGED, typeof(List<Printer>)),
                KeyValuePair.Create(LICENSE_CHANGED, typeof(EmptyEventData)),
                KeyValuePair.Create(MQTT_STATE_CHANGED, typeof(MqttStateChanged)), 
                KeyValuePair.Create(JOBS_CHANGED, typeof(PrinterEventData)), 
                KeyValuePair.Create(PRINT_JOB_ADDED, typeof(EmptyEventData)), 
                // KeyValuePair.Create("emergencyStop", typeof(EmptyEventData)), // TODO: Event 
                KeyValuePair.Create(CHANGE_FILAMENT_REQUESTED, typeof(EmptyEventData)), 
                KeyValuePair.Create(LOGIN_REQUIRED, typeof(ServerEventData)), 
                KeyValuePair.Create(JOB_FINISHED, typeof(JobState)),
                KeyValuePair.Create(JOB_KILLED, typeof(JobState)),
                KeyValuePair.Create(JOB_DEACTIVATED, typeof(JobState)),
                KeyValuePair.Create(JOB_STARTED, typeof(JobStarted)),
                KeyValuePair.Create(PRINTER_CONDITION_CHANGED, typeof(PrinterConditionChanged)),
                KeyValuePair.Create(PRINT_QUEUE_CHANGED, typeof(PrinterEventData)),
                KeyValuePair.Create(LAST_PRINTS_CHANGED, typeof(EmptyEventData)),
                
                KeyValuePair.Create(LOGOUT, typeof(ServerEventData)),
                KeyValuePair.Create(USER_CREDENTIALS, typeof(ServerEventData)),
                KeyValuePair.Create(MESSAGES_CHANGED, typeof(MessagesChanged)),
                KeyValuePair.Create(MOVE, typeof(MoveEntry)),
                KeyValuePair.Create(LOG, typeof(LogEntry)),
                KeyValuePair.Create(GCODE_INFO_UPDATED, typeof(GcodeInfo)),
                KeyValuePair.Create(GCODE_ANALYSIS_FINISHED, typeof(GcodeInfo)),
                KeyValuePair.Create(FOLDERS_CHANGED, typeof(EmptyEventData)),
                KeyValuePair.Create(EEPROM_CLEAR, typeof(EmptyEventData)),
                KeyValuePair.Create(EEPROM_DATA, typeof(EepromData)),
                KeyValuePair.Create(STATE, typeof(PrinterState)),
                KeyValuePair.Create(CONFIG, typeof(PrinterConfig)),
                KeyValuePair.Create(FIRMWARE_CHANGED, typeof(FirmwareData)),
                KeyValuePair.Create(WIFI_CHANGED, typeof(WifiChanged)),
                KeyValuePair.Create(RECOVER_CHANGED, typeof(RecoverChanged)),
                KeyValuePair.Create(HARDWARE_INFO, typeof(HardwareInfo)),
                KeyValuePair.Create(DISPATCHER_COUNT, typeof(DispatcherCount)),
                KeyValuePair.Create(WORKER_FINISHED, typeof(WorkerFinished)),
                KeyValuePair.Create(NEW_RENDER_IMAGE, typeof(NewRenderImage)),
                KeyValuePair.Create(TEMP, typeof(TempEntry)),
                KeyValuePair.Create(SETTING_CHANGED, typeof(SettingChanged)),
                KeyValuePair.Create(PRINTER_SETTING_CHANGED, typeof(PrinterSettingChanged)),
                KeyValuePair.Create(MODEL_GROUPLIST_CHANGED, typeof(EmptyEventData)),
                KeyValuePair.Create(PREPARE_JOB, typeof(EmptyEventData)),
                KeyValuePair.Create(PREPARE_JOB_FINIHSED, typeof(EmptyEventData)),
                KeyValuePair.Create(REMOTE_SERVERS_CHANGED, typeof(EmptyEventData)),
                KeyValuePair.Create(TIMELAPSE_CHANGED, typeof(TimelapseChanged)),
                KeyValuePair.Create(GPIO_PIN_CHANGED, typeof(GpioPinChanged)),
                KeyValuePair.Create(GPIO_LIST_CHANGED, typeof(EmptyEventData)),
                KeyValuePair.Create(EXTERNAL_LINKS_CHANGED, typeof(EmptyEventData)),
                KeyValuePair.Create(AUTO_UPDATE_STARTED, typeof(EmptyEventData)),
                KeyValuePair.Create(PONG, typeof(EmptyEventData)),
                KeyValuePair.Create("ping", typeof(EmptyEventData)),
                KeyValuePair.Create(TIMER_30, typeof(EmptyEventData)),
                KeyValuePair.Create(TIMER_60, typeof(EmptyEventData)),
                KeyValuePair.Create(TIMER_300, typeof(EmptyEventData)),
                KeyValuePair.Create(TIMER_1800, typeof(EmptyEventData)),
                KeyValuePair.Create(TIMER_3600, typeof(EmptyEventData)),
                KeyValuePair.Create(DUET_DIALOG_OPENED, typeof(DuetDialogOpened)),
                /* project event types */
                KeyValuePair.Create(PROJECT_CHANGED, typeof(ProjectStateChanged)),
                KeyValuePair.Create(PROJECT_DELETED, typeof(ProjectStateChanged)),
                KeyValuePair.Create(PROJECT_FOLDER_CHANGED, typeof(ProjectFolderChanged)),
                KeyValuePair.Create(GLOBAL_ERRORS_CHANGED, typeof(EmptyEventData)),
                KeyValuePair.Create(RELOAD_KLIPPER, typeof(EmptyEventData)),
                /* not listed in docs -> reverse engineered through web-gui */
                KeyValuePair.Create(LAYER_CHANGED, typeof(LayerChanged)),
                KeyValuePair.Create(UPDATE_PRINTER_STATE, typeof(PrinterState)),
                KeyValuePair.Create(MQTT_STATE_CHANGED, typeof(MqttStateChanged))
            }
        );

        public static bool RemoveDeserializationMapping(string eventType)
        {
            return s_extendableEventTypes.Remove(eventType);
        }

        public static bool AddDeserializationMapping(string eventType, Type type)
        {
            // prevent overriding of existing/build-in event types 
            return !s_eventTypes.ContainsKey(eventType) && s_extendableEventTypes.TryAdd(eventType, type);
        }

        public override IRepetierEvent Read(ref Utf8JsonReader reader, Type typeToConvert,
            JsonSerializerOptions options)
        {
            if ( reader.TokenType != JsonTokenType.StartObject )
            {
                throw new JsonException("Expected start of object.");
            }

            using var document = JsonDocument.ParseValue(ref reader);
            var jsonObject = document.RootElement;
            string? eventDiscriminator = null;
            string? printer = null;
            IEventData? repetierEvent = null;

            if ( jsonObject.TryGetProperty("event", out var eventJsonElement) )
                eventDiscriminator = eventJsonElement.GetString();

            if ( string.IsNullOrEmpty(eventDiscriminator) )
            {
                _logger.LogWarning("Missing event discriminator");
                throw new JsonException("Missing event discriminator.");
            }

            if ( jsonObject.TryGetProperty("printer", out var printerJsonElement) )
            {
                printer = printerJsonElement.GetString();
            }

            if ( jsonObject.TryGetProperty("data", out var dataJsonElement) )
            {
                var isInCustomEventType = s_extendableEventTypes.TryGetValue(eventDiscriminator, out var extendableType);
                if ( isInCustomEventType )
                {
                    var res = JsonSerializer.Deserialize(dataJsonElement.GetRawText(), extendableType, options);
                    if ( res == null )
                    {
                        _logger.LogWarning("Unable to deserialize event with type: {}", eventDiscriminator);
                        throw new JsonException($"Unable to deserialize event with type: {eventDiscriminator}");
                    }
                    repetierEvent = (IEventData)res;
                }

                var isInEventTypes = s_eventTypes.TryGetValue(eventDiscriminator, out var eventType);
                if ( isInEventTypes )
                {
                    switch ( eventDiscriminator )
                    {
                        case PRINTER_LIST_CHANGED:
                            var printerList = JsonSerializer.Deserialize<List<Printer>>(dataJsonElement.GetRawText(), options);
                            repetierEvent = new PrinterListChanged() {Printers = printerList ?? new List<Printer>()} ;
                            break;
                        default:
                            {
                                var res = JsonSerializer.Deserialize(dataJsonElement.GetRawText(), eventType, options);
                                if ( res == null )
                                {
                                    _logger.LogWarning("Unable to deserialize event with type: {}", eventDiscriminator);
                                    throw new JsonException($"Unable to deserialize event with type: {eventDiscriminator}");
                                }

                                repetierEvent = (IEventData)res;
                                break;
                            }
                    }
                }
                if ( !isInCustomEventType && !isInEventTypes )
                {
                    _logger.LogWarning("No type defined for event: {}", eventDiscriminator);
                    throw new JsonException($"No type defined for event: {eventDiscriminator}.");
                }
            }

            return new IRepetierEvent
            {
                Event = eventDiscriminator, Printer = printer, EventData = repetierEvent
            };
        }

        public override void Write(Utf8JsonWriter writer, IRepetierEvent value, JsonSerializerOptions options)
        {
            JsonSerializer.Serialize(writer, value, value.GetType(), options);
        }
    }
}
