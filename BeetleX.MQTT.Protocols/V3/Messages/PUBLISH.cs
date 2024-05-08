using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace BeetleX.MQTT.Protocols.V3.Messages
{
    public class Publish : MQTTMessage, IPublish
    {
        public override MQTTMessageType Type => MQTTMessageType.Publish;

        public string Topic { get; set; }

        public ushort Identifier { get; set; }

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

        public ArraySegment<byte> Payload { get; set; }

        public string ToString(Encoding encoding = null)
        {
            if (this.Payload == null)
                return string.Empty;
            if (encoding == null)
                encoding = Encoding.UTF8;
            return encoding.GetString(Payload.Array, 0, Payload.Count);
        }
        public override string ToString()
        {
            return ToString(null);
        }
        public T ToJsonObject<T>()
        {
            if (this.Payload == null)
                return default(T);
            string str = ToString();
            return System.Text.Json.JsonSerializer.Deserialize<T>(str);
        }

        public void SetString(string value, Encoding encoding = null)
        {
            if (encoding == null)
                encoding = Encoding.UTF8;
            int size = value.Length * 6;
            var buffer = RentPayloadBuffer(size);
            var count = encoding.GetBytes(value, 0, value.Length, buffer, 0);
            this.Payload = new ArraySegment<byte>(buffer, 0, count);
        }

        public void SetJson(object value)
        {
            string str = System.Text.Json.JsonSerializer.Serialize(value);
            SetString(str);

        }

        protected override void OnRead(MQTTParse parse, Stream stream, ISession session)
        {
            base.OnRead(parse, stream, session);
            Topic = parse.ReadString(stream);
            if (QoS == QoSType.LeastOnce | QoS == QoSType.ExactlyOnce)
                Identifier = parse.ReadUInt16(stream);
            int length = (int)(stream.Length - stream.Position);
            var buffer = RentPayloadBuffer((int)length);
            stream.Read(buffer, 0, length);
            Payload = new ArraySegment<byte>(buffer, 0, length);

        }

        protected override void OnWrite(MQTTParse parse, Stream stream, ISession session)
        {
            base.OnWrite(parse, stream, session);
            parse.WriteString(stream, Topic);
            if (QoS == QoSType.LeastOnce | QoS == QoSType.ExactlyOnce)
                parse.WriteUInt16(stream, Identifier);

            stream.Write(Payload.Array, Payload.Offset, Payload.Count);
        }


    }

}
