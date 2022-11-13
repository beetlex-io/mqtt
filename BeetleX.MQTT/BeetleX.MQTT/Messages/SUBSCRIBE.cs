﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace BeetleX.MQTT.Messages
{
    public class SUBSCRIBE : MQTTMessage
    {
        public override MQTTMessageType Type => MQTTMessageType.SUBSCRIBE;
        public ushort Identifier { get; set; }

        protected override void OnRead(Stream stream, ISession session)
        {
            base.OnRead(stream, session);
            Identifier = ReadUInt16(stream);
            while (stream.Position < stream.Length)
            {
                Subscription item = new Subscription();
                item.TopicFilter = ReadString(stream);
                item.QoSType = (QoSType)stream.ReadByte();
            }
        }

        protected override void OnWrite(Stream stream, ISession sessioni)
        {
            base.OnWrite(stream, sessioni);
            WriteUInt16(stream, Identifier);
            foreach (var item in Subscriptions)
            {
                WriteString(stream, item.TopicFilter);
                stream.WriteByte((byte)item.QoSType);
            }
        }

        public List<Subscription> Subscriptions { get; set; } = new List<Subscription>();
    }

    public class Subscription
    {
        public String TopicFilter { get; set; }

        public QoSType QoSType { get; set; }

    }
}
