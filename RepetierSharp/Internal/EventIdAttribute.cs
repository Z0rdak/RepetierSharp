using System;
using System.Collections.Generic;

namespace RepetierSharp.Models.Events
{
    public class EventIdAttribute : Attribute
    {
        public EventIdAttribute(string eventName)
        {
            EventNames.Add(eventName);
        }

        public EventIdAttribute(params string[] events)
        {
            EventNames.AddRange(events);
        }

        public List<string> EventNames { get; } = new();
    }


}
