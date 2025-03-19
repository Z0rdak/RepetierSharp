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
        public AsyncEvent<ChangeFilamentRequestedEventArgs> ChangeFilamentRequestedEvent { get; } = new();
    }

    public sealed class ChangeFilamentRequestedEventArgs : PrinterEventArgs
    {
        public ChangeFilamentRequestedEventArgs(string printer) : base(printer) { }
    }
    
    public sealed class LayerChangedEventArgs : PrinterEventArgs
    {
        public LayerChangedEventArgs(string printer, LayerChanged layerChange) : base(printer)
        {
            LayerChanged = layerChange;
        }
        public LayerChanged LayerChanged { get; }
    }

    public sealed class MovedEventArgs : PrinterEventArgs
    {
        public MovedEventArgs(string printer, MoveEntry moveEntry) : base(printer)
        {
            MoveEntry = moveEntry;
        }
        public MoveEntry MoveEntry { get; }
    }

    public sealed class TemperatureChangedEventArgs : PrinterEventArgs
    {
        public TemperatureChangedEventArgs(string printer, TempEntry tempEntry): base(printer) 
        {
            TempEntry = tempEntry;
        }
        public TempEntry TempEntry { get; }
    }

    public sealed class EmergencyStopTriggeredEventArgs : PrinterEventArgs
    {
        public EmergencyStopTriggeredEventArgs(string printer): base(printer) { }
    }

    public sealed class ActivatedEventArgs : PrinterEventArgs
    {
        public ActivatedEventArgs(string printer): base(printer) { }
    }

    public sealed class DeactivatedEventArgs : PrinterEventArgs
    {
        public DeactivatedEventArgs(string printer): base(printer) { }
    }

    public sealed class JobsChangedEventArgs : PrinterEventArgs
    {
        public JobsChangedEventArgs(string printer): base(printer) { }
    }

    public sealed class StateChangedEventArgs : PrinterEventArgs
    {
        public StateChangedEventArgs(string printer, PrinterState printerState): base(printer)
        {
            PrinterState = printerState;
        }
        public PrinterState PrinterState { get; }
    }

    public sealed class ConditionChangedEventArgs : PrinterEventArgs
    {
        public ConditionChangedEventArgs(string printer, PrinterConditionChanged printerCondition): base(printer)
        {
            PrinterCondition = printerCondition;
        }
        public PrinterConditionChanged PrinterCondition { get; }
    }

    public sealed class SettingChangedEventArgs : PrinterEventArgs
    {
        public SettingChangedEventArgs(string printer, PrinterSettingChanged settingChangedChanged): base(printer)
        {
            SettingChangedChanged = settingChangedChanged;
        }
        public PrinterSettingChanged SettingChangedChanged { get; }
    }
}
