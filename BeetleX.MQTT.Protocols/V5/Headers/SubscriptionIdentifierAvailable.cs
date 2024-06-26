﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace BeetleX.MQTT.Protocols.V5.Headers
{
    [Header(HeaderType.SubscriptionIdentifierAvailable)]
    public class SubscriptionIdentifierAvailable : IHeaderPropertyExpend<byte>
    {
        public byte Value { get; set; }

        public HeaderType Type => throw new NotImplementedException();

        public void Read(MQTTParse mqttParse, Stream stream)
        {
            Value = (byte)stream.ReadByte();
        }

        public void Write(MQTTParse mqttParse, Stream stream)
        {
            stream.WriteByte(Value);
        }

        public static implicit operator SubscriptionIdentifierAvailable(PropertyStream b) => b.To<SubscriptionIdentifierAvailable>(HeaderType.SubscriptionIdentifierAvailable);
    }
}
