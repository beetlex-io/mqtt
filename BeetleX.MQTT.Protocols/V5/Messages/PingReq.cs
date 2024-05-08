using System;
using System.Collections.Generic;
using System.Text;

namespace BeetleX.MQTT.Protocols.V5.Messages
{
    public class PingReq : MQTTMessage
    {
        public override MQTTMessageType Type => MQTTMessageType.PingReq;
    }
}
