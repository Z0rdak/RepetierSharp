using System.Text.Json.Serialization;
using RepetierSharp.Internal;
using RepetierSharp.Models.Communication;
using RepetierSharp.Util;

namespace RepetierSharp.Models.Commands
{
    [CommandId(CommandConstants.PREHEAT)]
    public class PreheatCommand(int extruderNo, int heatedBedNo, int heatedChamberNo) : ICommandData
    {
        public int ExtruderNo { get; set; } = extruderNo;
        public int HeatedBedNo { get; set; } = heatedBedNo;
        public int HeatedChamberNo { get; set; } = heatedChamberNo;

        [JsonIgnore] public string Action => CommandConstants.PREHEAT;
    }

    public enum ExtruderConstants
    {
        No = 0,
        All = 1,
        Active = 2
    }
}
