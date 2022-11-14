using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace BeetleX.MQTT.Messages
{
    public class SUBACK : MQTTMessage
    {
        public override MQTTMessageType Type => MQTTMessageType.SUBACK;

        public QoSType Status { get; set; }

        protected override void OnRead(Stream stream, ISession session)
        {
            base.OnRead(stream, session);
            Status = (QoSType)stream.ReadByte();
        }

        protected override void OnWrite(Stream stream, ISession sessioni)
        {
            base.OnWrite(stream, sessioni);
            stream.WriteByte((byte)Status);
        }
    }
}
