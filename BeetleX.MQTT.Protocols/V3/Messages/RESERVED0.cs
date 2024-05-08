using System;
using System.Collections.Generic;
using System.Text;

namespace BeetleX.MQTT.Protocols.V3.Messages
{
    public class RESERVED0 : MQTTMessage
    {
        public override MQTTMessageType Type => MQTTMessageType.Reserved0;
    }

    public class RESERVED15 : MQTTMessage
    {
        public override MQTTMessageType Type => MQTTMessageType.Auth;
    }
}
