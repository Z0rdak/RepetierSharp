﻿using RepetierSharp.Models.Commands;
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
                .WithHost("demo.repetier-server.com",4006)
                .WithApiKey("7075e377-7472-447a-b77e-86d481995e7b")
                .QueryPrinterInterval(RepetierTimer.Timer60)             
                .QueryStateInterval(RepetierTimer.Timer30)
                .Build();

            
            repetierConnection.OnEvent += (string eventName, string printer, IRepetierEvent eventData) =>
            {

            };

            repetierConnection.OnResponse += (id, command, response) =>
            {
                
            };

            repetierConnection.OnRawEvent += (e, p, data) =>
            {                
                if (!(e == "temp"))
                {
                    Console.WriteLine($"Event [{e}@{((string.IsNullOrEmpty(p)) ? "" : p)}]: {Encoding.UTF8.GetString(data)}");
                }                 
            };

            repetierConnection.OnRawResponse += (id, cmd, data) =>
            {
                Console.WriteLine($"Receive [{id}]: {cmd} | {Encoding.UTF8.GetString(data)}");
            };

            repetierConnection.OnRepetierConnected += () =>
            {
                Console.WriteLine("Connected!");
                repetierConnection.ActivatePrinter("Delta");
                repetierConnection.EnqueueJob(6);
            };
       
            repetierConnection.Connect();

            //repetierConnection.UploadAndStartPrint(@"D:\Fuss_0.2mm_ABS_EL-11_2h24m.gcode", "Delta");
            //repetierConnection.UploadGCode(@"D:\Fuss_0.2mm_ABS_EL-11_2h24m.gcode", "Default", "Delta");

            while (true)
            {
               
            }
        }
    }
}
