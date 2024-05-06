using BeetleX.FastHttpApi;
using BeetleX.MQTT.Messages;
using BeetleX.MQTT.Server.Storages;
using BeetleX.WebFamily;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace BeetleX.MQTT.Server
{
    public class MQTTApplication : IApplication
    {


        public MQTTApplication() { }

        public MQTTApplication(string id) { }

        public string APPID { get; set; }

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

        public virtual void RegisterSubscribe(SUBSCRIBE msg, MQTTUser user)
        {
            mSubscriptionMapper.Register(msg.Subscriptions, user);
        }


        public MQTTUser GetUser(string id)
        {
            Clients.TryGetValue(id, out MQTTUser user);
            return user;
        }

        public Func<string, MQTTUser> FindUser { get; set; }

        public virtual void UnRegisterSubscribe(UNSUBSCRIBE msg, MQTTUser user)
        {

            mSubscriptionMapper.Remove(msg.Subscription, user);
        }

        public bool Publish(PUBLISH msg)
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

        protected virtual void Reply(MQTTMessage msg, HttpRequest websocket, ISession tcp)
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

        public virtual bool OnContent(string clientid, CONNECT e, HttpRequest websocket, ISession tcp)
        {
            bool isNode = false;
            CONNACK ack = new CONNACK();
            string category = "";
            if (string.IsNullOrEmpty(e.UserName))
            {
                ack.ReturnCode = ReturnCode.NoAccess;
                Reply(ack, websocket, tcp);
                GetLoger(EventArgs.LogType.Info)?.Log(EventArgs.LogType.Info, websocket != null ? websocket.Session : tcp, $"{clientid} user notfound");
                return false;
            }
            if (SessionVerify)
            {
                using (MQTTDB context = new MQTTDB())
                {
                    var item = context.User.Find(e.UserName);
                    if (item == null || e.Password != item.PWD || !item.Enabled)
                    {

                        ack.ReturnCode = ReturnCode.UserInfoError;
                        Reply(ack, websocket, tcp);
                        GetLoger(EventArgs.LogType.Info)?.Log(EventArgs.LogType.Info, websocket != null ? websocket.Session : tcp, $"{clientid} invalid username or password");
                        return false;
                    }
                    category = item.Category;
                    isNode = item.IsNodeClient;
                }
            }
            var user = GetUser(e.ClientID);

            if (user != null)
            {
                user.WebRequest?.Session?.Dispose();
                user.NetSession?.Dispose();
            }
            else
            {
                user = new MQTTUser();
                user.ID = e.ClientID;
                SetUser(user);
            }
            user.Disconnected = false;
            user.Category = category;
            ack.ReturnCode = ReturnCode.Connected;
            Reply(ack, websocket, tcp);
            if (websocket == null)
            {
                user.RemoteIP = tcp.RemoteEndPoint.ToString();
                tcp.Name = e.ClientID;
                if (isNode)
                    tcp["node"] = true;
            }
            else
            {
                user.RemoteIP = websocket.RemoteEndPoint.ToString();
                websocket.Session.Name = e.ClientID;
                if (isNode)
                    tcp["node"] = true;
            }
            user.NodeClient = isNode;
            user.WebRequest?.Session?.Dispose();
            user.NetSession?.Dispose();
            user.WebRequest = websocket;
            user.NetSession = tcp;
            GetLoger(EventArgs.LogType.Info)?.Log(EventArgs.LogType.Info, websocket != null ? websocket.Session : tcp, $"{clientid} connected@{(websocket != null ? "WS" : "TCP")}");
            return true;
        }

        public virtual bool OnPublish(MQTTUser client, PUBLISH e)
        {
            GetLoger(EventArgs.LogType.Debug)?
                .Log(EventArgs.LogType.Debug, client.GetNetSession(), $"{client.ID} publish {e.Topic}@ {e.Identifier}");
            GetLoger(EventArgs.LogType.Debug)?
                .Log(EventArgs.LogType.Trace, client.GetNetSession(), $"{client.ID} publish {e.Topic}@ {e.Identifier} data:{BitConverter.ToString(e.PayLoadData.Array, e.PayLoadData.Offset, e.PayLoadData.Count)}");
            this.AddNumberOfPush();
            client.AddNumberOfPush();
            AddReceiveQuantity();
            if (e.QoS == QoSType.LeastOnce)
            {
                PUBACK ack = new PUBACK();
                ack.Identifier = e.Identifier;
                client.Send(ack);
            }
            return Publish(e);
        }

        public virtual void OnSubscribe(MQTTUser client, SUBSCRIBE e)
        {
            GetLoger(EventArgs.LogType.Info)?.Log(EventArgs.LogType.Info, client.GetNetSession(), $"{client.ID} subscribe {e}");

            SUBACK ack = new SUBACK();
            ack.Identifier = e.Identifier;
            ack.Status = QoSType.MostOnce;
            client.Send(ack);
            RegisterSubscribe(e, client);
        }

        public virtual void OnConnAck(MQTTUser client, CONNACK e)
        {

        }

        public virtual void OnDisconnect(MQTTUser client, DISCONNECT e)
        {
            GetLoger(EventArgs.LogType.Info)?
              .Log(EventArgs.LogType.Info, client.GetNetSession(), $"{client.ID} disconnect");
            Disconnect(client);

        }

        public virtual void OnPingREQ(MQTTUser client, PINGREQ e)
        {
            GetLoger(EventArgs.LogType.Info)?
               .Log(EventArgs.LogType.Info, client.GetNetSession(), $"{client.ID} ping");
            PINGRESP resp = new PINGRESP();
            client.Send(resp);
        }

        public virtual void OnPingResp(MQTTUser client, PINGRESP e)
        {

        }

        public virtual void OnPubAck(MQTTUser client, PUBACK e)
        {


        }

        public virtual void OnPubComp(MQTTUser client, PUBCOMP e)
        {

        }




        public virtual void OnPubRec(MQTTUser client, PUBREC e)
        {

        }


        public virtual void OnPubRel(MQTTUser client, PUBREL e)
        {

        }


        public virtual void OnSubAck(MQTTUser client, SUBACK e)
        {

        }


        public virtual void OnUnSubAck(MQTTUser client, UNSUBACK e)
        {


        }

        public virtual void OnUnSubscribe(MQTTUser client, UNSUBSCRIBE e)
        {
            GetLoger(EventArgs.LogType.Info)?.Log(EventArgs.LogType.Info, client.GetNetSession(), $"{client.ID} unsubscribe {e}");
            UNSUBACK ack = new UNSUBACK();
            ack.Identifier = e.Identifier;
            client.Send(ack);
            UnRegisterSubscribe(e, client);

        }

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
