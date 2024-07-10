using System.Text.Json.Serialization;
using RepetierSharp.Util;

namespace RepetierSharp.Models.Requests
{
    public abstract class SetTemperatureRequest
    {
        public SetTemperatureRequest(int temperature)
        {
            Temperature = temperature < 0 ? 0 : temperature;
        }

        [JsonPropertyName("temperature")] public int Temperature { get; set; }
    }

    public class SetExtruderTempRequest : SetTemperatureRequest, IRepetierRequest
    {
        public SetExtruderTempRequest(int temperature, int extruderNo) : base(temperature)
        {
            ExtruderId = extruderNo;
        }

        [JsonPropertyName("extruder")] public int ExtruderId { get; set; }

        [JsonIgnore] public string CommandIdentifier => CommandConstants.SET_EXTRUDER_TEMPERATURE;
    }

    public class SetHeatedBedTempRequest : SetTemperatureRequest, IRepetierRequest
    {
        public SetHeatedBedTempRequest(int temperature, int heatedBedId) : base(temperature)
        {
            HeatedBedId = heatedBedId;
        }

        [JsonPropertyName("bedId")] public int HeatedBedId { get; set; }

        [JsonIgnore] public string CommandIdentifier => CommandConstants.SET_BED_TEMPERATURE;
    }

    public class SetHeatedChamberTempRequest : SetTemperatureRequest, IRepetierRequest
    {
        public SetHeatedChamberTempRequest(int temperature, int heatedChamberId) : base(temperature)
        {
            HeatedChamberId = heatedChamberId;
        }

        [JsonPropertyName("chamberId")] public int HeatedChamberId { get; set; }

        [JsonIgnore] public string CommandIdentifier => CommandConstants.SET_CHAMBER_TEMPERATURE;
    }


    public enum TemperatureTarget
    {
        Extruder = 0,
        HeatedBed = 1,
        HeatedChamber = 2
    }
}
