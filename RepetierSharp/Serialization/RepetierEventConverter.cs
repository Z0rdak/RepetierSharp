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
using RepetierSharp.Util;

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
                KeyValuePair.Create("loginRequired", typeof(ServerEventData)),
                KeyValuePair.Create("logout", typeof(ServerEventData)),
                KeyValuePair.Create("userCredentials", typeof(ServerEventData)),
                KeyValuePair.Create(EventConstants.PRINTER_LIST_CHANGED, typeof(List<Printer>)),
                KeyValuePair.Create(EventConstants.LICENSE_CHANGED, typeof(EmptyEventData)),
                KeyValuePair.Create("messagesChanged", typeof(MessagesChanged)),
                KeyValuePair.Create("move", typeof(MoveEntry)),
                KeyValuePair.Create("log", typeof(LogEntry)),
                KeyValuePair.Create("gcodeInfoUpdated", typeof(GcodeInfo)),
                KeyValuePair.Create("gcodeAnalysisFinished", typeof(GcodeInfo)),
                KeyValuePair.Create("jobsChanged", typeof(PrinterEventData)), // TODO: Event
                KeyValuePair.Create("printJobAdded", typeof(EmptyEventData)), // TODO: Event
                KeyValuePair.Create("jobFinished", typeof(JobState)),
                KeyValuePair.Create("jobKilled", typeof(JobState)),
                KeyValuePair.Create("jobDeactivated", typeof(JobState)),
                KeyValuePair.Create("jobStarted", typeof(JobStarted)),
                KeyValuePair.Create("printerConditionChanged", typeof(PrinterConditionChanged)),
                KeyValuePair.Create("printqueueChanged", typeof(PrinterEventData)),
                KeyValuePair.Create("lastPrintsChanged", typeof(EmptyEventData)),
                KeyValuePair.Create("foldersChanged", typeof(EmptyEventData)),
                KeyValuePair.Create("eepromClear", typeof(EmptyEventData)),
                KeyValuePair.Create("eepromData", typeof(EepromData)),
                KeyValuePair.Create("state", typeof(PrinterState)),
                KeyValuePair.Create("config", typeof(PrinterConfig)),
                KeyValuePair.Create("firmwareChanged", typeof(FirmwareData)),
                KeyValuePair.Create("wifiChanged", typeof(WifiChanged)),
                KeyValuePair.Create("recoverChanged", typeof(RecoverChanged)),
                KeyValuePair.Create("hardwareInfo", typeof(HardwareInfo)),
                KeyValuePair.Create("dispatcherCount", typeof(DispatcherCount)),
                KeyValuePair.Create("workerFinished", typeof(WorkerFinished)),
                KeyValuePair.Create("newRenderImage", typeof(NewRenderImage)),
                KeyValuePair.Create("temp", typeof(TempEntry)),
                KeyValuePair.Create("settingChanged", typeof(SettingChanged)),
                KeyValuePair.Create("printerSettingChanged", typeof(PrinterSettingChanged)),
                KeyValuePair.Create("modelGroupListChanged", typeof(EmptyEventData)),
                KeyValuePair.Create("prepareJob", typeof(EmptyEventData)),
                KeyValuePair.Create("prepareJobFinished", typeof(EmptyEventData)),
                KeyValuePair.Create("changeFilamentRequested", typeof(EmptyEventData)),
                KeyValuePair.Create("remoteServersChanged", typeof(EmptyEventData)),
                KeyValuePair.Create("timelapseChanged", typeof(TimelapseChanged)),
                KeyValuePair.Create("gpioPinChanged", typeof(GpioPinChanged)),
                KeyValuePair.Create("gpioListChanged", typeof(EmptyEventData)),
                KeyValuePair.Create("externalLinksChanged", typeof(EmptyEventData)),
                KeyValuePair.Create("autoupdateStarted", typeof(EmptyEventData)),
                KeyValuePair.Create("pong", typeof(EmptyEventData)),
                KeyValuePair.Create("timer30", typeof(EmptyEventData)),
                KeyValuePair.Create("timer60", typeof(EmptyEventData)),
                KeyValuePair.Create("timer300", typeof(EmptyEventData)),
                KeyValuePair.Create("timer1800", typeof(EmptyEventData)),
                KeyValuePair.Create("timer3600", typeof(EmptyEventData)),
                KeyValuePair.Create("duetDialogOpened", typeof(DuetDialogOpened)),
                /* project event types */
                KeyValuePair.Create("projectChanged", typeof(ProjectStateChanged)),
                KeyValuePair.Create("projectDeleted", typeof(ProjectStateChanged)),
                KeyValuePair.Create("projectFolderChanged", typeof(ProjectFolderChanged)),
                KeyValuePair.Create("globalErrorsChanged", typeof(EmptyEventData)),
                KeyValuePair.Create("reloadKlipper", typeof(EmptyEventData)),
                /* not listed / custom event types */
                KeyValuePair.Create("layerChanged", typeof(LayerChanged)),
                KeyValuePair.Create(EventConstants.UPDATE_PRINTER_STATE, typeof(PrinterState))
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
                        case EventConstants.PRINTER_LIST_CHANGED:
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
