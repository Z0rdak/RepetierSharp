using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace RepetierSharp.Models.Commands
{
    public class PreheatCommand : ICommandData
    {
        // TODO:
        public int ExtruderNo { get; set; }
        public int HeatedBedNo { get; set; }
        public int HeatedChamberNo { get; set; }
        [JsonIgnore]
        public string CommandIdentifier => CommandConstants.PREHEAT;

        public PreheatCommand(int extruderNo, int heatedBedNo, int heatedChamberNo)
        {
            ExtruderNo = extruderNo;
            HeatedBedNo = heatedBedNo;
            HeatedChamberNo = heatedChamberNo;
        }
    }

    public enum ExtruderConstants
    {
        No = 0,
        All = 1,
        Active = 2,
    }
}
