using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace BeetleX.MQTT.Protocols.V3.Messages
{
    public class Connect : MQTTMessage
    {

        public Connect SetClientID(string id)
        {
            ClientID = id;
            return this;
        }

        public Connect SetWill(string topic, string message)
        {
            this.WillTopic = topic;
            this.WillMessage = message;
            return this;
        }

        public Connect SetUser(string username, string password)
        {
            if (!string.IsNullOrEmpty(username))
            {
                this.UserNameFlag = true;
                this.Name = username;
            }
            if (!string.IsNullOrEmpty(password))
            {
                this.PasswordFlag = true;
                this.Password = password;
            }
            return this;
        }
        public override MQTTMessageType Type => MQTTMessageType.Connect;

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

        public string Name { get; set; }

        public string Password { get; set; }

        public short KeepAlive { get; set; }

        protected override void OnRead(MQTTParse parse, Stream stream, ISession session)
        {
            base.OnRead(parse, stream, session);
            ProtocolName = parse.ReadString(stream);
            Level = (byte)stream.ReadByte();
            int flags = stream.ReadByte();
            ReservedFlag = (flags & 0x1) > 0;
            CleanSessionFlag = (flags & 0x2) > 0;
            WillFlag = (flags & 0x4) > 0;
            WillQoSFlag = (QoSType)(flags & 0b0001_1000);
            WillRetainFlag = (flags & 0b0010_0000) > 0;
            PasswordFlag = (flags & 0b0100_0000) > 0;
            UserNameFlag = (flags & 0b1000_0000) > 0;
            KeepAlive = parse.ReadInt16(stream);
            ClientID = parse.ReadString(stream);
            if (WillFlag)
            {
                WillTopic = parse.ReadString(stream);
                WillMessage = parse.ReadString(stream);
            }
            if (UserNameFlag)
                Name = parse.ReadString(stream);
            if (PasswordFlag)
                Password = parse.ReadString(stream);
        }

        protected override void OnWrite(MQTTParse parse, Stream stream, ISession sessioni)
        {
            base.OnWrite(parse, stream, sessioni);
            parse.WriteString(stream, ProtocolName);
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
            parse.WriteInt16(stream, KeepAlive);
            parse.WriteString(stream, ClientID);
            if (WillFlag)
            {
                parse.WriteString(stream, WillTopic);
                parse.WriteString(stream, WillMessage);
            }
            if (UserNameFlag)
            {
                parse.WriteString(stream, Name);
            }
            if (PasswordFlag)
            {
                parse.WriteString(stream, Password);
            }
        }
    }

   
}
