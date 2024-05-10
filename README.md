# MQTT

#### 介绍
基于BeetleX组件扩展的高性能MQTT通讯网关，可以轻松应对数十万的消费订阅转发.组件实现了3.x和5.0版本的协议。
#### 运行服务
``` csharp
    class Program
    {
        private static MQTTServer mServer;
        static void Main(string[] args)
        {

            mServer = new MQTTServer(ProtocolType.V3);
            mServer.RegisterComponent<BeetleX.MQTT.Server.Controller>();
            mServer.MQTTListen(o =>
            {
                o.DefaultListen.Port = 8089;//mqtt服务端口
            })
            .Setting(o =>
            {
                o.LogToConsole = true;
                o.Port = 80;//web管理端口
                o.LogLevel = EventArgs.LogType.Info;
            })
            .UseJWT()
            .UseEFCore<Storages.MQTTDB>()
            .UseElement(PageStyle.ElementDashboard)
            .Initialize((http, vue, resoure) =>
            {
                resoure.AddAssemblies(typeof(BeetleX.MQTT.Server.MQTTUser).Assembly);
                resoure.AddCss("website.css");
                resoure.AddScript("echarts.js");
                vue.Debug();
            })
           .Run();

        }
    }
```
运行成功后可以通过浏览器访问 http://localhost 进入管理界面（如果电脑上已经有服务占用了80端口，则根据自己需要调整）。
### 单独使用协议包
BeetleX.MQTT.Protocols是MQTT的协议分析组件，它是基于Stream来处理MQTT协议，如果不想使用BeetleX配套网服务功能可以单独引用BeetleX.MQTT.Protocols来进行MQTT的协议分析.组件是支持V3.X和V5.0
``` csharp
//v5
var mqttparse = new BeetleX.MQTT.Protocols.V5.MQTTParseV5();
mqttparse.Read(stream, null);
mqttparse.Write(msg, stream, null);
//v3.x
var mqttparse = new BeetleX.MQTT.MQTTParseV3();
mqttparse.Read(stream, null);
mqttparse.Write(msg, stream, null);
```
### 主界面
![image](https://github.com/beetlex-io/mqtt/assets/2564178/b962273c-0ea9-4651-b577-4a49fd3fe38c)

### 帐号界面
![image](https://github.com/beetlex-io/mqtt/assets/2564178/96abc308-5a86-46bc-92da-085bb7278531)

### 设备查看界面
![image](https://github.com/beetlex-io/mqtt/assets/2564178/302aeccf-0d69-4a00-92a0-a0137b83b871)

