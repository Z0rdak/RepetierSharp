using RepetierSharp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json.Serialization;

namespace RepetierSharp.Models.Messages
{
    public class LoginMessage : IRepetierMessage
    {
        [JsonPropertyName("ok")]
        public bool Authenticated { get; set; }

        [JsonPropertyName("permissions")]
        public int PermissionLevel { get; set; }

        [JsonPropertyName("login")]
        public string LoginName { get; set; }

        [JsonPropertyName("settings")]
        public UserSettings Settings { get; set; }

        [JsonPropertyName("error")]
        public string Error { get; set; } = "";

        public LoginMessage() { }

        public override string ToString()
        {
            return GetType().GetProperties()
                .Select(info => (info.Name, Value: info.GetValue(this, null) ?? "(null)"))
                .Aggregate(
                    new StringBuilder($"{GetType().Name}\n"),
                    (sb, pair) => sb.AppendLine($"- {pair.Name}: {pair.Value}"),
                    sb => sb.ToString());
        }
    }
}