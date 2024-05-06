using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace BeetleX.MQTT.Messages
{
    public class PUBLISH : MQTTMessage
    {
        public override MQTTMessageType Type => MQTTMessageType.PUBLISH;

        public string Topic { get; set; }

        public ushort Identifier { get; set; }

        public ArraySegment<byte> PayLoadData { get; set; }

        public string ToString(Encoding encoding = null)
        {
            if (this.PayLoadData == null)
                return string.Empty;
            if (encoding == null)
                encoding = Encoding.UTF8;
            return encoding.GetString(PayLoadData.Array, 0, PayLoadData.Count);
        }
        public override string ToString()
        {
            return ToString(null);
        }
        public T ToJsonObject<T>()
        {
            if (this.PayLoadData == null)
                return default(T);
            string str = ToString();
            return System.Text.Json.JsonSerializer.Deserialize<T>(str);
        }

        public void SetString(string value, Encoding encoding = null)
        {
            if (encoding == null)
                encoding = Encoding.UTF8;
            int size = value.Length * 6;
            var buffer = RentPayloadBuffer(size);
            var count = encoding.GetBytes(value, 0, value.Length, buffer, 0);
            this.PayLoadData = new ArraySegment<byte>(buffer, 0, count);
        }

        public void SetJson(object value)
        {
            string str = System.Text.Json.JsonSerializer.Serialize(value);
            SetString(str);

        }

        protected override void OnRead(Stream stream, ISession session)
        {
            base.OnRead(stream, session);
            Topic = ReadString(stream);
            if (QoS == QoSType.LeastOnce | QoS == QoSType.ExactlyOnce)
                Identifier = ReadUInt16(stream);
            int length = (int)(stream.Length - stream.Position);
            var buffer = RentPayloadBuffer((int)length);
            stream.Read(buffer, 0, length);
            OnReadHeaders(stream, session);
            PayLoadData = new ArraySegment<byte>(buffer, 0, length);

        }

        protected override void OnWrite(Stream stream, ISession session)
        {
            base.OnWrite(stream, session);
            WriteString(stream, Topic);
            if (QoS == QoSType.LeastOnce | QoS == QoSType.ExactlyOnce)
                WriteUInt16(stream, Identifier);
            OnWriteHeaders(stream, session);
            stream.Write(PayLoadData.Array, PayLoadData.Offset, PayLoadData.Count);
            ReturnPayloadBuffer(PayLoadData.Array);
        }


    }
    public class Node_PUBLISH : PUBLISH
    {

        public int NetClientID { get; set; }
        protected override void OnReadHeaders(Stream stream, ISession session)
        {
            base.OnReadHeaders(stream, session);
            NetClientID = ReadInt(stream);
        }
        protected override void OnWriteHeaders(Stream stream, ISession session)
        {
            base.OnWriteHeaders(stream, session);
            WriteInt(stream, NetClientID);
        }
    }

}
