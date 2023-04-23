using CommonLibrary;
using FeatureClassQuery.QueryForm;
using System;
using System.Collections.Generic;
using System.Data;
using System.Windows.Forms;

namespace FeatureClassQuery
{
    public partial class MainForm : Form
    {
        readonly AxRenderControlOperation controlOperation = null;
        private TreeNode selectNode = null;  //标记treeView控件中当前被选中的节点

        public MainForm()
        {
            InitializeComponent();

            controlOperation = new AxRenderControlOperation(axRenderControl1);
            controlOperation.InitializationAxRenderControl("JD.3DM");
            MyTreeNode treeNodes = controlOperation.BindDataToCatalogTree();

            SetTreeNodeWithContextMenuStrip(treeNodes.Nodes);
            this.treeView1.Nodes.Add(treeNodes.Nodes[0]);
        }

        private void SetTreeNodeWithContextMenuStrip(TreeNodeCollection treeNodes)
        {
            foreach (TreeNode node in treeNodes)
            {
                if (node.ImageIndex == 2)
                    node.ContextMenuStrip = this.contextMenuStrip2;
                else if (node.ImageIndex == -1)
                    node.ContextMenuStrip = this.contextMenuStrip1;

                SetTreeNodeWithContextMenuStrip(node.Nodes);
            }
        }

        private void toolStripAddDatasource_Click(object sender, EventArgs e)
        {
            DataSourceForm dsForm = new DataSourceForm(false, controlOperation);
            if (dsForm.ShowDialog() == DialogResult.OK)
            {
                DabaseConnectionInfo dcInfor = new DabaseConnectionInfo
                {
                    Server = dsForm.Server,
                    Port = dsForm.Port,
                    Database = dsForm.Database,
                    UserName = dsForm.UserName,
                    PassWord = dsForm.PassWord
                };

                controlOperation.ConnectAndGetDatabaseNames(dsForm.ConnectionType, dcInfor);
                //controlOperation.BindDataToCatalogTree();
            }
        }

        private void treeView1_MouseDown(object sender, MouseEventArgs e)
        {
            selectNode = this.treeView1.GetNodeAt(e.X, e.Y);
        }

        /// <summary>
        /// 点击字段属性
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripMenuItemFieldInfo_Click(object sender, EventArgs e)
        {
            string fieldinfo_name = selectNode.Text;
            var fieldinfo = controlOperation.GetFieldInfoByName(fieldinfo_name);
            if (fieldinfo != null)
            {
                FieldInfoForm form = new FieldInfoForm(fieldinfo);
                form.Show();
            }
        }

        /// <summary>
        /// 查看所有记录
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripMenuItemViewData_Click(object sender, EventArgs e)
        {
            string fc_name = selectNode.Text;
            var dt = controlOperation.BuildDataTableByFeatureName(fc_name);
            new AttributeForm(dt, fc_name).Show();
        }

        /// <summary>
        /// 按属性条件查询
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripMenuItemQuery_Click(object sender, EventArgs e)
        {
            QueryFilterDlg dlg = new QueryFilterDlg();
            string fc_name = selectNode.Text;
            List<string> fieldNames = controlOperation.GetFieldNamesByFeatureName(fc_name);
            dlg.FieldList_listBox.DataSource = fieldNames;

            if (dlg.ShowDialog() == DialogResult.OK)
            {
                string whereClause = dlg.QueryFilter_txt.Text.Trim();
                DataTable dt = controlOperation.BuildDataTableByFeatureName(fc_name, whereClause);
                new AttributeForm(dt, fc_name, whereClause).Show();
            }
        }
    }
}
