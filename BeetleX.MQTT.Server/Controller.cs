using BeetleX.Buffers;
using BeetleX.EFCore.Extension;
using BeetleX.FastHttpApi;
using BeetleX.FastHttpApi.EFCore.Extension;
using BeetleX.FastHttpApi.Jwt;
using BeetleX.MQTT.Server.Storages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BeetleX.MQTT.Server
{
    [Controller(BaseUrl = "/api/")]
    [AuthMark(AuthMarkType.Admin)]
    [DefaultJsonResultFilter]

    public class Controller
    {

        public Controller(MQTTApplication application)
        {
            mApplication = application;
        }

        private MQTTApplication mApplication;
        public object GetUser(string id, EFCoreDB<MQTTDB> db)
        {
            var item = db.DBContext.User.Find(id);
            if (item == null)
                item = new User();
            return item;

        }

        public string GetApplicationName()
        {
            return mApplication.Name +" Gateway";
        }

        public object ListUsers(int page, int size, EFCoreDB<MQTTDB> db)
        {
            Select<User> select = new Select<User>();
            var count = select.Count(db.DBContext);
            var items = select.List(db.DBContext, new Region(page, size));
            return new { count, items };
        }

        public void ChangePWD(string pwd, EFCoreDB<MQTTDB> db)
        {
            db.DBContext.Options.Where(o => o.Name == "SystemPassword").Update(o => new Options { Value = pwd });
            mApplication.LoadOptions();
        }
        public void ModifyUser(User body, EFCoreDB<MQTTDB> db)
        {
            if (db.DBContext.User.Count(u => u.Name == body.Name) > 0)
            {
                db.DBContext.User.Where(o => o.Name == body.Name)
                    .Update(o => new User
                    {
                        Enabled = body.Enabled,
                        IsNodeClient = body.IsNodeClient,
                        PWD = body.PWD,
                        Remark = body.Remark

                    }
                    );
            }
            else
            {
                db.DBContext.User.Add(body);
            }
        }

        public void DeleteUser(string id, EFCoreDB<MQTTDB> db)
        {
            db.DBContext.User.Where(u => u.Name == id).Delete();
        }

        public object Onlines(int page, int size)
        {
            return mApplication.ListOnlines(page, size);
        }
        private object mSystemInf;

        private RpsCounter mReceiveCounter = new RpsCounter();

        private RpsCounter mDistributionCounter = new RpsCounter();

        private RpsCounter mSocketReceiveCounter = new RpsCounter();

        private RpsCounter mSocketSendCounter = new RpsCounter();

        public object Status()
        {
            if (mApplication == null || mApplication.TCP == null)
                return new { };
            if (mSystemInf == null)
            {
                mSystemInf = new { Environment.OSVersion, Version = Environment.Version.ToString(), Environment.ProcessorCount };
            }
            var status = mApplication.Http.ServerCounter.Next();
            return new
            {
                Status = status,
                mApplication.TCP.Options.DefaultListen.Port,
                mApplication.TCP.Options.DefaultListen.Host,
                Memory = status.Memory / 1024,
                Buffers = (BufferMonitor.CreateCount - BufferMonitor.FreeCount),
                BufferSize = BufferMonitor.Size,
                System = mSystemInf,
                mApplication.TCP.Count,
                ReceiveBytes = mSocketReceiveCounter.Next(mApplication.TCP.ReceivBytes),
                SendBytes = mSocketSendCounter.Next(mApplication.TCP.SendBytes),
                ReceiveMsg = mReceiveCounter.Next(mApplication.ReceiveQuantity),
                DistributionMsg = mDistributionCounter.Next(mApplication.DistributionQuantity),
                TopicStatus = mApplication.SubscriptionMapper.ListTopicRps()
            };
        }
    }
}
