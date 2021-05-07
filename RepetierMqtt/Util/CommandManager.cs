using RepetierMqtt.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace RepetierMqtt.Util
{
    public class CommandManager
    {
        public static int CallbackId { get => Next(); set { CallbackId = value; } }

        // CallbackId -> Command
        private static Dictionary<int, string> CallbackMap = new Dictionary<int, string>();

        public static int Next()
        {
            if (CallbackId == int.MaxValue)
            {
                CallbackId = 0;
                return CallbackId;
            }
            return CallbackId += 1;
        }

        public static RepetierBaseCommand CommandWithId(ICommandData command, string printer = "")
        {
            var callbackId = Next();
            CallbackMap.Add(callbackId, command.CommandIdentifier);
            return new RepetierBaseCommand(command, printer, callbackId);
        }

        public static string CommandIdentifierFor(int callbackId)
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
