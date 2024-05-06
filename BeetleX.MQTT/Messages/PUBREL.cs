using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace BeetleX.MQTT.Messages
{
    public class PUBREL:MQTTMessage
    {
        public override MQTTMessageType Type => MQTTMessageType.PUBREL;

        public ushort Identifier { get; set; }

        protected override void OnRead(Stream stream, ISession session)
        {
            base.OnRead(stream, session);
            Identifier = ReadUInt16(stream);
            OnReadHeaders(stream, session);
        }

        protected override void OnWrite(Stream stream, ISession session)
        {
            base.OnWrite(stream, session);
            WriteUInt16(stream, Identifier);
            OnWriteHeaders(stream, session);
        }
    }
}
