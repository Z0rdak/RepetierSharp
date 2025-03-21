using System.Text.Json.Serialization;
using RepetierSharp.Util;

namespace RepetierSharp.Models.Commands
{
    public class PreheatCommand : ICommandData
    {
        public PreheatCommand(int extruderNo, int heatedBedNo, int heatedChamberNo)
        {
            ExtruderNo = extruderNo;
            HeatedBedNo = heatedBedNo;
            HeatedChamberNo = heatedChamberNo;
        }

        public int ExtruderNo { get; set; }
        public int HeatedBedNo { get; set; }
        public int HeatedChamberNo { get; set; }

        [JsonIgnore] public string Action => CommandConstants.PREHEAT;
    }

    public enum ExtruderConstants
    {
        No = 0,
        All = 1,
        Active = 2
    }
}
