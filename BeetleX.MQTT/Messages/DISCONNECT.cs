using System;
using System.Collections.Generic;
using System.Text;

namespace BeetleX.MQTT.Messages
{
    public class DISCONNECT:MQTTMessage
    {
        public override MQTTMessageType Type => MQTTMessageType.DISCONNECT;
    }
}
