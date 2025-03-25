using System.Collections.Generic;
using System.Text.Json.Serialization;
using RepetierSharp.Models.Communication;

namespace RepetierSharp.Models.Responses
{
    public class Message : IResponseData
    {
        [JsonPropertyName("id")] public int Id { get; set; }

        [JsonPropertyName("msg")] public string Msg { get; set; }

        [JsonPropertyName("link")] public string FinishLink { get; set; }

        [JsonPropertyName("slug")] public string PrinterSlug { get; set; }

        // Date ISO 8601
        [JsonPropertyName("date")] public string DateString { get; set; }

        [JsonPropertyName("pause")] public bool IsPaused { get; set; }
    }

    public class MessageList : IResponseData
    {
        public List<Message> Messages { get; set; } = new();
    }
}
