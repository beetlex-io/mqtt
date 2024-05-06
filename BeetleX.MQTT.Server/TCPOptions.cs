using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeetleX.MQTT.Server
{
    public class TCPOptions
    {
        public Action<BeetleX.ServerOptions> OptionsHandler { get; set; }

        public BeetleX.EventArgs.LogType LogType { get; set; }
    }
}
