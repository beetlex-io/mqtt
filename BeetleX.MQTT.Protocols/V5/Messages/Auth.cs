
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using BeetleX.MQTT.Protocols;
using BeetleX.MQTT.Protocols.V5.Headers;
namespace BeetleX.MQTT.Protocols.V5.Messages
{
    public class Auth : MQTTMessage
    {
        public override MQTTMessageType Type => MQTTMessageType.Auth;

        public ReturnType Status { get; set; }

        public AuthenticationMethod AuthenticationMethod { get; set; }

        public AuthenticationData AuthenticationData { get; set; }

        public ReasonString ReasonString { get; set; }

        public List<UserProperty> UserProperties { get; set; }

        protected override void OnRead(MQTTParse parse, Stream stream, ISession session)
        {
            base.OnRead(parse, stream, session);
            Status = (ReturnType)stream.ReadByte();
            var ps = GetPropertiesStream();
            ps.Read(parse, stream);
            AuthenticationMethod = ps;
            AuthenticationData = ps;
            ReasonString = ps;
            UserProperties = ps;
        }

        protected override void OnWrite(MQTTParse parse, Stream stream, ISession session)
        {
            base.OnWrite(parse, stream, session);
            stream.WriteByte((byte)Status);
            var ps = GetPropertiesStream()
                + AuthenticationMethod
                + AuthenticationData
                + ReasonString
                + UserProperties;

        }
    }
}
