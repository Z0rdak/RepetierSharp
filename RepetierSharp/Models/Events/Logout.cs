namespace RepetierSharp.Models.Events
{
    [EventId("logout")]
    public class Logout : IRepetierEvent
    {
        Logout() { }
    }

}