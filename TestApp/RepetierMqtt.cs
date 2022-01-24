using MQTTnet;
using MQTTnet.Protocol;
using RepetierSharp.RepetierMqtt.Util;
using System;
using System.Collections.Generic;
using System.Text;
using static RepetierSharp.RepetierConnection;
using static RepetierSharp.RepetierMqtt.RepetierMqttClient;
using static RepetierSharp.Extentions.RepetierConnectionExtentions;

namespace RepetierSharp.Example
{
    public class RepetierMqtt
    {
        public static void Main(string[] args)
        {
            var RepetierConnection = new RepetierConnectionBuilder()
                .WithHost("localhost", 3344)                
                .WithApiKey("api-key")
                .Build();

            RepetierConnection.Connect();

            var topics = new List<MqttTopicFilter>{
                new MqttTopicFilterBuilder()
                .WithQualityOfServiceLevel(MqttQualityOfServiceLevel.ExactlyOnce)
                .WithTopic("TestX")
                .Build()
            };

            var MqttClient = new RepetierMqttClientBuilder()
                .WithRepetierConnection(RepetierConnection)
                .WithBaseTopic("RepetierMqtt/Test")
                .WithMqttClientOptions(MqttOptionsProvider.DefaultMqttClientOptions)
                .DefaultQoSLevel(MqttQualityOfServiceLevel.ExactlyOnce)
                .WithReconnectDelay(1000)
                .WithSubscription("Hallo/Welt", () => Console.WriteLine("Hallo Welt!"))
                .WithSubscriptions(new List<string> { "Test", "Test1", "Test2" })
                .WithSubscriptions(topics)
                .Build();

            MqttClient.Connect();

            RepetierConnection.OnJobFinishedReceived += (printer, jobFinishedEvent, timestamp) =>
            {
                Console.WriteLine($"[Event]: Print job finished at {timestamp}");
                Console.WriteLine($"Printer name: {printer}");
                Console.WriteLine($"Started at: {jobFinishedEvent.StartTime}");
                Console.WriteLine($"Finished at: {jobFinishedEvent.EndTime}");
                Console.WriteLine($"Print duration: {jobFinishedEvent.Duration}");
                Console.WriteLine($"Total printed lines: {jobFinishedEvent.PrintedLines}");
            };

            RepetierConnection.OnJobStartedReceived += (printerName, jobStartedEvent, timestamp) =>
            {
                Console.WriteLine($"Print job started at {jobStartedEvent.StartTime} on printer '{printerName}'");
            };

            RepetierConnection.OnPrinterStateReceived += (printerName, printerState, timestamp) =>
            {
                /* */
            };

            RepetierConnection.OnChangeFilamentReceived += (printer, timestamp) =>
            {
                /* */
            };

            RepetierConnection.OnPrinterListReceived += (sender, printerList) =>
            {
                /* QueryPrinterList queries the list of all printers
                 * which contains information like print job progress */
            };
            
            RepetierConnection.OnPrinterStateReceived += (printerName, printerState, timestamp) =>
            {
                /* QueryPrinterStateList queries the state of all printers
                 * which includes information like temperatures of extruder, heatedbed and so on */
            };

            RepetierConnection.OnTempChangeReceived += (printer, tempChangeEvent, timestamp) =>
            {

            };

        }
    }
}
