﻿using System;
using System.Collections.Generic;
using System.Text;

namespace BeetleX.MQTT.Messages
{
    public class PINGREQ:MQTTMessage
    {
        public override MQTTMessageType Type => MQTTMessageType.PINGREQ;
    }
}
