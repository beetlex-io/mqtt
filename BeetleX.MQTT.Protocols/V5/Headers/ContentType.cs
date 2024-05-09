using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace BeetleX.MQTT.Protocols.V5.Headers
{
    [Header(HeaderType.ContentType)]
    public class ContentType : IHeaderPropertyExpend<string>
    {
        public string Value { get; set; }

        public HeaderType Type => HeaderType.ContentType;

        public void Read(MQTTParse mqttParse, Stream stream)
        {
            Value = mqttParse.ReadString(stream);
        }

        public void Write(MQTTParse mqttParse, Stream stream)
        {
            mqttParse.WriteString(stream, Value);
        }

        public static implicit operator ContentType(PropertyStream b) => b.To<ContentType>(HeaderType.ContentType);
    }
}
