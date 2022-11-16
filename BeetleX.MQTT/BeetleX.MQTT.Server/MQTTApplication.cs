using BeetleX.MQTT.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeetleX.MQTT.Server
{
    public class MQTTApplication : IApplication
    {

        public System.Collections.Generic.Dictionary<string, TopicSubscriber> mSubscriberTable = new Dictionary<string, TopicSubscriber>(StringComparer.OrdinalIgnoreCase);


        private TopicSubscriber GetTopicSubscriber(string topic)
        {

            lock (mSubscriberTable)
            {
                if (!mSubscriberTable.TryGetValue(topic, out TopicSubscriber topicSubscriber))
                {
                    topicSubscriber = new TopicSubscriber();
                    mSubscriberTable[topic] = topicSubscriber;
                }
                return topicSubscriber;
            }
        }

        public void RegisterSubscribe(SUBSCRIBE msg, MQTTUser user)
        {
            foreach (var item in msg.Subscriptions)
            {
                var topic = GetTopicSubscriber(item.TopicFilter);
                topic.Add(user, item);
            }
        }

        public void UnRegisterSubscribe(UNSUBSCRIBE msg, MQTTUser user)
        {
            foreach (var item in msg.Subscription)
            {
                GetTopicSubscriber(item)?.Remove(user);
            }
        }

        public void Publish(PUBLISH msg)
        {
            GetTopicSubscriber(msg.Topic)?.Publish(msg);
        }

        public IServer NetServer { get; private set; }

        public void Init(IServer server)
        {
            NetServer = server;
        }
    }
}
