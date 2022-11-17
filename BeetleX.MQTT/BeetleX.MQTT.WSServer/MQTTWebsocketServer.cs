using BeetleX.EventArgs;
using BeetleX.FastHttpApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace BeetleX.MQTT.WSServer
{
    public class MQTTWebsocketServer<APPLICATION, SESSION> : FastHttpApi.Hosting.HttpServer
        where APPLICATION : IApplication, new()
        where SESSION : ISessionToken, new()
    {
        public MQTTWebsocketServer(int port, string host = null) : base(port, host)
        {
            MQTTApplication = new APPLICATION();
        }

        public APPLICATION MQTTApplication { get; set; }




        protected override void OnCreating(HttpApiServer server)
        {
            base.OnCreating(server);
        }
        protected override void OnCompleted(HttpApiServer server)
        {
            MQTTApplication.Init(server.Server);
            server.WebSocketReceive = (o, e) =>
            {
                if (e.Frame.Body != null)
                {
                    var items = (List<MQTTMessage>)e.Frame.Body;
                    foreach (var item in items)
                    {
                        Type msgtype = item.GetType();
                        if (mMessageHandlers.TryGetValue(msgtype, out IMessageProcessHandler handler))
                        {
                            handler.Execute(server, e.Request.Session, MQTTApplication, e.Request.Session.Token<SESSION>(), item);
                        }
                        else
                        {
                            OnReceiveProcess(server, e.Request.Session, item);

                        }
                    }
                }
            };
            server.WebSocketConnect += (o, e) =>
            {
                e.Request.Session.Token<MQTTUser>();
                MQTTUpgradeWebsocketSuccess upgradeWebsocket = new MQTTUpgradeWebsocketSuccess(e.Request.Header[HeaderTypeFactory.SEC_WEBSOCKET_KEY]);
                e.UpgradeSuccess = upgradeWebsocket;
            };
            base.OnCompleted(server);
        }

        private System.Collections.Concurrent.ConcurrentDictionary<Type, IMessageProcessHandler> mMessageHandlers = new System.Collections.Concurrent.ConcurrentDictionary<Type, IMessageProcessHandler>();

        public MQTTWebsocketServer<APPLICATION, SESSION> OnMessageReceive<Message>(Action<EventWSMessageReceiveArgs<APPLICATION, SESSION, Message>> handler)
        {
            MessageProcessHandler<APPLICATION, SESSION, Message> e = new MessageProcessHandler<APPLICATION, SESSION, Message>();
            e.Handler = handler;
            mMessageHandlers[typeof(Message)] = e;
            return this;
        }


        private Action<EventWSMessageReceiveArgs<APPLICATION, SESSION, object>> mOnReceive;

        protected virtual void OnReceiveProcess(HttpApiServer server, ISession session, object message)
        {
            EventWSMessageReceiveArgs<APPLICATION, SESSION, object> e = new EventWSMessageReceiveArgs<APPLICATION, SESSION, object>();
            e.Application = MQTTApplication;
            e.HttpApiServer = server;
            e.NetSession = session;
            e.Session = session.Token<SESSION>();
            e.Message = message;
            mOnReceive?.Invoke(e);
        }

        public MQTTWebsocketServer<APPLICATION, SESSION> OnMessageReceive(Action<EventWSMessageReceiveArgs<APPLICATION, SESSION, object>> handler)
        {
            mOnReceive = handler;
            return this;
        }


    }
    public class MQTTUpgradeWebsocketSuccess : ResultBase
    {
        public MQTTUpgradeWebsocketSuccess(string websocketKey)
        {
            WebsocketKey = websocketKey;
        }

        public string WebsocketKey { get; set; }


        public override bool HasBody => false;

        public override void Setting(HttpResponse response)
        {
            response.Code = "101";
            response.CodeMsg = "Switching Protocols";
            response.Header.Add(HeaderTypeFactory.CONNECTION, "Upgrade");
            response.Header.Add(HeaderTypeFactory.UPGRADE, "websocket");
            response.Header.Add(HeaderTypeFactory.SEC_WEBSOCKET_VERSION, "13");
            SHA1 sha1 = new SHA1CryptoServiceProvider();
            byte[] bytes_sha1_in = Encoding.UTF8.GetBytes(WebsocketKey + "258EAFA5-E914-47DA-95CA-C5AB0DC85B11");
            byte[] bytes_sha1_out = sha1.ComputeHash(bytes_sha1_in);
            string str_sha1_out = Convert.ToBase64String(bytes_sha1_out);
            response.Header.Add(HeaderTypeFactory.SEC_WEBSOCKT_ACCEPT, str_sha1_out);
        }
    }

    public struct EventWSMessageReceiveArgs<APPLICATION, SESSION, MSG>
         where SESSION : ISessionToken, new()
   where APPLICATION : IApplication, new()
    {
        public ISession NetSession { get; set; }

        public HttpApiServer HttpApiServer { get; set; }
        public SESSION Session { get; set; }
        public APPLICATION Application { get; set; }
        public MSG Message { get; set; }

        public ILoger GetLoger(LogType type)
        {
            return HttpApiServer.Server.GetLoger(type);
        }
        public void Return(MQTTMessage message)
        {
            var frame = HttpApiServer.CreateBinaryFrame(message);
            NetSession.Send(frame);
        }
    }

    class MessageProcessHandler<APPLICATION, SESSION, MESSAGE> : IMessageProcessHandler
        where SESSION : ISessionToken, new()
   where APPLICATION : IApplication, new()
    {
        public Action<EventWSMessageReceiveArgs<APPLICATION, SESSION, MESSAGE>> Handler { get; set; }
        public void Execute(HttpApiServer server, object netsession, object application, object session, object message)
        {
            EventWSMessageReceiveArgs<APPLICATION, SESSION, MESSAGE> e = new EventWSMessageReceiveArgs<APPLICATION, SESSION, MESSAGE>();
            e.NetSession = (ISession)netsession;
            e.Application = (APPLICATION)application;
            e.Session = (SESSION)session;
            e.Message = (MESSAGE)message;
            e.HttpApiServer = server;
            Handler?.Invoke(e);
        }
    }

    interface IMessageProcessHandler
    {
        void Execute(HttpApiServer server, object netsession, object application, object session, object message);
    }
}
