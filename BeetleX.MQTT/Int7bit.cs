using System;
using System.Collections.Generic;
using System.Text;

namespace BeetleX.MQTT
{
    public class Int7bit
    {

        [ThreadStatic]
        private static Byte[] mDataBuffer;

        public void Write(System.IO.Stream stream, int value)
        {

            if (mDataBuffer == null)
                mDataBuffer = new byte[32];
            var count = 0;
            var num = (UInt64)value;
            while (num >= 0x80)
            {
                mDataBuffer[count++] = (Byte)(num | 0x80);
                num >>= 7;
            }
            mDataBuffer[count++] = (Byte)num;
            stream.Write(mDataBuffer, 0, count);
        }

        private uint mResult = 0;

        private byte mBits = 0;

        public int? Read(System.IO.Stream stream)
        {

            Byte b;
            while (true)
            {
                if (stream.Length < 1)
                    return null;
                var bt = stream.ReadByte();
                if (bt < 0)
                {
                    mBits = 0;
                    mResult = 0;
                    throw new BXException("Read 7bit int error:byte value cannot be less than zero!");
                }
                b = (Byte)bt;

                mResult |= (UInt32)((b & 0x7f) << mBits);
                if ((b & 0x80) == 0) break;
                mBits += 7;
                if (mBits >= 32)
                {
                    mBits = 0;
                    mResult = 0;
                    throw new BXException("Read 7bit int error:out of maximum value!");
                }
            }
            mBits = 0;
            var result = mResult;
            mResult = 0;
            return (Int32)result;
        }
    }
}
