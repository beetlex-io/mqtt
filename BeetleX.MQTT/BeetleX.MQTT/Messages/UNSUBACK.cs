using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace BeetleX.MQTT.Messages
{
    public class UNSUBACK : MQTTMessage
    {
        public override MQTTMessageType Type => MQTTMessageType.UNSUBACK;
        public ushort Identifier { get; set; }

        protected override void OnRead(Stream stream, ISession session)
        {
            base.OnRead(stream, session);
            Identifier = ReadUInt16(stream);

        }

        protected override void OnWrite(Stream stream, ISession sessioni)
        {
            base.OnWrite(stream, sessioni);
            WriteUInt16(stream, Identifier);

        }
    }
}
