using System.Collections.Generic;
using System.Text.Json.Serialization;
using RepetierSharp.Models.Communication;

namespace RepetierSharp.Models.Responses
{
    public class UserListResponse : IResponseData
    {
        [JsonPropertyName("data")] public UserListContainer UserListContainer { get; set; }
    }

    public class UserListContainer
    {
        [JsonPropertyName("loginRequired")] public bool LoginRequired { get; set; }

        [JsonPropertyName("users")] public List<User> Users { get; set; }
    }
}
