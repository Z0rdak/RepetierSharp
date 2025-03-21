﻿using System.Text.Json.Serialization;
using RepetierSharp.Util;

namespace RepetierSharp.Models.Commands
{
    public abstract class SetTemperatureRequest
    {
        public SetTemperatureRequest(int temperature)
        {
            Temperature = temperature < 0 ? 0 : temperature;
        }

        [JsonPropertyName("temperature")] public int Temperature { get; set; }
    }

    public class SetExtruderTempCommand : SetTemperatureRequest, ICommandData
    {
        public SetExtruderTempCommand(int temperature, int extruderNo) : base(temperature)
        {
            ExtruderId = extruderNo;
        }

        [JsonPropertyName("extruder")] public int ExtruderId { get; set; }

        [JsonIgnore] public string Action => CommandConstants.SET_EXTRUDER_TEMPERATURE;
    }

    public class SetHeatedBedTempCommand : SetTemperatureRequest, ICommandData
    {
        public SetHeatedBedTempCommand(int temperature, int heatedBedId) : base(temperature)
        {
            HeatedBedId = heatedBedId;
        }

        [JsonPropertyName("bedId")] public int HeatedBedId { get; set; }

        [JsonIgnore] public string Action => CommandConstants.SET_BED_TEMPERATURE;
    }

    public class SetHeatedChamberTempCommand : SetTemperatureRequest, ICommandData
    {
        public SetHeatedChamberTempCommand(int temperature, int heatedChamberId) : base(temperature)
        {
            HeatedChamberId = heatedChamberId;
        }

        [JsonPropertyName("chamberId")] public int HeatedChamberId { get; set; }

        [JsonIgnore] public string Action => CommandConstants.SET_CHAMBER_TEMPERATURE;
    }


    public enum TemperatureTarget
    {
        Extruder = 0,
        HeatedBed = 1,
        HeatedChamber = 2
    }
}
