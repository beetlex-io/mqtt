using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace BeetleX.MQTT.Protocols.V5.Headers
{
    [Header(HeaderType.WildcardSubscriptionAvailable)]
    public class WildcardSubscriptionAvailable : IHeaderPropertyExpend<byte>
    {
        public byte Value { get; set; }

        public HeaderType Type => HeaderType.WildcardSubscriptionAvailable;

        public void Read(MQTTParse mqttParse, Stream stream)
        {
            Value = (byte)stream.ReadByte();
        }

        public void Write(MQTTParse mqttParse, Stream stream)
        {
            stream.WriteByte(Value);
        }

        public static implicit operator WildcardSubscriptionAvailable(PropertyStream b) => b.To<WildcardSubscriptionAvailable>(HeaderType.WildcardSubscriptionAvailable);
    }
}
