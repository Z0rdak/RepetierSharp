using System.Collections.Generic;
using System.Threading.Tasks;
using RepetierSharp.Models;
using RepetierSharp.Models.Events;
using RepetierSharp.Models.Commands;

namespace RepetierSharp
{
    public sealed class CommandDispatcher
    {
        private Dictionary<RepetierTimer, List<IRepetierCommand>> TimerCommandMap { get; } = new();

        public List<IRepetierCommand> GetCommands(RepetierTimer timer)
        {
            return TimerCommandMap.TryGetValue(timer, out var commandsForTimer)
                ? commandsForTimer
                : new List<IRepetierCommand>();
        }
        
        public Task DispatchCommands(RepetierTimer timer, RepetierConnection repetierCon)
        {
            if (TimerCommandMap.TryGetValue(timer, out var commandsForTimer))
            {
                commandsForTimer.ForEach(async command =>
                {
                    await repetierCon.SendCommand(command);
                }); 
            }
            return Task.CompletedTask;
        }
        
        public void AddCommand(RepetierTimer timer, IRepetierCommand command)
        {
            if (TimerCommandMap.TryGetValue(timer, out var commandsForTimer))
            {
                commandsForTimer.Add(command);
            }
            else
            {
                TimerCommandMap.Add(timer, new List<IRepetierCommand> { command });
            }
        }
        
        public void RemoveCommand(RepetierTimer timer, IRepetierCommand command)
        {
            if (TimerCommandMap.TryGetValue(timer, out var commandsForTimer))
            {
                commandsForTimer.Remove(command);
            }
        }
    }
}
