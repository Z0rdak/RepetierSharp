namespace RepetierSharp.Models
{
    public interface IRepetierMessage
    {
    }

    public interface IRepetierResponse : IRepetierMessage
    {
    }

    public interface IRepetierEvent : IRepetierMessage
    {
    }
    
    public class EmptyEvent : IRepetierEvent {}
}
