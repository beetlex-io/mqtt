using BeetleX.FastHttpApi;
using BeetleX.MQTT.Protocols;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeetleX.MQTT.Server
{

    public class NetApplication : IApplication
    {
        public void Init(IServer server)
        {

        }
    }
    public class NetUser : ISessionToken
    {

        public string ClientID { get; set; }
        public void Dispose()
        {

        }

        public void Init(IServer server, ISession session)
        {

        }
    }

    public class MQTTUser
    {
        public MQTTUser()
        {

        }
        public string ID { get; set; }

        public string UserName { get; set; }

        public string Password { get; set; }

        public string Category { get; set; }

        public bool NodeClient { get; set; }

        public HttpRequest WebRequest { get; internal set; }

        public override int GetHashCode()
        {
            return this.ID.GetHashCode();
        }

        public bool IsWebSocket => WebRequest != null ? true : false;

        public override bool Equals(object obj)
        {
            return ((MQTTUser)obj).ID == this.ID;
        }

        public ISession NetSession { get; internal set; }

        private long mNumberOfPush;

        public long NumberOfPush => mNumberOfPush;

        public void AddNumberOfPush()
        {
            System.Threading.Interlocked.Increment(ref mNumberOfPush);
        }

        private long mNumberOfSubscribe;

        public long NumberOfSubscribe => mNumberOfSubscribe;


        public void AddDistributionQuantity()
        {
            System.Threading.Interlocked.Increment(ref mNumberOfSubscribe);
        }

        public virtual void Send(MQTTMessage msg)
        {

            if (WebRequest != null)
            {
                var frame = WebRequest.Server.CreateBinaryFrame(msg);
                NetSession?.Send(frame);
            }
            else
            {
                NetSession?.Send(msg);
            }
        }

        public List<Subscription> Subscriptions { get; set; } = new List<Subscription>();

        public void AddSubscription(Subscription item)
        {

            if (!Subscriptions.Contains(item))
                Subscriptions.Add(item);
        }

        public void RemoveSubscription(string topicFilter)
        {
            Subscriptions.RemoveAll(p => p.TopicFilter == topicFilter);
        }
        public void RemoveSubscription(Subscription item)
        {
            Subscriptions.Remove(item);
        }

        public bool Authenticated { get; set; }

        public DateTime LastActiveTime { get; set; }

        public void UpdataActiveTime()
        {

            LastActiveTime = DateTime.Now;
        }

        public string RemoteIP { get; set; }

        public ISession GetNetSession()
        {
            var session = WebRequest != null ? WebRequest.Session : NetSession;
            return session;
        }

        public System.Net.EndPoint ClientEndPoint { get; set; }

        public virtual void Init(IServer server, ISession session)
        {

            server.GetLoger(EventArgs.LogType.Info)?.Log(EventArgs.LogType.Info, session, "session data init");
            NetSession = session;
        }
        private long mPublishCount = 0;

        public long PublishCount => mPublishCount;

        public void PublicshIncrement()
        {
            System.Threading.Interlocked.Increment(ref mPublishCount);
        }
        public void Close()
        {
            Disconnected = true;
            NetSession?.Dispose();
            NetSession = null;
            WebRequest?.Session?.Dispose();
            WebRequest = null;
        }

        public RpsLimit RpsLimit { get; private set; } = new RpsLimit(50);

        public bool Disconnected { get; set; }

    }
}
