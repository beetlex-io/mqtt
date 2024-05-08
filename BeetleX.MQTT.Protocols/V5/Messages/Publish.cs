using BeetleX.MQTT.Protocols.V5.Headers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace BeetleX.MQTT.Protocols.V5.Messages
{
    public class Publish : MQTTMessage, IPublish
    {
        public override MQTTMessageType Type => MQTTMessageType.Publish;


        protected override void OnRead(MQTTParse parse, Stream stream, ISession session)
        {
            base.OnRead(parse, stream, session);
            Topic = parse.ReadString(stream);
            if (QoS != QoSType.MostOnce)
                Identifier = parse.ReadUInt16(stream);
            var ps = GetPropertiesStream();
            ps.Read(parse, stream);
            PayloadFormatIndicator = ps;
            MessageExpiryInterval = ps;
            TopicAlias = ps;
            ResponseTopic = ps;
            CorrelationData = ps;
            SubscriptionIdentifier = ps;
            ContentType = ps;
            UserProperties = ps;
            var len = (int)(stream.Length - stream.Position);
            var payload = MQTTMessage.RentPayloadBuffer(len);
            stream.Read(payload, 0, len);
            Payload = new ArraySegment<byte>(payload, 0, len);

        }

        protected override void OnWrite(MQTTParse parse, Stream stream, ISession session)
        {
            base.OnWrite(parse, stream, session);
            parse.WriteString(stream, Topic);
            if (QoS != QoSType.MostOnce)
                parse.WriteUInt16(stream, Identifier);
            var ps = GetPropertiesStream() + PayloadFormatIndicator + MessageExpiryInterval + TopicAlias + ResponseTopic + CorrelationData + SubscriptionIdentifier + UserProperties;
            ps.Write(parse, stream);
            stream.Write(Payload.Array, Payload.Offset, Payload.Count);
        }

        public QoSType QoS
        {
            get
            {
                return (QoSType)(Bit2 | Bit3 << 1);
            }
            set
            {
                Bit2 = (byte)(((byte)value) & MQTTParse.BIT_1);
                Bit3 = (byte)((MQTTParse.BIT_2 & (byte)value) >> 1);
            }
        }

        public bool Retain
        {
            get
            {
                return (Bit1 > 0);
            }
            set
            {
                Bit1 = (byte)(value ? 1 : 0);
            }
        }

        public bool DUP
        {
            get
            {
                return Bit4 > 0;
            }
            set
            {
                Bit4 = (byte)(value ? 1 : 0);
            }
        }

        public string Topic { get; set; }

        public ushort Identifier { get; set; }

        public PayloadFormatIndicator PayloadFormatIndicator { get; set; }

        public MessageExpiryInterval MessageExpiryInterval { get; set; }

        public TopicAlias TopicAlias { get; set; }

        public ResponseTopic ResponseTopic { get; set; }

        public CorrelationData CorrelationData { get; set; }

        public List<UserProperty> UserProperties { get; set; }

        public SubscriptionIdentifier SubscriptionIdentifier { get; set; }

        public ContentType ContentType { get; set; }

        public ArraySegment<byte> Payload { get; set; }
    }
}
