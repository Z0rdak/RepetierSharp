using RepetierSharp.Models.Communication;
using RepetierSharp.Util;

namespace RepetierSharp.Models.Events
{
    [EventId(EventConstants.LICENSE_CHANGED)]
    public class LicenseChanged : IEventData { }
}
