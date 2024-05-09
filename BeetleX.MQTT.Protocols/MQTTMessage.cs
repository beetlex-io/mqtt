using BeetleX.Buffers;
using BeetleX.EventArgs;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace BeetleX.MQTT.Protocols
{
    public abstract class MQTTMessage
    {

        public MQTTMessage()
        {

        }

        public abstract MQTTMessageType Type { get; }

        public byte Bit1 { get; set; }

        public byte Bit2 { get; set; }

        public byte Bit3 { get; set; }

        public byte Bit4 { get; set; }

        internal void Read(MQTTParse parse, System.IO.Stream stream, ISession session)
        {
            OnRead(parse, stream, session);
        }

        protected virtual void OnRead(MQTTParse parse, Stream stream, ISession session)
        {

        }

        internal void Write(MQTTParse parse, System.IO.Stream stream, ISession session)
        {
            OnWrite(parse, stream, session);
        }

        protected virtual void OnWrite(MQTTParse parse, Stream stream, ISession session)
        {

        }


        [ThreadStatic]
        private static PropertyStream mPropertiesStream;

        protected PropertyStream GetPropertiesStream()
        {
            if (mPropertiesStream == null)
            {
                mPropertiesStream = new PropertyStream();
            }
            mPropertiesStream.Refresh();
            return mPropertiesStream;
        }


        public static Func<int, byte[]> RentPayloadBufferHandler { get; set; }

        public static byte[] RentPayloadBuffer(int count)
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


      


    }
}
