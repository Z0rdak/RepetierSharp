﻿using System.Text.Json.Serialization;

namespace RepetierSharp.Models.Config
{
    public class ButtonCommand
    {
        [JsonPropertyName("command")] public string Command { get; set; }

        [JsonPropertyName("name")] public string Name { get; set; }
    }
}
