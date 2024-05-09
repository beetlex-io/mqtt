using BeetleX.MQTT.Protocols.V5.Headers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace BeetleX.MQTT.Protocols.V5.Messages
{
    public class PubComp : MQTTMessage
    {
        public override MQTTMessageType Type => MQTTMessageType.PubComp;

        public PubComp() { }

        public ushort Identifier { get; set; }

        public ReturnType Status { get; set; } = ReturnType.Success;

        public ArraySegment<byte> Payload { get; set; }

        protected override void OnRead(MQTTParse parse, Stream stream, ISession session)
        {
            base.OnRead(parse, stream, session);
            Identifier = parse.ReadUInt16(stream);
            Status = (ReturnType)stream.ReadByte();
            var ps = GetPropertiesStream();
            ps.Read(parse, stream);
            ReasonString = ps;
            UserProperties = ps;
            var len = (int)(stream.Length - stream.Position);
            if (len > 0)
            {
                var payload = MQTTMessage.RentPayloadBuffer(len);
                stream.Read(payload, 0, len);
                Payload = new ArraySegment<byte>(payload, 0, len);
            }
        }

        protected override void OnWrite(MQTTParse parse, Stream stream, ISession session)
        {
            base.OnWrite(parse, stream, session);
            parse.WriteUInt16(stream, Identifier);
            stream.WriteByte((byte)Status);
            var ps = GetPropertiesStream() + ReasonString + UserProperties;
            ps.Write(parse, stream);
            if (Payload != null)
                stream.Write(Payload.Array, Payload.Offset, Payload.Count);
        }

        public ReasonString ReasonString { get; set; }
        public List<UserProperty> UserProperties { get; set; }
    }
}
