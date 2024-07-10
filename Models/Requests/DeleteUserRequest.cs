using System.Text.Json.Serialization;
using RepetierSharp.Util;

namespace RepetierSharp.Models.Requests
{
    public class DeleteUserRequest : IRepetierRequest
    {
        public DeleteUserRequest(string user)
        {
            User = user;
        }

        [JsonPropertyName("login")] public string User { get; }

        [JsonIgnore] public string CommandIdentifier => CommandConstants.DELETE_USER;
    }
}
