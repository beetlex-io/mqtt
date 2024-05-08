using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace BeetleX.MQTT.Protocols.V5.Headers
{
    [Header(HeaderType.PayloadFormatIndicator)]
    public class PayloadFormatIndicator : IHeaderExpend<byte>
    {
        public byte Value { get; set; }

        public HeaderType Type => HeaderType.PayloadFormatIndicator;

        public void Read(MQTTParse mqttParse, Stream stream)
        {
            Value = (byte)stream.ReadByte();
        }

        public void Write(MQTTParse mqttParse, Stream stream)
        {
            stream.WriteByte(Value);
        }
        public static implicit operator PayloadFormatIndicator(PropertyStream b) => b.To<PayloadFormatIndicator>(HeaderType.PayloadFormatIndicator);
    }
}
