<div class="home-info-view">
    <el-descriptions class="margin-top " :column="8" :size="size" border>
        <el-descriptions-item span="2">
            <template slot="label">

                CPU
            </template>
            {{status.Status.Cpu}}%
        </el-descriptions-item>
        <el-descriptions-item span="2">
            <template slot="label">

                已使用内存
            </template>
            {{$nformat(status.Status.Memory)}}(KB)
        </el-descriptions-item>
        <el-descriptions-item span="2">
            <template slot="label">

                缓冲块总内存
            </template>
            {{$nformat(status.BufferSize/1024)}}KB
        </el-descriptions-item>
        <el-descriptions-item span="2">
            <template slot="label">

                缓冲块数量
            </template>
            {{$nformat(status.Buffers)}}
        </el-descriptions-item>

        <el-descriptions-item span="4">
            <template slot="label">

                服务地址
            </template>
            {{status.Host?status.Host:'*'}}:{{status.Port}}
        </el-descriptions-item>

        <el-descriptions-item span="4">
            <template slot="label">

                运行时长
            </template>
            {{status.Status.RunTime}}
        </el-descriptions-item>
        <el-descriptions-item span="4">
            <template slot="label">

                系统信息
            </template>
            {{status.System.OSVersion.VersionString}}
        </el-descriptions-item>
        <el-descriptions-item span="4">
            <template slot="label">

                CPU核数
            </template>
            {{status.System.ProcessorCount}}
        </el-descriptions-item>



        <el-descriptions-item span="4">
            <template slot="label">

                接收数据
            </template>
            <label v-if="status.ReceiveBytes.Count>1024*1024*1024">
                {{$nformat(status.ReceiveBytes.RPS/1024/1024)}}MBs&nbsp;/&nbsp;{{$nformat(status.ReceiveBytes.Count/1024/1024)}}MB
            </label>
            <label v-else>
                {{$nformat(status.ReceiveBytes.RPS/1024)}}KBs&nbsp;/&nbsp;{{$nformat(status.ReceiveBytes.Count/1024)}}KB
            </label>
        </el-descriptions-item>

        <el-descriptions-item span="4">
            <template slot="label">

                发送数据
            </template>
            <label v-if="status.SendBytes.Count>1024*1024*1024">
                {{$nformat(status.SendBytes.RPS/1024/1024)}}MBs&nbsp;/&nbsp;{{$nformat(status.SendBytes.Count/1024/1024)}}MB
            </label>
            <label v-else>
                {{$nformat(status.SendBytes.RPS/1024)}}KBs&nbsp;/&nbsp;{{$nformat(status.SendBytes.Count/1024)}}KB
            </label>
        </el-descriptions-item>
        <el-descriptions-item span="4">
            <template slot="label">

                设备数量
            </template>
            {{status.Count}}
        </el-descriptions-item>
        <el-descriptions-item span="4">
            <template slot="label">

                接收消息数量
            </template>
            {{$nformat(status.ReceiveMsg.RPS)}}s&nbsp;/&nbsp;{{$nformat(status.ReceiveMsg.Count)}}
        </el-descriptions-item>



    </el-descriptions>
    <br />

    <div class="topic-status-list">
        <ul>
            <li style="margin-bottom:10px;">
                <div style="padding: 5px; border-bottom-width: 1px; border-bottom-color: #808080;border-bottom-style:dashed;">

                    <el-link :underline="false" @click="expend=!expend" type="" style=" font-size: 15pt;cursor:pointer;">
                        <i v-if="expend" class="el-icon-arrow-down"></i>
                        <i v-else class="el-icon-arrow-right"></i>
                        <i class="fa-solid fa-database" style="color: #74C0FC;margin-right:5px;margin-left:5px;"></i>
                        <label style="margin-right:5px;"> [{{status.TopicStatus.length}}]</label>
                        订阅转发
                    </el-link>
                    <label style="float:right;font-weight:bold"> {{$nformat(status.DistributionMsg.RPS)}}s&nbsp;/&nbsp;{{$nformat(status.DistributionMsg.Count)}}</label>
                </div>
            </li>
            <li>
                <ul :class="expend?'show':'hide'">

                    <li style="padding-left:20px;margin-bottom:10px;" v-for="topic in status.TopicStatus">
                        <div style="padding: 5px; border-bottom-width: 1px; border-bottom-color: #808080;border-bottom-style:dashed;">
                            <el-link :underline="false" @click="topic.Extend=!topic.Extend" type="" style=" font-size: 12pt;cursor:pointer;">
                                <i v-if="topic.Extend" class="el-icon-arrow-down"></i>
                                <i v-else class="el-icon-arrow-right"></i>
                                <i class="fa-solid fa-envelopes-bulk" style="color: #74C0FC; font-size: 15pt; margin-right: 5px; margin-left: 5px;"></i>
                                <label style="margin-right:5px;">  [{{topic.Users.length}}]</label> {{topic.Url}}
                            </el-link>
                            <label style="float:right"> {{$nformat(topic.Data.RPS)}}s&nbsp;/&nbsp;{{$nformat(topic.Data.Count)}}</label>
                        </div>
                        <ul :class="topic.Extend?'show':'hide'">
                            <li v-for="user in topic.Users" style="padding-left:60px;padding-top:5px;">
                                <i v-if="user.Disconnected==true" class="fas fa-server" style="color: #a1a1a1; font-size: 11pt;"></i>
                                <i v-else class="fas fa-server" style="color: #63E6BE; font-size: 11pt;"></i>
                                <label style="font-size: 10pt; border-radius: 4px;  margin-right: 10px;"> {{user.ID}}  </label>
                                <label style=""> ({{user.RemoteIP}})</label>
                                <label style="float: right; font-size: 10pt;"> {{$nformat(user.RPS.RPS)}}s&nbsp;/&nbsp;{{$nformat(user.RPS.Count)}}</label>
                            </li>
                        </ul>
                    </li>
                    <li>
                        <div style="text-align: right; font-weight: bold; font-size: 15pt;">

                        </div>

                    </li>
                </ul>
            </li>
        </ul>

    </div>
</div>
<script>
    export default {
        data() {
            return {
                exvalues: new Object(),
                status: {
                    Status: { Cpu: '' },
                    System: {
                        OSVersion: {}
                    },
                    ReceiveBytes: {},
                    SendBytes: {},
                    ReceiveMsg: {},
                    DistributionMsg: {},
                    TopicStatus: [{ Data: {}, Users: [{ Expend: false, RPS: {} }] }]
                },
                size: '',
                expend: true,

            };
        },
        methods: {
            onExpend(item) {
                item.Expend = !item.Expend;
            },
            getStatus() {
                this.$get('/api/status').then(r => {
                    this.status.TopicStatus.forEach(e => {
                        for (i = 0; i < r.TopicStatus.length; i++) {

                            if (r.TopicStatus[i].Url == e.Url) {
                              
                                r.TopicStatus[i].Extend = e.Extend;
                            }
                        }
                    });
                    this.status = r;
                });
            }
        },
        mounted() {
            this.getStatus();
            this.$addTimer((i) => {
                if (i % 3 == 0) {
                    this.getStatus();
                }
            });
        }
    }
</script>
<style>
    .el-descriptions {
        font-size: 10pt;
    }

    .home-info-view th {
        font-weight: bold;
    }

    .topic-status-list ul {
        list-style: none;
        padding: 0px;
        margin: 0px;
    }

    .topic-status-list li {
        list-style: none;
    }

    .topic-static-item {
        height: 10px;
        margin-left: 20px;
    }

    .show {
        display: '';
    }

    .hide {
        display: none
    }

    .topic-static-item label {
        width: 300px;
        float: left;
        padding-top: 5px;
    }
</style>