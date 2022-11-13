using System;
using System.Collections.Generic;
using System.Text;

namespace BeetleX.MQTT.Messages
{
    public class CONNACK : MQTTMessage
    {
        public override MQTTMessageType Type => MQTTMessageType.CONNACK;

        public byte ConnectFlags { get; set; } = 1;

        public byte ReturnCode { get; set; } = 0;
    }
}
