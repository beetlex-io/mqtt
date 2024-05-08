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
                o.DefaultListen.Port = 8089;
            })
            .Setting(o =>
            {
                o.LogToConsole = true;
                o.Port = 80;
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
### 主界面
![image](https://github.com/beetlex-io/mqtt/assets/2564178/b962273c-0ea9-4651-b577-4a49fd3fe38c)

### 帐号界面
![image](https://github.com/beetlex-io/mqtt/assets/2564178/96abc308-5a86-46bc-92da-085bb7278531)

### 设备查看界面
![image](https://github.com/beetlex-io/mqtt/assets/2564178/302aeccf-0d69-4a00-92a0-a0137b83b871)

