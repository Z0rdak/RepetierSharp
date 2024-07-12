using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text.Json;
using System.Text.Json.Serialization;
using RepetierSharp.Models;
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
        
        private static readonly Dictionary<string, Type> EventTypes = new()
        {
            { "changeFilamentRequested", typeof(ChangeFilament) },
            { "eepromData", typeof(EepromData) },
            { "globalErrorsChanged", typeof(GlobalErrorsChanged) },
            { "jobsChanged", typeof(JobsChanged) },
            { "jobStarted", typeof(JobStarted) },
            { "jobKilled", typeof(JobState) },
            { "jobDeactivated", typeof(JobState) },
            { "jobFinished", typeof(JobState) },
            { "layerChanged", typeof(LayerChanged) },
            { "logEntry", typeof(LogEntry) },
            { "logout", typeof(Logout) },
            { "messagesChanged", typeof(MessagesChanged) },
            { "move", typeof(MoveEntry) },
            { "printerConditionChanged", typeof(PrinterConditionChanged) },
            { "printerListChanged", typeof(PrinterListChanged) },
            { "printerSettingChanged", typeof(PrinterSettingChanged) },
            { "state", typeof(PrinterStateChanged) },
            { "settingChanged", typeof(SettingChanged) },
            { "temp", typeof(TempEntry) },
            { "userCredentials", typeof(UserCredentials) },
        };
        
        public override RepetierBaseEvent Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
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
