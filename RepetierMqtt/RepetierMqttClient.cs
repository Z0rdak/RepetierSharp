using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Client.Options;
using RepetierMqtt.Util;
using System;
using System.Configuration;
using System.Threading;
using System.Threading.Tasks;

namespace RepetierMqtt
{
    public class RepetierMqttClient
    {
        public RepetierConnection RepetierServer { get; private set; }

        public IMqttClient MqttClient { get; private set; }

        private IMqttClientOptions MqttClientOptions { get; set; }

        public RepetierMqttClient(string repetierServerUrl, string apiKey = "", IMqttClientOptions mqttClientOptions = null)
        {
            RepetierServer = new RepetierConnection(repetierServerUrl);

            MqttClient = new MqttFactory().CreateMqttClient();
            MqttClientOptions = mqttClientOptions ?? MqttOptionsProvider.DefaultMqttClientOptions;

            MqttClient.UseConnectedHandler(async e => 
            {
                Console.WriteLine("### CONNECTED WITH SERVER ###");

                // Subscribe to a topic
                await MqttClient.SubscribeAsync(new MqttTopicFilterBuilder().WithTopic("my/topic").Build());

                Console.WriteLine("### SUBSCRIBED ###");
                RepetierServer.Connect();
            });

            MqttClient.UseDisconnectedHandler(async e =>
            {
                Console.WriteLine("### DISCONNECTED FROM SERVER ###");
                await Task.Delay(TimeSpan.FromSeconds(5));

                try
                {
                    await MqttClient.ConnectAsync(MqttClientOptions, CancellationToken.None);
                }
                catch
                {
                    Console.WriteLine("### RECONNECTING FAILED ###");
                }
            });

            MqttClient.UseApplicationMessageReceivedHandler(async e => 
            {

                var topic = e.ApplicationMessage.Topic;
                var message = e.ApplicationMessage.Payload;
                var printerSlug = "";
                
                if (topic == $"{repetierServerUrl}/{printerSlug}/Job/start")
                {

                }


                await MqttClient.PublishAsync("");
            });

        }


        public void Start()
        {
            MqttClient.ConnectAsync(MqttClientOptions);
        }

     
    }
}