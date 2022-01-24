using RepetierSharp.Models;
using RepetierSharp.Models.Commands;
using System;
using System.Collections.Generic;
using System.Text;

namespace RepetierSharp.Util
{
    /// <summary>
    /// 
    /// </summary>
    internal class CommandManager
    {
        public int CallbackId { get => Next(); private set { _callBackId = value; } }
        private int _callBackId;

        // CallbackId -> Command
        private readonly Dictionary<int, string> CallbackMap = new Dictionary<int, string>();

        public int Next()
        {
            if (_callBackId == int.MaxValue)
            {
                _callBackId = 1;
                return _callBackId;
            }
            return _callBackId += 1;
        }

        public RepetierBaseCommand CommandWithId(ICommandData command, Type commandType, string printer = "")
        {
            var callbackId = Next();
            CallbackMap.Add(callbackId, command.CommandIdentifier);
            return new RepetierBaseCommand(command, printer, callbackId, commandType);
        }

        public string CommandIdentifierFor(int callbackId)
        {
            if (CallbackMap.TryGetValue(callbackId, out var commandIdentifier))
            {
                return commandIdentifier;
            }
            else
            {
                return string.Empty;
            }
        }

    }
}
