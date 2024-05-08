using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace BeetleX.MQTT.Protocols.V5.Headers
{
    [Header(HeaderType.MessageExpiryInterval)]
    public class MessageExpiryInterval : IHeaderExpend<int>
    {
        public int Value { get; set; }

        public HeaderType Type => HeaderType.MessageExpiryInterval;

        public void Read(MQTTParse mqttParse, Stream stream)
        {
            Value = mqttParse.ReadInt(stream);
        }

        public void Write(MQTTParse mqttParse, Stream stream)
        {
            mqttParse.WriteInt(stream, Value);
        }

        public static implicit operator MessageExpiryInterval(PropertyStream b) => b.To<MessageExpiryInterval>(HeaderType.MessageExpiryInterval);
    }
}
