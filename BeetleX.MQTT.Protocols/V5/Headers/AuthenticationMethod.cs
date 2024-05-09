using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace BeetleX.MQTT.Protocols.V5.Headers
{
    [Header(HeaderType.AuthenticationMethod)]
    public class AuthenticationMethod : IHeaderPropertyExpend<string>
    {
        public string Value { get; set; }

        public HeaderType Type => HeaderType.AuthenticationMethod;

        public void Read(MQTTParse mqttParse, Stream stream)
        {
            Value = mqttParse.ReadString(stream);
        }

        public void Write(MQTTParse mqttParse, Stream stream)
        {
            mqttParse.WriteString(stream, Value);
        }
        public static implicit operator AuthenticationMethod(PropertyStream b) => b.To<AuthenticationMethod>( HeaderType.AuthenticationMethod);
    }
}
