using BeetleX.MQTT.Messages;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace BeetleX.MQTT
{
    public class MQTTParse
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

        private Int7bit mInt7BitHandler = new Int7bit();

        [ThreadStatic]
        private static System.IO.MemoryStream mProtocolStream;


        private bool mDUP;

        private QoSType mQoS;

        private bool mRetain;

        private MQTTMessageType? mType;

        private int? mLength;

        protected virtual CONNACK GetCONNACK(ISession session) => new CONNACK();
        protected virtual CONNECT GetCONNECT(ISession session) => new CONNECT();
        protected virtual DISCONNECT GetDISCONNECT(ISession session) => new DISCONNECT();
        protected virtual PINGREQ GetPINGREQ(ISession session) => new PINGREQ();
        protected virtual PINGRESP GetPINGRESP(ISession session) => new PINGRESP();

        protected virtual PUBACK GetPUBACK(ISession session) => new PUBACK();

        protected virtual PUBCOMP GetUBCOMP(ISession session) => new PUBCOMP();

        protected virtual PUBLISH GetPUBLISH(ISession session) => new PUBLISH();

        protected virtual PUBREC GetPUBREC(ISession session) => new PUBREC();

        protected virtual PUBREL GetPUBREL(ISession session) => new PUBREL();

        protected virtual RESERVED0 GetRESERVED0(ISession session) => new RESERVED0();

        protected virtual RESERVED15 GetRESERVED15(ISession session) => new RESERVED15();

        protected virtual SUBACK GetSUBACK(ISession session) => new SUBACK();

        protected virtual SUBSCRIBE GetSUBSCRIBE(ISession session) => new SUBSCRIBE();

        protected virtual UNSUBACK GetUNSUBACK(ISession session) => new UNSUBACK();

        protected virtual UNSUBSCRIBE GetUNSUBSCRIBE(ISession session) => new UNSUBSCRIBE();

        private MQTTMessage CreateMessage(MQTTMessageType type, ISession session)
        {
            switch (type)
            {
                case MQTTMessageType.CONNACK:
                    return GetCONNACK(session);
                case MQTTMessageType.CONNECT:
                    return GetCONNECT(session);
                case MQTTMessageType.DISCONNECT:
                    return GetDISCONNECT(session);
                case MQTTMessageType.PINGREQ:
                    return GetPINGREQ(session);
                case MQTTMessageType.PINGRESP:
                    return GetPINGRESP(session);
                case MQTTMessageType.PUBACK:
                    return GetPUBACK(session);
                case MQTTMessageType.PUBCOMP:
                    return GetUBCOMP(session);
                case MQTTMessageType.PUBLISH:
                    return GetPUBLISH(session);
                case MQTTMessageType.PUBREC:
                    return GetPUBREC(session);
                case MQTTMessageType.PUBREL:
                    return GetPUBREL(session);
                case MQTTMessageType.Reserved0:
                    return GetRESERVED0(session);
                case MQTTMessageType.Reserved15:
                    return GetRESERVED15(session);
                case MQTTMessageType.SUBACK:
                    return GetSUBACK(session);
                case MQTTMessageType.SUBSCRIBE:
                    return GetSUBSCRIBE(session);
                case MQTTMessageType.UNSUBACK:
                    return GetUNSUBACK(session);
                case MQTTMessageType.UNSUBSCRIBE:
                    return GetUNSUBSCRIBE(session);
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

        public MQTTMessage Read(Stream stream, ISession session)
        {
            IServer server = session?.Server;
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
                    if (server != null && server.EnableLog(EventArgs.LogType.Debug))
                        server.Log(EventArgs.LogType.Debug, session, "parse mqtt header souccess");

                }
                if (mLength == null)
                {
                    mLength = mInt7BitHandler.Read(stream);
                    if (server != null && server.EnableLog(EventArgs.LogType.Debug))
                        server.Log(EventArgs.LogType.Debug, session, $"parse mqtt size {mLength}");
                }
                if (mLength != null && stream.Length >= mLength.Value)
                {
                    Stream protocolStream = GetProtocolStream();
                    CopyStream(stream, protocolStream, mLength.Value);
                    MQTTMessage msg = CreateMessage(mType.Value, session);
                    msg.DUP = mDUP;
                    msg.QoS = mQoS;
                    msg.Retain = mRetain;
                    msg.Read(mProtocolStream, session);
                    if (server != null && server.EnableLog(EventArgs.LogType.Debug))
                        server.Log(EventArgs.LogType.Debug, session, $"parse mqtt type {msg} success");
                    mLength = null;
                    mType = null;
                    return msg;
                }

            }
            return null;
        }

        public void Write(MQTTMessage msg, Stream stream, ISession session)
        {
            IServer server = session?.Server;
            if (server != null && server.EnableLog(EventArgs.LogType.Debug))
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
            if (server != null && server.EnableLog(EventArgs.LogType.Debug))
                server.Log(EventArgs.LogType.Debug, session, $"write mqtt message body size {protocolStream.Length}");
            mInt7BitHandler.Write(stream, (int)protocolStream.Length);
            protocolStream.Position = 0;
            protocolStream.CopyTo(stream);
            if (server != null && server.EnableLog(EventArgs.LogType.Debug))
                server.Log(EventArgs.LogType.Debug, session, $"write mqtt message success");
        }
    }

    public class NodeMQTTParse : MQTTParse
    {
        protected override PUBLISH GetPUBLISH(ISession session)
        {
            return new Node_PUBLISH();
        }
    }

    public class GatewayMQTTParse : MQTTParse
    {
        protected override PUBACK GetPUBACK(ISession session)
        {
            if (session["node"] == null)
                return new PUBACK();
            else
                return new Node_PUBACK();
        }
    }
}
