using System.Text.Json.Serialization;

namespace RepetierSharp.Config
{
    public class Connection
    {
        [JsonPropertyName("connectionMethod")] public int ConnectionMethod { get; set; }

        [JsonPropertyName("continueAfterFastReconnect")] public bool ContinueAfterFastReconnect { get; set; }
        [JsonPropertyName("ackFixExceedingOnTimeout")] public bool AckFixExceedingOnTimeout { get; set; }
        [JsonPropertyName("ackFixMissingBeforeSlowCommands")] public bool AckFixMissingBeforeSlowCommands { get; set; }
        [JsonPropertyName("compressCommunication")] public bool CompressCommunication { get; set; }

        [JsonPropertyName("lcdTimeMode")] public int LcdTimeMode { get; set; }

        [JsonPropertyName("password")] public string Password { get; set; }

        [JsonPropertyName("pipe")] public Pipe Pipe { get; set; }

        [JsonPropertyName("ip")] public Ip Ip { get; set; }

        [JsonPropertyName("powerOffIdleMinutes")]
        public int PowerOffIdleMinutes { get; set; }

        [JsonPropertyName("powerOffMaxTemperature")]
        public int PowerOffMaxTemperature { get; set; }

        [JsonPropertyName("resetScript")] public string ResetScript { get; set; }

        [JsonPropertyName("serial")] public SerialConnection Serial { get; set; }
    }

    public class SerialConnection
    {
        [JsonPropertyName("baudrate")] public int Baudrate { get; set; }

        [JsonPropertyName("communicationTimeout")]
        public double CommuncationTimeout { get; set; }

        [JsonPropertyName("connectionDelay")] public int ConnectionDelay { get; set; }

        [JsonPropertyName("device")] public string Device { get; set; }

        [JsonPropertyName("dtr")] public int Dtr { get; set; }

        [JsonPropertyName("emergencySolution")]
        public int EmergencySolution { get; set; }

        [JsonPropertyName("inputBufferSize")] public int InputBufferSize { get; set; }

        [JsonPropertyName("interceptor")] public bool Interceptor { get; set; }

        [JsonPropertyName("malyanHack")] public bool MalyanHack { get; set; }

        [JsonPropertyName("maxParallelCommands")]
        public int MaxParallelCommands { get; set; }

        [JsonPropertyName("pingPong")] public bool PingPong { get; set; }

        [JsonPropertyName("rts")] public int RTS { get; set; }

        [JsonPropertyName("usbreset")] public int UsbReset { get; set; }

        [JsonPropertyName("visibleWithoutRunning")]
        public bool VisibleWithoutRunning { get; set; }
    }

    public class Pipe
    {
        [JsonPropertyName("file")] public string File { get; set; }
    }

    public class Ip
    {
        [JsonPropertyName("address")] public string Address { get; set; }

        [JsonPropertyName("port")] public int Port { get; set; }
        [JsonPropertyName("keepAliveInterval")] public int KeepAliveInterval { get; set; }
    }
}
