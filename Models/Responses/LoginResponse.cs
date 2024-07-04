using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using RepetierSharp.Models.Events;

namespace RepetierSharp.Models.Messages
{
    public class LoginResponse : IRepetierResponse
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

        public LoginResponse() { }

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
