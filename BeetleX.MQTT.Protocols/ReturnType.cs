using System;
using System.Collections.Generic;
using System.Text;

namespace BeetleX.MQTT.Protocols
{
    public enum ReturnType : byte
    {
        /// <summary>
        /// CONNACK, PUBACK, PUBREC, PUBREL, PUBCOMP, UNSUBACK, AUTH,DISCONNECT,SUBACK
        /// </summary>
        Success = 0x00,
        /// <summary>
        /// SUBACK
        /// </summary>
        GrantedQoS1 = 0x01,
        /// <summary>
        /// SUBACK
        /// </summary>
        GrantedQoS2 = 0x02,
        /// <summary>
        /// DISCONNECT
        /// </summary>
        DisconnectWithWillMessage = 0x04,
        /// <summary>
        /// PUBACK, PUBREC
        /// </summary>
        NoMatchingSubscribers = 0x010,
        /// <summary>
        /// UNSUBACK
        /// </summary>
        NoSubscriptionExisted = 0x11,
        /// <summary>
        /// AUTH
        /// </summary>
        ContinueAuthentication = 0x18,
        /// <summary>
        /// AUTH
        /// </summary>
        ReAuthenticate = 0x19,
        /// <summary>
        /// CONNACK, PUBACK, PUBREC, SUBACK, UNSUBACK, DISCONNECT
        /// </summary>
        UnspecifiedError = 0x80,
        /// <summary>
        /// CONNACK, DISCONNECT
        /// </summary>
        MalformedPacket = 0x81,
        /// <summary>
        /// CONNACK, DISCONNECT
        /// </summary>
        ProtocolError = 0x82,
        /// <summary>
        /// CONNACK, PUBACK, PUBREC, SUBACK, UNSUBACK, DISCONNECT
        /// </summary>
        ImplementationSpecificError = 0x83,
        /// <summary>
        /// CONNACK
        /// </summary>
        UnsupportedProtocolVersion = 0x84,
        /// <summary>
        /// CONNACK
        /// </summary>
        ClientIdentifierNotValid = 0x85,
        /// <summary>
        /// CONNACK
        /// </summary>
        BadUserNameOrPassword = 0x86,
        /// <summary>
        /// CONNACK, PUBACK, PUBREC, SUBACK, UNSUBACK, DISCONNECT
        /// </summary>
        NotAuthorized = 0x87,
        /// <summary>
        /// CONNACK
        /// </summary>
        ServerUnavailable = 0x88,
        /// <summary>
        /// CONNACK, DISCONNECT
        /// </summary>
        ServerBusy = 0x89,
        /// <summary>
        /// DISCONNECT
        /// </summary>
        Banned = 0x8A,
        /// <summary>
        /// CONNACK, DISCONNECT
        /// </summary>
        ServerShuttingDown = 0x8B,
        /// <summary>
        /// CONNACK, DISCONNECT
        /// </summary>
        BadAuthenticationMethod = 0x8C,
        /// <summary>
        /// DISCONNECT
        /// </summary>
        KeepAliveTimeout = 0x8D,
        /// <summary>
        /// DISCONNECT
        /// </summary>
        SessionTakenOver = 0x8E,
        /// <summary>
        /// SUBACK, UNSUBACK, DISCONNECT
        /// </summary>
        TopicFilterInvalid = 0x8F,
        /// <summary>
        /// CONNACK, PUBACK, PUBREC, DISCONNECT
        /// </summary>
        TopicNameInvalid = 0x90,
        /// <summary>
        /// PUBACK, PUBREC, SUBACK, UNSUBACK
        /// </summary>
        PacketIdentifierInUse = 0x91,
        /// <summary>
        /// PUBREL, PUBCOMP
        /// </summary>
        PacketIdentifierNotFound = 0x92,
        /// <summary>
        /// DISCONNECT
        /// </summary>
        ReceiveMaximumExceeded = 0x93,
        /// <summary>
        /// DISCONNECT
        /// </summary>
        TopicAliasInvalid = 0x94,
        /// <summary>
        /// CONNACK, DISCONNECT
        /// </summary>
        PacketTooLarge = 0x95,
        /// <summary>
        /// DISCONNECT
        /// </summary>
        MessageTateTooHigh = 0x96,
        /// <summary>
        /// CONNACK, PUBACK, PUBREC, SUBACK, DISCONNECT
        /// </summary>
        QuotaExceeded = 0x97,
        /// <summary>
        /// DISCONNECT
        /// </summary>
        AdministrativeAction = 0x98,
        /// <summary>
        /// CONNACK, PUBACK, PUBREC, DISCONNECT
        /// </summary>
        PayloadFormatInvalid = 0x99,
        /// <summary>
        /// CONNACK, DISCONNECT
        /// </summary>
        RetainNotSupported = 0x9A,
        /// <summary>
        /// CONNACK, DISCONNECT
        /// </summary>
        QoSNotSupported = 0x9B,
        /// <summary>
        /// CONNACK, DISCONNECT
        /// </summary>
        UseAnotherServer = 0x9C,
        /// <summary>
        /// CONNACK, DISCONNECT
        /// </summary>
        ServerMoved = 0x9D,
        /// <summary>
        /// SUBACK, DISCONNECT
        /// </summary>
        SharedSubscriptionsNotSupported = 0x9E,
        /// <summary>
        /// CONNACK, DISCONNECT
        /// </summary>
        ConnectionRateExceeded = 0x9F,
        /// <summary>
        /// DISCONNECT
        /// </summary>
        MaximumConnectTime = 0xA0,
        /// <summary>
        /// SUBACK, DISCONNECT
        /// </summary>
        SubscriptionIdentifiersNotSupported = 0xA1,
        /// <summary>
        /// SUBACK, DISCONNECT
        /// </summary>
        WildcardSubscriptionsNotSupported = 0xA2
    }
}
