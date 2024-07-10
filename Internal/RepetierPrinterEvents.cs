using System;
using RepetierSharp.Models;
using RepetierSharp.Models.Events;

namespace RepetierSharp.Internal
{
    public sealed class RepetierPrinterEvents
    {
        public AsyncEvent<StateReceivedEventArgs> StateReceivedEvent { get; } = new();
        public AsyncEvent<ConditionChangedEventArgs> ChangedEvent { get; } = new();
        public AsyncEvent<SettingChangedEventArgs> SettingChangedEvent { get; } = new();
        public AsyncEvent<JobsChangedEventArgs> JobsChangedEvent { get; } = new();

        /// <summary>
        ///     Event which is fired after the response for the printer activation request from was received.
        /// </summary>
        public AsyncEvent<ActivatedEventArgs> PrinterActivatedEvent { get; } = new();

        /// <summary>
        ///     Event which is fired after the response for the printer deactivation request from was received.
        /// </summary>
        public AsyncEvent<DeactivatedEventArgs> PrinterDeactivatedEvent { get; } = new();

        /// <summary>
        ///     Event which is fired after the response for the emergency stop request from the printer was received.
        /// </summary>
        public AsyncEvent<EmergencyStopTriggeredEventArgs> EmergencyStopTriggeredEvent { get; } = new();
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


    public sealed class StateReceivedEventArgs : EventArgs
    {
        public StateReceivedEventArgs(string printer, PrinterState printerState)
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
