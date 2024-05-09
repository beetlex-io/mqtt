using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace BeetleX.MQTT.Protocols.V5.Headers
{
    [Header(HeaderType.TopicAliasMaximum)]
    public class TopicAliasMaximum : IHeaderPropertyExpend<ushort>
    {
        public ushort Value { get; set; }

        public HeaderType Type => HeaderType.TopicAliasMaximum;

        public void Read(MQTTParse mqttParse, Stream stream)
        {
            Value = mqttParse.ReadUInt16(stream);
        }

        public void Write(MQTTParse mqttParse, Stream stream)
        {
            mqttParse.WriteUInt16(stream, Value);
        }

        public static implicit operator TopicAliasMaximum(PropertyStream b) => b.To<TopicAliasMaximum>(HeaderType.TopicAliasMaximum);
    }
}
