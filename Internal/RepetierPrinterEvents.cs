using System;
using RepetierSharp.Models;
using RepetierSharp.Models.Events;

namespace RepetierSharp.Internal
{
    public sealed class RepetierPrinterEvents
    {
        public AsyncEvent<StateChangedEventArgs> StateChangedEvent { get; } = new();
        public AsyncEvent<ConditionChangedEventArgs> ConditionChangedEvent { get; } = new();
        public AsyncEvent<SettingChangedEventArgs> SettingChangedEvent { get; } = new();
        public AsyncEvent<JobsChangedEventArgs> JobsChangedEvent { get; } = new();
        public AsyncEvent<ActivatedEventArgs> PrinterActivatedEvent { get; } = new();
        public AsyncEvent<DeactivatedEventArgs> PrinterDeactivatedEvent { get; } = new();
        public AsyncEvent<EmergencyStopTriggeredEventArgs> EmergencyStopTriggeredEvent { get; } = new();
        public AsyncEvent<TemperatureChangedEventArgs> TemperatureChangedEvent { get; } = new();
        public AsyncEvent<MovedEventArgs> MovedEvent { get; } = new();
    }

    public sealed class MovedEventArgs : EventArgs
    {
        public MovedEventArgs(string printer, Move moveEntry)
        {
            Printer = printer;
            MoveEntry = moveEntry;
        }

        public string Printer { get; }
        public Move MoveEntry { get; }
    }

    public sealed class TemperatureChangedEventArgs : EventArgs
    {
        public TemperatureChangedEventArgs(string printer, Temp tempChange)
        {
            Printer = printer;
            TempChange = tempChange;
        }

        public string Printer { get; }
        public Temp TempChange { get; }
    }
    
    public sealed class EmergencyStopTriggeredEventArgs : EventArgs
    {
        public EmergencyStopTriggeredEventArgs(string printer)
        {
            Printer = printer;
        }

        public string Printer { get; }
    }

    public sealed class ActivatedEventArgs : EventArgs
    {
        public ActivatedEventArgs(string printer)
        {
            Printer = printer;
        }

        public string Printer { get; }
    }

    public sealed class DeactivatedEventArgs : EventArgs
    {
        public DeactivatedEventArgs(string printer)
        {
            Printer = printer;
        }

        public string Printer { get; }
    }


    public sealed class JobsChangedEventArgs : EventArgs
    {
        public JobsChangedEventArgs(string printer)
        {
            Printer = printer;
        }

        public string Printer { get; }
    }


    public sealed class StateChangedEventArgs : EventArgs
    {
        public StateChangedEventArgs(string printer, PrinterState printerState)
        {
            Printer = printer;
            PrinterState = printerState;
        }

        public string Printer { get; }
        public PrinterState PrinterState { get; }
    }

    public sealed class ConditionChangedEventArgs : EventArgs
    {
        public ConditionChangedEventArgs(string printer, PrinterConditionChanged printerCondition)
        {
            Printer = printer;
            PrinterCondition = printerCondition;
        }

        public string Printer { get; }
        public PrinterConditionChanged PrinterCondition { get; }
    }

    public sealed class SettingChangedEventArgs : EventArgs
    {
        public SettingChangedEventArgs(string printer, SettingChanged settingChanged)
        {
            Printer = printer;
            SettingChanged = settingChanged;
        }

        public string Printer { get; }
        public SettingChanged SettingChanged { get; }
    }
}
