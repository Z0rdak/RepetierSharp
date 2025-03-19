using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text.Json;
using System.Text.Json.Serialization;
using RepetierSharp.Config;
using RepetierSharp.Models;
using RepetierSharp.Models.Config;
using RepetierSharp.Models.Events;

namespace RepetierSharp.Util
{
    public class RepetierBaseEventConverter : JsonConverter<RepetierBaseEvent>
    {
        private static readonly Dictionary<string, Type> s_extendableEventTypes = new();

        public static ImmutableDictionary<string, Type> GetExtendableEventTypes()
        {
            return s_extendableEventTypes.ToImmutableDictionary();
        }

        private static readonly ImmutableDictionary<string, Type> s_eventTypes = ImmutableDictionary.CreateRange
        (new[]
            {
                /* core event types */
                KeyValuePair.Create("loginRequired", typeof(LoginRequired)),
                KeyValuePair.Create("logout", typeof(EmptyEvent)),
                KeyValuePair.Create("userCredentials", typeof(UserCredentials)),
                KeyValuePair.Create("printerListChanged", typeof(PrinterListChanged)),
                KeyValuePair.Create("messagesChanged", typeof(MessagesChanged)),
                KeyValuePair.Create("move", typeof(MoveEntry)),
                KeyValuePair.Create("log", typeof(LogEntry)),
                KeyValuePair.Create("gcodeInfoUpdated", typeof(GcodeInfoUpdated)),
                KeyValuePair.Create("jobsChanged", typeof(EmptyEvent)),
                KeyValuePair.Create("printJobAdded", typeof(EmptyEvent)),
                KeyValuePair.Create("jobFinished", typeof(JobState)),
                KeyValuePair.Create("jobKilled", typeof(JobState)),
                KeyValuePair.Create("jobDeactivated", typeof(JobState)),
                KeyValuePair.Create("jobStarted", typeof(JobStarted)),
                KeyValuePair.Create("printerConditionChanged", typeof(PrinterConditionChanged)),
                KeyValuePair.Create("printqueueChanged", typeof(EmptyEvent)),
                KeyValuePair.Create("lastPrintsChanged", typeof(EmptyEvent)),
                KeyValuePair.Create("foldersChanged", typeof(EmptyEvent)),
                KeyValuePair.Create("eepromClear", typeof(EmptyEvent)),
                KeyValuePair.Create("eepromData", typeof(EepromData)),
                KeyValuePair.Create("state", typeof(PrinterState)),
                KeyValuePair.Create("config", typeof(PrinterConfig)),
                KeyValuePair.Create("firmwareChanged", typeof(FirmwareData)),
                KeyValuePair.Create("wifiChanged", typeof(WifiChanged)),
                KeyValuePair.Create("hardwareInfo", typeof(HardwareInfo)),
                KeyValuePair.Create("temp", typeof(TempEntry)),
                KeyValuePair.Create("settingChanged", typeof(SettingChanged)),
                KeyValuePair.Create("printerSettingChanged", typeof(PrinterSettingChanged)),
                KeyValuePair.Create("modelGroupListChanged", typeof(EmptyEvent)),
                KeyValuePair.Create("prepareJob", typeof(EmptyEvent)),
                KeyValuePair.Create("prepareJobFinished", typeof(EmptyEvent)),
                KeyValuePair.Create("changeFilamentRequested", typeof(EmptyEvent)),
                KeyValuePair.Create("remoteServersChanged", typeof(EmptyEvent)),
                KeyValuePair.Create("timelapseChanged", typeof(TimelapseChanged)),
                KeyValuePair.Create("gpioPinChanged", typeof(GpioPinChanged)),
                KeyValuePair.Create("gpioListChanged", typeof(EmptyEvent)),
                KeyValuePair.Create("externalLinksChanged", typeof(EmptyEvent)),
                KeyValuePair.Create("autoupdateStarted", typeof(EmptyEvent)),
                KeyValuePair.Create("pong", typeof(EmptyEvent)),
                KeyValuePair.Create("timer30", typeof(EmptyEvent)),
                KeyValuePair.Create("timer60", typeof(EmptyEvent)),
                KeyValuePair.Create("timer300", typeof(EmptyEvent)),
                KeyValuePair.Create("timer1800", typeof(EmptyEvent)),
                KeyValuePair.Create("timer3600", typeof(EmptyEvent)),
                KeyValuePair.Create("duetDialogOpened", typeof(DuetDialogOpened)),
                /* project event types */
                KeyValuePair.Create("projectChanged", typeof(ProjectStateChanged)),
                KeyValuePair.Create("projectDeleted", typeof(ProjectStateChanged)),
                KeyValuePair.Create("projectFolderChanged", typeof(ProjectFolderChanged)),
                KeyValuePair.Create("globalErrorsChanged", typeof(EmptyEvent)),
                KeyValuePair.Create("reloadKlipper", typeof(EmptyEvent)),
                /* not listed / custom event types */
                KeyValuePair.Create("layerChanged", typeof(LayerChanged)),
                KeyValuePair.Create("updatePrinterState", typeof(PrinterState))
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

        public override RepetierBaseEvent Read(ref Utf8JsonReader reader, Type typeToConvert,
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
            IRepetierEvent? repetierEvent = null;

            if (jsonObject.TryGetProperty("event", out var eventJsonElement)) 
                eventDiscriminator = eventJsonElement.GetString();
            if ( eventDiscriminator == "config" )
            {
                Console.WriteLine(document.RootElement.GetRawText());
            }

            if ( string.IsNullOrEmpty(eventDiscriminator) )
            {
                throw new JsonException("Missing event discriminator.");
            }

            if ( jsonObject.TryGetProperty("printer", out var printerJsonElement) )
            {
                printer = printerJsonElement.GetString();
            }
            
            if (jsonObject.TryGetProperty("data", out var dataJsonElement))
            {
                var isInCustomEventType = s_extendableEventTypes.TryGetValue(eventDiscriminator, out var extendableType);
                if (isInCustomEventType)
                {
                    var res = JsonSerializer.Deserialize(dataJsonElement.GetRawText(), extendableType, options);
                    if ( res == null )
                        throw new JsonException($"Unable to deserialize event with type: {eventDiscriminator}");
                    repetierEvent = (IRepetierEvent)res;
                }

                var isInEventTypes = s_eventTypes.TryGetValue(eventDiscriminator, out var eventType);
                if (isInEventTypes)
                {
                    var res = JsonSerializer.Deserialize(dataJsonElement.GetRawText(), eventType, options);
                    if ( res == null ) 
                        throw new JsonException($"Unable to deserialize event with type: {eventDiscriminator}");
                    repetierEvent = (IRepetierEvent)res;
                }
                if (!isInCustomEventType && !isInEventTypes)
                {
                    Console.WriteLine(JsonSerializer.Serialize(jsonObject, new JsonSerializerOptions{WriteIndented = true}));
                    throw new JsonException($"No type defined for event: {eventDiscriminator}.");
                }
            }

           
            return new RepetierBaseEvent
            {
                Event = eventDiscriminator,
                Printer = printer,
                RepetierEvent = repetierEvent
            };
        }

        public override void Write(Utf8JsonWriter writer, RepetierBaseEvent value, JsonSerializerOptions options)
        {
            JsonSerializer.Serialize(writer, value, value.GetType(), options);
        }
    }
}
