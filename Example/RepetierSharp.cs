using RepetierSharp.Models.Commands;
using RepetierSharp.Models.Events;
using RepetierSharp.Models.Messages;
using RepetierSharp.Util;
using System;
using System.Text.Json;
using static RepetierSharp.RepetierConnection;

namespace RepetierSharp.Example
{
    public class RepetierSharp
    {
        public static void Main(string[] args)
        {
            RepetierConnection repetierConnection = new RepetierConnectionBuilder()
                //.WithHost("sfm-evotech-el11.mni.thm.de", 3344)
                .WithHost("demo.repetier-server.com",4000)
                .WithApiKey("7075e377-7472-447a-b77e-86d481995e7b")
                .WithCredentials("el11-bridge", "sfm_2020", rememberSession: true)
                //.WithApiKey("fe2c8a4c-9ca1-4511-9779-6b98a635eacc")
                .QueryPrinterInterval(RepetierTimer.Timer60)             
                .QueryStateInterval(RepetierTimer.Timer30)
                .WithCyclicCommand(RepetierTimer.Timer3600, UpdateAvailableCommand.Instance)
                //.WithCredentials("Repetier-Admin", "sfm_2020", true)
                // .UseLang("US")
                .WithTls(false)
                .Build();

            repetierConnection.Connect();

            repetierConnection.OnRepetierEvent += (string eventName, string printer, IRepetierEvent eventData) =>
            {
                
            };

            repetierConnection.OnResponse += (id, command, response) =>
            {

            };

            repetierConnection.OnJobFinishedReceived += (printer, jobFinishedEvent, timestamp) =>
            {
                Console.WriteLine($"[Event]: Print job finished at {timestamp}");
                Console.WriteLine($"Printer name: {printer}");
                Console.WriteLine($"Started at: {jobFinishedEvent.StartTime}");
                Console.WriteLine($"Finished at: {jobFinishedEvent.EndTime}");
                Console.WriteLine($"Print duration: {jobFinishedEvent.Duration}");
                Console.WriteLine($"Total printed lines: {jobFinishedEvent.PrintedLines}");
            };

            repetierConnection.OnJobStartedReceived += (printerName, jobStartedEvent, timestamp) =>
            {
                Console.WriteLine("4");
                /* */
            };

            repetierConnection.OnPrinterStateReceived += (printerName, printerState, timestamp) =>
            {
                Console.WriteLine("5");
                /* */
            };

            repetierConnection.OnPrinterListReceived += (sender, printerList) => 
            {
                Console.WriteLine("3");
                /* QueryPrinterList queries the list of all printers
                 * which contains information like print job progress */
            };

            repetierConnection.OnPrinterStateReceived += (printerName, printerState, timestamp) =>
            {
                /* QueryPrinterStateList queries the state of all printers
                 * which includes information like temperatures of extruder, heatedbed and so on */
                Console.WriteLine("2");
            };

            repetierConnection.OnTempChangeReceived += (printer, tempChangeEvent, timestamp) =>
            {
               
            };

  

            while (true)
            {

            }
        }
    }
}
