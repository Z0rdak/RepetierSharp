using System.Text.Json.Serialization;

namespace RepetierSharp.Models.Commands
{
    public class CooldownCommand : ICommandData
    {

        public int ExtruderNo { get; set; }
        public int HeatedBedNo { get; set; }
        public int HeatedChamberNo { get; set; }
        [JsonIgnore]
        public string CommandIdentifier => CommandConstants.COOLDOWN;

        public CooldownCommand(int extruderNo, int heatedBedNo, int heatedChamberNo)
        {
            ExtruderNo = extruderNo;
            HeatedBedNo = heatedBedNo;
            HeatedChamberNo = heatedChamberNo;
        }
    }
}
