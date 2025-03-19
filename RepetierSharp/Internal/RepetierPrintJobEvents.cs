using System;
using RepetierSharp.Models.Events;
using RestSharp;

namespace RepetierSharp.Internal
{
    public sealed class RepetierPrintJobEvents
    {
        public AsyncEvent<PrintJobStartedEventArgs> PrintStartedEvent { get; } = new();
        public AsyncEvent<PrintJobFinishedEventArgs> PrintFinishedEvent { get; } = new();
        public AsyncEvent<PrintJobKilledEventArgs> PrintKilledEvent { get; } = new();
        public AsyncEvent<PrintJobDeactivatedEventArgs> PrintDeactivatedEvent { get; } = new();
        public AsyncEvent<PrintJobStartFailedEventArgs> PrintStartFailedEvent { get; } = new();
        public AsyncEvent<PrintJobAddedEventArgs> PrintJobAddedEvent { get; } = new();
        public AsyncEvent<GcodeStorageChangedEventArgs> GcodeStorageChangedEvent { get; } = new();
    }

    public sealed class PrintJobAddedEventArgs : PrinterEventArgs
    {
        public PrintJobAddedEventArgs(string printer) : base(printer) { }
    }

    public sealed class GcodeStorageChangedEventArgs : PrinterEventArgs
    {
        public GcodeStorageChangedEventArgs(string printer, GcodeStorageChange storageChange) : base(printer)
        {
            this.StorageChange = storageChange;
        }
        public GcodeStorageChange StorageChange { get; }
    }

    public sealed class PrintJobStartFailedEventArgs : PrinterEventArgs
    {
        public PrintJobStartFailedEventArgs(string printer, RestResponse response) : base(printer)
        {
            RestResponse = response;
        }
        public RestResponse RestResponse { get; }
    }

    public sealed class PrintJobDeactivatedEventArgs : PrinterEventArgs
    {
        public PrintJobDeactivatedEventArgs(string printer, JobState jobDeactivated) : base(printer)
        {
            JobState = jobDeactivated;
        }
        public JobState JobState { get; }
    }

    public sealed class PrintJobKilledEventArgs : PrinterEventArgs
    {
        public PrintJobKilledEventArgs(string printer, JobState jobKilled) : base(printer)
        {
            JobState = jobKilled;
        }
        public JobState JobState { get; }
    }

    public sealed class PrintJobFinishedEventArgs : PrinterEventArgs
    {
        public PrintJobFinishedEventArgs(string printer, JobState jobFinished) : base(printer)
        {
            JobState = jobFinished;
        }
        public JobState JobState { get; }
    }

    public sealed class PrintJobStartedEventArgs : PrinterEventArgs
    {
        public PrintJobStartedEventArgs(string printer, JobStarted jobStarted) : base(printer)
        {
            JobStarted = jobStarted;
        }
        public JobStarted JobStarted { get; }
    }
}
