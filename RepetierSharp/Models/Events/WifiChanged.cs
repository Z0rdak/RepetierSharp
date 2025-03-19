using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace RepetierSharp.Models.Events
{

     public class Connection
    {
        [JsonPropertyName("SSID")]
        public string SSID { get; set; }

        [JsonPropertyName("device")]
        public string Device { get; set; }
    }

    public class Data
    {
        [JsonPropertyName("manualManaged")]
        public bool ManualManaged { get; set; }

        [JsonPropertyName("active")]
        public bool Active { get; set; }

        [JsonPropertyName("ssid")]
        public string Ssid { get; set; }

        [JsonPropertyName("password")]
        public string Password { get; set; }

        [JsonPropertyName("passwordMethod")]
        public string PasswordMethod { get; set; }

        [JsonPropertyName("hidden")]
        public bool Hidden { get; set; }

        [JsonPropertyName("ignore")]
        public bool Ignore { get; set; }

        [JsonPropertyName("ipv4Mode")]
        public string Ipv4Mode { get; set; }

        [JsonPropertyName("ipv4Gateway")]
        public string Ipv4Gateway { get; set; }

        [JsonPropertyName("ipv4Nameserver")]
        public string Ipv4Nameserver { get; set; }

        [JsonPropertyName("ipv4Address")]
        public string Ipv4Address { get; set; }

        [JsonPropertyName("ipv4MaskBits")]
        public int Ipv4MaskBits { get; set; }

        [JsonPropertyName("ipv6Mode")]
        public string Ipv6Mode { get; set; }

        [JsonPropertyName("ipv6Gateway")]
        public string Ipv6Gateway { get; set; }

        [JsonPropertyName("ipv6Nameserver")]
        public string Ipv6Nameserver { get; set; }

        [JsonPropertyName("ipv6Address")]
        public string Ipv6Address { get; set; }

        [JsonPropertyName("ipv6MaskBits")]
        public int Ipv6MaskBits { get; set; }
    }

    public class Ethernet
    {
        [JsonPropertyName("ipv4_method")]
        public string Ipv4Method { get; set; }

        [JsonPropertyName("ipv4_addresses")]
        public string Ipv4Addresses { get; set; }

        [JsonPropertyName("ipv4_gateway")]
        public string Ipv4Gateway { get; set; }

        [JsonPropertyName("ipv4_dns")]
        public string Ipv4Dns { get; set; }

        [JsonPropertyName("ipv6_method")]
        public string Ipv6Method { get; set; }

        [JsonPropertyName("ipv6_addresses")]
        public string Ipv6Addresses { get; set; }

        [JsonPropertyName("ipv6_gateway")]
        public string Ipv6Gateway { get; set; }

        [JsonPropertyName("ipv6_dns")]
        public string Ipv6Dns { get; set; }
    }

    [EventId("wifiChanged")]
    public class WifiChanged : IRepetierEvent
    {
        [JsonPropertyName("activeRouter")]
        public bool ActiveRouter { get; set; }

        [JsonPropertyName("routerList")]
        public List<RouterList> RouterList { get; set; }

        [JsonPropertyName("connections")]
        public List<Connection> Connections { get; set; }

        [JsonPropertyName("channels")]
        public List<int> Channels { get; set; }

        [JsonPropertyName("manageable")]
        public bool Manageable { get; set; }

        [JsonPropertyName("manualWifi")]
        public bool ManualWifi { get; set; }

        [JsonPropertyName("supportAP")]
        public bool SupportAP { get; set; }

        [JsonPropertyName("apSSID")]
        public string ApSSID { get; set; }

        [JsonPropertyName("country")]
        public string Country { get; set; }

        [JsonPropertyName("channel")]
        public int Channel { get; set; }

        [JsonPropertyName("mode")]
        public int Mode { get; set; }

        [JsonPropertyName("hostname")]
        public string Hostname { get; set; }

        [JsonPropertyName("apMode")]
        public int ApMode { get; set; }

        [JsonPropertyName("version")]
        public int Version { get; set; }

        [JsonPropertyName("screensaver")]
        public bool Screensaver { get; set; }

        [JsonPropertyName("activeSSID")]
        public string ActiveSSID { get; set; }

        [JsonPropertyName("timezone")]
        public string Timezone { get; set; }

        [JsonPropertyName("ethernet")]
        public Ethernet Ethernet { get; set; }
    }

    public class RouterList
    {
        [JsonPropertyName("data")]
        public Data Data { get; set; }

        [JsonPropertyName("SSID")]
        public string SSID { get; set; }

        [JsonPropertyName("active")]
        public bool Active { get; set; }

        [JsonPropertyName("mode")]
        public string Mode { get; set; }

        [JsonPropertyName("channel")]
        public int Channel { get; set; }

        [JsonPropertyName("rate")]
        public string Rate { get; set; }

        [JsonPropertyName("signal")]
        public int Signal { get; set; }

        [JsonPropertyName("bars")]
        public int Bars { get; set; }

        [JsonPropertyName("secure")]
        public bool Secure { get; set; }
    }
    
    
}
