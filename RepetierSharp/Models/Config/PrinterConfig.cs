using RepetierSharp.Models.Config;
using RepetierSharp.Models.Events;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace RepetierSharp.Config
{
    public class PrinterConfig : IRepetierEvent
    {
        [JsonPropertyName("buttonCommands")]
        public List<ButtonCommand> ButtonCommands { get; set; }

        [JsonPropertyName("connection")]
        public Connection Connection { get; set; }

        [JsonPropertyName("extruders")]
        public List<Extruder> Extruders { get; set; }

        [JsonPropertyName("gcodeReplacements")]
        public List<GCodeReplacement> GcodeReplacements { get; set; }

        [JsonPropertyName("general")]
        public General General { get; set; }

        [JsonPropertyName("heatedBeds")]
        public List<HeatedBed> HeatedBed { get; set; }

        [JsonPropertyName("heatedChambers")]
        public List<HeatedChamber> HeatedChambers { get; set; }

        [JsonPropertyName("movement")]
        public Movement Movement { get; set; }

        // TODO: type unknown
        [JsonPropertyName("properties")]
        public object Properties { get; set; }

        [JsonPropertyName("quickCommands")]
        public List<QuickCommand> QuickCommands { get; set; }

        [JsonPropertyName("recover")]
        public Recover Recover { get; set; }

        [JsonPropertyName("responseEvents")]
        public List<ResponseEvent> ResponseEvents { get; set; }

        [JsonPropertyName("shape")]
        public Shape Shape { get; set; }

        [JsonPropertyName("webcam")]
        public Webcam Webcam { get; set; }

        [JsonPropertyName("wizardCommands")]
        public List<WizardCommand> WizardCommands { get; set; }
    }

    public class QuickCommand
    {
        [JsonPropertyName("command")]
        public string Command { get; set; }

        [JsonPropertyName("icon")]
        public string Icon { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }
    }

    public class WizardCommand : QuickCommand
    {

    }

    // TODO: unify types?
    public class ResponseEvent
    {
        [JsonPropertyName("event")]
        public string Event { get; set; }

        [JsonPropertyName("expression")]
        public string Expression { get; set; }

        [JsonPropertyName("script")]
        public string Script { get; set; }
    }

    public class GCodeReplacement
    {

        [JsonPropertyName("comment")]
        public string Comment { get; set; }

        [JsonPropertyName("expression")]
        public string Expression { get; set; }

        [JsonPropertyName("script")]
        public string Script { get; set; }
    }

    public class Recover
    {
        [JsonPropertyName("delayBeforeReconnect")]
        public int DelayBeforeReconnect { get; set; }

        [JsonPropertyName("enabled")]
        public bool Enabled { get; set; }

        [JsonPropertyName("extraZOnFirmwareDetect")]
        public double ExtraZOnFirmwareDetect { get; set; }

        [JsonPropertyName("firmwarePowerlossSignal")]
        public string FirmwarePowerlossSignal { get; set; }

        [JsonPropertyName("maxTimeForAutocontinue")]
        public int MaxTimeForAutocontinue { get; set; }

        [JsonPropertyName("procedure")]
        public string Procedure { get; set; }

        [JsonPropertyName("reactivateBedOnConnect")]
        public bool ReactivateBedOnConnect { get; set; }

        [JsonPropertyName("replayExtruderSwitches")]
        public bool ReplayExtruderSwitches { get; set; }

        [JsonPropertyName("runOnConnect")]
        public string RunOnConnect { get; set; }

        public Recover() { }
    }
}
