using System.Text.Json.Serialization;
using RepetierSharp.Util;

namespace RepetierSharp.Models.Requests
{

    public abstract class SetTemperatureRequest
    {
        [JsonPropertyName("temperature")]
        public int Temperature { get; set; }

        public SetTemperatureRequest(int temperature)
        {
            Temperature = temperature < 0 ? 0 : temperature;
        }
    }

    public class SetExtruderTempRequest : SetTemperatureRequest, IRepetierRequest
    {
        [JsonPropertyName("extruder")]
        public int ExtruderId { get; set; }
        [JsonIgnore]
        public string CommandIdentifier => CommandConstants.SET_EXTRUDER_TEMPERATURE;

        public SetExtruderTempRequest(int temperature, int extruderNo) : base(temperature)
        {
            ExtruderId = extruderNo;
        }
    }

    public class SetHeatedBedTempRequest : SetTemperatureRequest, IRepetierRequest
    {
        [JsonPropertyName("bedId")]
        public int HeatedBedId { get; set; }
        [JsonIgnore]
        public string CommandIdentifier => CommandConstants.SET_BED_TEMPERATURE;

        public SetHeatedBedTempRequest(int temperature, int heatedBedId) : base(temperature)
        {
            HeatedBedId = heatedBedId;
        }
    }

    public class SetHeatedChamberTempRequest : SetTemperatureRequest, IRepetierRequest
    {
        [JsonPropertyName("chamberId")]
        public int HeatedChamberId { get; set; }
        [JsonIgnore]
        public string CommandIdentifier => CommandConstants.SET_CHAMBER_TEMPERATURE;

        public SetHeatedChamberTempRequest(int temperature, int heatedChamberId) : base(temperature)
        {
            HeatedChamberId = heatedChamberId;
        }
    }


    public enum TemperatureTarget
    {
        Extruder = 0,
        HeatedBed = 1,
        HeatedChamber = 2
    }
}
