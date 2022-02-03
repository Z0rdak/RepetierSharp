using System.Text.Json.Serialization;

namespace RepetierSharp.Models
{
    public class Printer
    {
        [JsonPropertyName("active")]
        public bool IsActive { get; set; }

        [JsonPropertyName("analysed")]
        public int Analysed { get; set; }

        [JsonPropertyName("done")]
        public double Progress { get; set; }

        [JsonPropertyName("job")]
        public string Job { get; set; }

        [JsonPropertyName("jobid")]
        public int JobId { get; set; }

        [JsonPropertyName("linesSend")]
        public int LinesSend { get; set; }

        [JsonPropertyName("name")]
        public string PrinterName { get; set; }

        [JsonPropertyName("ofLayer")]
        public int LayerCount { get; set; }

        [JsonPropertyName("online")]
        public int IsOnline { get; set; }

        [JsonPropertyName("pauseState")]
        public int PauseState { get; set; }

        [JsonPropertyName("paused")]
        public bool IsPaused { get; set; }

        [JsonPropertyName("printStart")]
        public double PrintStarted { get; set; }

        [JsonPropertyName("printTime")]
        public double TotalPrintTime { get; set; }

        [JsonPropertyName("printedTimeComp")] // time elapsed ?
        public double PrintedTimeComp { get; set; }

        [JsonPropertyName("slug")]
        public string PrinterSlug { get; set; }

        [JsonPropertyName("start")]
        public long Start { get; set; }

        [JsonPropertyName("totalLines")]
        public long TotalLines { get; set; }

        public Printer() { }
    }
}