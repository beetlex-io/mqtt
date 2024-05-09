using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace BeetleX.MQTT.Protocols.V5.Headers
{
    [Header(HeaderType.ResponseInformation)]
    public class ResponseInformation : IHeaderPropertyExpend<string>
    {
        public string Value { get; set; }

        public HeaderType Type => HeaderType.ResponseInformation;

        public void Read(MQTTParse mqttParse, Stream stream)
        {
            Value = mqttParse.ReadString(stream);
        }

        public void Write(MQTTParse mqttParse, Stream stream)
        {
            mqttParse.WriteString(stream, Value);
        }

        public static implicit operator ResponseInformation(PropertyStream b) => b.To<ResponseInformation>(HeaderType.ResponseInformation);

    }
}
