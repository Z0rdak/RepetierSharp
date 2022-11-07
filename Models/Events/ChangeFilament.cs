namespace RepetierSharp.Models.Events
{
    [EventId("changeFilamentRequested")]
    public class ChangeFilament : IRepetierEvent
    {
        public ChangeFilament() { }
    }

}
