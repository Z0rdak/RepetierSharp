using System.Text.Json.Serialization;
using RepetierSharp.Util;

namespace RepetierSharp.Models.Requests
{
    public class DeleteUserRequest : IRepetierRequest
    {
        [JsonPropertyName("login")]
        public string User { get; }
        [JsonIgnore]
        public string CommandIdentifier => CommandConstants.DELETE_USER;

        public DeleteUserRequest(string user)
        {
            this.User = user;
        }
    }
}
