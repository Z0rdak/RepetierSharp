using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace RepetierMqtt.Models.Commands
{

    public abstract class SetTemperatureCommand
    {
        [JsonPropertyName("temperature")]
        public int Temperature { get; set; }

        public SetTemperatureCommand(int temperature)
        {
            Temperature = temperature < 0 ? 0 : temperature;
        }
    }

    public class SetExtruderTempCommand : SetTemperatureCommand, ICommandData
    {
        [JsonPropertyName("extruder")]
        public int ExtruderId { get; set; }

        public string CommandIdentifier => CommandConstants.SET_EXTRUDER_TEMPERATURE;

        public SetExtruderTempCommand(int temperature, int extruderNo) : base(temperature)
        {
            ExtruderId = extruderNo;
        }
    }

    public class SetHeatedBedTempCommand : SetTemperatureCommand, ICommandData
    {
        [JsonPropertyName("bedId")]
        public int HeatedBedId { get; set; }
        
        public string CommandIdentifier => CommandConstants.SET_BED_TEMPERATURE;

        public SetHeatedBedTempCommand(int temperature, int heatedBedId) : base(temperature)
        {
            HeatedBedId = heatedBedId;
        }
    }

    public class SetHeatedChamberTempCommand : SetTemperatureCommand, ICommandData
    {
        [JsonPropertyName("chamberId")]
        public int HeatedChamberId { get; set; }

        public string CommandIdentifier => CommandConstants.SET_CHAMBER_TEMPERATURE;

        public SetHeatedChamberTempCommand(int temperature, int heatedChamberId) : base(temperature)
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
