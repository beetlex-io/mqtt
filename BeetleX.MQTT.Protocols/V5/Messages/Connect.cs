using BeetleX.MQTT.Protocols.V5.Headers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using BeetleX.Buffers;

namespace BeetleX.MQTT.Protocols.V5.Messages
{
    public class Connect : MQTTMessage
    {
        public override MQTTMessageType Type => MQTTMessageType.Connect;

        protected override void OnRead(MQTTParse parse, Stream stream, ISession session)
        {
            base.OnRead(parse, stream, session);
            ProtocolName = parse.ReadString(stream);
            Version = (byte)stream.ReadByte();
            byte flats = (byte)stream.ReadByte();
            Reserved = (MQTTParse.BIT_1 & flats) > 0;
            ClearStart = (MQTTParse.BIT_2 & flats) > 0;
            WillFalg = (MQTTParse.BIT_3 & flats) > 0;
            WillRetian = (MQTTParse.BIT_6 & flats) > 0;
            PasswordFlag = (MQTTParse.BIT_7 & flats) > 0;
            UserFlag = (MQTTParse.BIT_8 & flats) > 0;
            this.WillQos = (QoSType)((0b0001_1000 & flats) >> 3);
            KeepAlive = parse.ReadUInt16(stream);
            var ps = GetPropertiesStream();
            ps.Read(parse, stream);
            SessionExpiryInterval = ps;
            ReceiveMaximum = ps;
            MaximumPacketSize = ps;
            TopicAliasMaximum = ps;
            RequestResponseInformation = ps;
            RequestProblemInformation = ps;
            AuthenticationMethod = ps;
            AuthenticationData = ps;
            UserProperties = ps;
            ClientID = parse.ReadString(stream);
            if (WillFalg)
            {
                this.WillProperty.Read(this, parse, stream);
                WillTopic = parse.ReadString(stream);
                WillPayload = parse.ReadBinary(stream);
            }

            if (UserFlag)
                Name = parse.ReadString(stream);

            if (PasswordFlag)
                Password = parse.ReadString(stream);
        }

        protected override void OnWrite(MQTTParse parse, Stream stream, ISession session)
        {
            base.OnWrite(parse, stream, session);
            parse.WriteString(stream, ProtocolName);
            stream.WriteByte(Version);
            byte flats = 0x0;
            if (Reserved)
                flats |= MQTTParse.BIT_1;
            if (ClearStart)
                flats |= MQTTParse.BIT_2;
            if (WillFalg)
                flats |= MQTTParse.BIT_3;
            flats |= (byte)((byte)this.WillQos << 3);
            if (this.WillRetian)
                flats |= MQTTParse.BIT_6;
            if (this.UserFlag)
                flats |= MQTTParse.BIT_7;
            if (this.PasswordFlag)
                flats |= MQTTParse.BIT_8;
            stream.WriteByte(flats);
            parse.WriteUInt16(stream, KeepAlive);

            var ps = GetPropertiesStream()
                + SessionExpiryInterval
                + ReceiveMaximum
                + MaximumPacketSize
                + TopicAliasMaximum
                + RequestResponseInformation
                + RequestProblemInformation
                + AuthenticationMethod
                + AuthenticationData
                + UserProperties;

            ps.Write(parse, stream);

            parse.WriteString(stream, ClientID);
            if (WillFalg)
            {
                this.WillProperty.Write(this, parse, stream);
                parse.WriteString(stream, WillTopic);
                if (WillPayload == null)
                    WillPayload = new byte[0];
                parse.WriteBinary(stream, WillPayload);
            }


            if (UserFlag)
                parse.WriteString(stream, Name);
            if (PasswordFlag)
                parse.WriteString(stream, Password);

        }


        public Connect SetWillPayload(byte[] payload)
        {
            WillProperty.PayloadFormatIndicator = new PayloadFormatIndicator();
            WillProperty.PayloadFormatIndicator.Value = 0x01;
            WillPayload = payload;
            return this;
        }


        public Connect SetUser(string name, string password)
        {
            this.UserFlag = true;
            this.PasswordFlag = true;
            this.Name = name;
            this.Password = password;
            return this;
        }

        /// <summary>
        /// 保留
        /// </summary>
        public bool Reserved { get; set; }

        /// <summary>
        /// 协议名称
        /// </summary>
        public string ProtocolName { get; set; } = "MQTT";

        /// <summary>
        /// 服务版本
        /// </summary>
        public byte Version { get; set; } = 0x05;

        /// <summary>
        /// 用户标识
        /// </summary>
        public bool UserFlag { get; set; }
        /// <summary>
        /// 密码标识
        /// </summary>
        public bool PasswordFlag { get; set; }
        /// <summary>
        /// 遗嘱保留消息
        /// </summary>
        public bool WillRetian { get; set; }
        /// <summary>
        /// 遗嘱QoS
        /// </summary>
        public QoSType WillQos { get; set; }
        /// <summary>
        /// 遗嘱标识
        /// </summary>
        public bool WillFalg { get; set; }
        /// <summary>
        /// 全新开始
        /// </summary>
        public bool ClearStart { get; set; }
        /// <summary>
        /// 过期时间
        /// </summary>
        public ushort KeepAlive { get; set; }

        public string ClientID { get; set; }

        public string WillTopic { get; set; }

        public byte[] WillPayload { get; set; }

        public string Name { get; set; }

        public string Password { get; set; }


        public SessionExpiryInterval SessionExpiryInterval { get; set; }

        public ReceiveMaximum ReceiveMaximum { get; set; }

        public MaximumPacketSize MaximumPacketSize { get; set; }

        public TopicAliasMaximum TopicAliasMaximum { get; set; }

        public RequestResponseInformation RequestResponseInformation { get; set; }

        public RequestProblemInformation RequestProblemInformation { get; set; }

        public AuthenticationMethod AuthenticationMethod { get; set; }

        public AuthenticationData AuthenticationData { get; set; }

        public List<UserProperty> UserProperties { get; set; }



        public Connect SetWill(string topic, byte[] payload, Action<WillProperties> setting)
        {

            WillFalg = true;
            WillTopic = topic;
            WillPayload = payload;
            setting?.Invoke(WillProperty);
            return this;
        }

        public WillProperties WillProperty { get; set; } = new WillProperties();

        public class WillProperties
        {

            public WillDelayInterval WillDelayInterval { get; set; }

            public PayloadFormatIndicator PayloadFormatIndicator { get; set; }

            public MessageExpiryInterval MessageExpiryInterval { get; set; }

            public ContentType ContentType { get; set; }

            public ResponseTopic ResponseTopic { get; set; }

            public List<UserProperty> UserProperties { get; set; }

            public void Read(Connect connect, MQTTParse parse, Stream stream)
            {
                var ps = connect.GetPropertiesStream();
                ps.Read(parse, stream);
                WillDelayInterval = ps;
                PayloadFormatIndicator = ps;
                MessageExpiryInterval = ps;
                ContentType = ps;
                ResponseTopic = ps;
                UserProperties = ps;

            }

            public void Write(Connect connect, MQTTParse parse, Stream stream)
            {
                var ps = connect.GetPropertiesStream()
                    + WillDelayInterval
                    + PayloadFormatIndicator
                    + MessageExpiryInterval
                    + ContentType
                    + ResponseTopic
                    + UserProperties;
                ps.Write(parse, stream);
            }
        }

    }



}
