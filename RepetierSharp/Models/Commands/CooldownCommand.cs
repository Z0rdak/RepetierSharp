using System;
using System.Collections.Generic;
using System.Text;

namespace RepetierSharp.Models.Commands
{
    public class CooldownCommand : ICommandData
    {

        public int ExtruderNo { get; set; }
        public int HeatedBedNo { get; set; }
        public int HeatedChamberNo { get; set; }

        public string CommandIdentifier => CommandConstants.COOLDOWN;

        public CooldownCommand(int extruderNo, int heatedBedNo, int heatedChamberNo)
        {
            ExtruderNo = extruderNo;
            HeatedBedNo = heatedBedNo;
            HeatedChamberNo = heatedChamberNo;
        }
    }
}
