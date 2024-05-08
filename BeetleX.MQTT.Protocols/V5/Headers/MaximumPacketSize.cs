using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace BeetleX.MQTT.Protocols.V5.Headers
{
    [Header(HeaderType.MaximumPacketSize)]
    public class MaximumPacketSize : IHeaderExpend<int>
    {
        public int Value { get; set; }

        public HeaderType Type => HeaderType.MaximumPacketSize;

        public void Read(MQTTParse mqttParse, Stream stream)
        {
            Value = mqttParse.ReadInt(stream);
        }

        public void Write(MQTTParse mqttParse, Stream stream)
        {
            mqttParse.WriteInt(stream, Value);
        }

        public static implicit operator MaximumPacketSize(PropertyStream b) => b.To<MaximumPacketSize>(HeaderType.MaximumPacketSize);
    }
}
