using BeetleX.FastHttpApi;
using BeetleX.MQTT.Messages;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeetleX.MQTT.Server
{
    public class MQTTServer<APPLICATION, MQTTPARSE> : WebFamily.WebHost
        where APPLICATION : MQTTApplication, new()
        where MQTTPARSE : MQTTParse, new()
    {
        public MQTTServer()
        {
            Application.APPID = "1";
        }

        private TCPOptions mTCPOptions = new TCPOptions();

        public TCPOptions TCPOptions => mTCPOptions;

        public static APPLICATION Application { get; private set; } = new APPLICATION();



        public MQTTServer<APPLICATION, MQTTPARSE> EnabledTCP(Action<BeetleX.ServerOptions> setting)
        {
            mTCPOptions.OptionsHandler = setting;
            return this;
        }
        protected override void OnHttpSetting(IServiceCollection services, HttpOptions option)
        {
            base.OnHttpSetting(services, option);
            mTCPOptions.LogType = option.LogLevel;
            option.WebSocketFrameSerializer = new MQTTFormater<MQTTPARSE>();


        }
        protected override void OnConfigureServices(HostBuilderContext context, IServiceCollection service)
        {
            base.OnConfigureServices(context, service);
            service.AddSingleton(mTCPOptions);
            service.AddSingleton(Application);
            service.AddHostedService<TCPHost<APPLICATION, MQTTPARSE>>();

        }

        protected override void OnInitServer(HttpApiServer server)
        {
            base.OnInitServer(server);
            Application.Http = server;

        }
        protected override void OnCompleted(HttpApiServer server)
        {
            base.OnCompleted(server);
            Application.Init(server.Server);
            server.WebSocketReceive = (o, e) =>
            {
                Console.WriteLine(e.Frame.FIN);
                if (e.Frame.Body != null)
                {
                    var items = (List<MQTTMessage>)e.Frame.Body;
                    MQTTUser mqttuser = null;
                    foreach (var item in items)
                    {
                        string userid = null;
                        if (item is CONNECT conn)
                        {
                            userid = conn.ClientID;
                        }
                        else
                        {
                            userid = e.Request.Session.Token<NetUser>().ClientID;
                            mqttuser = Application.GetUser(userid);
                            if (e.Sesson.Authentication != AuthenticationType.Security)
                            {
                                Application.Disconnect(mqttuser);
                                Application.GetLoger(EventArgs.LogType.Info).Log(EventArgs.LogType.Info, e.Sesson, "No permission to operate!");
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
                                if (Application.OnContent(userid, (CONNECT)item, e.Request, null))
                                {
                                    e.Sesson.Token<NetUser>().ClientID = userid;
                                    e.Sesson.Name = userid;
                                    e.Sesson.Authentication = AuthenticationType.Security;
                                }
                                else
                                {
                                    Application.GetLoger(EventArgs.LogType.Warring).Log(EventArgs.LogType.Info, e.Sesson, "Login verification error!");
                                    Application.Disconnect(e.Sesson);
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
                    }
                }
            };
            server.WebSocketConnect += (o, e) =>
            {
                e.Request.Session.Token<NetUser>();
                MQTTUpgradeWebsocketSuccess upgradeWebsocket = new MQTTUpgradeWebsocketSuccess(e.Request.Header[HeaderTypeFactory.SEC_WEBSOCKET_KEY]);
                e.UpgradeSuccess = upgradeWebsocket;
            };
        }

    }
}
