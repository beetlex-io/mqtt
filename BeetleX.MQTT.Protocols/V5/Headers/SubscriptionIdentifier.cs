using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace BeetleX.MQTT.Protocols.V5.Headers
{
    [Header(HeaderType.SubscriptionIdentifier)]
    public class SubscriptionIdentifier : IHeaderPropertyExpend<int>
    {
        public int Value { get; set; }

        public HeaderType Type => HeaderType.SubscriptionIdentifier;

        public void Read(MQTTParse mqttParse, Stream stream)
        {
            Value = mqttParse.Int7BitHandler.Read(stream).Value;
        }

        public void Write(MQTTParse mqttParse, Stream stream)
        {
            mqttParse.Int7BitHandler.Write(stream, Value);
        }

        public static implicit operator SubscriptionIdentifier(PropertyStream b) => b.To<SubscriptionIdentifier>(HeaderType.SubscriptionIdentifier);
    }
}
