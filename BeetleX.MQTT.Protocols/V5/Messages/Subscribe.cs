using BeetleX.MQTT.Protocols.V5.Headers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace BeetleX.MQTT.Protocols.V5.Messages
{
    public class Subscribe : MQTTMessage
    {
        public Subscribe()
        {
            Bit2 = 1;
        }

        public ushort Identifier { get; set; }
        public override MQTTMessageType Type => MQTTMessageType.Subscribe;

        public SubscriptionIdentifier SubscriptionIdentifier { get; set; }

        public List<UserProperty> UserProperties { get; set; }

        public List<Subscription> Subscriptions { get; set; } = new List<Subscription>();

        protected override void OnRead(MQTTParse parse, Stream stream, ISession session)
        {
            base.OnRead(parse, stream, session);
            Identifier = parse.ReadUInt16(stream);
            var ps = GetPropertiesStream();
            ps.Read(parse, stream);
            SubscriptionIdentifier = ps;
            UserProperties = ps;
            while (stream.Position < stream.Length)
            {
                Subscription item = new Subscription();
                item.TopicFilter = parse.ReadString(stream);
                item.QoSType = (QoSType)stream.ReadByte();
                Subscriptions.Add(item);
            }
        }

        protected override void OnWrite(MQTTParse parse, Stream stream, ISession session)
        {
            base.OnWrite(parse, stream, session);
            parse.WriteUInt16(stream, Identifier);
            var ps = GetPropertiesStream() + SubscriptionIdentifier + UserProperties;
            ps.Write(parse, stream);
            foreach (var item in Subscriptions)
            {
                parse.WriteString(stream, item.TopicFilter);
                stream.WriteByte((byte)item.QoSType);
            }
        }

        public Subscribe AddTopicFilter(string name, QoSType type)
        {
            Subscriptions.Add(new Subscription { TopicFilter = name, QoSType = type });
            return this;
        }

        
    }
}
