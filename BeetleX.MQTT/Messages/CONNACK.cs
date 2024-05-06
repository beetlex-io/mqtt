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

        public ReturnCode ReturnCode { get; set; } = ReturnCode.Connected;

        protected override void OnRead(Stream stream, ISession session)
        {
            base.OnRead(stream, session);
            ConnectFlags = (byte)stream.ReadByte();
            ReturnCode = (ReturnCode)stream.ReadByte();
        }

        protected override void OnWrite(Stream stream, ISession sessioni)
        {
            base.OnWrite(stream, sessioni);
            stream.WriteByte(ConnectFlags);
            stream.WriteByte((byte)ReturnCode);
        }
    }

    public enum ReturnCode : Byte
    {
        Connected = 0x0,
        VersionError = 0x1,
        ClientTagError = 0x2,
        ServerError = 0x3,
        UserInfoError = 0x4,
        NoAccess = 0x5
    }
}
