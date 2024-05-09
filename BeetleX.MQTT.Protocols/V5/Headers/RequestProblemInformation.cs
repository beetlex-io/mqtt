using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace BeetleX.MQTT.Protocols.V5.Headers
{
    [Header(HeaderType.RequestProblemInformation)]
    public class RequestProblemInformation : IHeaderPropertyExpend<byte>
    {
        public byte Value { get; set; }

        public HeaderType Type => HeaderType.RequestProblemInformation;

        public void Read(MQTTParse mqttParse, Stream stream)
        {
            Value = (byte)stream.ReadByte();
        }

        public void Write(MQTTParse mqttParse, Stream stream)
        {
            stream.WriteByte(Value);
        }

        public static implicit operator RequestProblemInformation(PropertyStream b) => b.To<RequestProblemInformation>(HeaderType.RequestProblemInformation);
    }
}
