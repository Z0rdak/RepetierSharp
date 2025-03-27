using RepetierSharp.Models.Communication;
using RepetierSharp.Util;

namespace RepetierSharp.Models.Events
{
    [EventId(EventConstants.ADD_ERROR_LOG_LINE)]
    public class AddErrorLogLine : EmptyEventData { }
}
