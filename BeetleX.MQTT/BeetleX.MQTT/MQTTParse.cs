using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace BeetleX.MQTT
{
    public class MQTTParse
    {
        private Int7bit mInt7BitHandler = new Int7bit();

        [ThreadStatic]
        private static System.IO.MemoryStream mProtocolStream;


        private bool mDUP;

        private QoSType mQoS;

        private bool mRetain;

        private MQTTMessageType? mType;

        private int? mLength;

        private MQTTMessage CreateMessage(MQTTMessageType type)
        {
            switch (type)
            {
                case MQTTMessageType.CONNACK:
                    return new Messages.CONNACK();
                case MQTTMessageType.CONNECT:
                    return new Messages.CONNECT();
                case MQTTMessageType.DISCONNECT:
                    return new Messages.DISCONNECT();
                case MQTTMessageType.PINGREQ:
                    return new Messages.PINGREQ();
                case MQTTMessageType.PINGRESP:
                    return new Messages.PINGRESP();
                case MQTTMessageType.PUBACK:
                    return new Messages.PUBACK();
                case MQTTMessageType.PUBCOMP:
                    return new Messages.PUBCOMP();
                case MQTTMessageType.PUBLISH:
                    return new Messages.PUBLISH();
                case MQTTMessageType.PUBREC:
                    return new Messages.PUBREC();
                case MQTTMessageType.PUBREL:
                    return new Messages.PUBREL();
                case MQTTMessageType.Reserved0:
                    return new Messages.RESERVED0();
                case MQTTMessageType.Reserved15:
                    return new Messages.RESERVED15();
                case MQTTMessageType.SUBACK:
                    return new Messages.SUBACK();
                case MQTTMessageType.SUBSCRIBE:
                    return new Messages.SUBSCRIBE();
                case MQTTMessageType.UNSUBACK:
                    return new Messages.UNSUBACK();
                case MQTTMessageType.UNSUBSCRIBE:
                    return new Messages.UNSUBSCRIBE();
                default:
                    throw new BXException($"{type} message type notfound!");
            }
        }

        [ThreadStatic]
        private static byte[] mCopyBuffer;

        private void CopyStream(Stream source, Stream target, int length)
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

        private System.IO.MemoryStream GetProtocolStream()
        {
            if (mProtocolStream == null)
            {
                mProtocolStream = new System.IO.MemoryStream();
            }
            mProtocolStream.SetLength(0);
            return mProtocolStream;
        }

        public MQTTMessage Read(BeetleX.Buffers.PipeStream stream, ISession session)
        {
            IServer server = session.Server;
            if (stream.Length > 0)
            {
                if (mType == null)
                {
                    var value = stream.ReadByte();
                    if (value < 0)
                    {
                        throw new BXException("parse mqtt message error:fixed header byte value cannot be less than zero!");
                    }
                    mType = (MQTTMessageType)((value & 0b1111_0000) >> 4);
                    mDUP = ((value & 0b0000_1000) >> 3) > 0;
                    mQoS = (QoSType)((value & 0b0000_0110) >> 1);
                    mRetain = ((value & 0b0000_0001) >> 0) > 0;
                    if (server.EnableLog(EventArgs.LogType.Debug))
                        server.Log(EventArgs.LogType.Debug, session, "parse mqtt header souccess");

                }
                if (mLength == null)
                {
                    mLength = mInt7BitHandler.Read(stream);
                    if (server.EnableLog(EventArgs.LogType.Debug))
                        server.Log(EventArgs.LogType.Debug, session, $"parse mqtt size {mLength}");
                }
                if (mLength != null && stream.Length >= mLength.Value)
                {
                    Stream protocolStream = GetProtocolStream();
                    CopyStream(stream, protocolStream, mLength.Value);
                    MQTTMessage msg = CreateMessage(mType.Value);
                    msg.DUP = mDUP;
                    msg.QoS = mQoS;
                    msg.Retain = mRetain;
                    msg.Read(mProtocolStream, session);
                    if (server.EnableLog(EventArgs.LogType.Debug))
                        server.Log(EventArgs.LogType.Debug, session, $"parse mqtt type {msg} success");
                    mLength = null;
                    mType = null;
                    return msg;
                }

            }
            return null;
        }

        public void Write(MQTTMessage msg, BeetleX.Buffers.PipeStream stream, ISession session)
        {
            IServer server = session.Server;
            if (server.EnableLog(EventArgs.LogType.Debug))
                server.Log(EventArgs.LogType.Debug, session, $"write mqtt message {msg}");
            var protocolStream = GetProtocolStream();
            int header = 0;
            header |= ((int)msg.Type << 4);
            if (msg.DUP)
            {
                header |= (1 << 3);
            }
            header |= ((int)msg.QoS << 1);
            if (msg.Retain)
            {
                header |= 1;
            }
            stream.WriteByte((byte)header);
            msg.Write(protocolStream, session);
            if (server.EnableLog(EventArgs.LogType.Debug))
                server.Log(EventArgs.LogType.Debug, session, $"write mqtt message body size {protocolStream.Length}");
            mInt7BitHandler.Write(stream, (int)protocolStream.Length);
            protocolStream.Position = 0;
            protocolStream.CopyTo(stream);
            if (server.EnableLog(EventArgs.LogType.Debug))
                server.Log(EventArgs.LogType.Debug, session, $"write mqtt message success");
        }
    }
}
