using BeetleX.MQTT.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeetleX.MQTT.WSServer
{
    public class TopicSubscriber
    {
        public List<SubscriberItem> Subscribers { get; private set; } = new List<SubscriberItem>();

        public void Add(MQTTUser user, Subscription subscription)
        {
            lock (Subscribers)
            {
                if (Subscribers.FirstOrDefault(p => p.User == user) == null)
                {

                    Subscribers.Add(new SubscriberItem { User = user, Subscription = subscription });
                }
            }
        }

        public void Remove(MQTTUser user)
        {
            lock (Subscribers)
            {
                Subscribers.RemoveAll(p => p.User == user);
            }
        }

        public void Publish(PUBLISH msg)
        {

            lock (Subscribers)
            {
                foreach (var item in Subscribers)
                {
                    item.Publish(msg);
                }
            }
        }

        public class SubscriberItem
        {

            public override int GetHashCode()
            {
                return this.User.ID.GetHashCode();
            }

            public override bool Equals(object obj)
            {
                return ((SubscriberItem)obj).User.ID == this.User.ID;
            }

            public MQTTUser User { get; set; }

            public Subscription Subscription { get; set; }

            public void Publish(PUBLISH msg)
            {
                User.Send(msg);
            }
        }
    }
}
