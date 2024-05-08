using BeetleX.MQTT.Protocols;
using BeetleX.Redis.Commands;
using Microsoft.EntityFrameworkCore.Query.Internal;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace BeetleX.MQTT.Server
{
    public class SubscriptionMapper
    {

        private System.Collections.Concurrent.ConcurrentDictionary<string, TopicItem> mTopicTable = new System.Collections.Concurrent.ConcurrentDictionary<string, TopicItem>(StringComparer.OrdinalIgnoreCase);

        private long mVersion = 0;

        private System.Collections.Concurrent.ConcurrentDictionary<string, MatchItem> mMatchCached = new System.Collections.Concurrent.ConcurrentDictionary<string, MatchItem>(StringComparer.OrdinalIgnoreCase);

        private TopicItem GetTopic(string url)
        {
            if (!mTopicTable.TryGetValue(url, out var users))
            {
                users = new TopicItem(url);
                mTopicTable[url] = users;
            }
            return users;
        }

        public object ListTopicRps()
        {

            return from a in mTopicTable.Values select new { a.Url, Data = a.GetRPS(), Users = a.ListUserStatus(), Extend = false };
        }

        private MatchItem SearchSubscriber(string topic)
        {
            mMatchCached.TryGetValue(topic, out MatchItem match);
            if (match != null && match.Version == mVersion)
                return match;
            lock (mTopicTable)
            {
                match = new MatchItem();
                match.Topic = topic;
                foreach (var value in mTopicTable.Values)
                {
                    if (value.Match(topic))
                    {
                        match.SubTopic.Add(value.Url);
                    }
                }
                match.Version = mVersion;
                mMatchCached[topic] = match;
                return match;
            }
        }

        public void Register(List<Subscription> urls, MQTTUser user)
        {
            lock (mTopicTable)
            {
                foreach (var item in urls)
                {
                    user.AddSubscription(item);
                    var topic = GetTopic(item.TopicFilter);
                    if (topic.Users.Find(p => p.Info.ID == user.ID) == null)
                    {
                        topic.Users.Add(new TopicItem.TopicUser(user));
                    }
                }
                System.Threading.Interlocked.Increment(ref mVersion);
            }
        }

        public void Remove(List<string> tops, MQTTUser user)
        {
            lock (mTopicTable)
            {
                foreach (var item in tops)
                {
                    user.RemoveSubscription(item);
                    var topic = GetTopic(item);
                    topic.Users.RemoveAll(p => p.Info.ID == user.ID);
                }
                System.Threading.Interlocked.Increment(ref mVersion);
            }
        }

        public void Publish(IPublish data, MQTTApplication app)
        {
            var match = SearchSubscriber(data.Topic);
            if (match != null)
            {
                foreach (string subtopic in match.SubTopic)
                {
                    var topic = GetTopic(subtopic);

                    foreach (var item in topic.Users)
                    {
                        if (!item.Info.Disconnected)
                        {
                            app.AddDistributionQuantity();
                            topic.AddDistributionQuantity();
                            item.AddDistributionQuantity();
                            item.Info.AddDistributionQuantity();
                            item.Info.Send((MQTTMessage)data);
                        }
                    }
                }
            }
        }

        public class MatchItem
        {
            public string Topic { get; set; }
            public List<string> SubTopic { get; set; } = new List<string>();

            public long Version { get; set; }
        }

        public class TopicItem
        {

            public TopicItem(string url)
            {
                url = url.ToLower();
                Url = url;
                UrlValues = url.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
                if (url.IndexOf('+') >= 0)
                    Type = TopicType.OneLayer;
                if (url.IndexOf('#') >= 0)
                    Type = TopicType.MoreLayer;
            }

            private RpsCounter mRPSCounter = new RpsCounter();

            public string Url { get; set; }

            public List<TopicUser> Users { get; set; } = new List<TopicUser>();

            public string[] UrlValues { get; set; }

            public TopicType Type { get; set; } = TopicType.Full;

            public bool Match(string url)
            {
                url = url.ToLower();
                if (Type == TopicType.Full)
                    return string.Compare(url, Url, false) == 0;
                if (Type == TopicType.MoreLayer)
                {
                    return url.IndexOf(Url.Substring(0, Url.Length - 1)) == 0;
                }
                var values = url.Split('/', StringSplitOptions.RemoveEmptyEntries);
                if (values.Length == UrlValues.Length)
                {
                    int count = 0;
                    for (int i = 0; i < values.Length; i++)
                    {
                        if (values[i] != UrlValues[i])
                            count++;
                    }
                    return count == 1;
                }
                return false;
            }

            public enum TopicType
            {
                Full,
                OneLayer,
                MoreLayer
            }

            private long mDistributionQuantity = 0;

            public long DistributionQuantity => mDistributionQuantity;


            public void AddDistributionQuantity(int value = 1)
            {
                System.Threading.Interlocked.Add(ref mDistributionQuantity, value);
            }
            public RpsCounter.Value GetRPS()
            {
                return mRPSCounter.Next(mDistributionQuantity);
            }

            public object ListUserStatus()
            {
                return from a in Users select new { RPS = a.GetRPS(), a.Info.ID, a.Info.RemoteIP, a.Info.Disconnected };
            }



            public class TopicUser
            {
                public TopicUser(MQTTUser user)
                {
                    Info = user;
                }
                public MQTTUser Info { get; set; }
                public long DistributionQuantity => mDistributionQuantity;

                private RpsCounter mRPSCounter = new RpsCounter();

                private long mDistributionQuantity = 0;

                public override bool Equals(object obj)
                {
                    return this.Info.ID == ((TopicUser)obj).Info.ID;
                }

                public override int GetHashCode()
                {
                    return this.Info.GetHashCode();
                }

                public void AddDistributionQuantity(int value = 1)
                {
                    System.Threading.Interlocked.Add(ref mDistributionQuantity, value);
                }
                public RpsCounter.Value GetRPS()
                {
                    return mRPSCounter.Next(mDistributionQuantity);
                }
            }
        }
    }
}
