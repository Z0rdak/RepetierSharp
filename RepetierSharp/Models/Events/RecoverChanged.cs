using System.Collections.Generic;
using System.Text.Json.Serialization;
using RepetierSharp.Models.Communication;
using RepetierSharp.Util;

namespace RepetierSharp.Models.Events
{
    [EventId(EventConstants.RECOVER_CHANGED)]
    public class RecoverChanged : IEventData
    {
        [JsonPropertyName("state")] public RecoverState State { get; }
        [JsonPropertyName("job")] public int JobId { get; }
        [JsonPropertyName("name")] public int JobName { get; }
        // TODO: Are field with *Pos int or objects?
        [JsonPropertyName("lastSendPos")] public int LastSendPos { get; }
        [JsonPropertyName("lastACKPos")] public int LastACKPos { get; }
        [JsonPropertyName("lastExecKnown")] public bool LastExecKnown { get; }
        [JsonPropertyName("lastExecPos")] public int LastExecPos { get; }
        [JsonPropertyName("lastSendZ")] public double LastSendZ { get; }
        [JsonPropertyName("lastExecZ")] public double LastExecZ { get; }
        [JsonPropertyName("lines")] public List<GcodeLine> Lines { get; set;}
    }
    
    // TODO: Verify
    public class GcodeLine
    {
        [JsonPropertyName("p")] public int Position { get; set; }
        [JsonPropertyName("g")] public string Gcode { get; set; }
    }

    public enum RecoverState
    {
        RecoverDisabled,
        Recording,
        LastPrintFailedUnknownReason,
        LastPrintFailedPrinterDisc,
        LastPrintFailedPowerLoss
    }
}
