<div>
    <div style="padding-bottom:20px;">
        <el-button plain size="small" @click="onList">刷新</el-button>
        <el-divider direction="vertical"></el-divider>
        <el-pagination style="float:left;" background layout="prev, pager, next" :page-size="pageSize" :total="count" @current-change="onPageChange">
        </el-pagination>
    </div>
    <div class="online-item" v-for="item in items" style="width:500px;padding-right:20px;padding-bottom:20px;float:left;">
        <el-card :class="item.Enabled==true?'driver-enabled box-card':'box-card'">
            <el-descriptions class="margin-top" :column="2" :size="size" border>
                <el-descriptions-item>
                    <template slot="label">

                        设备ID
                    </template>
                    {{item.ID}}
                </el-descriptions-item>
                <el-descriptions-item>
                    <template slot="label">

                        类别
                    </template>
                    {{item.Category}}
                </el-descriptions-item>
                <el-descriptions-item>
                    <template slot="label">

                        IP地址
                    </template>
                    {{item.RemoteIP}}
                </el-descriptions-item>
                <el-descriptions-item span="2">
                    <template slot="label">

                        活跃时间
                    </template>
                    {{item.LastActiveTime}}
                </el-descriptions-item>

                <el-descriptions-item :contentStyle="{'text-align': 'right'}">
                    <template slot="label">
                        推送数量
                    </template>
                    {{$nformat(item.NumberOfPush)}}
                </el-descriptions-item>

                <el-descriptions-item :contentStyle="{'text-align': 'right'}">
                    <template slot="label">
                        订阅数量
                    </template>
                    {{$nformat(item.NumberOfSubscribe)}}
                </el-descriptions-item>

                <el-descriptions-item>
                    <template slot="label">
                        订阅主题
                    </template>
                    {{item.Subscription}}
                </el-descriptions-item>
            </el-descriptions>
        </el-card>
    </div>
    <div style="clear:both"></div>
    <div style="text-align:left;padding:5px;">
        <el-pagination style="float:left;" background layout="prev, pager, next" :page-size="pageSize" :total="count" @current-change="onPageChange">
        </el-pagination>
    </div>

</div>
<script>
    export default {
        props: [],
        data() {
            return {
                items: [],
                pageSize: 10,
                count: 0,
                size: '',
                page: 0,
                dialogVisible: false,
            };
        },
        methods: {
            onOpen() {
                this.$refs.editor.onGet(this.selectID);
            },
            onPageChange(e) {
                this.page = e - 1;
                this.onList();
            },
            onList() {
                this.$get('/api/onlines', { page: this.page, size: this.pageSize }).then(r => {
                    this.items = r.items;
                    this.count = r.count;
                });
            },
            onDelete(e) {
                this.$confirmMsg('是否删除数据?', () => {
                    this.$get('', { id: e.ID }).then(r => {
                        this.page = 0;
                        this.onList();
                    });
                });
            }
        },
        mounted() {
            this.onList();
            //this.$addTimer((i) => {
            //    if (i % 10 == 0) {
            //        this.onList();
            //    }
            //});
        }
    }
</script>
<style>
    .online-item .el-card__body {
        padding: 0px;
    }
    .driver-enabled {
     
        border: 1px solid #67c23a; 
   
    }
</style>