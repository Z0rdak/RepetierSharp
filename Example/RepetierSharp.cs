using RepetierSharp.Models.Commands;
using RepetierSharp.Models.Events;
using RepetierSharp.Models.Messages;
using RepetierSharp.Util;
using RepetierSharp.Extentions;
using System;
using System.Text.Json;
using System.Threading;
using static RepetierSharp.RepetierConnection;
using System.Text;
using RepetierSharp.Config;

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

            //repetierConnection.UploadAndStartPrint(@"D:\Fuss_0.2mm_ABS_EL-11_2h24m.gcode", "Cartesian");
            //repetierConnection.UploadGCode(@"D:\Fuss_0.2mm_ABS_EL-11_2h24m.gcode", "Default", "Delta");

            repetierConnection.OnEvent += (string eventName, string printer, IRepetierEvent eventData) =>
            {
                if (!(eventName == "temp"))
                {
                    //Console.WriteLine($"Event={eventName}, Printer={printer}");
                }
            };

            repetierConnection.OnResponse += (id, command, response) =>
            {
                //Console.WriteLine($"Receive [{id}]: {command}");
            };


            repetierConnection.OnRawEvent += (e, p, data) =>
            {                
               
                    Console.WriteLine($"Event [{e}@{((string.IsNullOrEmpty(p)) ? "" : p)}]: {Encoding.UTF8.GetString(data)}");
                
            };

            repetierConnection.OnRawResponse += (id, cmd, data) =>
            {
                Console.WriteLine($"Receive [{id}]: {cmd} | {Encoding.UTF8.GetString(data)}");
            };


            // Activate
            // Deactivate
            // Stop
            // Pause (Send)
            // Continue
            // SetFan,Extruder,Etc
            // repetierConnection.EnqueueJob(6);

            // repetierConnection.PreheatAll(); // permission denied with demo server
            // repetierConnection.Cooldown();// permission denied with demo server




            Thread.Sleep(2000);
            repetierConnection.SendCommand(new ActivateCommand("Cartesian"));
            Thread.Sleep(2000);

            repetierConnection.RemoveJob(6);



            while (true)
            {
               
            }
        }
    }
}
