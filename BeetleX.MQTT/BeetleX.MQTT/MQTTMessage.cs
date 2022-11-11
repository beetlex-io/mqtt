using System;
using System.Collections.Generic;
using System.Text;

namespace BeetleX.MQTT
{
    public abstract class MQTTMessage
    {

        public MQTTMessage()
        {

        }

        public abstract MessageType Type { get; }

        public bool DUP { get; set; }

        public QoSType QoS { get; set; } = QoSType.MostOnce;

        public bool Retain { get; set; }

        public virtual void Read(System.IO.Stream stream, ISession session)
        {
            int a = 0b1111_0000;
        }

        public virtual void Write(System.IO.Stream stream, ISession session)
        {

        }

    }
}
