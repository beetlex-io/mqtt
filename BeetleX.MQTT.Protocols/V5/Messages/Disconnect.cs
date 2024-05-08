using BeetleX.MQTT.Protocols.V5.Headers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace BeetleX.MQTT.Protocols.V5.Messages
{
    public class Disconnect : MQTTMessage
    {
        public override MQTTMessageType Type => MQTTMessageType.Disconnect;

        protected override void OnRead(MQTTParse parse, Stream stream, ISession session)
        {
            base.OnRead(parse, stream, session);
            Status = (ReturnType)stream.ReadByte();
            var ps = GetPropertiesStream();
            ps.Read(parse, stream);
            SessionExpiryInterval = ps;
            ReasonString = ps;
            ServerReference = ps;
            UserProperties = ps;
        }

        protected override void OnWrite(MQTTParse parse, Stream stream, ISession session)
        {
            base.OnWrite(parse, stream, session);
            stream.WriteByte((byte)Status);
            var ps = GetPropertiesStream()
            + UserProperties 
            + SessionExpiryInterval 
            + ReasonString 
            + ServerReference;
        }

        public ReturnType Status { get; set; }

        public SessionExpiryInterval SessionExpiryInterval { get; set; }

        public ReasonString ReasonString { get; set; }

        public ServerReference ServerReference { get; set; }

        public List<UserProperty> UserProperties { get; set; } = new List<UserProperty>();

    }
}
