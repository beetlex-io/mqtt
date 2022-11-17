using BeetleX.Buffers;
using BeetleX.FastHttpApi;
using BeetleX.FastHttpApi.WebSockets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeetleX.MQTT.WSServer
{
    public class MQTTFormater : BeetleX.FastHttpApi.WebSockets.IDataFrameSerializer
    {
        private MQTTParse mMQTTParse = new MQTTParse();
        public object FrameDeserialize(DataFrame data, PipeStream stream, HttpRequest request)
        {
            List<MQTTMessage> items = new List<MQTTMessage>();
            while (true)
            {
                var msg = mMQTTParse.Read(stream, request.Session);
                if (msg == null)
                    break;
                items.Add(msg);

            }
            return items;
        }

        public void FrameRecovery(byte[] buffer)
        {

        }

        [ThreadStatic]
        private static System.IO.MemoryStream mSerializeStream = null;

        public ArraySegment<byte> FrameSerialize(DataFrame packet, object body, HttpRequest request)
        {
            if (body is MQTTMessage msg)
            {
                if (mSerializeStream == null)
                {
                    mSerializeStream = new System.IO.MemoryStream();
                }
                mSerializeStream.SetLength(0);
                mMQTTParse.Write(msg, mSerializeStream, request.Session);
                byte[] data = new byte[mSerializeStream.Length];
                mSerializeStream.Position = 0;
                mSerializeStream.Read(data, 0, data.Length);
                return new ArraySegment<byte>(data, 0, data.Length);
            }
            else
            {
                throw new BXException($"{body} message type notfound!");
            }
        }
    }
}
