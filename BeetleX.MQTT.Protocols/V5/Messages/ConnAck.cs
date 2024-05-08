using BeetleX.MQTT.Protocols.V5.Headers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace BeetleX.MQTT.Protocols.V5.Messages
{
    public class ConnAck : MQTTMessage
    {
        public ConnAck(
            )
        {

        }

        public byte SessionFlag { get; set; }

        public ReturnType Status { get; set; } = ReturnType.Success;

        protected override void OnRead(MQTTParse parse, Stream stream, ISession session)
        {
            base.OnRead(parse, stream, session);
            SessionFlag = (byte)stream.ReadByte();
            Status = (ReturnType)stream.ReadByte();
            var ps = GetPropertiesStream();
            ps.Read(parse, stream);
            SessionExpiryInterval = ps;
            ReceiveMaximum = ps;
            MaximumQoS = ps;
            RetainAvailable = ps;
            AssignedClientIdentifier = ps;
            TopicAliasMaximum = ps;
            ReasonString = ps;
            UserProperties = ps;
            WildcardSubscriptionAvailable = ps;
            SubscriptionIdentifierAvailable = ps;
            SharedSubscriptionAvailable = ps;
            ServerKeepAlive = ps;
            ResponseInformation = ps;
            ServerReference = ps;
            AuthenticationData = ps;
        }

        protected override void OnWrite(MQTTParse parse, Stream stream, ISession session)
        {
            base.OnWrite(parse, stream, session);
            stream.WriteByte(SessionFlag);
            stream.WriteByte((byte)Status);
            var ps = GetPropertiesStream()
                + SessionExpiryInterval
                + ReceiveMaximum
                + MaximumQoS
                + RetainAvailable
                + AssignedClientIdentifier
                + TopicAliasMaximum 
                + ReasonString 
                + WildcardSubscriptionAvailable 
                + SubscriptionIdentifierAvailable 
                + SharedSubscriptionAvailable 
                + ServerKeepAlive 
                + ResponseInformation 
                + ServerReference 
                + AuthenticationData 
                + UserProperties;
            ps.Write(parse, stream);
        }

        #region properties
        public SessionExpiryInterval SessionExpiryInterval { get; set; }

        public ReceiveMaximum ReceiveMaximum { get; set; }

        public MaximumQoS MaximumQoS { get; set; }

        public RetainAvailable RetainAvailable { get; set; }

        public AssignedClientIdentifier AssignedClientIdentifier { get; set; }

        public TopicAliasMaximum TopicAliasMaximum { get; set; }

        public ReasonString ReasonString { get; set; }

        public List<UserProperty> UserProperties { get; set; } = new List<UserProperty>();

        public WildcardSubscriptionAvailable WildcardSubscriptionAvailable { get; set; }

        public SubscriptionIdentifierAvailable SubscriptionIdentifierAvailable { get; set; }

        public SharedSubscriptionAvailable SharedSubscriptionAvailable { get; set; }

        public ServerKeepAlive ServerKeepAlive { get; set; }

        public ResponseInformation ResponseInformation { get; set; }

        public ServerReference ServerReference { get; set; }

        public AuthenticationData AuthenticationData { get; set; }

        public override MQTTMessageType Type => MQTTMessageType.ConnectAck;

        #endregion

    }
}
