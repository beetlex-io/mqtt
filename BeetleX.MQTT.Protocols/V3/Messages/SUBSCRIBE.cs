using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace BeetleX.MQTT.Protocols.V3.Messages
{
    public class Subscribe : MQTTMessage
    {

        public Subscribe()
        {
            Bit2 = 1;
        }
        public override MQTTMessageType Type => MQTTMessageType.Subscribe;
        public ushort Identifier { get; set; }

        protected override void OnRead(MQTTParse parse, Stream stream, ISession session)
        {
            base.OnRead(parse, stream, session);
            Identifier = parse.ReadUInt16(stream);
            while (stream.Position < stream.Length)
            {
                Subscription item = new Subscription();
                item.TopicFilter = parse.ReadString(stream);
                item.QoSType = (QoSType)stream.ReadByte();
                Subscriptions.Add(item);
            }
        }

        protected override void OnWrite(MQTTParse parse, Stream stream, ISession sessioni)
        {
            base.OnWrite(parse, stream, sessioni);
            parse.WriteUInt16(stream, Identifier);
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

        public List<Subscription> Subscriptions { get; set; } = new List<Subscription>();

        public override string ToString()
        {
            string result = "";
            foreach (var item in Subscriptions)
            {
                result += $"[Topic:{item.TopicFilter}|{item.QoSType}]";
            }
            return result;
        }
    }

}
