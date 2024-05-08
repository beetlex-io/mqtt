using System;
using System.Collections.Generic;
using System.Text;

namespace BeetleX.MQTT.Protocols
{
    public interface IPublish
    {

        string Topic { get; set; }

        ushort Identifier { get; set; }

        QoSType QoS { get; set; }

        bool Retain { get; set; }

        bool DUP { get; set; }

        ArraySegment<byte> Payload { get; set; }
    }
}
