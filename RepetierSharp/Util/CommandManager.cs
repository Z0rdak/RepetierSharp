using System;
using System.Collections.Generic;
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
        private readonly Dictionary<int, string> _callbackMap = new();
        private int _callBackId;
        public int CallbackId => Next();

        public int Next()
        {
            if ( _callBackId == int.MaxValue )
            {
                _callBackId = 1;
                return _callBackId;
            }

            return _callBackId += 1;
        }

        public RepetierBaseRequest CommandWithId(IRepetierCommand command, Type commandType, string printer = "")
        {
            var callbackId = Next();
            _callbackMap.Add(callbackId, command.CommandIdentifier);
            return new RepetierBaseRequest(command, printer, callbackId, commandType);
        }

        public RepetierBaseRequest CommandWithId(string command, string printer, Dictionary<string, object> data)
        {
            var callbackId = Next();
            _callbackMap.Add(callbackId, command);
            return new RepetierBaseRequest(data, command, printer, callbackId);
        }

        public void AcknowledgeCommand(int callbackId)
        {
            _callbackMap.Remove(callbackId);
        }
        
        public string CommandIdentifierFor(int callbackId)
        {
            return _callbackMap.TryGetValue(callbackId, out var commandIdentifier)
                ? commandIdentifier
                : string.Empty;
        }
    }
}
