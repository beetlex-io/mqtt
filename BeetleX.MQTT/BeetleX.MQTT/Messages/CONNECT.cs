using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace BeetleX.MQTT.Messages
{
    public class CONNECT : MQTTMessage
    {

        public CONNECT(string clientid)
        {
            ClientID = clientid;
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

        public bool WillQoSFlag { get; set; }

        public bool WillRetainFlag { get; set; }

        public bool PasswordFlag { get; set; }

        public bool UserNameFlag { get; set; }

        public string ClientID { get; set; }

        public string WillTopic { get; set; }

        public string WillMessage { get; set; }

        public string UserName { get; set; }

        public string Password { get; set; }

        protected override void OnRead(Stream stream, ISession session)
        {
            base.OnRead(stream, session);
        }

        protected override void OnWrite(Stream stream, ISession sessioni)
        {
            base.OnWrite(stream, sessioni);
        }
    }
}
