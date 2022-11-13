using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace BeetleX.MQTT.Messages
{
    public class CONNACK : MQTTMessage
    {
        public override MQTTMessageType Type => MQTTMessageType.CONNACK;

        public byte ConnectFlags { get; set; } = 0x1;

        public byte ReturnCode { get; set; } = 0;

        protected override void OnRead(Stream stream, ISession session)
        {
            base.OnRead(stream, session);
            ConnectFlags = (byte)stream.ReadByte();
            ReturnCode = (byte)stream.ReadByte();

        }

        protected override void OnWrite(Stream stream, ISession sessioni)
        {
            base.OnWrite(stream, sessioni);
            stream.WriteByte(ConnectFlags);
            stream.WriteByte(ReturnCode);
        }
    }
}
