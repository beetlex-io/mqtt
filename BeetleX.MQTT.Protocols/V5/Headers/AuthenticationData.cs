using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace BeetleX.MQTT.Protocols.V5.Headers
{
    [Header(HeaderType.AuthenticationData)]
    public class AuthenticationData : IHeaderPropertyExpend<byte[]>
    {
        public byte[] Value { get; set; }

        public HeaderType Type => HeaderType.AuthenticationData;

        public void Read(MQTTParse mqttParse, Stream stream)
        {
            var len = mqttParse.ReadUInt16(stream);
            Value = new byte[len];
            stream.Read(Value, 0, len);
        }

        public void Write(MQTTParse mqttParse, Stream stream)
        {
            mqttParse.WriteUInt16(stream, (ushort)Value.Length);
            stream.Write(Value, 0, Value.Length);
        }
        public static implicit operator AuthenticationData(PropertyStream b) => b.To<AuthenticationData>(HeaderType.AuthenticationData);
    }
}
