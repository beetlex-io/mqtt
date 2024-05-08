using System;
using System.Collections.Generic;
using System.Text;

namespace BeetleX.MQTT.Protocols.V5.Headers
{
    public enum HeaderType : byte
    {
        PayloadFormatIndicator = 0x01,
        MessageExpiryInterval = 0x02,
        ContentType = 0x03,
        ResponseTopic = 0x08,
        CorrelationData = 0x09,
        SubscriptionIdentifier = 0x0B,
        SessionExpiryInterval = 0x11,
        AssignedClientIdentifier = 0x12,
        ServerKeepAlive = 0x13,
        AuthenticationMethod = 0x15,
        AuthenticationData = 0x16,
        RequestProblemInformation = 0x17,
        WillDelayInterval = 0x18,
        RequestResponseInformation = 0x19,
        ResponseInformation = 0x1A,
        ServerReference = 0x1C,
        ReasonString = 0x1F,
        ReceiveMaximum = 0x21,
        TopicAliasMaximum = 0x22,
        TopicAlias = 0x23,
        MaximumQoS = 0x24,
        RetainAvailable = 0x25,
        UserProperty = 0x26,
        MaximumPacketSize = 0x27,
        WildcardSubscriptionAvailable = 0x28,
        SubscriptionIdentifierAvailable = 0x29,
        SharedSubscriptionAvailable = 0x2A

    }
}
