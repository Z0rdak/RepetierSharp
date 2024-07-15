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
        private static readonly Dictionary<string, Type> ExtendableEventTypes = new();

        public static ImmutableDictionary<string, Type> GetExtendableEventTypes()
        {
            return ExtendableEventTypes.ToImmutableDictionary();
        }

        private static readonly ImmutableDictionary<string, Type> EventTypes = ImmutableDictionary.CreateRange
        (new[]
            {
                /* core event types */ KeyValuePair.Create("loginRequired", typeof(LoginRequired)),
                KeyValuePair.Create("logout", typeof(EmptyEvent)),
                KeyValuePair.Create("userCredentials", typeof(UserCredentials)),
                KeyValuePair.Create("printerListChanged", typeof(PrinterListChanged)),
                KeyValuePair.Create("messagesChanged", typeof(MessagesChanged)),
                KeyValuePair.Create("move", typeof(MoveEntry)), KeyValuePair.Create("log", typeof(LogEntry)),
                KeyValuePair.Create("gcodeInfoUpdated", typeof(GcodeInfoUpdated)),
                KeyValuePair.Create("jobsChanged", typeof(EmptyEvent)),
                KeyValuePair.Create("printJobAdded", typeof(EmptyEvent)),
                KeyValuePair.Create("jobFinished", typeof(JobState)),
                KeyValuePair.Create("jobKilled", typeof(JobState)),
                KeyValuePair.Create("jobDeactivated", typeof(JobState)),
                KeyValuePair.Create("jobStarted", typeof(JobStarted)),
                KeyValuePair.Create("printerConditionChanged", typeof(PrinterConditionChanged)),
                KeyValuePair.Create("printqueueChanged", typeof(EmptyEvent)),
                KeyValuePair.Create("foldersChanged", typeof(EmptyEvent)),
                KeyValuePair.Create("eepromClear", typeof(EmptyEvent)),
                KeyValuePair.Create("eepromData", typeof(EepromData)),
                KeyValuePair.Create("state", typeof(PrinterStateChanged)),
                KeyValuePair.Create("config", typeof(PrinterConfig)),
                KeyValuePair.Create("firmwareChanged", typeof(FirmwareData)),
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
                KeyValuePair.Create("timer30", typeof(EmptyEvent)), KeyValuePair.Create("timer60", typeof(EmptyEvent)),
                KeyValuePair.Create("timer300", typeof(EmptyEvent)),
                KeyValuePair.Create("timer1800", typeof(EmptyEvent)),
                KeyValuePair.Create("timer3600", typeof(EmptyEvent)),
                KeyValuePair.Create("duetDialogOpened", typeof(DuetDialogOpened)),
                /* project event types */ KeyValuePair.Create("projectChanged", typeof(ProjectStateChanged)),
                KeyValuePair.Create("projectDeleted", typeof(ProjectStateChanged)),
                KeyValuePair.Create("projectFolderChanged", typeof(ProjectFolderChanged)),
                KeyValuePair.Create("globalErrorsChanged", typeof(EmptyEvent)),
                KeyValuePair.Create("reloadKlipper", typeof(EmptyEvent)),
                /* not listed / custom event types */ KeyValuePair.Create("layerChanged", typeof(LayerChanged)),
                KeyValuePair.Create("updatePrinterState", typeof(PrinterState))
            }
        );

        public static bool RemoveDeserializationMapping(string eventType)
        {
            return ExtendableEventTypes.Remove(eventType);
        }

        public static bool AddDeserializationMapping(string eventType, Type type)
        {
            // prevent overriding of existing/build-in event types 
            if ( EventTypes.ContainsKey(eventType) )
            {
                return false;
            }

            return ExtendableEventTypes.TryAdd(eventType, type);
        }

        public override RepetierBaseEvent Read(ref Utf8JsonReader reader, Type typeToConvert,
            JsonSerializerOptions options)
        {
            if ( reader.TokenType != JsonTokenType.StartObject )
            {
                throw new JsonException("Expected start of object.");
            }

            string eventDiscriminator = null;
            string printer = null;
            IRepetierEvent repetierEvent = null;

            while ( reader.Read() )
            {
                if ( reader.TokenType == JsonTokenType.EndObject )
                {
                    break;
                }

                if ( reader.TokenType == JsonTokenType.PropertyName )
                {
                    var propertyName = reader.GetString();
                    reader.Read();

                    switch ( propertyName )
                    {
                        case "event":
                            eventDiscriminator = reader.GetString();
                            break;
                        case "printer":
                            printer = reader.GetString();
                            break;
                        case "data":
                            if ( eventDiscriminator != null )
                            {
                                if ( ExtendableEventTypes.TryGetValue(eventDiscriminator, out var extendableType) )
                                {
                                    var res = JsonSerializer.Deserialize(ref reader, extendableType, options);
                                    if ( res == null )
                                    {
                                        throw new JsonException(
                                            $"Unable to deserialize event with type: {eventDiscriminator}");
                                    }

                                    repetierEvent = (IRepetierEvent)res;
                                }

                                if ( EventTypes.TryGetValue(eventDiscriminator, out var eventType) )
                                {
                                    var res = JsonSerializer.Deserialize(ref reader, eventType, options);
                                    if ( res == null )
                                    {
                                        throw new JsonException(
                                            $"Unable to deserialize event with type: {eventDiscriminator}");
                                    }

                                    repetierEvent = (IRepetierEvent)res;
                                }
                            }
                            else
                            {
                                throw new JsonException($"Unknown event type: {eventDiscriminator}");
                            }

                            break;
                    }
                }
            }

            if ( eventDiscriminator == null )
            {
                throw new JsonException("Missing event discriminator.");
            }

            return new RepetierBaseEvent
            {
                Event = eventDiscriminator, Printer = printer, RepetierEvent = repetierEvent
            };
        }

        public override void Write(Utf8JsonWriter writer, RepetierBaseEvent value, JsonSerializerOptions options)
        {
            throw new NotImplementedException("Serialization is not implemented.");
        }
    }
}
