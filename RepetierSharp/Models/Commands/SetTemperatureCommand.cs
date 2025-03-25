using System.Text.Json.Serialization;
using RepetierSharp.Internal;
using RepetierSharp.Models.Communication;
using RepetierSharp.Util;

namespace RepetierSharp.Models.Commands
{
    public abstract class SetTemperatureCommand(int temperature)
    {
        [JsonPropertyName("temperature")] public int Temperature { get; set; } = temperature < 0 ? 0 : temperature;
    }

    [CommandId(CommandConstants.SET_EXTRUDER_TEMPERATURE)]
    public class SetExtruderTempCommand(int temperature, int extruderNo) : SetTemperatureCommand(temperature), ICommandData
    {
        [JsonPropertyName("extruder")] public int ExtruderId { get; set; } = extruderNo;

        [JsonIgnore] public string Action => CommandConstants.SET_EXTRUDER_TEMPERATURE;
    }

    [CommandId(CommandConstants.SET_BED_TEMPERATURE)]
    public class SetHeatedBedTempCommand(int temperature, int heatedBedId) : SetTemperatureCommand(temperature), ICommandData
    {
        [JsonPropertyName("bedId")] public int HeatedBedId { get; set; } = heatedBedId;

        [JsonIgnore] public string Action => CommandConstants.SET_BED_TEMPERATURE;
    }

    [CommandId(CommandConstants.SET_CHAMBER_TEMPERATURE)]
    public class SetHeatedChamberTempCommand(int temperature, int heatedChamberId) : SetTemperatureCommand(temperature), ICommandData
    {
        [JsonPropertyName("chamberId")] public int HeatedChamberId { get; set; } = heatedChamberId;

        [JsonIgnore] public string Action => CommandConstants.SET_CHAMBER_TEMPERATURE;
    }


    public enum TemperatureTarget
    {
        Extruder = 0,
        HeatedBed = 1,
        HeatedChamber = 2
    }
}
