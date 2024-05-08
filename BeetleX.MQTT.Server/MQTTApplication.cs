using BeetleX.FastHttpApi;
using BeetleX.MQTT.Protocols;
using BeetleX.MQTT.Server.Storages;
using BeetleX.WebFamily;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace BeetleX.MQTT.Server
{
    public abstract class MQTTApplication : IApplication
    {


        public MQTTApplication() { }

        public string APPID { get; set; }

        public abstract string Name { get; }

        public System.Collections.Concurrent.ConcurrentDictionary<string, MQTTUser> Clients { get; private set; }
        = new System.Collections.Concurrent.ConcurrentDictionary<string, MQTTUser>(StringComparer.OrdinalIgnoreCase);

        private SubscriptionMapper mSubscriptionMapper = new SubscriptionMapper();

        public SubscriptionMapper SubscriptionMapper => mSubscriptionMapper;

        private long mDistributionQuantity = 0;

        private long mReceiveQuantity = 0;

        public long DistributionQuantity => mDistributionQuantity;

        public long ReceiveQuantity => mReceiveQuantity;

        public void AddDistributionQuantity(int value = 1)
        {
            System.Threading.Interlocked.Add(ref mDistributionQuantity, value);
        }
        public void AddReceiveQuantity(int value = 1)
        {
            System.Threading.Interlocked.Add(ref mReceiveQuantity, value);
        }

        public object ListOnlines(int page, int size)
        {
            int count = Clients.Count;
            var items = from a in Clients.Values.Skip(page * size).Take(size).ToList()
                        select new
                        {
                            a.ID,
                            a.UserName,
                            a.NodeClient,
                            a.RemoteIP,
                            LastActiveTime = a.LastActiveTime.ToString("yyyy-MM-dd  HH:mm:ss"),
                            Enabled = (DateTime.Now - a.LastActiveTime).TotalSeconds < 60,
                            a.NumberOfPush,
                            a.NumberOfSubscribe,
                            Subscription = string.Join("", (from i in a.Subscriptions select i.ToString()).ToArray()),
                            a.Category
                        };
            return new
            {
                count,
                items
            };

        }


        public void SetUser(MQTTUser user)
        {
            Clients[user.ID] = user;
        }

        public virtual void RegisterSubscribe(List<Subscription> subscription, MQTTUser user)
        {
            mSubscriptionMapper.Register(subscription, user);
        }


        public MQTTUser GetUser(string id)
        {
            Clients.TryGetValue(id, out MQTTUser user);
            return user;
        }

        public Func<string, MQTTUser> FindUser { get; set; }

        public virtual void UnRegisterSubscribe(List<string> subscription, MQTTUser user)
        {

            mSubscriptionMapper.Remove(subscription, user);
        }

        public bool Publish(IPublish msg)
        {
            mSubscriptionMapper.Publish(msg, this);
            return true;
        }


        public HttpApiServer Http { get; internal set; }

        public IServer TCP { get; internal set; }

        public HttpApiServer GetLoger(EventArgs.LogType type)
        {
            return Http.GetLog(type);
        }


        private long mNumberOfPush;

        public long NumberOfPush => mNumberOfPush;

        public void AddNumberOfPush()
        {
            System.Threading.Interlocked.Increment(ref mNumberOfPush);
        }


        public string SystemUserName { get; set; } = "admin";

        public string SystemPassword { get; set; } = "123456";

        public int SessionPublishLimit { get; set; } = 50;

        public bool SessionVerify { get; set; } = true;

        private void ChangeOptions(Storages.Options[] options)
        {

            var item = options.FirstOrDefault(p => p.Name == "SystemUserName");
            if (item != null)
                SystemUserName = item.Value;
            item = options.FirstOrDefault(p => p.Name == "SystemPassword");
            if (item != null)
                SystemPassword = item.Value;

            item = options.FirstOrDefault(p => p.Name == "SessionPublishLimit");
            if (item != null)
                SessionPublishLimit = int.Parse(item.Value);

            item = options.FirstOrDefault(p => p.Name == "SessionVerify");
            if (item != null)
                SessionVerify = item.Value == "True";

        }

        public void LoadOptions()
        {
            using (MQTTDB db = new MQTTDB())
            {
                ChangeOptions((from a in db.Options select a).ToArray());
            }
        }

        public Task<LoginResult> Login(string user, string pwd, IHttpContext context)
        {
            LoginResult result;
            if (user == SystemUserName && pwd == SystemPassword)
            {
                result = new LoginResult(user, "admin");

            }
            else
            {
                result = new LoginResult("用户或密码不正确!");
            }
            return Task.FromResult(result);
        }

        public virtual void Init(IServer server)
        {
            GetLoger(EventArgs.LogType.Info)?.Log(EventArgs.LogType.Info, null, "MQTT appliction initialize!");
            LoadOptions();
        }

        public virtual void Reply(MQTTMessage msg, HttpRequest websocket, ISession tcp)
        {
            if (websocket != null)
            {
                var frame = websocket.Server.CreateBinaryFrame(msg);
                websocket.Session.Send(frame);
            }
            else
            {
                tcp.Send(msg);
            }
        }

        public abstract void Receive(EventMessageReceiveArgs<NetApplication, NetUser, object> e);

        public async void Disconnect(ISession session, int delay = 3000)
        {
            try
            {
                GetLoger(EventArgs.LogType.Info)?.Log(EventArgs.LogType.Info, session, "Disconnect!");
                await Task.Delay(delay);
                session?.Dispose();

            }
            catch
            {

            }
        }
        public async void Disconnect(MQTTUser user, int delay = 2000)
        {
            try
            {
                GetLoger(EventArgs.LogType.Info)?.Log(EventArgs.LogType.Info, user.GetNetSession(), $"client {user.ID} disconnect!");
                await Task.Delay(delay);
                user.Close();
            }
            catch
            {

            }
        }
    }
}
