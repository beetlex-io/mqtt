using System;
using System.Collections.Generic;
using System.Text;

namespace BeetleX.MQTT.Protocols
{
    public class Subscription
    {
        public String TopicFilter { get; set; }

        public override bool Equals(object obj)
        {
            if (obj is Subscription)
                return this.TopicFilter == ((Subscription)obj).TopicFilter;
            return TopicFilter == obj.ToString();
        }

        public override int GetHashCode()
        {
            return TopicFilter.GetHashCode();
        }

        public QoSType QoSType { get; set; }

        public override string ToString()
        {
            return $"[{TopicFilter}|{QoSType}]";
        }

    }
}
