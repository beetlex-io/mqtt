using BeetleX.MQTT.Messages;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BeetleX.MQTT.Server
{
    class TCPHost<APPLICATION, PARSE> : IHostedService
        where APPLICATION : MQTTApplication
        where PARSE : MQTTParse, new()
    {
        public TCPHost(TCPOptions options, APPLICATION application)
        {
            mTCPOption = options;
            mApplication = application;
        }

        private TCPOptions mTCPOption;

        private MQTTApplication mApplication;

        private MQTTApplication Application => mApplication;



        private ServerBuilder<NetApplication, NetUser, MQTTPacket<PARSE>> mServer;

        public Task StartAsync(CancellationToken cancellationToken)
        {

            if (mTCPOption.OptionsHandler != null)
            {
                mServer = new ServerBuilder<NetApplication, NetUser, MQTTPacket<PARSE>>();

                mServer.OnOpened((server) =>
                {
                    mApplication.TCP = server;
                });
                mServer.SetOptions((o) =>
                {
                    mTCPOption.OptionsHandler(o);
                    o.LogLevel = mTCPOption.LogType;

                });
                mServer.OnDisconnect((i, n) =>
                {
                    var user = Application.GetUser(n.ClientID);
                    if (user != null)
                        Application.Disconnect(user, 500);
                });
                mServer.OnLog((server, e) =>
                {
                    Application.GetLoger(e.Type)?.Log(e.Type, e.Session, e.Message);
                });
                mServer.OnMessageReceive(e =>
                {
                    MQTTUser mqttuser = null;
                    MQTTMessage item = (MQTTMessage)e.Message;
                    string userid = null;
                    if (item is CONNECT conn)
                    {
                        userid = conn.ClientID;
                    }
                    else
                    {
                        userid = e.NetSession.Token<NetUser>().ClientID;
                        mqttuser = Application.GetUser(userid);
                        if (e.NetSession.Authentication != AuthenticationType.Security)
                        {
                            Application.Disconnect(mqttuser);
                            Application.GetLoger(EventArgs.LogType.Info).Log(EventArgs.LogType.Info, e.NetSession, "No permission to operate!");
                            return;
                        }
                        mqttuser.UpdataActiveTime();
                    }
                    switch (item.Type)
                    {
                        case MQTTMessageType.CONNACK:
                            Application.OnConnAck(mqttuser, (CONNACK)item);
                            break;
                        case MQTTMessageType.CONNECT:
                            if (Application.OnContent(userid, (CONNECT)item, null, e.NetSession))
                            {
                                e.NetSession.Token<NetUser>().ClientID = userid;
                                e.NetSession.Authentication = AuthenticationType.Security;
                            }
                            else
                            {
                                Application.GetLoger(EventArgs.LogType.Warring).Log(EventArgs.LogType.Info, e.NetSession, "Login verification error!");
                                Application.Disconnect(e.NetSession);
                                return;
                            }
                            break;
                        case MQTTMessageType.DISCONNECT:
                            Application.OnDisconnect(mqttuser, (DISCONNECT)item);
                            break;
                        case MQTTMessageType.PINGREQ:
                            Application.OnPingREQ(mqttuser, (PINGREQ)item);
                            break;
                        case MQTTMessageType.PINGRESP:
                            Application.OnPingResp(mqttuser, (PINGRESP)item);
                            break;
                        case MQTTMessageType.PUBACK:
                            Application.OnPubAck(mqttuser, (PUBACK)item);
                            break;
                        case MQTTMessageType.PUBCOMP:
                            Application.OnPubComp(mqttuser, (PUBCOMP)item);
                            break;
                        case MQTTMessageType.PUBLISH:
                            Application.OnPublish(mqttuser, (PUBLISH)item);
                            break;
                        case MQTTMessageType.PUBREC:
                            Application.OnPubRec(mqttuser, (PUBREC)item);
                            break;
                        case MQTTMessageType.PUBREL:
                            Application.OnPubRel(mqttuser, (PUBREL)item);
                            break;
                        case MQTTMessageType.SUBACK:
                            Application.OnSubAck(mqttuser, (SUBACK)item);
                            break;
                        case MQTTMessageType.SUBSCRIBE:
                            Application.OnSubscribe(mqttuser, (SUBSCRIBE)item);
                            break;
                        case MQTTMessageType.UNSUBACK:
                            Application.OnUnSubAck(mqttuser, (UNSUBACK)item);
                            break;
                        case MQTTMessageType.UNSUBSCRIBE:
                            Application.OnUnSubscribe(mqttuser, (UNSUBSCRIBE)item);
                            break;
                    }
                });
                mServer.Run();
            }
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            mServer?.Dispose();
            return Task.CompletedTask;
        }
    }
}
