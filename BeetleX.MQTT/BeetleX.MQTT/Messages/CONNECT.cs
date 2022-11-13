using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace BeetleX.MQTT.Messages
{
    public class CONNECT : MQTTMessage
    {

        public CONNECT SetClientID(string id)
        {
            ClientID = id;
            return this;
        }

        public CONNECT SetWill(string topic, string message)
        {
            this.WillTopic = topic;
            this.WillMessage = message;
            return this;
        }

        public CONNECT SetUser(string username, string password)
        {
            if (!string.IsNullOrEmpty(username))
            {
                this.UserNameFlag = true;
                this.UserName = username;
            }
            if (!string.IsNullOrEmpty(password))
            {
                this.PasswordFlag = true;
                this.Password = password;
            }
            return this;
        }
        public override MQTTMessageType Type => MQTTMessageType.CONNECT;

        public string ProtocolName { get; set; } = "MQTT";

        public byte Level { get; set; }

        public bool ReservedFlag { get; set; }

        public bool CleanSessionFlag { get; set; }

        public bool WillFlag { get; set; }

        public QoSType WillQoSFlag { get; set; } = QoSType.MostOnce;

        public bool WillRetainFlag { get; set; }

        public bool PasswordFlag { get; set; }

        public bool UserNameFlag { get; set; }

        public string ClientID { get; set; }

        public string WillTopic { get; set; }

        public string WillMessage { get; set; }

        public string UserName { get; set; }

        public string Password { get; set; }

        public short KeepAlive { get; set; }

        protected override void OnRead(Stream stream, ISession session)
        {
            base.OnRead(stream, session);
            ProtocolName = ReadString(stream);
            Level = (byte)stream.ReadByte();
            int flags = stream.ReadByte();
            ReservedFlag = (flags & 0x1) > 0;
            CleanSessionFlag = (flags & 0x2) > 0;
            WillFlag = (flags & 0x4) > 0;
            WillQoSFlag = (QoSType)(flags & 0b0001_1000);
            WillRetainFlag = (flags & 0b0010_0000) > 0;
            PasswordFlag = (flags & 0b0100_0000) > 0;
            UserNameFlag = (flags & 0b1000_0000) > 0;
            KeepAlive = ReadInt16(stream);
            ClientID = ReadString(stream);
            if (WillFlag)
            {
                WillTopic = ReadString(stream);
                WillMessage = ReadString(stream);
            }
            if (UserNameFlag)
                UserName = ReadString(stream);
            if (PasswordFlag)
                Password = ReadString(stream);
        }

        protected override void OnWrite(Stream stream, ISession sessioni)
        {
            base.OnWrite(stream, sessioni);
            WriteString(stream, ProtocolName);
            stream.WriteByte(Level);
            int flags = 0;
            if (ReservedFlag)
                flags |= 0x1;
            if (CleanSessionFlag)
                flags |= 0x2;
            if (WillFlag)
                flags |= 0x4;
            flags |= (int)WillQoSFlag;
            if (WillRetainFlag)
                flags |= 0b0010_0000;
            if (PasswordFlag)
                flags |= 0b0100_0000;
            if (UserNameFlag)
                flags |= 0b1000_0000;
            stream.WriteByte((byte)flags);
            WriteInt16(stream, KeepAlive);
            WriteString(stream, ClientID);
            if (WillFlag)
            {
                WriteString(stream, WillTopic);
                WriteString(stream, WillMessage);
            }
            if (UserNameFlag)
            {
                WriteString(stream, UserName);
            }
            if (PasswordFlag)
            {
                WriteString(stream, Password);
            }
        }
    }
}
