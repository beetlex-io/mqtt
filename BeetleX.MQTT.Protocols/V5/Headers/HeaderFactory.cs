using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace BeetleX.MQTT.Protocols.V5.Headers
{
    public class HeaderFactory
    {
        private static Dictionary<HeaderType, Type> mHeaderTypeTable = new Dictionary<HeaderType, Type>();

        public static void Init()
        {
            foreach (var type in typeof(HeaderFactory).Assembly.GetTypes())
            {
                var attr = type.GetCustomAttribute<HeaderAttribute>();
                if (attr != null)
                {
                    mHeaderTypeTable[attr.Type] = type;
                }
            }
        }
        public static IHeaderProperty GetHeader(HeaderType key)
        {
            if (mHeaderTypeTable.TryGetValue(key, out Type type))
            {
                return (IHeaderProperty)Activator.CreateInstance(type);
            }
            throw new MQTTException($"{key} property type not found!");
        }
    }

    [AttributeUsage(AttributeTargets.Struct| AttributeTargets.Class)]
    public class HeaderAttribute : Attribute
    {
        public HeaderAttribute(HeaderType type)
        {
            Type = type;
        }

        public HeaderType Type { get; set; }
    }
}
