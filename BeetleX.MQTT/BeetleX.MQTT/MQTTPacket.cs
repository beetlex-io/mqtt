using BeetleX.Clients;
using BeetleX.EventArgs;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace BeetleX.MQTT
{
    public class MQTTPacket : BeetleX.IPacket
    {
        public EventHandler<PacketDecodeCompletedEventArgs> Completed { get; set; }

        private MQTTParse mMQTTParse = new MQTTParse();

        public IPacket Clone()
        {
            return new MQTTPacket();
        }



        public void Decode(ISession session, Stream stream)
        {

        }

        public void Dispose()
        {

        }

        public void Encode(object data, ISession session, Stream stream)
        {

        }

        public byte[] Encode(object data, IServer server)
        {

        }

        public ArraySegment<byte> Encode(object data, IServer server, byte[] buffer)
        {

        }
    }

    public class MQTTClientPacket : BeetleX.Clients.IClientPacket
    {
        public EventClientPacketCompleted Completed { get; set; }

        private MQTTParse mMQTTParse = new MQTTParse();

        public IClientPacket Clone()
        {
            return new MQTTClientPacket();
        }

        public void Decode(IClient client, Stream stream)
        {

        }

        public void Dispose()
        {

        }

        public void Encode(object data, IClient client, Stream stream)
        {

        }
    }
}
