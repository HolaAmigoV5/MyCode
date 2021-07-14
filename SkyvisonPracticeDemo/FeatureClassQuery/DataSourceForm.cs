using CommonLibrary;
using System;
using System.Windows.Forms;

namespace FeatureClassQuery
{
    public partial class DataSourceForm : Form
    {
        public string Server { get { return txtHost.Text; } set { txtHost.Text = value; } }
        public string ConnectionType { get { return cbConnectionType.Text; } set { cbConnectionType.Text = value; } }
        public uint Port { get { return txtPort.Text == "" ? 0 : uint.Parse(txtPort.Text); } set { txtPort.Text = value.ToString(); } }
        public string Database { get { return txtUserName.Text; } set { txtUserName.Text = value; } }
        public string UserName { get { return txtUserName.Text; } set { txtUserName.Text = value; } }
        public string PassWord { get { return txtPassword.Text; } set { txtPassword.Text = value; } }

        private bool _isCreate;
        private AxRenderControlOperation axOperation;

        public DataSourceForm(bool isCreate, AxRenderControlOperation operation)
        {
            InitializeComponent();
            _isCreate = isCreate;
            axOperation = operation;
            this.Load += DataSourceForm_Load;
        }

        #region Event
        private void DataSourceForm_Load(object sender, EventArgs e)
        {
            this.cbConnectionType.SelectedIndex = 0;
        }

        private void cbConnectionType_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (this.cbConnectionType.SelectedIndex)
            {
                case 0:   //i3dConnectionMySql5x
                    SetControlEnabled(true);
                    break;
                case 1:   //i3dConnectionFireBird2x
                case 2:   //i3dConnectionSQLite3
                    {
                        ClearControl();
                        SetControlEnabled(false);
                    }
                    break;
            }
        }

        private void SetControlEnabled(bool enable)
        {
            this.txtHost.Enabled = enable;
            this.txtPort.Enabled = enable;
            this.txtUserName.Enabled = enable;
            this.txtPassword.Enabled = enable;
            //this.txtDatabase.Enabled = enable;
            this.btnFileSelect.Enabled = !enable;
            this.btnConnect.Enabled = enable;
            this.cbDatabases.Enabled = enable;
        }

        private void ClearControl()
        {
            this.txtHost.Text = "";
            this.txtPort.Text = "";
            this.txtUserName.Text = "";
            this.txtPassword.Text = "";
        }

        /// <summary>
        /// 为大文件选择存储路径
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnFileSelect_Click(object sender, EventArgs e)
        {
            if (_isCreate)
            {
                SaveFileDialog dlg = new SaveFileDialog();
                FileSelect(dlg);
            }
            else
            {
                OpenFileDialog dlg = new OpenFileDialog
                {
                    InitialDirectory = @"D:\SkyScenery\beginner\beginner\data\Media",
                    RestoreDirectory = true
                };
                FileSelect(dlg);
            }
        }

        private void FileSelect(FileDialog dlg)
        {
            if (this.cbConnectionType.SelectedIndex == 1) //i3dConnectionFireBird2x
            {
                dlg.Filter = "FDB File(*.fdb)|*.fdb";
                dlg.DefaultExt = ".fdb";
            }
            else if (this.cbConnectionType.SelectedIndex == 2) //i3dConnectionSQLite3
            {
                dlg.Filter = "SDB File(*.sdb)|*.sdb";
                dlg.DefaultExt = ".sdb";
            }
            if (dlg.ShowDialog() == DialogResult.OK)
                this.txtDatabase.Text = dlg.FileName;
        }

        /// <summary>
        /// 连接数据库
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnConnect_Click(object sender, EventArgs e)
        {
            //ConnectAndGetDatabaseNames()
            DabaseConnectionInfo dcInfo = new DabaseConnectionInfo
            {
                Server = this.Server,
                Port = this.Port,
                UserName = this.UserName,
                PassWord = this.PassWord
            };

            string[] databaseNames = axOperation.ConnectAndGetDatabaseNames(this.ConnectionType, dcInfo);
            this.cbDatabases.DataSource = databaseNames;
            this.cbDatabases.SelectedIndex = 0;
        }

        private void cbDatabases_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.txtDatabase.Text = this.cbDatabases.Text;
        } 
        #endregion
    }
}
