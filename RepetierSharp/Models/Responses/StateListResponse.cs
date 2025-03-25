using System.Collections.Generic;
using RepetierSharp.Models.Communication;

namespace RepetierSharp.Models.Responses
{
    public class StateListResponse : IResponseData
    {
        public Dictionary<string, PrinterState> PrinterStates { get; set; } = new();
    }
}
