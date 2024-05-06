using System;
using System.Collections.Generic;
using System.Text;

namespace BeetleX.MQTT
{
    public enum QoSType
    {
        MostOnce = 0x0,
        LeastOnce = 0x1,
        ExactlyOnce = 0x2,
        Reserved = 0x3,
        Failure = 0x80
    }
}
