using BeetleX.MQTT.Protocols;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace BeetleX.MQTT.Protocols.V3.Messages
{
    public class ConnAck : MQTTMessage
    {
        public override MQTTMessageType Type => MQTTMessageType.ConnectAck;

        public byte ConnectFlags { get; set; } = 0x1;

        public ConnectStatus Status { get; set; } = ConnectStatus.Success;

        protected override void OnRead(MQTTParse parse, Stream stream, ISession session)
        {
            base.OnRead(parse, stream, session);
            ConnectFlags = (byte)stream.ReadByte();
            Status = (ConnectStatus)stream.ReadByte();
        }

        protected override void OnWrite(MQTTParse parse, Stream stream, ISession sessioni)
        {
            base.OnWrite(parse, stream, sessioni);
            stream.WriteByte(ConnectFlags);
            stream.WriteByte((byte)Status);
        }
    }
    public enum ConnectStatus
    {
        Success = 0x00,
        NotSupport = 0x01,
        InvalidClientID = 0x02,
        ServerUnavailable = 0x03,
        BadNameOrPassword = 0x04
    }

}
