using BeetleX.MQTT.Protocols;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BeetleX.MQTT.Server
{
    class TCPHost<APPLICATION, PACKET> : IHostedService
        where APPLICATION : MQTTApplication
        where PACKET : IPacket, new()
    {
        public TCPHost(TCPOptions options, MQTTApplication application)
        {
            mTCPOption = options;
            mApplication = application;
        }

        private TCPOptions mTCPOption;

        private MQTTApplication mApplication;

        private MQTTApplication Application => mApplication;



        private ServerBuilder<NetApplication, NetUser, PACKET> mServer;

        public Task StartAsync(CancellationToken cancellationToken)
        {

            if (mTCPOption.OptionsHandler != null)
            {
                mServer = new ServerBuilder<NetApplication, NetUser, PACKET>();

                mServer.OnOpened((server) =>
                {
                    mApplication.TCP = server;
                });
                mServer.SetOptions((o) =>
                {
                    mTCPOption.OptionsHandler(o);
                    o.LogLevel = mTCPOption.LogType;
                    o.ApplicationName = Application.Name;

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
                    Application.Receive(e);
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
