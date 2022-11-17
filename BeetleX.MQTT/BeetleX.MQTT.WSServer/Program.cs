using BeetleX.MQTT.Messages;
using System;
using System.Text;

namespace BeetleX.MQTT.WSServer
{
    class Program
    {
        private static MQTTWebsocketServer<MQTTApplication, MQTTUser> mServer;
        static void Main(string[] args)
        {
            mServer = new MQTTWebsocketServer<MQTTApplication, MQTTUser>(8081);
            mServer.Setting((service, options) => {
                options.LogLevel = EventArgs.LogType.Trace;
                options.LogToConsole = true;
                options.WebSocketFrameSerializer = new MQTTFormater();
            });
            mServer.OnMessageReceive<CONNECT>(e =>
            {
                e.GetLoger(EventArgs.LogType.Info)?.Log(EventArgs.LogType.Info, e.NetSession, $"{e.NetSession.RemoteEndPoint} connect name:{e.Message.UserName} password:{e.Message.Password}");
                e.Session.UserName = e.Message.UserName;
                e.Session.ID = e.Message.ClientID;
                CONNACK ack = new CONNACK();
                e.Return(ack);
            })
            .OnMessageReceive<SUBSCRIBE>(e =>
            {
                e.GetLoger(EventArgs.LogType.Info)?.Log(EventArgs.LogType.Info, e.NetSession, $"{e.Session.ID} subscribe {e.Message}");
                SUBACK ack = new SUBACK();
                ack.Identifier = e.Message.Identifier;
                ack.Status = QoSType.MostOnce;
                e.Return(ack);
                e.Application.RegisterSubscribe(e.Message, e.Session);
            })
            .OnMessageReceive<UNSUBSCRIBE>(e =>
            {
                e.GetLoger(EventArgs.LogType.Info)?.Log(EventArgs.LogType.Info, e.NetSession, $"{e.Session.ID} unsubscribe {e.Message}");
                UNSUBACK ack = new UNSUBACK();
                e.Return(ack);
                e.Application.UnRegisterSubscribe(e.Message, e.Session);
            })
            .OnMessageReceive<PUBLISH>(e =>
            {
                var data = Encoding.UTF8.GetString(e.Message.PayLoadData.Array, e.Message.PayLoadData.Offset, e.Message.PayLoadData.Count);
                e.GetLoger(EventArgs.LogType.Info)?.Log(EventArgs.LogType.Info, e.NetSession, $"{e.Session.ID} publish {e.Message.Topic}@ {e.Message.Identifier} data:{data}");
                PUBACK ack = new PUBACK();
                ack.Identifier = e.Message.Identifier;
                e.Return(ack);
                e.Application.Publish(e.Message);
            })
            .OnMessageReceive<PINGREQ>(e =>
            {
                PINGRESP resp = new PINGRESP();
                e.Return(resp);
            })
            .OnMessageReceive(e =>
            {

            })
            .Run();
            Console.Read();
        }
    }
}
