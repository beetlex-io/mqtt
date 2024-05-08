
using BeetleX.MQTT.Protocols.V5.Headers;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace BeetleX.MQTT.Protocols
{
    public class PropertyStream : System.IO.MemoryStream
    {

        private Dictionary<HeaderType, object> mReadProperties = new Dictionary<HeaderType, object>();

        public List<UserProperty> UserProperties { get; set; }

        protected List<IHeaderProperty> mWriteProperties = new List<IHeaderProperty>();


        public void Refresh()
        {
            mReadProperties.Clear();
            mWriteProperties.Clear();
            UserProperties = null;
            SetLength(0);
        }
        public T To<T>(HeaderType type)
            where T : IHeaderProperty
        {
            mReadProperties.TryGetValue(type, out var property);
            return (T)property;
        }

        public PropertyStream Add(IHeaderProperty property)
        {
            if (property != null)
                mWriteProperties.Add(property);
            return this;
        }

        public void Read(MQTTParse parse, Stream stream, Action<IHeaderProperty> handler = null)
        {
            var len = parse.Int7BitHandler.Read(stream);
            if (len > 0)
            {
                SetLength(0);
                parse.CopyStream(stream, this, (int)len);
                this.Position = 0;
                while (this.Position < this.Length)
                {
                    HeaderType htype = (HeaderType)this.ReadByte();
                    var property = HeaderFactory.GetHeader(htype);
                    property.Read(parse, this);
                    handler?.Invoke(property);
                    if (property is UserProperty)
                    {
                        if (UserProperties == null)
                            UserProperties = new List<UserProperty>();
                        UserProperties.Add((UserProperty)property);
                    }
                    else
                    {
                        mReadProperties.Add(property.Type, property);
                    }

                }
            }
        }
        public void Write(MQTTParse parse, Stream stream)
        {
            foreach (var item in mWriteProperties)
            {
                WriteByte((byte)item.Type);
                item.Write(parse, this);
            }
            parse.Int7BitHandler.Write(stream, (int)Length);
            this.Position = 0;
            CopyTo(stream);

        }


        public static PropertyStream operator +(PropertyStream stream, Tuple<IHeaderProperty, IHeaderProperty, IHeaderProperty> properties)
        {
            stream.Add(properties.Item1).Add(properties.Item2).Add(properties.Item3);
            return stream;
        }

        public static PropertyStream operator +(PropertyStream stream, Tuple<IHeaderProperty, IHeaderProperty> properties)
        {
            stream.Add(properties.Item1).Add(properties.Item2);
            return stream;
        }

        public static PropertyStream operator +(PropertyStream stream, IHeaderProperty property) => stream.Add(property);

        public static PropertyStream operator +(PropertyStream stream, IEnumerable<IHeaderProperty> properties)
        {
            if (properties != null)
                foreach (var item in properties)
                {
                    stream.Add(item);
                }
            return stream;
        }


        public static implicit operator List<UserProperty>(PropertyStream d) => d.UserProperties;

    }
}
