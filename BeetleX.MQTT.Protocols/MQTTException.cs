using System;
using System.Collections.Generic;
using System.Text;

namespace BeetleX.MQTT.Protocols
{
    public class MQTTException : Exception
    {
        public MQTTException(string message) : base(message) { }

        public MQTTException(string message, params object[] parameters) : base(string.Format(message, parameters)) { }

        public MQTTException(string message, Exception baseError) : base(message, baseError) { }

        public MQTTException(Exception baseError, string message, params object[] parameters) : base(string.Format(message, parameters), baseError) { }
    }
}
