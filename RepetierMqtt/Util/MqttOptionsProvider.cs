using MQTTnet.Client.Options;
using System;
using System.Collections.Generic;
using System.Text;

namespace RepetierMqtt.Util
{
    public class MqttOptionsProvider
    {
        public static IMqttClientOptions DefaultMqttClientOptions
        {
            get { return DefaultClientOptions(); }
        }

        public static string DefaultBrokerUrl
        {
            get { return _brokerUrl ?? "broker.hivemq.com"; }
            set { _brokerUrl = value ?? "broker.hivemq.com"; }
        }

        private static string _brokerUrl;

        public static IMqttClientOptions DefaultClientOptions(string clientId = null)
        {
            return new MqttClientOptionsBuilder()
                .WithClientId(clientId ?? $"{Guid.NewGuid()}")
                .WithTcpServer(_brokerUrl)
                .WithCleanSession()
                .Build();
        }
    }

}
