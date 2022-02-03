namespace RepetierSharp.Models.Events
{
    [EventId("jobsChanged")]
    public class JobsChanged : IRepetierEvent
    {
        // Gets triggered if a printer has a change in it's stored g-file list.
    }

}