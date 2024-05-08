using System;
using System.Collections.Generic;
using System.Text;

namespace BeetleX.MQTT.Protocols
{
    public enum MQTTMessageType
    {
        Reserved0 = 0,
        Connect = 1,
        ConnectAck = 2,
        Publish = 3,
        PubAck = 4,
        PubRec = 5,
        PubRel = 6,
        PubComp = 7,
        Subscribe = 8,
        SubAck = 9,
        UnSubscribe = 10,
        UnSubAck = 11,
        PingReq = 12,
        PingResp = 13,
        Disconnect = 14,
        Auth = 15
    }
}
