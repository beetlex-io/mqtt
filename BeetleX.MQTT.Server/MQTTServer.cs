using BeetleX.FastHttpApi;
using BeetleX.MQTT.Protocols;
using BeetleX.MQTT.Protocols.V3;
using BeetleX.MQTT.Protocols.V5;
using BeetleX.WebFamily;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeetleX.MQTT.Server
{
    public class MQTTServer : WebHost
    {
        public MQTTServer(ProtocolType type = ProtocolType.V3)
        {
            mProtocolType = type;
            if (mProtocolType == ProtocolType.V3)
            {
                Application = new MQTTApplicationV3();
            }
            else
            {

                Application = new MQTTApplicationV5();
            }

        }

        private ProtocolType mProtocolType = ProtocolType.V3;

        private TCPOptions mTCPOptions = new TCPOptions();

        public TCPOptions TCPOptions => mTCPOptions;

        public static MQTTApplication Application { get; private set; }



        public MQTTServer MQTTListen(Action<BeetleX.ServerOptions> setting)
        {
            mTCPOptions.OptionsHandler = setting;
            return this;
        }
        protected override void OnHttpSetting(IServiceCollection services, HttpOptions option)
        {
            base.OnHttpSetting(services, option);
            mTCPOptions.LogType = option.LogLevel;

        }
        protected override void OnConfigureServices(HostBuilderContext context, IServiceCollection service)
        {
            base.OnConfigureServices(context, service);
            service.AddSingleton(mTCPOptions);
            service.AddSingleton<MQTTApplication>(Application);
            if (mProtocolType == ProtocolType.V3)
            {
                service.AddHostedService<TCPHost<MQTTApplicationV3, MQTTPacketV3>>();
            }
            else
            {
                service.AddHostedService<TCPHost<MQTTApplicationV5, MQTTPacketV5>>();
            }

        }

        private void OnWebSetting(HttpApiServer server)
        {
            WebHost.Title = "BEETLEX-MQTT";
            WebHost.AppName = "BEETLEX-MQTT";
            WebHost.HeaderModel = "mqtt-header";
            WebHost.Login = true;
            WebHost.HomeModel = "mqtt-home";
            WebHost.HomeName = "主页";
            WebHost.GetMenus = (user, role, context) =>
            {
                List<Menu> result = new List<Menu>();
                Menu item = new Menu();
                item.ID = "home";
                item.Model = "mqtt-home";
                item.Name = "主页";
                item.Icon = "fa-solid fa-house";
                result.Add(item);



                item = new Menu();
                item.ID = "mqtt-onlines";
                item.Model = "mqtt-onlines";
                item.Icon = "fa-solid fa-hard-drive";
                item.Name = "在线设备";
                result.Add(item);

                item = new Menu();
                item.ID = "mqtt-users";
                item.Model = "mqtt-users";
                item.Icon = "fa-solid fa-user";
                item.Name = "帐户管理";
                result.Add(item);

                return Task.FromResult(result);
            };
            WebHost.LoginHandler = Application.Login;
        }
        protected override void OnInitServer(HttpApiServer server)
        {
            base.OnInitServer(server);
            Application.Http = server;
            Protocols.V5.Headers.HeaderFactory.Init();
            OnWebSetting(server);
        }
        protected override void OnCompleted(HttpApiServer server)
        {
            base.OnCompleted(server);
            Application.Init(server.Server);
            server.WebSocketConnect += (o, e) =>
            {
                e.Request.Session.Token<NetUser>();
                MQTTUpgradeWebsocketSuccess upgradeWebsocket = new MQTTUpgradeWebsocketSuccess(e.Request.Header[HeaderTypeFactory.SEC_WEBSOCKET_KEY]);
                e.UpgradeSuccess = upgradeWebsocket;
            };
        }

    }



}
