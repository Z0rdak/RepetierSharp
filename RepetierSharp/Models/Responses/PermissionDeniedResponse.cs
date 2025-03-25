using System.Text.Json.Serialization;
using RepetierSharp.Models.Communication;

namespace RepetierSharp.Models.Responses
{
    public class PermissionDeniedResponse : IResponseData
    {
        [JsonPropertyName("permissionDenied")] public bool PermissionDenied { get; set; }
        [JsonPropertyName("loggedIn")] public bool LoggedIn { get; set; }
    }
}
