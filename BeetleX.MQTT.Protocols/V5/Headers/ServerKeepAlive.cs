using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace BeetleX.MQTT.Protocols.V5.Headers
{
    [Header(HeaderType.ServerKeepAlive)]
    public class ServerKeepAlive : IHeaderPropertyExpend<ushort>
    {
        public ushort Value { get; set; }

        public HeaderType Type => HeaderType.ServerKeepAlive;

        public void Read(MQTTParse mqttParse, Stream stream)
        {
            Value = mqttParse.ReadUInt16(stream);
        }

        public void Write(MQTTParse mqttParse, Stream stream)
        {
            mqttParse.WriteUInt16(stream, Value);
        }

        public static implicit operator ServerKeepAlive(PropertyStream b) => b.To<ServerKeepAlive>(HeaderType.ServerKeepAlive);
    }
}
