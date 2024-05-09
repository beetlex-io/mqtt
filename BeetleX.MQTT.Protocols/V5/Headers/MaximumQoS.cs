using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace BeetleX.MQTT.Protocols.V5.Headers
{
    [Header(HeaderType.MaximumQoS)]
    public class MaximumQoS : IHeaderPropertyExpend<byte>
    {
        public byte Value { get; set; }

        public HeaderType Type => HeaderType.MaximumQoS;

        public void Read(MQTTParse mqttParse, Stream stream)
        {
            Value = (byte)stream.ReadByte();
        }

        public void Write(MQTTParse mqttParse, Stream stream)
        {
            stream.WriteByte(Value);
        }

        public static implicit operator MaximumQoS(PropertyStream b) => b.To<MaximumQoS>(HeaderType.MaximumQoS);
    }
}
