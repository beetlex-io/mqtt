using BeetleX.Clients;
using BeetleX.MQTT.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeetleX.MQTT
{


    public class MQTTClient<PARSE>
        where PARSE : MQTTParse, new()
    {
        public MQTTClient(string id, string host, int port, string sslName = null)
        {
            ClientInfo = new CONNECT();
            ClientInfo.SetClientID(id);
            Host = host;
            Port = port;
            NetClient = new AwaiterClient(host, port, new MQTTClientPacket<PARSE>(), sslName);
        }


        private string Host { get; set; }

        private int Port { get; set; }

        public CONNECT ClientInfo { get; private set; }

        public AwaiterClient NetClient { get; private set; }

        public async Task Connect()
        {
            var result = await NetClient.ReceiveFrom<CONNACK>(ClientInfo);
            if (result.ReturnCode != ReturnCode.Connected)
            {
                throw new MQTTException($"Connect MQTT server error {result.ReturnCode}");
            }
        }

        public async Task ReceivePublish(Action<AwaiterClient, PUBLISH> handler, params Subscription[] subscriptions)
        {
            SUBSCRIBE cmd = new SUBSCRIBE();
            cmd.Subscriptions = subscriptions.ToList();
            var result = await NetClient.ReceiveFrom<SUBACK>(cmd);
            while (true)
            {
                var msg = await NetClient.Receive();
                if (msg is PUBLISH item)
                {
                    handler(NetClient, item);
                }
            }

        }

        public async Task PublishJson(string topic, ushort id, object body)
        {
            PUBLISH msg = new PUBLISH();
            msg.Topic = topic;
            msg.Identifier = id;
            msg.SetJson(body);
            await NetClient.Send(msg);
        }

        public async Task PublishString(string topic, ushort id, string body)
        {
            PUBLISH msg = new PUBLISH();
            msg.Topic = topic;
            msg.Identifier = id;
            msg.SetString(body);
            await NetClient.Send(msg);
        }

        public async Task<Messages.SUBACK> Sub(string topic, QoSType type)
        {
            SUBSCRIBE msg = new SUBSCRIBE();
            msg.AddTopicFilter(topic, type);
            var result = await NetClient.ReceiveFrom<SUBACK>(msg);
            return result;
        }

        public async Task<object> Receive()
        {
            return await NetClient.Receive();
        }
    }
}
