<div>
    <div>
        <el-button plain size="small" @click="onAdd">添加用户</el-button>
        <el-divider direction="vertical"></el-divider>
      
    </div>
    <el-table size="mini" :data="items">

        <el-table-column label="帐户名" width="200">
            <template slot-scope="item">
                <el-link @click="onSelect(item.row)">{{item.row.Name}}</el-link>
            </template>
        </el-table-column>
        <el-table-column label="类别" width="200">
            <template slot-scope="item">
                <label>{{item.row.Category}}</label>
            </template>
        </el-table-column>
        <el-table-column label="备注">
            <template slot-scope="item">
                <label>{{item.row.Remark}}</label>
            </template>
        </el-table-column>
       
        <el-table-column label="可用" width="80">
            <template slot-scope="item">
                <el-switch @change="onChange(item.row)" size="mini" v-model="item.row.Enabled"></el-switch>
            </template>
        </el-table-column>

        <el-table-column width="80">
            <template slot-scope="item">
                <el-button size="mini" style="padding-left:10px; padding-right:10px;" @click="onDelete(item.row)">删除</el-button>
            </template>
        </el-table-column>
        <div slot="append" style="text-align:right;padding:5px;">
            <el-pagination background layout="prev, pager, next" :page-size="pageSize" :total="count" @current-change="onPageChange">
            </el-pagination>
        </div>
    </el-table>
    <el-dialog title="编辑信息" :visible.sync="dialogVisible" @opened="onOpen" width="600px" :append-to-body="true" :close-on-click-modal="false">
        <mqtt-user-modify ref="editor" @close="onModify($event)"></mqtt-user-modify>
    </el-dialog>
</div>
<script>
    export default {
        props: [],
        data() {
            return {
                items: [],
                pageSize: 20,
                count: 0,
                page: 0,
                dialogVisible: false,
                selectID: null,
            };
        },
        methods: {
            onAdd() {
                this.selectID = null;
                this.dialogVisible = true;
            },
            onSelect(item) {
                this.selectID = item.Name;
                this.dialogVisible = true;
            },
            onChange(e) {
                this.$post('/api/ModifyUser', e).then(r => {
                    
                });
            },
            onModify(refresh) {
                this.dialogVisible = false;
                if (refresh) {
                   
                    this.onList();
                }
            },
            onOpen() {
                this.$refs.editor.onGet(this.selectID);
            },
            onPageChange(e) {
                this.page = e - 1;
                this.onList();
            },
            onList() {
                this.$get('/api/ListUsers', { page: this.page, size: this.pageSize }).then(r => {
                    this.items = r.items;
                    this.count = r.count;
                });
            },
            onDelete(e) {
                this.$confirmMsg('是否删除' + e.Name+'用户?', () => {
                    this.$get('/api/DeleteUser', { id: e.Name }).then(r => {
                        this.page = 0;
                        this.onList();
                    });
                });
            }
        },
        mounted() {
            this.onList();
        }
    }
</script>
