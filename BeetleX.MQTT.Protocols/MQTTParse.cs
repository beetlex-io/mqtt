using BeetleX.Buffers;
using BeetleX.MQTT.Protocols.V5.Messages;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using static BeetleX.RpsCounter;

namespace BeetleX.MQTT.Protocols
{
    public abstract class MQTTParse
    {

        public const int BIT_1 = 0b0000_0000_0000_0001;

        public const int BIT_2 = 0b0000_0000_0000_0010;

        public const int BIT_3 = 0b0000_0000_0000_0100;

        public const int BIT_4 = 0b0000_0000_0000_1000;

        public const int BIT_5 = 0b0000_0000_0001_0000;

        public const int BIT_6 = 0b0000_0000_0010_0000;

        public const int BIT_7 = 0b0000_0000_0100_0000;

        public const int BIT_8 = 0b0000_0000_1000_0000;

        public const int BIT_9 = 0b0000_0001_0000_0000;

        public const int BIT_10 = 0b0000_0010_0000_0000;

        public const int BIT_11 = 0b0000_0100_0000_0000;

        public const int BIT_12 = 0b0000_1000_0000_0000;

        public const int BIT_13 = 0b0001_0000_0000_0000;

        public const int BIT_14 = 0b0010_0000_0000_0000;

        public const int BIT_15 = 0b0100_0000_0000_0000;

        public const int BIT_16 = 0b1000_0000_0000_0000;

        public Int7bit Int7BitHandler { get; set; } = new Int7bit();

        [ThreadStatic]
        private static System.IO.MemoryStream mProtocolStream;

        private MQTTMessageType? mType;

        private byte mHeaderByte;

        private int? mLength;

        protected virtual T GetMessage<T>(ISession session)
            where T : MQTTMessage, new()
        {
            return new T();
        }

        protected abstract MQTTMessage CreateMessage(MQTTMessageType type, ISession session);


        [ThreadStatic]
        private static byte[] mCopyBuffer;

        public void CopyStream(Stream source, Stream target, int length)
        {
            if (mCopyBuffer == null)
            {
                mCopyBuffer = new byte[1024 * 4];
            }
            while (length > 0)
            {
                int count = length > mCopyBuffer.Length ? mCopyBuffer.Length : length;
                source.Read(mCopyBuffer, 0, count);
                target.Write(mCopyBuffer, 0, count);
                length -= count;
            }
            target.Position = 0;
        }

        protected System.IO.MemoryStream GetProtocolStream()
        {
            if (mProtocolStream == null)
            {
                mProtocolStream = new System.IO.MemoryStream();
            }
            mProtocolStream.SetLength(0);
            return mProtocolStream;
        }

        public abstract MQTTMessage Read(Stream stream, ISession session);


        public abstract void Write(MQTTMessage msg, Stream stream, ISession session);


        public virtual void WriteString(System.IO.Stream stream, string value, Encoding encoding = null)
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

        public virtual string ReadString(System.IO.Stream stream, Encoding encoding = null)
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

        public virtual void WriteUInt16(System.IO.Stream stream, ushort value)
        {
            value = BitHelper.SwapUInt16(value);
            var data = BitConverter.GetBytes(value);
            stream.Write(data, 0, data.Length);
        }

        public virtual ushort ReadUInt16(System.IO.Stream stream)
        {
            var buffer = GetIntBuffer();
            stream.Read(buffer, 0, 2);
            var result = BitConverter.ToUInt16(buffer, 0);
            return BitHelper.SwapUInt16(result);
        }

        public virtual void WriteInt16(System.IO.Stream stream, short value)
        {
            value = BitHelper.SwapInt16(value);
            var data = BitConverter.GetBytes(value);
            stream.Write(data, 0, data.Length);
        }

        public virtual short ReadInt16(System.IO.Stream stream)
        {
            var buffer = GetIntBuffer();
            stream.Read(buffer, 0, 2);
            var result = BitConverter.ToInt16(buffer, 0);
            return BitHelper.SwapInt16(result);
        }


        public virtual void WriteInt(System.IO.Stream stream, int value)
        {
            value = BitHelper.SwapInt32(value);
            var data = BitConverter.GetBytes(value);
            stream.Write(data, 0, data.Length);
        }

        public virtual int ReadInt(System.IO.Stream stream)
        {
            var buffer = GetIntBuffer();
            stream.Read(buffer, 0, 4);
            var result = BitConverter.ToInt32(buffer, 0);
            return BitHelper.SwapInt32(result);

        }

        public void WriteBinary(System.IO.Stream stream, byte[] data)
        {
            WriteUInt16(stream, (ushort)data.Length);
            stream.Write(data, 0, data.Length);

        }

        public byte[] ReadBinary(System.IO.Stream stream)
        {
            var len = ReadUInt16(stream);
            byte[] result = new byte[len];
            stream.Read(result, 0, len);
            return result;
        }


        [ThreadStatic]
        private static byte[] mIntBuffer;

        private byte[] GetIntBuffer()
        {
            if (mIntBuffer == null)
                mIntBuffer = new byte[8];
            return mIntBuffer;
        }


    }


}
