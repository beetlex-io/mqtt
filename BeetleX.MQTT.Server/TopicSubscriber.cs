using BeetleX.MQTT.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeetleX.MQTT.Server
{
    public class TopicSubscriber
    {


        public List<SubscriberItem> Subscribers { get; private set; } = new List<SubscriberItem>();

        public bool MultiPush { get; set; }

        public void Add(MQTTUser user, Subscription subscription)
        {
            lock (Subscribers)
            {
                if (!MultiPush)
                    Subscribers.Clear();
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

        private long mCount;

        public long NumberOfPush => mCount;

        public bool Publish(PUBLISH msg)
        {
            System.Threading.Interlocked.Increment(ref mCount);
            lock (Subscribers)
            {
                if (Subscribers.Count == 0)
                    return false;
                foreach (var item in Subscribers)
                {
                    item.Publish(msg);

                }
                return true;
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
                User.AddDistributionQuantity();
            }
        }
    }
}
