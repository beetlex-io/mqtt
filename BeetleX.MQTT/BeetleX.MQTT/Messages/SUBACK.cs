using System;
using System.Collections.Generic;
using System.Text;

namespace BeetleX.MQTT.Messages
{
    public class SUBACK:MQTTMessage
    {
        public override MQTTMessageType Type => MQTTMessageType.SUBACK;
    }
}
