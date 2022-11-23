
namespace CarModelPath
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
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.button9 = new System.Windows.Forms.Button();
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.button2 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.button5 = new System.Windows.Forms.Button();
            this.button8 = new System.Windows.Forms.Button();
            this.button4 = new System.Windows.Forms.Button();
            this.button7 = new System.Windows.Forms.Button();
            this.button6 = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.axRenderControl1 = new Axi3dRenderEngine.AxRenderControl();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.axRenderControl1)).BeginInit();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.groupBox1);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.axRenderControl1);
            this.splitContainer1.Size = new System.Drawing.Size(1798, 846);
            this.splitContainer1.SplitterDistance = 97;
            this.splitContainer1.TabIndex = 0;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.button9);
            this.groupBox1.Controls.Add(this.comboBox1);
            this.groupBox1.Controls.Add(this.button2);
            this.groupBox1.Controls.Add(this.button3);
            this.groupBox1.Controls.Add(this.button5);
            this.groupBox1.Controls.Add(this.button8);
            this.groupBox1.Controls.Add(this.button4);
            this.groupBox1.Controls.Add(this.button7);
            this.groupBox1.Controls.Add(this.button6);
            this.groupBox1.Controls.Add(this.button1);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox1.Location = new System.Drawing.Point(0, 0);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(1798, 97);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "操作";
            // 
            // button9
            // 
            this.button9.Location = new System.Drawing.Point(1535, 23);
            this.button9.Name = "button9";
            this.button9.Size = new System.Drawing.Size(107, 34);
            this.button9.TabIndex = 2;
            this.button9.Text = "拾取";
            this.button9.UseVisualStyleBackColor = true;
            this.button9.Click += new System.EventHandler(this.button9_Click);
            // 
            // comboBox1
            // 
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Items.AddRange(new object[] {
            "Box001.osg",
            "Box002.osg",
            "Box003.osg",
            "Box004.osg",
            "Box005.osg",
            "Box006.osg",
            "Box007.osg",
            "Box008.osg",
            "hddk07car015.osg",
            "hddk07car017.osg",
            "hddk07car018.osg",
            "hddk07car019.osg",
            "hddk07car020.osg",
            "hddk07car021.osg",
            "hddk07car022.osg",
            "hddk07car023.osg",
            "hddk07car024.osg",
            "hddk07car025.osg",
            "hddk07car026.osg",
            "hddk07car027.osg",
            "hddk07car028.osg",
            "hddk07car029.osg",
            "hddk07car030.osg",
            "hddk07car031.osg",
            "hddk07car032.osg",
            "hddk07car033.osg",
            "hddk07car034.osg",
            "hddk07car035.osg"});
            this.comboBox1.Location = new System.Drawing.Point(793, 31);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(212, 26);
            this.comboBox1.TabIndex = 1;
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(1218, 24);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(107, 34);
            this.button2.TabIndex = 0;
            this.button2.Text = "暂停";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(1389, 24);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(107, 34);
            this.button3.TabIndex = 0;
            this.button3.Text = "继续";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // button5
            // 
            this.button5.Location = new System.Drawing.Point(206, 24);
            this.button5.Name = "button5";
            this.button5.Size = new System.Drawing.Size(107, 34);
            this.button5.TabIndex = 0;
            this.button5.Text = "保存轨迹";
            this.button5.UseVisualStyleBackColor = true;
            this.button5.Click += new System.EventHandler(this.button5_Click);
            // 
            // button8
            // 
            this.button8.Location = new System.Drawing.Point(369, 24);
            this.button8.Name = "button8";
            this.button8.Size = new System.Drawing.Size(107, 34);
            this.button8.TabIndex = 0;
            this.button8.Text = "初始化位置";
            this.button8.UseVisualStyleBackColor = true;
            this.button8.Click += new System.EventHandler(this.button8_Click);
            // 
            // button4
            // 
            this.button4.Location = new System.Drawing.Point(49, 24);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(107, 34);
            this.button4.TabIndex = 0;
            this.button4.Text = "画轨迹";
            this.button4.UseVisualStyleBackColor = true;
            this.button4.Click += new System.EventHandler(this.button4_Click);
            // 
            // button7
            // 
            this.button7.Location = new System.Drawing.Point(547, 24);
            this.button7.Name = "button7";
            this.button7.Size = new System.Drawing.Size(107, 34);
            this.button7.TabIndex = 0;
            this.button7.Text = "漫游";
            this.button7.UseVisualStyleBackColor = true;
            this.button7.Click += new System.EventHandler(this.button7_Click);
            // 
            // button6
            // 
            this.button6.Location = new System.Drawing.Point(660, 24);
            this.button6.Name = "button6";
            this.button6.Size = new System.Drawing.Size(107, 34);
            this.button6.TabIndex = 0;
            this.button6.Text = "加载模型";
            this.button6.UseVisualStyleBackColor = true;
            this.button6.Click += new System.EventHandler(this.button6_Click);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(1061, 24);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(107, 34);
            this.button1.TabIndex = 0;
            this.button1.Text = "播放轨迹";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // axRenderControl1
            // 
            this.axRenderControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.axRenderControl1.Enabled = true;
            this.axRenderControl1.Location = new System.Drawing.Point(0, 0);
            this.axRenderControl1.Name = "axRenderControl1";
            this.axRenderControl1.Size = new System.Drawing.Size(1798, 745);
            this.axRenderControl1.TabIndex = 0;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 18F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1798, 846);
            this.Controls.Add(this.splitContainer1);
            this.Name = "Form1";
            this.Text = "Form1";
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.axRenderControl1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.Button button1;
        private Axi3dRenderEngine.AxRenderControl axRenderControl1;
        private System.Windows.Forms.Button button5;
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.Button button6;
        private System.Windows.Forms.Button button7;
        private System.Windows.Forms.Button button8;
        private System.Windows.Forms.ComboBox comboBox1;
        private System.Windows.Forms.Button button9;
    }
}

