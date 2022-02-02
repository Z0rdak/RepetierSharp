using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Client.Connecting;
using MQTTnet.Client.Options;
using MQTTnet.Protocol;
using RepetierSharp.Models.Commands;
using RepetierSharp.RepetierMqtt.Util;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RepetierSharp.RepetierMqtt
{
    public class RepetierMqttClient
    {

        public class RepetierMqttClientBuilder
        {

            private RepetierMqttClient _repetierMqttClient = new RepetierMqttClient();

            public RepetierMqttClientBuilder()
            {

            }

            public RepetierMqttClientBuilder WithRepetierConnection(RepetierConnection repetierConnection)
            {
                _repetierMqttClient.RepetierConnection = repetierConnection;
                return this;
            }

            public RepetierMqttClientBuilder WithMqttClientOptions(IMqttClientOptions options)
            {
                _repetierMqttClient.MqttClientOptions = options;
                return this;
            }

            public RepetierMqttClientBuilder WithBaseTopic(string baseTopic)
            {
                _repetierMqttClient.BaseTopic = baseTopic;
                // TODO: remove trailing /
                return this;
            }

            public RepetierMqttClientBuilder TopicConfiguration(Tuple<string, string> commandTopicTuple)
            {
                _repetierMqttClient.TopicsForEvents.Add(commandTopicTuple.Item1, commandTopicTuple.Item2);
                return this;
            }

            public RepetierMqttClientBuilder TopicConfiguration(string command, string topic)
            {
                _repetierMqttClient.TopicsForEvents.Add(command, topic);
                return this;
            }

            public RepetierMqttClientBuilder WithSubscriptions(List<string> topics)
            {
                _repetierMqttClient.Topics.AddRange(topics);
                return this;
            }

            public RepetierMqttClientBuilder WithSubscriptions(List<MqttTopicFilter> topics)
            {
                _repetierMqttClient.Subscriptions.AddRange(topics);
                return this;
            }

            public RepetierMqttClientBuilder WithSubscription(string topic, string command)
            {
                _repetierMqttClient.TopicActions.Add(topic, command);
                return this;
            }

            public RepetierMqttClientBuilder WithReconnectDelay(uint delayInMs = 3000)
            {
                _repetierMqttClient.ReconnectDelay = delayInMs;
                return this;
            }

            public RepetierMqttClientBuilder DefaultQoSLevel(uint qos = 0)
            {
                _repetierMqttClient.DefaultQoS = (MqttQualityOfServiceLevel) qos;
                return this;
            }

            public RepetierMqttClientBuilder DefaultQoSLevel(MqttQualityOfServiceLevel qos = MqttQualityOfServiceLevel.AtMostOnce)
            {
                _repetierMqttClient.DefaultQoS = qos;
                return this;
            }



            public RepetierMqttClient Build()
            {
                if (_repetierMqttClient.RepetierConnection == null)
                {
                    throw new ArgumentNullException("RepetierConnection needs to be provided");
                }

                if (_repetierMqttClient.MqttClientOptions == null)
                {
                    _repetierMqttClient.MqttClientOptions = MqttOptionsProvider.DefaultMqttClientOptions;
                }

                // create subscriptions
                if (!string.IsNullOrEmpty(_repetierMqttClient.BaseTopic))
                {
                    _repetierMqttClient.Topics.ForEach(topic =>
                    {
                        var topicFilter = new MqttTopicFilterBuilder()
                        .WithQualityOfServiceLevel(_repetierMqttClient.DefaultQoS)
                        .WithTopic(topic)
                        .Build();
                        _repetierMqttClient.Subscriptions.Add(topicFilter);
                    });
                    _repetierMqttClient.Subscriptions.ForEach(topicFilter => topicFilter.Topic = $"{_repetierMqttClient.BaseTopic}/{topicFilter.Topic}");
                }

                _repetierMqttClient.MqttClient = new MqttFactory().CreateMqttClient();

                _repetierMqttClient.MqttClient.UseConnectedHandler(async connectedArgs =>
                {
                    await _repetierMqttClient.MqttClient.SubscribeAsync(_repetierMqttClient.Subscriptions.ToArray());
                });

                _repetierMqttClient.MqttClient.UseDisconnectedHandler(async disconnectedArgs =>
                {
                    await Task.Delay(TimeSpan.FromMilliseconds(_repetierMqttClient.ReconnectDelay));
                    try
                    {
                        await _repetierMqttClient.MqttClient.ConnectAsync(_repetierMqttClient.MqttClientOptions, CancellationToken.None);
                    }
                    catch (Exception e)
                    {
                        Console.Error.WriteLine(e);
                    }
                });

                _repetierMqttClient.RepetierConnection.OnResponse += async (callbackID, command, response) =>
                {
                    // TODO: check mapping and publish response
                    if (_repetierMqttClient.TopicActions.TryGetValue(command, out var topic))
                    {
                        // map for serialization? include printer? placeholder for printer?
                        await _repetierMqttClient.MqttClient.PublishAsync(topic, Encoding.UTF8.GetBytes(""));
                    }
                };

                _repetierMqttClient.RepetierConnection.OnEvent += async (eventName, printer, eventData) =>
                {
                    // TODO: check mapping and publish eventData
                    if(_repetierMqttClient.TopicsForEvents.TryGetValue(eventName, out var topic))
                    {
                        // map for serialization? include printer? placeholder for printer?
                        await _repetierMqttClient.MqttClient.PublishAsync(topic, Encoding.UTF8.GetBytes(""));
                    }
                };

                _repetierMqttClient.MqttClient.UseApplicationMessageReceivedHandler(async e =>
                {
                    
                });

                return _repetierMqttClient;
            }



        }

        private List<MqttTopicFilter> Subscriptions { get; set; } = new List<MqttTopicFilter>();

        private List<string> Topics { get; set; } = new List<string>();

        private Dictionary<string, string> TopicActions { get; set; } = new Dictionary<string, string>();

        // Topic -> Command to execute
        private Dictionary<string, ICommandData> CommandMapping { get; set; } = new Dictionary<string, ICommandData>();

        /// <summary>
        /// TODO: add QOS?
        /// Event -> Topic
        /// </summary>
        private Dictionary<string, string> TopicsForEvents { get; set; } = new Dictionary<string, string>();

        private RepetierConnection RepetierConnection { get; set; }

        public string BaseTopic { get; private set; }

        private IMqttClient MqttClient { get; set; }

        private IMqttClientOptions MqttClientOptions { get; set; }

        private uint ReconnectDelay { get; set; } = 3000;

        private MqttQualityOfServiceLevel DefaultQoS { get; set; } = 0;

        public RepetierMqttClient() { }

        public Task<MqttClientConnectResult> Connect()
        {
            return MqttClient.ConnectAsync(MqttClientOptions);
        }

        public Task Disconnect()
        {
            return MqttClient.DisconnectAsync();
        }
    }
}
