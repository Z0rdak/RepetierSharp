using System;
using System.Collections.Generic;

namespace RepetierSharp.Internal
{
    public class CommandIdAttribute : Attribute
    {
        public CommandIdAttribute(string eventName)
        {
            CommandNames.Add(eventName);
        }

        public CommandIdAttribute(params string[] events)
        {
            CommandNames.AddRange(events);
        }

        public List<string> CommandNames { get; } = new();
    }
}
