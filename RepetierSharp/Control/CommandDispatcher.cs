using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RepetierSharp.Models.Commands;
using RepetierSharp.Models.Communication;

namespace RepetierSharp.Control
{

    public abstract class ScheduledCmd(string id, BaseCommand cmd)
    {
        public string Id { get => id; }
        public BaseCommand Cmd { get => cmd; }
    }

    public class ScheduledPrinterCmd(string id, string printer, BaseCommand cmd) : ScheduledCmd(id, cmd)
    {
        public string Printer { get; } = printer;
    }   
    
    public class ScheduledServerCmd(string id, BaseCommand cmd) : ScheduledCmd(id, cmd) { }   
    
    
    public sealed class CommandDispatcher
    {
        private Dictionary<RepetierTimer, List<ScheduledPrinterCmd>> PrinterCommandMap { get; } = new();
        private Dictionary<RepetierTimer, List<ScheduledServerCmd>> ServerCommandMap { get; } = new();
        
        public Task DispatchCommands(RepetierTimer timer, RepetierConnection repetierCon)
        {
            if (PrinterCommandMap.TryGetValue(timer, out var scheduledPrinterCmds))
            {
                scheduledPrinterCmds.ForEach(async scheduledCmd =>
                {
                    var printerCmd = (PrinterCommand)scheduledCmd.Cmd;
                    await repetierCon.SendPrinterCommand(printerCmd.Data, printerCmd.Printer);
                }); 
            }
            
            if (ServerCommandMap.TryGetValue(timer, out var scheduledServerCmds))
            {
                scheduledServerCmds.ForEach(async scheduledCmd =>
                {
                    var serverCmd = (ServerCommand)scheduledCmd.Cmd;
                    await repetierCon.SendServerCommand(serverCmd.Data);
                }); 
            }
            return Task.CompletedTask;
        }
        
        public ScheduledPrinterCmd AddPrinterCommand(string printer, ICommandData command, RepetierTimer timer)
        {
            var printerCmd = new PrinterCommand(command.Action, command, printer, -1);
            var scheduledCmd = new ScheduledPrinterCmd(Guid.NewGuid().ToString(), printer, printerCmd);
            if (PrinterCommandMap.TryGetValue(timer, out var scheduledCmds)) 
                scheduledCmds.Add(scheduledCmd);
            else
                PrinterCommandMap.Add(timer, new List<ScheduledPrinterCmd> { scheduledCmd });
            return scheduledCmd;
        }
        
        public ScheduledServerCmd AddServerCommand(ICommandData command, RepetierTimer timer)
        {
            var serverCmd = new ServerCommand(command.Action, command, -1);
            var scheduledCmd = new ScheduledServerCmd(Guid.NewGuid().ToString(), serverCmd);
            if (ServerCommandMap.TryGetValue(timer, out var scheduledCmds)) 
                scheduledCmds.Add(scheduledCmd);
            else
                ServerCommandMap.Add(timer, new List<ScheduledServerCmd> { scheduledCmd });
            return scheduledCmd;
        }
        
        public void RemovePrinterCommand(RepetierTimer timer, string uuid)
        {
            if ( !PrinterCommandMap.TryGetValue(timer, out var scheduledCmds) )
                return;
            
            var scheduledCmd = scheduledCmds.FirstOrDefault(e => e != null && e.Id == uuid, null);
            if (scheduledCmd != null) 
                scheduledCmds.Remove(scheduledCmd);
        }
        
        public void RemoveServerCommand(RepetierTimer timer, string uuid)
        {
            if ( !ServerCommandMap.TryGetValue(timer, out var scheduledCmds) )
                return;
            
            var scheduledCmd = scheduledCmds.FirstOrDefault(e => e != null && e.Id == uuid, null);
            if (scheduledCmd != null) 
                scheduledCmds.Remove(scheduledCmd);
        }

    }
}
