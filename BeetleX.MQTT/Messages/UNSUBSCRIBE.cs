using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace BeetleX.MQTT.Messages
{
    public class UNSUBSCRIBE : MQTTMessage
    {
        public override MQTTMessageType Type => MQTTMessageType.UNSUBSCRIBE;

        public ushort Identifier { get; set; }

        protected override void OnRead(Stream stream, ISession session)
        {
            base.OnRead(stream, session);
            Identifier = ReadUInt16(stream);
            while (stream.Position < stream.Length)
            {
                string value = ReadString(stream);
                Subscription.Add(value);
            }
        }

        protected override void OnWrite(Stream stream, ISession sessioni)
        {
            base.OnWrite(stream, sessioni);
            WriteUInt16(stream, Identifier);
            foreach (var item in Subscription)
            {
                WriteString(stream, item);
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
