using BeetleX.Buffers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace BeetleX.MQTT
{
    public abstract class MQTTMessage
    {

        public MQTTMessage()
        {

        }

        public abstract MQTTMessageType Type { get; }

        public bool DUP { get; set; }

        public QoSType QoS { get; set; } = QoSType.MostOnce;

        public bool Retain { get; set; }


        [ThreadStatic]
        private static byte[] mIntBuffer;

        private byte[] GetIntBuffer()
        {
            if (mIntBuffer == null)
                mIntBuffer = new byte[8];
            return mIntBuffer;
        }

        public static Func<int, byte[]> RentPayloadBufferHandler { get; set; }

        public byte[] RentPayloadBuffer(int count)
        {
            if (RentPayloadBufferHandler != null)
            {
                return RentPayloadBufferHandler(count);
            }

            return new byte[count];
        }

        public static Action<byte[]> ReturnPayloadBufferHandler { get; set; }
        public void ReturnPayloadBuffer(byte[] data)
        {
            ReturnPayloadBufferHandler?.Invoke(data);
        }

        protected virtual void WriteString(System.IO.Stream stream, string value, Encoding encoding = null)
        {
            if (encoding == null)
                encoding = Encoding.UTF8;
            if (string.IsNullOrEmpty(value))
            {
                WriteUInt16(stream, 0);
                return;
            }

            byte[] buffer = System.Buffers.ArrayPool<byte>.Shared.Rent(value.Length * 6);
            var count = encoding.GetBytes(value, 0, value.Length, buffer, 0);
            WriteUInt16(stream, (ushort)count);
            stream.Write(buffer, 0, count);
            System.Buffers.ArrayPool<byte>.Shared.Return(buffer);

        }

        protected virtual string ReadString(System.IO.Stream stream, Encoding encoding = null)
        {
            if (encoding == null)
                encoding = Encoding.UTF8;
            UInt16 len = ReadUInt16(stream);
            if (len == 0)
                return string.Empty;
            byte[] buffer = System.Buffers.ArrayPool<byte>.Shared.Rent(len);
            stream.Read(buffer, 0, len);
            string result = encoding.GetString(buffer, 0, len);
            System.Buffers.ArrayPool<byte>.Shared.Return(buffer);
            return result;
        }

        protected virtual void WriteUInt16(System.IO.Stream stream, ushort value)
        {
            value = BitHelper.SwapUInt16(value);
            var data = BitConverter.GetBytes(value);
            stream.Write(data, 0, data.Length);
        }

        protected virtual ushort ReadUInt16(System.IO.Stream stream)
        {
            var buffer = GetIntBuffer();
            stream.Read(buffer, 0, 2);
            var result = BitConverter.ToUInt16(buffer, 0);
            return BitHelper.SwapUInt16(result);
        }

        protected virtual void WriteInt16(System.IO.Stream stream, short value)
        {
            value = BitHelper.SwapInt16(value);
            var data = BitConverter.GetBytes(value);
            stream.Write(data, 0, data.Length);
        }

        protected virtual short ReadInt16(System.IO.Stream stream)
        {
            var buffer = GetIntBuffer();
            stream.Read(buffer, 0, 2);
            var result = BitConverter.ToInt16(buffer, 0);
            return BitHelper.SwapInt16(result);
        }


        protected virtual void WriteInt(System.IO.Stream stream, int value)
        {
            value = BitHelper.SwapInt32(value);
            var data = BitConverter.GetBytes(value);
            stream.Write(data, 0, data.Length);
        }

        protected virtual int ReadInt(System.IO.Stream stream)
        {
            var buffer = GetIntBuffer();
            stream.Read(buffer, 0, 4);
            var result = BitConverter.ToInt32(buffer, 0);
            return BitHelper.SwapInt32(result);

        }



        internal void Read(System.IO.Stream stream, ISession session)
        {
            OnRead(stream, session);
        }

        protected virtual void OnRead(Stream stream, ISession session)
        {

        }

        internal void Write(System.IO.Stream stream, ISession session)
        {
            OnWrite(stream, session);
        }

        protected virtual void OnWrite(Stream stream, ISession sessioni)
        {

        }


    }
}
