using System;
using System.Collections.Generic;
using System.Text;

namespace BeetleX.MQTT.Messages
{
    public class PUBCOMP : MQTTMessage
    {
        public override MQTTMessageType Type => MQTTMessageType.PUBCOMP;
    }
}
