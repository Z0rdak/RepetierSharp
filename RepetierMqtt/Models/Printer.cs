using System.Text.Json.Serialization;

namespace RepetierMqtt.Models
{
    public class Printer
    {
        [JsonPropertyName("active")]
        public bool IsActive { get; }

        [JsonPropertyName("analysed")]
        public int Analysed { get; }

        [JsonPropertyName("done")]
        public double Progress { get; set; }

        [JsonPropertyName("job")]
        public string Job { get; }

        [JsonPropertyName("jobid")]
        public int JobId { get; }

        [JsonPropertyName("linesSend")]
        public int LinesSend { get; }

        [JsonPropertyName("name")]
        public string PrinterName { get; set; }

        [JsonPropertyName("ofLayer")]
        public int LayerCount { get; }

        [JsonPropertyName("online")]
        public int IsOnline { get; }

        [JsonPropertyName("pauseState")]
        public int PauseState { get; }

        [JsonPropertyName("paused")]
        public bool IsPaused { get; }

        [JsonPropertyName("printStart")]
        public double PrintStarted { get; }

        [JsonPropertyName("printTime")]
        public double TotalPrintTime { get; }

        [JsonPropertyName("printedTimeComp")] // time elapsed ?
        public double PrintedTimeComp { get; }

        [JsonPropertyName("slug")]
        public string PrinterSlug { get; }

        [JsonPropertyName("start")]
        public long Start { get; }

        [JsonPropertyName("totalLines")]
        public long TotalLines { get; }
    }
}