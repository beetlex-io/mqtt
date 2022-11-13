using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace BeetleX.MQTT.Messages
{
    public class PUBLISH : MQTTMessage
    {
        public override MQTTMessageType Type => MQTTMessageType.PUBLISH;

        public string Topic { get; set; }

        public ushort Identifier { get; set; }

        public ArraySegment<byte> PayLoadData { get; set; }

        protected override void OnRead(Stream stream, ISession session)
        {
            base.OnRead(stream, session);
            Topic = ReadString(stream);
            if (QoS == QoSType.LeastOnce | QoS == QoSType.ExactlyOnce)
                Identifier = ReadUInt16(stream);
            int length = (int)(stream.Length - stream.Position);
            var buffer = RentPayloadBuffer((int)length);
            stream.Read(buffer, 0, length);
            PayLoadData = new ArraySegment<byte>(buffer, 0, length);
        }

        protected override void OnWrite(Stream stream, ISession sessioni)
        {
            base.OnWrite(stream, sessioni);
            WriteString(stream, Topic);
            if (QoS == QoSType.LeastOnce | QoS == QoSType.ExactlyOnce)
                WriteUInt16(stream, Identifier);
            stream.Write(PayLoadData.Array, PayLoadData.Offset, PayLoadData.Count);
            ReturnPayloadBuffer(PayLoadData.Array);
        }
    }
}
