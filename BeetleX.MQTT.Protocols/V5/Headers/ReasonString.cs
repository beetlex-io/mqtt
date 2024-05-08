using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace BeetleX.MQTT.Protocols.V5.Headers
{
    [Header(HeaderType.ReasonString)]
    public class ReasonString : IHeaderExpend<string>
    {
        public string Value { get; set; }

        public HeaderType Type => HeaderType.ReasonString;

        public void Read(MQTTParse mqttParse, Stream stream)
        {
            Value = mqttParse.ReadString(stream);
        }

        public void Write(MQTTParse mqttParse, Stream stream)
        {
            mqttParse.WriteString(stream, Value);
        }
        public static implicit operator ReasonString(PropertyStream b) => b.To<ReasonString>(HeaderType.ReasonString);
    }
}
