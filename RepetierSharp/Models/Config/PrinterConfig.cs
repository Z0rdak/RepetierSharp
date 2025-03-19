using System.Collections.Generic;
using System.Text.Json.Serialization;
using RepetierSharp.Models;
using RepetierSharp.Models.Config;
using RepetierSharp.Models.Events;

namespace RepetierSharp.Config
{
    public class PrinterConfig : IRepetierEvent
    {
        [JsonPropertyName("general")] public General General { get; set; }
        [JsonPropertyName("connection")] public Connection Connection { get; set; }
        
        
        [JsonPropertyName("movement")] public Movement Movement { get; set; }
        
        
        [JsonPropertyName("buttonCommands")] public List<ButtonCommand> ButtonCommands { get; set; }
        [JsonPropertyName("quickCommands")] public List<QuickCommand> QuickCommands { get; set; }
        [JsonPropertyName("wizardCommands")] public List<WizardCommand> WizardCommands { get; set; }
        [JsonPropertyName("wizardTemplates")] public List<WizardTemplate> WizardTemplates { get; set; }
        [JsonPropertyName("responseEvents")] public List<ResponseEvent> ResponseEvents { get; set; }
        [JsonPropertyName("gcodeReplacements")] public List<GCodeReplacement> GcodeReplacements { get; set; }
  
        [JsonPropertyName("speedPresets")] public List<ToolPreset> SpeedPresets { get; set; }
        [JsonPropertyName("flowPresets")] public List<ToolPreset> FlowPresets { get; set; }
        [JsonPropertyName("fanPresets")] public List<ToolPreset> FanPresets { get; set; }
        [JsonPropertyName("shape")] public Shape Shape { get; set; }
        
        [JsonPropertyName("extruders")] public List<Extruder> Extruders { get; set; }
        [JsonPropertyName("heatedBeds")] public List<HeatedBed> HeatedBed { get; set; }
        [JsonPropertyName("heatedChambers")] public List<HeatedChamber> HeatedChambers { get; set; }
        
        [JsonPropertyName("webcams")] public List<Webcam> Webcam { get; set; }
        [JsonPropertyName("fans")] public List<FanDefinition> Fans { get; set; }
        [JsonPropertyName("recover")] public Recover Recover { get; set; }
        [JsonPropertyName("properties")] public Dictionary<string, string> Properties { get; set; }
    }
    
    public class ToolPreset
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("value")]
        public int Value { get; set; }
    }
    
    public class FanDefinition
    {
        [JsonPropertyName("pos")]
        public int Pos { get; set; }

        [JsonPropertyName("min")]
        public int Min { get; set; }

        [JsonPropertyName("max")]
        public int Max { get; set; }

        [JsonPropertyName("firmwareId")]
        public int FirmwareId { get; set; }

        [JsonPropertyName("readOnly")]
        public bool ReadOnly { get; set; }

        [JsonPropertyName("fanType")]
        public int FanType { get; set; }

        [JsonPropertyName("alias")]
        public string Alias { get; set; }

        [JsonPropertyName("firmwareName")]
        public string FirmwareName { get; set; }
    }

    public class QuickCommand
    {
        [JsonPropertyName("command")] public string Command { get; set; }

        [JsonPropertyName("icon")] public string Icon { get; set; }

        [JsonPropertyName("name")] public string Name { get; set; }
    }

    public class WizardCommand : QuickCommand
    {
    }
    
    public class WizardTemplate
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("command")]
        public string Command { get; set; }

        [JsonPropertyName("icon")]
        public string Icon { get; set; }

        [JsonPropertyName("description")]
        public string Description { get; set; }

        [JsonPropertyName("source")]
        public long Source { get; set; }

        [JsonPropertyName("author")]
        public string Author { get; set; }

        [JsonPropertyName("uuid")]
        public string Uuid { get; set; }

        [JsonPropertyName("version")]
        public long Version { get; set; }

        [JsonPropertyName("visibleWhenPrinting")]
        public bool VisibleWhenPrinting { get; set; }

        [JsonPropertyName("condition")]
        public string Condition { get; set; }

        [JsonPropertyName("seePermission")]
        public string ReadPermission { get; set; }

        [JsonPropertyName("editPermission")]
        public string EditPermission { get; set; }
    }

    // TODO: unify types?
    public class ResponseEvent
    {
        [JsonPropertyName("event")] public string Event { get; set; }

        [JsonPropertyName("expression")] public string Expression { get; set; }

        [JsonPropertyName("script")] public string Script { get; set; }
    }

    public class GCodeReplacement
    {
        [JsonPropertyName("comment")] public string Comment { get; set; }

        [JsonPropertyName("expression")] public string Expression { get; set; }

        [JsonPropertyName("script")] public string Script { get; set; }
    }

    public class Recover
    {
        [JsonPropertyName("delayBeforeReconnect")]
        public int DelayBeforeReconnect { get; set; }

        [JsonPropertyName("enabled")] public bool Enabled { get; set; }

        [JsonPropertyName("extraZOnFirmwareDetect")]
        public double ExtraZOnFirmwareDetect { get; set; }

        [JsonPropertyName("firmwarePowerlossSignal")]
        public string FirmwarePowerlossSignal { get; set; }

        [JsonPropertyName("maxTimeForAutocontinue")]
        public int MaxTimeForAutocontinue { get; set; }

        [JsonPropertyName("procedure")] public string Procedure { get; set; }

        [JsonPropertyName("reactivateBedOnConnect")]
        public bool ReactivateBedOnConnect { get; set; }

        [JsonPropertyName("replayExtruderSwitches")]
        public bool ReplayExtruderSwitches { get; set; }

        [JsonPropertyName("runOnConnect")] public string RunOnConnect { get; set; }
    }
}
