using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace BeetleX.MQTT
{
    public abstract class MQTTMessage
    {

        public MQTTMessage()
        {

        }

        public abstract MQTTMessageType Type { get; }

        public bool DUP { get; set; }

        public QoSType QoS { get; set; } = QoSType.MostOnce;

        public bool Retain { get; set; }


        internal void Read(System.IO.Stream stream, ISession session)
        {
            OnRead(stream, session);
        }

        protected virtual void OnRead(Stream stream, ISession session)
        {

        }

        internal void Write(System.IO.Stream stream, ISession session)
        {
            OnWrite(stream, session);
        }

        protected virtual void OnWrite(Stream stream, ISession sessioni)
        {

        }

    }
}
