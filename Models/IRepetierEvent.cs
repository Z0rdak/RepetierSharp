using RepetierSharp.Models.Messages;

namespace RepetierSharp.Models.Events
{
    public interface IRepetierMessage { }
    
    public interface IRepetierResponse : IRepetierMessage { }
    
    public interface IRepetierEvent : IRepetierMessage { }
}
