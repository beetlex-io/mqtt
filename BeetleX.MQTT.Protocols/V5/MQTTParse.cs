using BeetleX.Buffers;
using BeetleX.MQTT.Protocols.V5.Messages;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using static BeetleX.RpsCounter;

namespace BeetleX.MQTT.Protocols.V5
{
    public class MQTTParseV5:MQTTParse
    {

        private MQTTMessageType? mType;

        private byte mHeaderByte;

        private int? mLength;

        protected override MQTTMessage CreateMessage(MQTTMessageType type, ISession session)
        {
            switch (type)
            {
                case MQTTMessageType.ConnectAck:
                    return GetMessage<ConnAck>(session);
                case MQTTMessageType.Connect:
                    return GetMessage<Connect>(session);
                case MQTTMessageType.Disconnect:
                    return GetMessage<Disconnect>(session);
                case MQTTMessageType.PingReq:
                    return GetMessage<PingReq>(session);
                case MQTTMessageType.PingResp:
                    return GetMessage<PingResp>(session);
                case MQTTMessageType.PubAck:
                    return GetMessage<PubAck>(session);
                case MQTTMessageType.PubComp:
                    return GetMessage<PubComp>(session);
                case MQTTMessageType.Publish:
                    return GetMessage<Publish>(session);
                case MQTTMessageType.PubRec:
                    return GetMessage<PubRec>(session);
                case MQTTMessageType.PubRel:
                    return GetMessage<PubRel>(session);
                case MQTTMessageType.Auth:
                    return GetMessage<Auth>(session);
                case MQTTMessageType.SubAck:
                    return GetMessage<SubAck>(session);
                case MQTTMessageType.Subscribe:
                    return GetMessage<Subscribe>(session);
                case MQTTMessageType.UnSubAck:
                    return GetMessage<UnSubAck>(session);
                case MQTTMessageType.UnSubscribe:
                    return GetMessage<UnSubscribe>(session);
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
                    mHeaderByte = (byte)stream.ReadByte();
                    if (mHeaderByte < 0)
                    {
                        throw new BXException("parse mqtt message error:fixed header byte value cannot be less than zero!");
                    }
                    mType = (MQTTMessageType)((mHeaderByte & 0b1111_0000) >> 4);

                    if (server != null && server.EnableLog(EventArgs.LogType.Debug))
                        server.Log(EventArgs.LogType.Debug, session, "parse mqtt header souccess");

                }
                if (mLength == null)
                {
                    mLength = Int7BitHandler.Read(stream);
                    if (server != null && server.EnableLog(EventArgs.LogType.Debug))
                        server.Log(EventArgs.LogType.Debug, session, $"parse mqtt size {mLength}");
                }
                if (mLength != null && stream.Length >= mLength.Value)
                {
                    Stream protocolStream = GetProtocolStream();
                    CopyStream(stream, protocolStream, mLength.Value);
                    MQTTMessage msg = CreateMessage(mType.Value, session);
                    msg.Bit1 = (byte)(mHeaderByte & 0b0000_0001);
                    msg.Bit2 = (byte)((mHeaderByte & 0b0000_0010) >> 1);
                    msg.Bit3 = (byte)((mHeaderByte & 0b0000_0100) >> 2);
                    msg.Bit4 = (byte)((mHeaderByte & 0b0000_1000) >> 3);
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
            header |= (msg.Bit1);
            header |= (msg.Bit2 << 1);
            header |= (msg.Bit3 << 2);
            header |= (msg.Bit4 << 3);

            stream.WriteByte((byte)header);
            msg.Write(this, protocolStream, session);
            if (server != null && server.EnableLog(EventArgs.LogType.Debug))
                server.Log(EventArgs.LogType.Debug, session, $"write mqtt message body size {protocolStream.Length}");
            Int7BitHandler.Write(stream, (int)protocolStream.Length);
            protocolStream.Position = 0;
            protocolStream.CopyTo(stream);
            if (server != null && server.EnableLog(EventArgs.LogType.Debug))
                server.Log(EventArgs.LogType.Debug, session, $"write mqtt message success");
        }

    }


}
