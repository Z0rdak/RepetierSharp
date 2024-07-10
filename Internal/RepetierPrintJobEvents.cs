using System;
using RepetierSharp.Models.Events;
using RestSharp;

namespace RepetierSharp.Internal
{
    public sealed class RepetierPrintJobEvents
    { 
        public AsyncEvent<PrintJobStartedEventArgs> PrintStartedEvent { get; } = new AsyncEvent<PrintJobStartedEventArgs>();
        public AsyncEvent<PrintJobFinishedEventArgs> PrintFinishedEvent { get; } = new AsyncEvent<PrintJobFinishedEventArgs>();
        public AsyncEvent<PrintJobKilledEventArgs> PrintKilledEvent { get; } = new AsyncEvent<PrintJobKilledEventArgs>();
        public AsyncEvent<PrintJobDeactivatedEventArgs> PrintDeactivatedEvent { get; } = new AsyncEvent<PrintJobDeactivatedEventArgs>();
        public AsyncEvent<PrintJobStartFailedEventArgs> PrintStartFailedEvent { get; } = new AsyncEvent<PrintJobStartFailedEventArgs>();
        public AsyncEvent<PrintJobAddedEventArgs> PrintJobAddedEvent { get; } = new AsyncEvent<PrintJobAddedEventArgs>();
    }
    
        
    public sealed class PrintJobAddedEventArgs : EventArgs
    {
        public PrintJobAddedEventArgs(string printer)
        {
            Printer = printer;
        }

        public string Printer { get; }
    }
    
    public sealed class PrintJobsChangedEventArgs : EventArgs
    {
        public PrintJobsChangedEventArgs(string printer)
        {
            Printer = printer;
        }

        public string Printer { get; }
    }
    
    public sealed class PrintJobStartFailedEventArgs : EventArgs
    {
        public PrintJobStartFailedEventArgs(string printer, RestResponse response)
        {
            Printer = printer;
            RestResponse = response;
        }

        public string Printer { get; }
        public RestResponse RestResponse { get; }
    }
    
    public sealed class PrintJobDeactivatedEventArgs : EventArgs
    {
        public PrintJobDeactivatedEventArgs(string printer, JobState jobDeactivated)
        {
            Printer = printer;
            JobState = jobDeactivated;
        }

        public string Printer { get; }
        public JobState JobState { get; }
    }
    
    public sealed class PrintJobKilledEventArgs : EventArgs
    {
        public PrintJobKilledEventArgs(string printer, JobState jobKilled)
        {
            Printer = printer;
            JobState = jobKilled;
        }

        public string Printer { get; }
        public JobState JobState { get; }
    }
    
    public sealed class PrintJobFinishedEventArgs : EventArgs
    {
        public PrintJobFinishedEventArgs(string printer, JobState jobFinished)
        {
            Printer = printer;
            JobState = jobFinished;
        }

        public string Printer { get; }
        public JobState JobState { get; }
    }

    public sealed class PrintJobStartedEventArgs : EventArgs
    {
        public PrintJobStartedEventArgs(string printer, JobStarted jobStarted)
        {
            Printer = printer;
            JobStarted = jobStarted;
        }

        public string Printer { get; }
        public JobStarted JobStarted { get; }
    }
}
