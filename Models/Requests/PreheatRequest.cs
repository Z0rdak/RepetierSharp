using System.Text.Json.Serialization;
using RepetierSharp.Util;

namespace RepetierSharp.Models.Requests
{
    public class PreheatRequest : IRepetierRequest
    {
        public PreheatRequest(int extruderNo, int heatedBedNo, int heatedChamberNo)
        {
            ExtruderNo = extruderNo;
            HeatedBedNo = heatedBedNo;
            HeatedChamberNo = heatedChamberNo;
        }

        // TODO:
        public int ExtruderNo { get; set; }
        public int HeatedBedNo { get; set; }
        public int HeatedChamberNo { get; set; }

        [JsonIgnore] public string CommandIdentifier => CommandConstants.PREHEAT;
    }

    public enum ExtruderConstants
    {
        No = 0,
        All = 1,
        Active = 2
    }
}
