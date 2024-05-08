
using BeetleX.MQTT.Protocols;
using BeetleX.MQTT.Protocols.V3.Messages;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace BeetleX.MQTT
{
    public class MQTTParseV3 : MQTTParse
    {


        private Int7bit mInt7BitHandler = new Int7bit();


        private MQTTMessageType? mType;

        private int? mLength;

        private byte mHeader;

        protected virtual ConnAck GetCONNACK(ISession session) => new ConnAck();
        protected virtual Connect GetCONNECT(ISession session) => new Connect();
        protected virtual Disconnect GetDISCONNECT(ISession session) => new Disconnect();
        protected virtual PingReq GetPINGREQ(ISession session) => new PingReq();
        protected virtual PingResp GetPINGRESP(ISession session) => new PingResp();

        protected virtual PubAck GetPUBACK(ISession session) => new PubAck();

        protected virtual PubComp GetUBCOMP(ISession session) => new PubComp();

        protected virtual Publish GetPUBLISH(ISession session) => new Publish();

        protected virtual PubRec GetPUBREC(ISession session) => new PubRec();

        protected virtual PubRel GetPUBREL(ISession session) => new PubRel();

        protected virtual RESERVED0 GetRESERVED0(ISession session) => new RESERVED0();

        protected virtual RESERVED15 GetRESERVED15(ISession session) => new RESERVED15();

        protected virtual SubAck GetSUBACK(ISession session) => new SubAck();

        protected virtual Subscribe GetSUBSCRIBE(ISession session) => new Subscribe();

        protected virtual UnSubAck GetUNSUBACK(ISession session) => new UnSubAck();

        protected virtual UnSubscribe GetUNSUBSCRIBE(ISession session) => new UnSubscribe();

        protected override MQTTMessage CreateMessage(MQTTMessageType type, ISession session)
        {
            switch (type)
            {
                case MQTTMessageType.ConnectAck:
                    return GetCONNACK(session);
                case MQTTMessageType.Connect:
                    return GetCONNECT(session);
                case MQTTMessageType.Disconnect:
                    return GetDISCONNECT(session);
                case MQTTMessageType.PingReq:
                    return GetPINGREQ(session);
                case MQTTMessageType.PingResp:
                    return GetPINGRESP(session);
                case MQTTMessageType.PubAck:
                    return GetPUBACK(session);
                case MQTTMessageType.PubComp:
                    return GetUBCOMP(session);
                case MQTTMessageType.Publish:
                    return GetPUBLISH(session);
                case MQTTMessageType.PubRec:
                    return GetPUBREC(session);
                case MQTTMessageType.PubRel:
                    return GetPUBREL(session);
                case MQTTMessageType.Reserved0:
                    return GetRESERVED0(session);
                case MQTTMessageType.Auth:
                    return GetRESERVED15(session);
                case MQTTMessageType.SubAck:
                    return GetSUBACK(session);
                case MQTTMessageType.Subscribe:
                    return GetSUBSCRIBE(session);
                case MQTTMessageType.UnSubAck:
                    return GetUNSUBACK(session);
                case MQTTMessageType.UnSubscribe:
                    return GetUNSUBSCRIBE(session);
                default:
                    throw new BXException($"{type} message type notfound!");
            }
        }

        public override MQTTMessage Read(Stream stream, ISession session)
        {
            IServer server = session?.Server;
            if (stream.Length > 0)
            {
                if (mType == null)
                {
                    mHeader = (byte)stream.ReadByte();
                    if (mHeader < 0)
                    {
                        throw new BXException("parse mqtt message error:fixed header byte value cannot be less than zero!");
                    }
                    mType = (MQTTMessageType)((mHeader & 0b1111_0000) >> 4);
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
                    msg.Bit1 = (byte)(BIT_1 & mHeader);
                    msg.Bit2 = (byte)(BIT_2 & mHeader);
                    msg.Bit3 = (byte)(BIT_3 & mHeader);
                    msg.Bit4 = (byte)(BIT_4 & mHeader);
                    msg.Read(this, protocolStream, session);
                    if (server != null && server.EnableLog(EventArgs.LogType.Debug))
                        server.Log(EventArgs.LogType.Debug, session, $"parse mqtt type {msg} success");
                    mLength = null;
                    mType = null;
                    return msg;
                }

            }
            return null;
        }

        public override void Write(MQTTMessage msg, Stream stream, ISession session)
        {
            IServer server = session?.Server;
            if (server != null && server.EnableLog(EventArgs.LogType.Debug))
                server.Log(EventArgs.LogType.Debug, session, $"write mqtt message {msg}");
            var protocolStream = GetProtocolStream();
            int header = 0;
            header |= ((int)msg.Type << 4);
            header |= msg.Bit1;
            header |= msg.Bit2 << 1;
            header |= msg.Bit3 << 2;
            header |= msg.Bit4 << 3;
            stream.WriteByte((byte)header);
            msg.Write(this, protocolStream, session);
            if (server != null && server.EnableLog(EventArgs.LogType.Debug))
                server.Log(EventArgs.LogType.Debug, session, $"write mqtt message body size {protocolStream.Length}");
            mInt7BitHandler.Write(stream, (int)protocolStream.Length);
            protocolStream.Position = 0;
            protocolStream.CopyTo(stream);
            if (server != null && server.EnableLog(EventArgs.LogType.Debug))
                server.Log(EventArgs.LogType.Debug, session, $"write mqtt message success");
        }
    }

}
