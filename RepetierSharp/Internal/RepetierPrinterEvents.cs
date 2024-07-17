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
        public AsyncEvent<LayerChangedEventArgs> LayerChangedEvent { get; } = new();
    }
    
    public sealed class LayerChangedEventArgs : EventArgs
    {
        public LayerChangedEventArgs(string printer, LayerChanged layerChange)
        {
            Printer = printer;
            LayerChanged = layerChange;
        }
        public string Printer { get; }
        public LayerChanged LayerChanged { get; }
    }

    public sealed class MovedEventArgs : EventArgs
    {
        public MovedEventArgs(string printer, MoveEntry moveEntry)
        {
            Printer = printer;
            MoveEntry = moveEntry;
        }

        public string Printer { get; }
        public MoveEntry MoveEntry { get; }
    }

    public sealed class TemperatureChangedEventArgs : EventArgs
    {
        public TemperatureChangedEventArgs(string printer, TempEntry tempEntry)
        {
            Printer = printer;
            TempEntry = tempEntry;
        }

        public string Printer { get; }
        public TempEntry TempEntry { get; }
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
        public SettingChangedEventArgs(string printer, PrinterSettingChanged settingChangedChanged)
        {
            Printer = printer;
            SettingChangedChanged = settingChangedChanged;
        }

        public string Printer { get; }
        public PrinterSettingChanged SettingChangedChanged { get; }
    }
}
