using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace BeetleX.MQTT.Protocols.V3.Messages
{
    public class UnSubAck : MQTTMessage
    {
        public override MQTTMessageType Type => MQTTMessageType.UnSubAck;
        public ushort Identifier { get; set; }

        protected override void OnRead(MQTTParse parse, Stream stream, ISession session)
        {
            base.OnRead(parse, stream, session);
            Identifier = parse.ReadUInt16(stream);

        }

        protected override void OnWrite(MQTTParse parse, Stream stream, ISession sessioni)
        {
            base.OnWrite(parse, stream, sessioni);
            parse.WriteUInt16(stream, Identifier);

        }
    }
}
