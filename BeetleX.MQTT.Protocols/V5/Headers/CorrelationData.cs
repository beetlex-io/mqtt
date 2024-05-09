using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace BeetleX.MQTT.Protocols.V5.Headers
{
    [Header(HeaderType.CorrelationData)]
    public class CorrelationData : IHeaderPropertyExpend<byte[]>
    {
        public byte[] Value { get; set; }

        public HeaderType Type => HeaderType.CorrelationData;

        public void Read(MQTTParse mqttParse, Stream stream)
        {
            int len = mqttParse.ReadUInt16(stream);
            Value = new byte[len];
        }

        public void Write(MQTTParse mqttParse, Stream stream)
        {
            mqttParse.WriteUInt16(stream, (ushort)Value.Length);
            stream.Write(Value, 0, Value.Length);
        }

        public static implicit operator CorrelationData(PropertyStream b) => b.To<CorrelationData>(HeaderType.CorrelationData);
    }
}
