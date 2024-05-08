using BeetleX.Buffers;
using BeetleX.Clients;
using BeetleX.EventArgs;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace BeetleX.MQTT.Protocols.V5
{
    public class MQTTPacketV5 : IPacket
    {
        public EventHandler<PacketDecodeCompletedEventArgs> Completed { get; set; }

        public string Name => "MQTT 5.0";

        private MQTTParseV5 mMQTTParse = new MQTTParseV5();

        public IPacket Clone()
        {
            return new MQTTPacketV5();
        }

        private PacketDecodeCompletedEventArgs mPacketDecodeCompletedEventArgs = new PacketDecodeCompletedEventArgs();
        public void Decode(ISession session, Stream stream)
        {
            START:
            var msg = mMQTTParse.Read(stream.ToPipeStream(), session);
            if (msg != null)
            {
                mPacketDecodeCompletedEventArgs.SetInfo(session, msg);
                Completed?.Invoke(this, mPacketDecodeCompletedEventArgs);
                goto START;
            }
        }

        public void Dispose()
        {

        }

        public void Encode(object data, ISession session, Stream stream)
        {
            if (data is MQTTMessage msg)
            {
                mMQTTParse.Write(msg, stream.ToPipeStream(), session);
            }
            else
            {
                throw new BXException("Invalid message type!");
            }
        }

        public byte[] Encode(object data, IServer server)
        {
            byte[] result = null;
            using (PipeStream stream = new PipeStream(server.SendBufferPool.Next(), server.Options.LittleEndian, server.Options.Encoding))
            {
                Encode(data, null, stream);
                stream.Position = 0;
                result = new byte[stream.Length];
                stream.Read(result, 0, result.Length);
            }
            return result;
        }

        public ArraySegment<byte> Encode(object data, IServer server, byte[] buffer)
        {
            using (PipeStream stream = new PipeStream(server.SendBufferPool.Next(), server.Options.LittleEndian, server.Options.Encoding))
            {
                Encode(data, null, stream);
                stream.Position = 0;
                int count = (int)stream.Length;
                stream.Read(buffer, 0, count);
                return new ArraySegment<byte>(buffer, 0, count);
            }
        }
    }

    public class MQTTClientPacketV5 : IClientPacket
    {
        public EventClientPacketCompleted Completed { get; set; }

        private MQTTParseV5 mMQTTParse = new MQTTParseV5();

        public IClientPacket Clone()
        {
            return new MQTTClientPacketV5();
        }

        public void Decode(IClient client, Stream stream)
        {
            START:
            var msg = mMQTTParse.Read(stream.ToPipeStream(), null);
            if (msg != null)
            {
                Completed?.Invoke(client, msg);
                goto START;
            }
        }

        public void Dispose()
        {

        }

        public void Encode(object data, IClient client, Stream stream)
        {
            if (data is MQTTMessage msg)
            {
                mMQTTParse.Write(msg, stream.ToPipeStream(), null);
            }
            else
            {
                throw new BXException("Invalid message type!");
            }
        }
    }
}
