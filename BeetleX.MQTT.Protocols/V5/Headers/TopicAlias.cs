using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace BeetleX.MQTT.Protocols.V5.Headers
{
    [Header(HeaderType.TopicAlias)]
    public class TopicAlias : IHeaderPropertyExpend<ushort>
    {
        public ushort Value { get; set; }

        public HeaderType Type => HeaderType.TopicAlias;

        public void Read(MQTTParse mqttParse, Stream stream)
        {
            Value = mqttParse.ReadUInt16(stream);
        }

        public void Write(MQTTParse mqttParse, Stream stream)
        {
            mqttParse.WriteUInt16(stream, Value);
        }

        public static implicit operator TopicAlias(PropertyStream b) => b.To<TopicAlias>(HeaderType.TopicAlias);
    }
}
