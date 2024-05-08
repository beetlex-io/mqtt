<div style="padding:10px">
    <label class="mqtt-server-header">{{MQTTAppName}}</label>
    <div style="float:right;">

        <el-button icon="fas fa-key" @click="onChangePwd" size="small" round>修改密码</el-button>
        <el-divider direction="vertical"></el-divider>
        <el-button icon="el-icon-warning-outline" @click="aboutDialogVisible=true" size="small" round>关于
            
        </el-button>
        <el-divider direction="vertical"></el-divider>
        <el-button icon="fas fa-sign-out-alt" @click="onSignout" size="small" round>退出系统</el-button>


    </div>
    <el-dialog title="修改密码" :visible.sync="dialogVisible" @opened="onOpen" width="400px" :append-to-body="true" :close-on-click-modal="false">
        <webfamily-changepwd @close="submitForm($event)"></webfamily-changepwd>
    </el-dialog>
    <el-dialog title="关于" :visible.sync="aboutDialogVisible" width="600px" :append-to-body="true" :close-on-click-modal="false">
        <mqtt-about></mqtt-about>
    </el-dialog>
</div>
<script>
    export default {
        data() {
            return {
                dialogVisible: false,
                aboutDialogVisible: false,
                NewPWD_rules: [{ required: true, message: '新密码不能为空！', trigger: 'blur' },],
                ConfirmPWD_rules: [{ required: true, message: '确认密码不能为空！', trigger: 'blur' },],
                record: {
                    NewPWD: '',
                    ConfirmPWD: '',
                },
                MQTTAppName: ''
            };
        },
        methods: {
            onOpen() {

            },
            submitForm(pwd) {
                if (!pwd)
                    this.dialogVisible = false;
                else {
                    this.$post('/api/ChangePWD', { pwd: pwd }).then(r => {
                        this.$userSignout();
                    });
                }
            },

            onSignout() {
                this.$userSignout();
            },
            onChangePwd() {
                this.dialogVisible = true;
            }
        },
        mounted() {
            this.$get('/api/GetApplicationName').then(r => { this.MQTTAppName = r; });
        }
    }
</script>
<style>
    .mqtt-server-header {
        font-size: 14pt;
        padding-left: 20px;
    }
</style>