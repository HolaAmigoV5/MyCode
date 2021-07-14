
namespace HelloWorld
{
    partial class Form1
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.ListViewItem listViewItem1 = new System.Windows.Forms.ListViewItem("无", 0);
            System.Windows.Forms.ListViewItem listViewItem2 = new System.Windows.Forms.ListViewItem("金色晨曦", 1);
            System.Windows.Forms.ListViewItem listViewItem3 = new System.Windows.Forms.ListViewItem("光暗之手", 2);
            System.Windows.Forms.ListViewItem listViewItem4 = new System.Windows.Forms.ListViewItem("天马行空", 3);
            System.Windows.Forms.ListViewItem listViewItem5 = new System.Windows.Forms.ListViewItem("飘絮人间", 4);
            System.Windows.Forms.ListViewItem listViewItem6 = new System.Windows.Forms.ListViewItem("七彩紫罗", 5);
            System.Windows.Forms.ListViewItem listViewItem7 = new System.Windows.Forms.ListViewItem("云中之触", 6);
            System.Windows.Forms.ListViewItem listViewItem8 = new System.Windows.Forms.ListViewItem("鲲鹏万里", 7);
            System.Windows.Forms.ListViewItem listViewItem9 = new System.Windows.Forms.ListViewItem("血色苍穹", 8);
            System.Windows.Forms.ListViewItem listViewItem10 = new System.Windows.Forms.ListViewItem("白云旋天", 9);
            System.Windows.Forms.ListViewItem listViewItem11 = new System.Windows.Forms.ListViewItem("长空破日", 10);
            System.Windows.Forms.ListViewItem listViewItem12 = new System.Windows.Forms.ListViewItem("霞光掩影", 11);
            System.Windows.Forms.ListViewItem listViewItem13 = new System.Windows.Forms.ListViewItem("混沌沧海", 12);
            System.Windows.Forms.ListViewItem listViewItem14 = new System.Windows.Forms.ListViewItem("梦境之末", 13);
            System.Windows.Forms.ListViewItem listViewItem15 = new System.Windows.Forms.ListViewItem("玄浑宇宙", 14);
            System.Windows.Forms.ListViewItem listViewItem16 = new System.Windows.Forms.ListViewItem("月神之眼", 15);
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.toolToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.fullScreenToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.captureScreenToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.请设置天气ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripComboBoxWeather = new System.Windows.Forms.ToolStripComboBox();
            this.雾效果ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripFog = new System.Windows.Forms.ToolStripMenuItem();
            this.上一视图ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.下一视图ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.帮助ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.axRenderControl1 = new Axi3dRenderEngine.AxRenderControl();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.skyboxListView = new System.Windows.Forms.ListView();
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.btnPause = new System.Windows.Forms.Button();
            this.btnStop = new System.Windows.Forms.Button();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.helpProvider1 = new System.Windows.Forms.HelpProvider();
            this.menuStrip1.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.axRenderControl1)).BeginInit();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolToolStripMenuItem,
            this.请设置天气ToolStripMenuItem,
            this.toolStripComboBoxWeather,
            this.雾效果ToolStripMenuItem,
            this.上一视图ToolStripMenuItem,
            this.下一视图ToolStripMenuItem,
            this.帮助ToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(1282, 36);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // toolToolStripMenuItem
            // 
            this.toolToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fullScreenToolStripMenuItem,
            this.captureScreenToolStripMenuItem});
            this.toolToolStripMenuItem.Name = "toolToolStripMenuItem";
            this.toolToolStripMenuItem.Size = new System.Drawing.Size(71, 32);
            this.toolToolStripMenuItem.Text = "Tools";
            // 
            // fullScreenToolStripMenuItem
            // 
            this.fullScreenToolStripMenuItem.Name = "fullScreenToolStripMenuItem";
            this.fullScreenToolStripMenuItem.Size = new System.Drawing.Size(236, 34);
            this.fullScreenToolStripMenuItem.Text = "FullScreen";
            this.fullScreenToolStripMenuItem.Click += new System.EventHandler(this.fullScreenToolStripMenuItem_Click);
            // 
            // captureScreenToolStripMenuItem
            // 
            this.captureScreenToolStripMenuItem.Name = "captureScreenToolStripMenuItem";
            this.captureScreenToolStripMenuItem.Size = new System.Drawing.Size(236, 34);
            this.captureScreenToolStripMenuItem.Text = "CaptureScreen";
            this.captureScreenToolStripMenuItem.Click += new System.EventHandler(this.captureScreenToolStripMenuItem_Click);
            // 
            // 请设置天气ToolStripMenuItem
            // 
            this.请设置天气ToolStripMenuItem.Name = "请设置天气ToolStripMenuItem";
            this.请设置天气ToolStripMenuItem.Size = new System.Drawing.Size(134, 32);
            this.请设置天气ToolStripMenuItem.Text = "请设置天气：";
            // 
            // toolStripComboBoxWeather
            // 
            this.toolStripComboBoxWeather.Items.AddRange(new object[] {
            "晴天",
            "小雨",
            "中雨",
            "大雨",
            "小雪",
            "中雪",
            "大雪"});
            this.toolStripComboBoxWeather.Name = "toolStripComboBoxWeather";
            this.toolStripComboBoxWeather.Size = new System.Drawing.Size(121, 32);
            this.toolStripComboBoxWeather.SelectedIndexChanged += new System.EventHandler(this.toolStripComboBoxWeather_SelectedIndexChanged);
            // 
            // 雾效果ToolStripMenuItem
            // 
            this.雾效果ToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripFog});
            this.雾效果ToolStripMenuItem.Name = "雾效果ToolStripMenuItem";
            this.雾效果ToolStripMenuItem.Size = new System.Drawing.Size(80, 32);
            this.雾效果ToolStripMenuItem.Text = "雾效果";
            // 
            // toolStripFog
            // 
            this.toolStripFog.Name = "toolStripFog";
            this.toolStripFog.Size = new System.Drawing.Size(182, 34);
            this.toolStripFog.Text = "开启雾效";
            this.toolStripFog.Click += new System.EventHandler(this.toolStripFog_Click);
            // 
            // 上一视图ToolStripMenuItem
            // 
            this.上一视图ToolStripMenuItem.Name = "上一视图ToolStripMenuItem";
            this.上一视图ToolStripMenuItem.Size = new System.Drawing.Size(98, 32);
            this.上一视图ToolStripMenuItem.Text = "上一视图";
            this.上一视图ToolStripMenuItem.Click += new System.EventHandler(this.上一视图ToolStripMenuItem_Click);
            // 
            // 下一视图ToolStripMenuItem
            // 
            this.下一视图ToolStripMenuItem.Name = "下一视图ToolStripMenuItem";
            this.下一视图ToolStripMenuItem.Size = new System.Drawing.Size(98, 32);
            this.下一视图ToolStripMenuItem.Text = "下一视图";
            this.下一视图ToolStripMenuItem.Click += new System.EventHandler(this.下一视图ToolStripMenuItem_Click);
            // 
            // 帮助ToolStripMenuItem
            // 
            this.帮助ToolStripMenuItem.Name = "帮助ToolStripMenuItem";
            this.帮助ToolStripMenuItem.Size = new System.Drawing.Size(62, 32);
            this.帮助ToolStripMenuItem.Text = "帮助";
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 210F));
            this.tableLayoutPanel1.Controls.Add(this.axRenderControl1, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.tabControl1, 1, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 36);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 1;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(1282, 658);
            this.tableLayoutPanel1.TabIndex = 1;
            // 
            // axRenderControl1
            // 
            this.axRenderControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.axRenderControl1.Enabled = true;
            this.axRenderControl1.Location = new System.Drawing.Point(3, 3);
            this.axRenderControl1.Name = "axRenderControl1";
            this.axRenderControl1.Size = new System.Drawing.Size(1066, 652);
            this.axRenderControl1.TabIndex = 0;
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(1075, 3);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(204, 652);
            this.tabControl1.TabIndex = 1;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.skyboxListView);
            this.tabPage1.Location = new System.Drawing.Point(4, 28);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(196, 620);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "SkyBox";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // skyboxListView
            // 
            this.skyboxListView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.skyboxListView.HideSelection = false;
            listViewItem1.Tag = "";
            listViewItem2.Tag = "";
            this.skyboxListView.Items.AddRange(new System.Windows.Forms.ListViewItem[] {
            listViewItem1,
            listViewItem2,
            listViewItem3,
            listViewItem4,
            listViewItem5,
            listViewItem6,
            listViewItem7,
            listViewItem8,
            listViewItem9,
            listViewItem10,
            listViewItem11,
            listViewItem12,
            listViewItem13,
            listViewItem14,
            listViewItem15,
            listViewItem16});
            this.skyboxListView.LargeImageList = this.imageList1;
            this.skyboxListView.Location = new System.Drawing.Point(3, 3);
            this.skyboxListView.Name = "skyboxListView";
            this.skyboxListView.Size = new System.Drawing.Size(190, 614);
            this.skyboxListView.TabIndex = 0;
            this.skyboxListView.UseCompatibleStateImageBehavior = false;
            this.skyboxListView.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.skyboxListView_MouseDoubleClick);
            // 
            // imageList1
            // 
            this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList1.Images.SetKeyName(0, "00_BK.jpg");
            this.imageList1.Images.SetKeyName(1, "金色晨曦");
            this.imageList1.Images.SetKeyName(2, "光暗之手");
            this.imageList1.Images.SetKeyName(3, "天马行空");
            this.imageList1.Images.SetKeyName(4, "飘絮人间");
            this.imageList1.Images.SetKeyName(5, "七彩紫罗");
            this.imageList1.Images.SetKeyName(6, "云中之触");
            this.imageList1.Images.SetKeyName(7, "鲲鹏万里");
            this.imageList1.Images.SetKeyName(8, "血色苍穹");
            this.imageList1.Images.SetKeyName(9, "白云旋天");
            this.imageList1.Images.SetKeyName(10, "长空破日");
            this.imageList1.Images.SetKeyName(11, "霞光掩影");
            this.imageList1.Images.SetKeyName(12, "混沌沧海");
            this.imageList1.Images.SetKeyName(13, "梦境之末");
            this.imageList1.Images.SetKeyName(14, "玄浑宇宙");
            this.imageList1.Images.SetKeyName(15, "月神之眼");
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.groupBox1);
            this.tabPage2.Controls.Add(this.dataGridView1);
            this.tabPage2.Location = new System.Drawing.Point(4, 28);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(190, 620);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "CameraTour";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.btnPause);
            this.groupBox1.Controls.Add(this.btnStop);
            this.groupBox1.Location = new System.Drawing.Point(6, 382);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(174, 63);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            // 
            // btnPause
            // 
            this.btnPause.Image = ((System.Drawing.Image)(resources.GetObject("btnPause.Image")));
            this.btnPause.Location = new System.Drawing.Point(101, 20);
            this.btnPause.Margin = new System.Windows.Forms.Padding(4);
            this.btnPause.Name = "btnPause";
            this.btnPause.Size = new System.Drawing.Size(34, 36);
            this.btnPause.TabIndex = 2;
            this.btnPause.UseVisualStyleBackColor = true;
            // 
            // btnStop
            // 
            this.btnStop.Image = ((System.Drawing.Image)(resources.GetObject("btnStop.Image")));
            this.btnStop.Location = new System.Drawing.Point(29, 20);
            this.btnStop.Margin = new System.Windows.Forms.Padding(4);
            this.btnStop.Name = "btnStop";
            this.btnStop.Size = new System.Drawing.Size(34, 36);
            this.btnStop.TabIndex = 1;
            this.btnStop.UseVisualStyleBackColor = true;
            // 
            // dataGridView1
            // 
            this.dataGridView1.BackgroundColor = System.Drawing.Color.WhiteSmoke;
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Location = new System.Drawing.Point(6, 6);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.RowHeadersWidth = 62;
            this.dataGridView1.RowTemplate.Height = 30;
            this.dataGridView1.Size = new System.Drawing.Size(179, 360);
            this.dataGridView1.TabIndex = 0;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 18F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1282, 694);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "Form1";
            this.Text = "Form1";
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.tableLayoutPanel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.axRenderControl1)).EndInit();
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage2.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem toolToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem fullScreenToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem captureScreenToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 请设置天气ToolStripMenuItem;
        private System.Windows.Forms.ToolStripComboBox toolStripComboBoxWeather;
        private System.Windows.Forms.ToolStripMenuItem 雾效果ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem toolStripFog;
        private System.Windows.Forms.ToolStripMenuItem 上一视图ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 下一视图ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 帮助ToolStripMenuItem;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private Axi3dRenderEngine.AxRenderControl axRenderControl1;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.ListView skyboxListView;
        private System.Windows.Forms.ImageList imageList1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button btnPause;
        private System.Windows.Forms.Button btnStop;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.HelpProvider helpProvider1;
    }
}

