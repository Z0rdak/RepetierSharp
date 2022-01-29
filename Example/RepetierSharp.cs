using RepetierSharp.Models.Commands;
using RepetierSharp.Models.Events;
using RepetierSharp.Models.Messages;
using RepetierSharp.Util;
using RepetierSharp.Extentions;
using System;
using System.Text.Json;
using System.Threading;
using static RepetierSharp.RepetierConnection;

namespace RepetierSharp.Example
{
    public class RepetierSharp
    {
        public static void Main(string[] args)
        {
            RepetierConnection repetierConnection = new RepetierConnectionBuilder()
                //.WithHost("sfm-evotech-el11.mni.thm.de", 3344)
                .WithHost("demo.repetier-server.com",4006)
                .WithApiKey("7075e377-7472-447a-b77e-86d481995e7b")
                //.WithApiKey("fe2c8a4c-9ca1-4511-9779-6b98a635eacc")
                .QueryPrinterInterval(RepetierTimer.Timer60)             
                .QueryStateInterval(RepetierTimer.Timer30)
                //.WithCredentials("Repetier-Admin", "sfm_2020", true)
                //.UseLang("US")
                //.WithTls(true)
                .Build();

            repetierConnection.Connect();

            
            repetierConnection.OnRepetierEvent += (string eventName, string printer, IRepetierEvent eventData) =>
            {
                if (!(eventName == "temp"))
                {
                    Console.WriteLine($"Event={eventName}, Printer={printer}");
                }
                
            };

            repetierConnection.OnResponse += (id, command, response) =>
            {
                Console.WriteLine($"Response to: [{id}] {command}");
            };


            while (true)
            {

            }
        }
    }
}
