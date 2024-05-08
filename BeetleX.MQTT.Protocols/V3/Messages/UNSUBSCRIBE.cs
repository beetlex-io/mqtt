using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace BeetleX.MQTT.Protocols.V3.Messages
{
    public class UnSubscribe : MQTTMessage
    {
        public override MQTTMessageType Type => MQTTMessageType.UnSubscribe;

        public ushort Identifier { get; set; }

        protected override void OnRead(MQTTParse parse, Stream stream, ISession session)
        {
            base.OnRead(parse, stream, session);
            Identifier = parse.ReadUInt16(stream);
            while (stream.Position < stream.Length)
            {
                string value = parse.ReadString(stream);
                Subscription.Add(value);
            }
        }

        protected override void OnWrite(MQTTParse parse, Stream stream, ISession sessioni)
        {
            base.OnWrite(parse, stream, sessioni);
            parse.WriteUInt16(stream, Identifier);
            foreach (var item in Subscription)
            {
                parse.WriteString(stream, item);
            }
        }

        public List<string> Subscription { get; set; } = new List<string>();

        public override string ToString()
        {
            string result = "";
            foreach (var item in Subscription)
            {
                result += $"[{item}]";
            }
            return result;
        }
    }
}
