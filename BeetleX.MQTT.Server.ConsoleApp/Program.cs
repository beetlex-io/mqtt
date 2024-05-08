using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BeetleX.Buffers;
using BeetleX.FastHttpApi;
using BeetleX.MQTT.Protocols.V3;
using BeetleX.WebFamily;
namespace BeetleX.MQTT.Server.ConsoleApp
{
    class Program
    {
        private static MQTTServer mServer;
        static void Main(string[] args)
        {

            mServer = new MQTTServer(ProtocolType.V3);
            mServer.RegisterComponent<BeetleX.MQTT.Server.Controller>();
            mServer.MQTTListen(o =>
            {
                o.DefaultListen.Port = 8089;
                //o.DefaultListen.SSL = true;
                //o.DefaultListen.CertificateFile = "";
                //o.DefaultListen.CertificatePassword = "";
            })
            .Setting(o =>
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
