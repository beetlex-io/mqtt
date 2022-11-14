using System;

namespace BeetleX.MQTT.Server
{
    class Program : IApplication
    {
        private static ServerBuilder<Program, MQTTUser, MQTTPacket> server;
        static void Main(string[] args)
        {
            server = new ServerBuilder<Program, MQTTUser, MQTTPacket>();
            server.ConsoleOutputLog = true;
            server.SetOptions(option =>
            {
                option.DefaultListen.Port = 9090;
                option.DefaultListen.Host = "127.0.0.1";
                option.LogLevel = EventArgs.LogType.Trace;
            })
            .OnMessageReceive<Messages.CONNECT>(e =>
            {
                Messages.CONNACK ack = new Messages.CONNACK();
                e.Return(ack);
            })
            .OnMessageReceive<Messages.SUBSCRIBE>(e =>
            {

            })
            .OnMessageReceive<Messages.PINGREQ>(e =>
            {
                Messages.PINGRESP resp = new Messages.PINGRESP();
                e.Return(resp);
            })
            .OnMessageReceive<Messages.PUBLISH>(e =>
            {


            })
            .OnMessageReceive(e =>
            {

            })
            .Run();
            Console.Read();
        }

        public void Init(IServer server)
        {

        }
    }

    public class MQTTUser : ISessionToken
    {
        public void Dispose()
        {

        }

        public void Init(IServer server, ISession session)
        {

        }
    }
}
