<div>
    <el-form size="mini" :model="record" label-width="120px" ref="dataform">
        <el-row>
            <el-col :span="24">
                <el-form-item label="帐户名" prop="Name" :rules="Name_rules"><el-input size="mini" v-model="record.Name"></el-input></el-form-item>
            </el-col>
        </el-row>
        <el-row>
            <el-col :span="24">
                <el-form-item label="密码" prop="PWD" :rules="PWD_rules"><el-input size="mini" v-model="record.PWD"></el-input></el-form-item>
            </el-col>
        </el-row>
        <el-row>
            <el-col :span="24">
                <el-form-item label="类别" prop="Category"><el-input  size="mini" v-model="record.Category"></el-input></el-form-item>
            </el-col>
        </el-row>
        <!--<el-row>
            <el-col :span="24">
                <el-form-item label="节点用户" prop="IsNodeClient"><el-switch size="mini" v-model="record.IsNodeClient"></el-switch></el-form-item>
            </el-col>
        </el-row>-->
        <el-row>
            <el-col :span="24">
                <el-form-item label="有效" prop="Enabled"><el-switch size="mini" v-model="record.Enabled"></el-switch></el-form-item>
            </el-col>
        </el-row>
        <el-row>
            <el-col :span="24">
                <el-form-item label="备注" prop="Remark"><el-input size="mini" v-model="record.Remark" type="textarea"></el-input></el-form-item>
            </el-col>
        </el-row>
        <el-row>
            <el-col span="24" style="text-align:right">
                <el-button size="mini" style="padding-left:10px; padding-right:10px;" @click="$emit('close')">取消</el-button>
                <el-button size="mini" style="padding-left:10px; padding-right:10px;" @click="submitForm">确定</el-button>
            </el-col>
        </el-row>
    </el-form>
</div>
<script>
    export default {
        props: [],
        data() {
            return {
                Name_rules: [{ required: true, message: '值不能为空！', trigger: 'blur' },],
                PWD_rules: [{ required: true, message: '值不能为空！', trigger: 'blur' }, { type: 'string', min: 6, trigger: 'blur', message: '类型无效!' }],
                record: {
                    Name: null,
                    PWD: null,
                    IsNodeClient: null,
                    Enabled: null,
                    Remark: null,
                }
            };
        },
        methods: {

            onGet(id) {
                if (id) {
                    this.$get('/api/GetUser', { id: id }).then(r => {
                        this.record = r;
                    });
                }
                else {
                    this.record = {

                    }
                }
            },

            submitForm() {
                this.$refs['dataform'].validate((valid) => {
                    if (valid) {
                        this.$confirmMsg('是否保存数据?', () => {
                            this.$post('/api/ModifyUser', this.record).then(r => {
                                this.$emit('close', true);
                            });
                        });
                    }
                });
            },
            resetForm() {
                this.$refs['dataform'].resetFields();
            }
        },
        mounted() {
        }
    }
</script>
