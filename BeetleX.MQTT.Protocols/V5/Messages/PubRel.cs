using BeetleX.MQTT.Protocols.V5.Headers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace BeetleX.MQTT.Protocols.V5.Messages
{
    public class PubRel : MQTTMessage
    {
        public override MQTTMessageType Type => MQTTMessageType.PubRel;

        public PubRel()
        {
            Bit2 = 1;
        }

        public ushort Identifier { get; set; }

        public ReturnType Status { get; set; } = ReturnType.Success;

        protected override void OnRead(MQTTParse parse, Stream stream, ISession session)
        {
            base.OnRead(parse, stream, session);
            Identifier = parse.ReadUInt16(stream);
            Status = (ReturnType)stream.ReadByte();
            var ps = GetPropertiesStream();
            ps.Read(parse, stream);
            ReasonString = ps;
            UserProperties = ps;
        }

        protected override void OnWrite(MQTTParse parse, Stream stream, ISession session)
        {
            base.OnWrite(parse, stream, session);
            parse.WriteUInt16(stream, Identifier);
            stream.WriteByte((byte)Status);
            var ps = GetPropertiesStream() + ReasonString + UserProperties;
            ps.Write(parse, stream);
        }

        public ReasonString ReasonString { get; set; }
        public List<UserProperty> UserProperties { get; set; }
    }
}
