using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeetleX.MQTT.Server
{
    public class MQTTUser : ISessionToken
    {
        public string ID { get; set; }

        public string UserName { get; set; }
        public void Dispose()
        {
            NetSession = null;
        }

        public override int GetHashCode()
        {
            return this.ID.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            return ((MQTTUser)obj).ID == this.ID;
        }

        public ISession NetSession { get; private set; }

        public void Send(MQTTMessage msg)
        {
            NetSession?.Send(msg);
        }

        public void Init(IServer server, ISession session)
        {

            server.GetLoger(EventArgs.LogType.Info)?.Log(EventArgs.LogType.Info, session, "session data init");
            NetSession = session;
        }
    }
}
