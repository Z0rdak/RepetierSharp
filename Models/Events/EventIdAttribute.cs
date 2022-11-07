using System;
using System.Collections.Generic;

namespace RepetierSharp.Models.Events
{
    internal class EventIdAttribute : Attribute
    {
        public List<string> EventNames { get; } = new List<string>();

        public EventIdAttribute(string eventName)
        {
            EventNames.Add(eventName);
        }

        public EventIdAttribute(params string[] events)
        {
            EventNames.AddRange(events);
        }
    }
}
