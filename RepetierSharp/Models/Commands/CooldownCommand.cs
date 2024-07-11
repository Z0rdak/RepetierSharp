using System.Text.Json.Serialization;
using RepetierSharp.Util;

namespace RepetierSharp.Models.Commands
{
    public class CooldownCommand : IRepetierCommand
    {
        public CooldownCommand(int extruderNo, int heatedBedNo, int heatedChamberNo)
        {
            ExtruderNo = extruderNo;
            HeatedBedNo = heatedBedNo;
            HeatedChamberNo = heatedChamberNo;
        }

        public int ExtruderNo { get; set; }
        public int HeatedBedNo { get; set; }
        public int HeatedChamberNo { get; set; }

        [JsonIgnore] public string CommandIdentifier => CommandConstants.COOLDOWN;
    }
}
