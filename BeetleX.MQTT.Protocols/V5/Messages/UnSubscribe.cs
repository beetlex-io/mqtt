using BeetleX.MQTT.Protocols.V5.Headers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace BeetleX.MQTT.Protocols.V5.Messages
{
    public class UnSubscribe : MQTTMessage
    {
        public UnSubscribe()
        {
            Bit2 = 1;
        }


        protected override void OnRead(MQTTParse parse, Stream stream, ISession session)
        {
            base.OnRead(parse, stream, session);
            Identifier = parse.ReadUInt16(stream);
            var ps = GetPropertiesStream();
            ps.Read(parse, stream);
            UserProperties = ps;
            while (stream.Position < stream.Length)
            {
                string value = parse.ReadString(stream);
                Subscribers.Add(value);
            }
        }

        protected override void OnWrite(MQTTParse parse, Stream stream, ISession session)
        {
            base.OnWrite(parse, stream, session);
            parse.WriteUInt16(stream, Identifier);
            var ps = GetPropertiesStream() + UserProperties;
            ps.Write(parse, stream);
            foreach (var item in Subscribers)
            {
                parse.WriteString(stream, item);
            }
        }

        public ushort Identifier { get; set; }
        public List<UserProperty> UserProperties { get; set; }

        public override MQTTMessageType Type => MQTTMessageType.UnSubscribe;

        public List<string> Subscribers { get; set; } = new List<string>();
    }
}
