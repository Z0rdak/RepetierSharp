using System.Text.Json.Serialization;

namespace RepetierSharp.Models.Events
{
    public class GpioPinChanged
    {
        [JsonPropertyName("uuid")] public string Uuid { get; set; }
        [JsonPropertyName("state")] public bool State { get; set; }
        [JsonPropertyName("pwmDutyCycle")] public int PwmDutyCycle { get; set; }
    }
}
