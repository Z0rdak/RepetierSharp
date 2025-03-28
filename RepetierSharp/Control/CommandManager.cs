using System.Collections.Concurrent;
using System.Threading;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using RepetierSharp.Models.Communication;

namespace RepetierSharp.Control
{
    public enum CommandType
    {
        Printer = 0,
        Server = 1,
    }

    public record CallbackInfo(string Action, CommandType CmdType, string Printer);
    
    /// <summary>
    ///     CommandManager manages the commands that are sent to the Repetier-Server.
    ///     It is used to generate unique callback ids for each command.
    ///     The callback id is used to identify the command that is being sent to the server.
    /// </summary>
    internal class CommandManager(ILogger<RepetierConnection>? logger = null)
    {
        // CallbackId -> Command
        private readonly ConcurrentDictionary<int, CallbackInfo> _callbackMap = new();
        private readonly ILogger<RepetierConnection> _logger = logger ?? NullLogger<RepetierConnection>.Instance;
        private volatile int _callBackId;
        private readonly object _lockObj = new object();

        private int Next()
        {
            int res;
            lock (_lockObj)
            {
                if ( _callBackId == int.MaxValue )
                {
                    _callBackId = 1;
                    return _callBackId;
                }
                _callBackId = Interlocked.Increment(ref _callBackId);
                res = _callBackId;
            }
            return res;
        }

        public ServerCommand ServerCommandWithId(ICommandData command)
        {
            return ServerCommandWithId(command.Action, command);
        }
        
        public ServerCommand ServerCommandWithId(string action, ICommandData command)
        {
            var callbackId = Next();
            while (!_callbackMap.TryAdd(callbackId, new CallbackInfo(action, CommandType.Server, "Server")))
            {
                _logger.LogWarning("[CommandManager::ServerCommandWithId] Failed to add callbackId for command {callbackId} with id {CommandIdentifier}.", action, callbackId);
                callbackId = Next();
            }
            return new ServerCommand(action, command, callbackId);
        }
        
        public PrinterCommand PrinterCommandWithId(ICommandData command, string printer)
        {
            return this.PrinterCommandWithId(command.Action, command, printer);
        }
        
        public PrinterCommand PrinterCommandWithId(string action, ICommandData command, string printer)
        {
            var callbackId = Next();
            while (!_callbackMap.TryAdd(callbackId, new CallbackInfo(action, CommandType.Printer, printer)))
            {
                _logger.LogWarning("[CommandManager::PrinterCommandWithId] Failed to add callbackId for command {callbackId} with id {CommandIdentifier}.", action, callbackId);
                callbackId = Next();
            }
            return new PrinterCommand(action, command, printer, callbackId);
        }

        public void AcknowledgeCommand(int callbackId)
        {
            if (!_callbackMap.ContainsKey(callbackId)) return;
            if (_callbackMap.TryRemove(callbackId, out var command) )
            {
                _logger.LogDebug("[CommandManager] Removed command='{command}' with Id={callbackId}.", command, callbackId);
            } else 
            {
                _logger.LogWarning("Failed to remove command with {id}", callbackId);
            }
        }
        
        public string CommandIdentifierFor(int callbackId)
        {
            return _callbackMap.TryGetValue(callbackId, out var commandIdentifier)
                ? commandIdentifier.Action
                : string.Empty;
        }
        
        public CallbackInfo? CallbackInfoFor(int callbackId)
        {
            return _callbackMap.TryGetValue(callbackId, out var commandIdentifier)
                ? commandIdentifier
                : null;
        }
    }
}
