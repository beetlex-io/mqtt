using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace BeetleX.MQTT.Protocols.V5.Headers
{
    [Header(HeaderType.RetainAvailable)]
    public class RetainAvailable : IHeaderExpend<byte>
    {
        public byte Value { get; set; }

        public HeaderType Type => HeaderType.RetainAvailable;

        public void Read(MQTTParse mqttParse, Stream stream)
        {
            Value = (byte)stream.ReadByte();
        }

        public void Write(MQTTParse mqttParse, Stream stream)
        {
            stream.WriteByte(Value);
        }

        public static implicit operator RetainAvailable(PropertyStream b) => b.To<RetainAvailable>(HeaderType.RetainAvailable);
    }
}
