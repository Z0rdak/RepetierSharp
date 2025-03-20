using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using RepetierSharp.Models;
using RepetierSharp.Models.Commands;

namespace RepetierSharp.Util
{
    /// <summary>
    ///     CommandManager manages the commands that are sent to the Repetier-Server.
    ///     It is used to generate unique callback ids for each command.
    ///     The callback id is used to identify the command that is being sent to the server.
    /// </summary>
    internal class CommandManager
    {
        // CallbackId -> Command
        private readonly ConcurrentDictionary<int, string> _callbackMap = new();
        private readonly ILogger<RepetierConnection> _logger;
        private volatile int _callBackId;

        public CommandManager(ILogger<RepetierConnection>? logger = null)
        {
            _logger = logger ?? NullLogger<RepetierConnection>.Instance;
        }
        
        public int Next()
        {
            if ( _callBackId == int.MaxValue )
            {
                _callBackId = 1;
                return _callBackId;
            }
            Interlocked.Add(ref _callBackId, 1);
            return _callBackId;
        }

        public RepetierBaseRequest CommandWithId(IRepetierCommand command, Type commandType, string printer = "")
        {
            var callbackId = Next();
            while (!_callbackMap.TryAdd(callbackId, command.CommandIdentifier))
            {
                _logger.LogTrace("[CommandManager::CommandWithId1] Failed to add callbackId for command {callbackId} with id {CommandIdentifier}.", command.CommandIdentifier, callbackId);
                callbackId = Next();
            }
            return new RepetierBaseRequest(command, printer, callbackId, commandType);
        }

        public RepetierBaseRequest CommandWithId(string command, string printer, Dictionary<string, object> data)
        {
            var callbackId = Next();
            while (!_callbackMap.TryAdd(callbackId, command))
            {
                _logger.LogTrace("[CommandManager::CommandWithId2] Failed to add callbackId for command {callbackId} with id {CommandIdentifier}.", command, callbackId);
                callbackId = Next();
            }
            return new RepetierBaseRequest(data, command, printer, callbackId);
        }

        public void AcknowledgeCommand(int callbackId)
        {
            if (!_callbackMap.ContainsKey(callbackId)) return;
            if (_callbackMap.TryRemove(callbackId, out var command) )
            {
                _logger.LogTrace("[CommandManager] Removed command='{command}' with Id={callbackId}.", command, callbackId);
            }
        }
        
        public string CommandIdentifierFor(int callbackId)
        {
            return _callbackMap.TryGetValue(callbackId, out var commandIdentifier)
                ? commandIdentifier
                : string.Empty;
        }
    }
}
