using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace BeetleX.MQTT.Messages
{
    public class PUBACK : MQTTMessage
    {
        public override MQTTMessageType Type => MQTTMessageType.PUBACK;

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
    public class Node_PUBACK : PUBACK
    {
        public int NetClientID { get; set; }
        protected override void OnReadHeaders(Stream stream, ISession session)
        {
            base.OnReadHeaders(stream, session);
            NetClientID = ReadInt(stream);
        }
        protected override void OnWriteHeaders(Stream stream, ISession session)
        {
            base.OnWriteHeaders(stream, session);
            WriteInt(stream, NetClientID);
        }
    }
}
