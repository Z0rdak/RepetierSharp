using System.Text.Json.Serialization;

namespace RepetierSharp.Models
{
    public class User
    {
        [JsonPropertyName("id")] public int Id { get; set; }

        [JsonPropertyName("login")] public string Login { get; set; }

        [JsonPropertyName("permissions")] public int Permissions { get; set; }
    }
}
