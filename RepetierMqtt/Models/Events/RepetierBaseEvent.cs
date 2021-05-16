using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

namespace RepetierMqtt.Models.Events
{
    public class RepetierBaseEvent
    {
        public static Dictionary<string, Type> RepetierEventType { get; } = new Dictionary<string, Type>
        {
            ["logout"] = typeof(LogoutEvent),
            ["loginRequired"] = typeof(LoginRequiredEvent),
            ["userCredentials"] = typeof(UserCredentialsEvent),
            ["printerListChanged"] = typeof(PrinterListChangedEvent),
            ["messagesChanged"] = typeof(MessageChangedEvent),
            ["log"] = typeof(LogEvent),
            ["jobsChanged"] = typeof(JobsChangedEvent),
            ["jobFinished"] = typeof(JobFinishedEvent),
            ["jobKilled"] = typeof(JobKilledEvent),
            ["jobStarted"] = typeof(JobStartedEvent),
            ["state"] = typeof(PrinterStateChangeEvent),
            ["temp"] = typeof(TempChangeEvent),
            ["changeFilament"] = typeof(ChangeFilamentEvent),
        };

        [JsonPropertyName("event")]
        public string Event { get; }

        [JsonPropertyName("printer")]
        public string Printer { get; }

        [JsonPropertyName("data")]
        public byte[] Data { get; } // IRepetierEvent

        public RepetierBaseEvent() { }
    }
}