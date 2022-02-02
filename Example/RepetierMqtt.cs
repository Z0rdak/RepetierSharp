using MQTTnet;
using MQTTnet.Protocol;
using RepetierSharp.RepetierMqtt.Util;
using System;
using System.Collections.Generic;
using System.Text;
using static RepetierSharp.RepetierConnection;
using static RepetierSharp.RepetierMqtt.RepetierMqttClient;
using static RepetierSharp.Extentions.RepetierConnectionExtentions;
using RepetierSharp.Models.Events;

namespace RepetierSharp.Example
{
    public class RepetierMqtt
    {
        public static void Main(string[] args)
        {
            RepetierConnection rc = new RepetierConnectionBuilder()
                .WithHost("demo.repetier-server.com", 4006)
                .WithApiKey("7075e377-7472-447a-b77e-86d481995e7b")
                .QueryPrinterInterval(RepetierTimer.Timer60)
                .QueryStateInterval(RepetierTimer.Timer30)
                .Build();

            rc.Connect();


            rc.OnEvent += (string eventName, string printer, IRepetierEvent eventData) =>
            {
                if (!(eventName == "temp"))
                {
                    Console.WriteLine($"Event={eventName}, Printer={printer}");
                }

            };

            rc.OnResponse += (id, command, response) =>
            {
                Console.WriteLine($"Receive [{id}]: {command}");
            };



            var topics = new List<MqttTopicFilter>{
                new MqttTopicFilterBuilder()
                .WithQualityOfServiceLevel(MqttQualityOfServiceLevel.ExactlyOnce)
                .WithTopic("TestX")
                .Build()
            };

            var MqttClient = new RepetierMqttClientBuilder()
                .WithRepetierConnection(rc)
                .WithBaseTopic("RepetierMqtt/Test")
                .WithMqttClientOptions(MqttOptionsProvider.DefaultMqttClientOptions)
                .DefaultQoSLevel(MqttQualityOfServiceLevel.ExactlyOnce)
                .WithReconnectDelay(1000)
                .WithSubscription("Hallo/Welt", "ping")
                .WithSubscriptions(new List<string> { "Test", "Test1", "Test2" })
                .WithSubscriptions(topics)
                .Build();

            MqttClient.Connect();

            
      
        }
    }
}
