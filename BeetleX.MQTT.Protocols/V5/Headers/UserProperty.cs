using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace BeetleX.MQTT.Protocols.V5.Headers
{
    [Header(HeaderType.UserProperty)]
    public class UserProperty : IHeaderExpend<string>
    {
        public string Value { get; set; }

        public string Name { get; set; }

        public HeaderType Type => HeaderType.UserProperty;

        public void Read(MQTTParse mqttParse, Stream stream)
        {
            Name = mqttParse.ReadString(stream);
            Value = mqttParse.ReadString(stream);
        }

        public void Write(MQTTParse mqttParse, Stream stream)
        {
            mqttParse.WriteString(stream, Name);
            mqttParse.WriteString(stream, Value);
        }

        public static implicit operator UserProperty(PropertyStream b) => b.To<UserProperty>(HeaderType.UserProperty);
    }
}
