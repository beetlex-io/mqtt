using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BeetleX.Buffers;
using BeetleX.FastHttpApi;
using BeetleX.WebFamily;
namespace BeetleX.MQTT.Server.ConsoleApp
{
    public class TxtResultBase : ResultBase
    {
        public TxtResultBase(string data)
        {
            Data = data;
        }

        public override IHeaderItem ContentType => ContentTypes.TEXT;

        private string Data { get; set; }

        public override bool HasBody => true;


        public override void Setting(HttpResponse response)
        {

        }

        public override void Write(PipeStream stream, HttpResponse response)
        {

        }
    }
    class Program
    {
        private static MQTTServer<MQTTApplication, MQTTParse> mSever;
        static void Main(string[] args)
        {

            mSever = new Server.MQTTServer<MQTTApplication, MQTTParse>();
            //MQTTUser user = new MQTTUser();
            //user.ID = "b71cf3a6-c734-4e7c-b1fc-9b291870e3e6";
            //user.UserName = "henry";
            //user.Password = "123456";
            //mSever.Application.AddClient(user);
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
            WebHost.LoginHandler = MQTTServer<MQTTApplication, MQTTParse>.Application.Login;
            mSever.RegisterComponent<BeetleX.MQTT.Server.Controller>();
            mSever.EnabledTCP(o =>
            {
                o.DefaultListen.Port = 8089;
                o.ApplicationName = "MQTT Server 3.1.x";
            });

            mSever.Setting(o =>
            {
                o.SetDebug(@"E:\GITHUB\MQTT\BeetleX.MQTT.Server\views");
                o.LogToConsole = true;
                o.Port = 80;
                o.LogLevel = EventArgs.LogType.Info;
            })
            .UseJWT()
            .UseEFCore<Storages.MQTTDB>()
            .UseElement(PageStyle.ElementDashboard)
            .Initialize((http, vue, resoure) =>
            {
                resoure.AddAssemblies(typeof(BeetleX.MQTT.Server.MQTTUser).Assembly);
                resoure.AddCss("website.css");
                resoure.AddScript("echarts.js");
                vue.Debug();
            })
           .Run();

        }
    }
}
