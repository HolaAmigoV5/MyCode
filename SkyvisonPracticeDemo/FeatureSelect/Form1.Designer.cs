
namespace FeatureSelect
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.cbCtrlEnable = new System.Windows.Forms.CheckBox();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.toolStripLabel1 = new System.Windows.Forms.ToolStripLabel();
            this.toolStripSelectModeSetting = new System.Windows.Forms.ToolStripComboBox();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripUnHightLight = new System.Windows.Forms.ToolStripButton();
            this.axRenderControl1 = new Axi3dRenderEngine.AxRenderControl();
            this.tableLayoutPanel1.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.axRenderControl1)).BeginInit();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 150F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.cbCtrlEnable, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.toolStrip1, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.axRenderControl1, 1, 1);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 45F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(1324, 799);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // cbCtrlEnable
            // 
            this.cbCtrlEnable.AutoSize = true;
            this.cbCtrlEnable.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cbCtrlEnable.Font = new System.Drawing.Font("微软雅黑", 9F);
            this.cbCtrlEnable.Location = new System.Drawing.Point(3, 3);
            this.cbCtrlEnable.Name = "cbCtrlEnable";
            this.cbCtrlEnable.Size = new System.Drawing.Size(144, 39);
            this.cbCtrlEnable.TabIndex = 0;
            this.cbCtrlEnable.Text = "支持Ctrl键";
            this.cbCtrlEnable.UseVisualStyleBackColor = true;
            this.cbCtrlEnable.CheckedChanged += new System.EventHandler(this.cbCtrlEnable_CheckedChanged);
            // 
            // toolStrip1
            // 
            this.toolStrip1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.toolStrip1.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripLabel1,
            this.toolStripSelectModeSetting,
            this.toolStripSeparator1,
            this.toolStripUnHightLight});
            this.toolStrip1.Location = new System.Drawing.Point(150, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(1174, 45);
            this.toolStrip1.TabIndex = 1;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // toolStripLabel1
            // 
            this.toolStripLabel1.Name = "toolStripLabel1";
            this.toolStripLabel1.Size = new System.Drawing.Size(136, 40);
            this.toolStripLabel1.Text = "拾取模式设置：";
            // 
            // toolStripSelectModeSetting
            // 
            this.toolStripSelectModeSetting.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.toolStripSelectModeSetting.Items.AddRange(new object[] {
            "漫游不拾取",
            "仅点选",
            "仅框选",
            "点选+框选"});
            this.toolStripSelectModeSetting.Name = "toolStripSelectModeSetting";
            this.toolStripSelectModeSetting.Size = new System.Drawing.Size(121, 45);
            this.toolStripSelectModeSetting.SelectedIndexChanged += new System.EventHandler(this.toolStripSelectModeSetting_SelectedIndexChanged);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 45);
            // 
            // toolStripUnHightLight
            // 
            this.toolStripUnHightLight.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripUnHightLight.Image = ((System.Drawing.Image)(resources.GetObject("toolStripUnHightLight.Image")));
            this.toolStripUnHightLight.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripUnHightLight.Name = "toolStripUnHightLight";
            this.toolStripUnHightLight.Size = new System.Drawing.Size(34, 40);
            this.toolStripUnHightLight.Text = "取消高亮";
            this.toolStripUnHightLight.Click += new System.EventHandler(this.toolStripUnHightLight_Click);
            // 
            // axRenderControl1
            // 
            this.axRenderControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.axRenderControl1.Enabled = true;
            this.axRenderControl1.Location = new System.Drawing.Point(153, 48);
            this.axRenderControl1.Name = "axRenderControl1";
            this.axRenderControl1.Size = new System.Drawing.Size(1168, 748);
            this.axRenderControl1.TabIndex = 2;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 18F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1324, 799);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Form1";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.axRenderControl1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.CheckBox cbCtrlEnable;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripLabel toolStripLabel1;
        private System.Windows.Forms.ToolStripComboBox toolStripSelectModeSetting;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripButton toolStripUnHightLight;
        private Axi3dRenderEngine.AxRenderControl axRenderControl1;
    }
}

