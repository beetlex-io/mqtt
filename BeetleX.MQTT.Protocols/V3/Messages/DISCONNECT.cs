using System;
using System.Collections.Generic;
using System.Text;

namespace BeetleX.MQTT.Protocols.V3.Messages
{
    public class Disconnect:MQTTMessage
    {
        public override MQTTMessageType Type => MQTTMessageType.Disconnect;
    }
}
