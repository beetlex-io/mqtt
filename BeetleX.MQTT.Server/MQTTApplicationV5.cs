using BeetleX.FastHttpApi;
using BeetleX.MQTT.Protocols;
using BeetleX.MQTT.Protocols.V5.Messages;
using BeetleX.MQTT.Server.Storages;
using System;
using System.Collections.Generic;
using System.Text;

namespace BeetleX.MQTT.Server
{
    public class MQTTApplicationV5 : MQTTApplication
    {

        public override string Name => "MQTT V5.0 Protocol";

        #region messages
        public override void Receive(EventMessageReceiveArgs<NetApplication, NetUser, object> e)
        {
            MQTTUser mqttuser = null;
            MQTTMessage item = (MQTTMessage)e.Message;
            string userid = null;
            if (item is Connect conn)
            {
                userid = conn.ClientID;
            }
            else
            {
                userid = e.NetSession.Token<NetUser>().ClientID;
                mqttuser = GetUser(userid);
                if (e.NetSession.Authentication != AuthenticationType.Security)
                {
                    Disconnect(mqttuser);
                    GetLoger(EventArgs.LogType.Info).Log(EventArgs.LogType.Info, e.NetSession, "No permission to operate!");
                    return;
                }
                mqttuser.UpdataActiveTime();
            }
            switch (item.Type)
            {
                case MQTTMessageType.ConnectAck:
                    OnConnAck(mqttuser, (ConnAck)item);
                    break;
                case MQTTMessageType.Connect:
                    if (OnContent(userid, (Connect)item, null, e.NetSession))
                    {
                        e.NetSession.Token<NetUser>().ClientID = userid;
                        e.NetSession.Authentication = AuthenticationType.Security;
                    }
                    else
                    {
                        GetLoger(EventArgs.LogType.Warring).Log(EventArgs.LogType.Info, e.NetSession, "Login verification error!");
                        Disconnect(e.NetSession);
                        return;
                    }
                    break;
                case MQTTMessageType.Disconnect:
                    OnDisconnect(mqttuser, (Disconnect)item);
                    break;
                case MQTTMessageType.PingReq:
                    OnPingREQ(mqttuser, (PingReq)item);
                    break;
                case MQTTMessageType.PingResp:
                    OnPingResp(mqttuser, (PingResp)item);
                    break;
                case MQTTMessageType.PubAck:
                    OnPubAck(mqttuser, (PubAck)item);
                    break;
                case MQTTMessageType.PubComp:
                    OnPubComp(mqttuser, (PubComp)item);
                    break;
                case MQTTMessageType.Publish:
                    OnPublish(mqttuser, (Publish)item);
                    break;
                case MQTTMessageType.PubRec:
                    OnPubRec(mqttuser, (PubRec)item);
                    break;
                case MQTTMessageType.PubRel:
                    OnPubRel(mqttuser, (PubRel)item);
                    break;
                case MQTTMessageType.SubAck:
                    OnSubAck(mqttuser, (SubAck)item);
                    break;
                case MQTTMessageType.Subscribe:
                    OnSubscribe(mqttuser, (Subscribe)item);
                    break;
                case MQTTMessageType.UnSubAck:
                    OnUnSubAck(mqttuser, (UnSubAck)item);
                    break;
                case MQTTMessageType.UnSubscribe:
                    OnUnSubscribe(mqttuser, (UnSubscribe)item);
                    break;
            }
        }

        protected virtual bool OnContent(string clientid, Connect e, HttpRequest websocket, ISession tcp)
        {
            bool isNode = false;
            ConnAck ack = new ConnAck();
            string category = "";
            if (string.IsNullOrEmpty(e.Name))
            {
                ack.Status = ReturnType.NotAuthorized;
                Reply(ack, websocket, tcp);
                GetLoger(EventArgs.LogType.Info)?.Log(EventArgs.LogType.Info, websocket != null ? websocket.Session : tcp, $"{clientid} user notfound");
                return false;
            }
            if (SessionVerify)
            {
                using (MQTTDB context = new MQTTDB())
                {
                    var item = context.User.Find(e.Name);
                    if (item == null || e.Password != item.PWD || !item.Enabled)
                    {

                        ack.Status = ReturnType.BadUserNameOrPassword;
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
            ack.Status = ReturnType.Success;
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

        protected virtual bool OnPublish(MQTTUser client, Publish e)
        {
            GetLoger(EventArgs.LogType.Debug)?
                .Log(EventArgs.LogType.Debug, client.GetNetSession(), $"{client.ID} publish {e.Topic}@ {e.Identifier}");
            GetLoger(EventArgs.LogType.Trace)?
                .Log(EventArgs.LogType.Trace, client.GetNetSession(), $"{client.ID} publish {e.Topic}@ {e.Identifier} data:{BitConverter.ToString(e.Payload.Array, e.Payload.Offset, e.Payload.Count)}");
            this.AddNumberOfPush();
            client.AddNumberOfPush();
            AddReceiveQuantity();
            if (e.QoS == QoSType.LeastOnce)
            {
                PubAck ack = new PubAck();
                ack.Identifier = e.Identifier;
                client.Send(ack);
            }
            return Publish(e);
        }

        protected virtual void OnSubscribe(MQTTUser client, Subscribe e)
        {
            GetLoger(EventArgs.LogType.Info)?.Log(EventArgs.LogType.Info, client.GetNetSession(), $"{client.ID} subscribe {e}");

            SubAck ack = new SubAck();
            ack.Identifier = e.Identifier;
            client.Send(ack);
            RegisterSubscribe(e.Subscriptions, client);
        }

        protected virtual void OnConnAck(MQTTUser client, ConnAck e)
        {

        }

        protected virtual void OnDisconnect(MQTTUser client, Disconnect e)
        {
            GetLoger(EventArgs.LogType.Info)?
              .Log(EventArgs.LogType.Info, client.GetNetSession(), $"{client.ID} disconnect");
            Disconnect(client);

        }

        protected virtual void OnPingREQ(MQTTUser client, PingReq e)
        {
            GetLoger(EventArgs.LogType.Info)?
               .Log(EventArgs.LogType.Info, client.GetNetSession(), $"{client.ID} ping");
            PingResp resp = new PingResp();
            client.Send(resp);
        }

        protected virtual void OnPingResp(MQTTUser client, PingResp e)
        {

        }

        protected virtual void OnPubAck(MQTTUser client, PubAck e)
        {


        }

        protected virtual void OnPubComp(MQTTUser client, PubComp e)
        {

        }

        protected virtual void OnPubRec(MQTTUser client, PubRec e)
        {

        }


        protected virtual void OnPubRel(MQTTUser client, PubRel e)
        {

        }


        protected virtual void OnSubAck(MQTTUser client, SubAck e)
        {

        }


        protected virtual void OnUnSubAck(MQTTUser client, UnSubAck e)
        {


        }

        protected virtual void OnUnSubscribe(MQTTUser client, UnSubscribe e)
        {
            GetLoger(EventArgs.LogType.Info)?.Log(EventArgs.LogType.Info, client.GetNetSession(), $"{client.ID} unsubscribe {e}");
            UnSubAck ack = new UnSubAck();
            ack.Identifier = e.Identifier;
            client.Send(ack);
            UnRegisterSubscribe(e.Subscribers, client);

        }
        #endregion
    }
}
