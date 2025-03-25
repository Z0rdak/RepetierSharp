using System.Collections.Generic;
using System.Text.Json.Serialization;
using RepetierSharp.Serialization;

namespace RepetierSharp.Models.Communication
{

    /// <summary>
    /// Wrapper around the list of events contained in the data field for an event message
    /// </summary>
    /// <param name="data"></param>
    public class RepetierEventList(List<IRepetierEvent> data) : RepetierMessageHeader
    {
        [JsonPropertyName("data")] public List<IRepetierEvent> Data { get; } = data;
    }
    
    /// <summary>
    /// 
    /// </summary>
    public class RepetierMessageHeader 
    {
        /// <summary>
        ///     A callback_id of -1 indicates that the message contains events and is not a response to a command.
        /// </summary>
        [JsonPropertyName("callback_id")] public int CallBackId { get; set; }
        [JsonPropertyName("session")] public string SessionId { get; set; }
        /// <summary>
        ///     Indicates if the message contains events
        /// </summary>
        [JsonPropertyName("eventList")] public bool EventList { get; set; }
        
        [JsonIgnore] public bool IsEventList => CallBackId == -1 || EventList;
        [JsonIgnore] public bool IsResponse => CallBackId > -1;
    }
    
    /// <summary>
    ///  Wrapper around a response to add the retrieved commandId from the command manager.
    /// </summary>
    public class RepetierResponse : RepetierMessageHeader
    {
        [JsonPropertyName("commandId")] public string CommandId { get; set; }
        [JsonPropertyName("data")] public IResponseData Data { get; set; }
    }
    
    
}
