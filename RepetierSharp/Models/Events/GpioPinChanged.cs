using System.Text.Json.Serialization;
using RepetierSharp.Util;

namespace RepetierSharp.Models.Events
{
    [EventId(EventConstants.GPIO_PIN_CHANGED)]
    public class GpioPinChanged
    {
        [JsonPropertyName("uuid")] public string Uuid { get; set; }
        [JsonPropertyName("state")] public bool State { get; set; }
        [JsonPropertyName("pwmDutyCycle")] public int PwmDutyCycle { get; set; }
    }
}
