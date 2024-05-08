using BeetleX.Buffers;
using BeetleX.FastHttpApi;
using BeetleX.FastHttpApi.WebSockets;
using BeetleX.MQTT.Protocols;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeetleX.MQTT.Server
{
    public class MQTTFormater<MQTTPARSE> : BeetleX.FastHttpApi.WebSockets.IDataFrameSerializer
        where MQTTPARSE : MQTTParse, new()
    {


        private MQTTParse GetQTTParse(ISession session)
        {
            MQTTParse result = (MQTTParse)session["mqtt-parse"];
            if (result == null)
            {
                result = new MQTTPARSE();
                session["mqtt-parse"] = result;
                return result;
            }
            return result;
        }
        public object FrameDeserialize(DataFrame data, PipeStream stream, HttpRequest request)
        {
            MemoryStream protocolMemory = (MemoryStream)request.Session["_protocolMemory"];
            if (protocolMemory == null)
            {
                protocolMemory = new MemoryStream();
                request.Session["_protocolMemory"] = protocolMemory;
            }
            stream.CopyTo(protocolMemory);

            List<MQTTMessage> items = new List<MQTTMessage>();
            if (protocolMemory.Length >= 2)
            {
                while (true)
                {
                    var msg = GetQTTParse(request.Session).Read(protocolMemory, request.Session);
                    if (msg == null)
                        break;
                    items.Add(msg);

                }
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
                GetQTTParse(request.Session).Write(msg, mSerializeStream, request.Session);
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
