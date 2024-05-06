using BeetleX.FastHttpApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace BeetleX.MQTT.Server
{
    public class MQTTUpgradeWebsocketSuccess : ResultBase
    {
        public MQTTUpgradeWebsocketSuccess(string websocketKey)
        {
            WebsocketKey = websocketKey;
        }

        public string WebsocketKey { get; set; }


        public override bool HasBody => false;

        public override void Setting(HttpResponse response)
        {
            response.Code = "101";
            response.CodeMsg = "Switching Protocols";
            response.Header.Add(HeaderTypeFactory.CONNECTION, "Upgrade");
            response.Header.Add(HeaderTypeFactory.UPGRADE, "websocket");
            response.Header.Add("Sec-Websocket-Protocol", "mqtt");
            SHA1 sha1 = new SHA1CryptoServiceProvider();
            byte[] bytes_sha1_in = Encoding.UTF8.GetBytes(WebsocketKey + "258EAFA5-E914-47DA-95CA-C5AB0DC85B11");
            byte[] bytes_sha1_out = sha1.ComputeHash(bytes_sha1_in);
            string str_sha1_out = Convert.ToBase64String(bytes_sha1_out);
            response.Header.Add(HeaderTypeFactory.SEC_WEBSOCKT_ACCEPT, str_sha1_out);
        }
    }
}
