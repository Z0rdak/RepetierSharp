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

      
        }
    }
}
