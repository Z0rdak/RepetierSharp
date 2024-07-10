﻿using System.Text.Json.Serialization;
using RepetierSharp.Util;

namespace RepetierSharp.Models.Requests
{
    public class SetSpeedMultiplyRequest : IRepetierRequest
    {
        public SetSpeedMultiplyRequest(int speedMultiplier)
        {
            SpeedMultiplierInPercent = speedMultiplier;
        }

        [JsonPropertyName("speed")] public int SpeedMultiplierInPercent { get; set; }

        [JsonIgnore] public string CommandIdentifier => CommandConstants.SET_SPEED_MULTIPLY;
    }
}
