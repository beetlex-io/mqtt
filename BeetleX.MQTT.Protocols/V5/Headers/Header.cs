using System;
using System.Collections.Generic;
using System.Text;

namespace BeetleX.MQTT.Protocols.V5.Headers
{
    public interface IHeaderProperty
    {
        HeaderType Type { get; }

        void Read(MQTTParse mqttParse, System.IO.Stream stream);

        void Write(MQTTParse mqttParse, System.IO.Stream stream);
    }

    public interface IHeaderExpend<T> : IHeaderProperty
    {
        T Value { get; set; }

        
    }
}
