using BeetleX.MQTT.Protocols.V5.Headers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace BeetleX.MQTT.Protocols.V5.Messages
{
    public class UnSubAck : MQTTMessage
    {

        public override MQTTMessageType Type => MQTTMessageType.UnSubAck;

        public ushort Identifier { get; set; }

        public ReasonString ReasonString { get; set; }

        public List<UserProperty> UserProperties { get; set; }

        public List<ReturnType> Status { get; set; } = new List<ReturnType>();

        protected override void OnRead(MQTTParse parse, Stream stream, ISession session)
        {
            base.OnRead(parse, stream, session);
            Identifier = parse.ReadUInt16(stream);
            var ps = GetPropertiesStream();
            ps.Read(parse, stream);
            ReasonString = ps;
            UserProperties = ps;
            while (stream.Position < stream.Length)
            {
                ReturnType status = (ReturnType)stream.ReadByte();
                Status.Add(status);
            }
        }

        protected override void OnWrite(MQTTParse parse, Stream stream, ISession session)
        {
            base.OnWrite(parse, stream, session);
            parse.WriteUInt16(stream, Identifier);
            var ps = GetPropertiesStream() + ReasonString + UserProperties;
            ps.Write(parse, stream);
            foreach (var item in Status)
            {
                stream.WriteByte((byte)item);
            }
        }
    }
}
