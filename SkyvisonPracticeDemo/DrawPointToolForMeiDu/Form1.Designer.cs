
namespace DrawPointToolForMeiDu
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.button6 = new System.Windows.Forms.Button();
            this.button5 = new System.Windows.Forms.Button();
            this.button4 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.labelTotalPointCount = new System.Windows.Forms.Label();
            this.label14 = new System.Windows.Forms.Label();
            this.labelPointCount = new System.Windows.Forms.Label();
            this.label13 = new System.Windows.Forms.Label();
            this.labelEndPointY = new System.Windows.Forms.Label();
            this.labelEndPointX = new System.Windows.Forms.Label();
            this.labelStartPointY = new System.Windows.Forms.Label();
            this.labelStartPointX = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.txtMaxBackwardSpeed = new System.Windows.Forms.TextBox();
            this.txtMaxForwardSpeed = new System.Windows.Forms.TextBox();
            this.comboBox = new System.Windows.Forms.ComboBox();
            this.label8 = new System.Windows.Forms.Label();
            this.labelID = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.txtPriority = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.txtTarget = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.txtSource = new System.Windows.Forms.TextBox();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.richTextBox1 = new System.Windows.Forms.RichTextBox();
            this.axRenderControl1 = new Axi3dRenderEngine.AxRenderControl();
            this.tableLayoutPanel1.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.axRenderControl1)).BeginInit();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.groupBox1, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.groupBox2, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel2, 0, 2);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 3;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 100F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 100F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(2289, 1092);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.button6);
            this.groupBox1.Controls.Add(this.button5);
            this.groupBox1.Controls.Add(this.button4);
            this.groupBox1.Controls.Add(this.button3);
            this.groupBox1.Controls.Add(this.button2);
            this.groupBox1.Controls.Add(this.button1);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox1.Location = new System.Drawing.Point(3, 3);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(2283, 94);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "操作";
            // 
            // button6
            // 
            this.button6.Location = new System.Drawing.Point(648, 38);
            this.button6.Name = "button6";
            this.button6.Size = new System.Drawing.Size(102, 33);
            this.button6.TabIndex = 6;
            this.button6.Text = "删除";
            this.button6.UseVisualStyleBackColor = true;
            this.button6.Click += new System.EventHandler(this.button6_Click);
            // 
            // button5
            // 
            this.button5.Location = new System.Drawing.Point(435, 38);
            this.button5.Name = "button5";
            this.button5.Size = new System.Drawing.Size(102, 33);
            this.button5.TabIndex = 5;
            this.button5.Text = "选取";
            this.button5.UseVisualStyleBackColor = true;
            this.button5.Click += new System.EventHandler(this.button5_Click);
            // 
            // button4
            // 
            this.button4.Location = new System.Drawing.Point(1074, 38);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(102, 33);
            this.button4.TabIndex = 1;
            this.button4.Text = "保存";
            this.button4.UseVisualStyleBackColor = true;
            this.button4.Click += new System.EventHandler(this.button4_Click);
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(222, 38);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(102, 33);
            this.button3.TabIndex = 2;
            this.button3.Text = "画线";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(861, 38);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(102, 33);
            this.button2.TabIndex = 3;
            this.button2.Text = "坐标";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(9, 38);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(102, 33);
            this.button1.TabIndex = 4;
            this.button1.Text = "漫游";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.labelTotalPointCount);
            this.groupBox2.Controls.Add(this.label14);
            this.groupBox2.Controls.Add(this.labelPointCount);
            this.groupBox2.Controls.Add(this.label13);
            this.groupBox2.Controls.Add(this.labelEndPointY);
            this.groupBox2.Controls.Add(this.labelEndPointX);
            this.groupBox2.Controls.Add(this.labelStartPointY);
            this.groupBox2.Controls.Add(this.labelStartPointX);
            this.groupBox2.Controls.Add(this.label11);
            this.groupBox2.Controls.Add(this.label10);
            this.groupBox2.Controls.Add(this.label9);
            this.groupBox2.Controls.Add(this.label7);
            this.groupBox2.Controls.Add(this.checkBox1);
            this.groupBox2.Controls.Add(this.txtMaxBackwardSpeed);
            this.groupBox2.Controls.Add(this.txtMaxForwardSpeed);
            this.groupBox2.Controls.Add(this.comboBox);
            this.groupBox2.Controls.Add(this.label8);
            this.groupBox2.Controls.Add(this.labelID);
            this.groupBox2.Controls.Add(this.label3);
            this.groupBox2.Controls.Add(this.label6);
            this.groupBox2.Controls.Add(this.label5);
            this.groupBox2.Controls.Add(this.label4);
            this.groupBox2.Controls.Add(this.txtPriority);
            this.groupBox2.Controls.Add(this.label2);
            this.groupBox2.Controls.Add(this.txtTarget);
            this.groupBox2.Controls.Add(this.label1);
            this.groupBox2.Controls.Add(this.txtSource);
            this.groupBox2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox2.Location = new System.Drawing.Point(3, 103);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(2283, 94);
            this.groupBox2.TabIndex = 1;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "参数";
            // 
            // labelTotalPointCount
            // 
            this.labelTotalPointCount.AutoSize = true;
            this.labelTotalPointCount.Location = new System.Drawing.Point(2060, 62);
            this.labelTotalPointCount.Name = "labelTotalPointCount";
            this.labelTotalPointCount.Size = new System.Drawing.Size(17, 18);
            this.labelTotalPointCount.TabIndex = 32;
            this.labelTotalPointCount.Text = "0";
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label14.Location = new System.Drawing.Point(1877, 62);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(177, 18);
            this.label14.TabIndex = 31;
            this.label14.Text = "TotalPointCount：";
            // 
            // labelPointCount
            // 
            this.labelPointCount.AutoSize = true;
            this.labelPointCount.Location = new System.Drawing.Point(2060, 26);
            this.labelPointCount.Name = "labelPointCount";
            this.labelPointCount.Size = new System.Drawing.Size(17, 18);
            this.labelPointCount.TabIndex = 30;
            this.labelPointCount.Text = "0";
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label13.Location = new System.Drawing.Point(1927, 26);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(127, 18);
            this.label13.TabIndex = 29;
            this.label13.Text = "PointCount：";
            // 
            // labelEndPointY
            // 
            this.labelEndPointY.AutoSize = true;
            this.labelEndPointY.Location = new System.Drawing.Point(1680, 62);
            this.labelEndPointY.Name = "labelEndPointY";
            this.labelEndPointY.Size = new System.Drawing.Size(17, 18);
            this.labelEndPointY.TabIndex = 28;
            this.labelEndPointY.Text = "0";
            // 
            // labelEndPointX
            // 
            this.labelEndPointX.AutoSize = true;
            this.labelEndPointX.Location = new System.Drawing.Point(1680, 26);
            this.labelEndPointX.Name = "labelEndPointX";
            this.labelEndPointX.Size = new System.Drawing.Size(17, 18);
            this.labelEndPointX.TabIndex = 27;
            this.labelEndPointX.Text = "0";
            // 
            // labelStartPointY
            // 
            this.labelStartPointY.AutoSize = true;
            this.labelStartPointY.Location = new System.Drawing.Point(1360, 62);
            this.labelStartPointY.Name = "labelStartPointY";
            this.labelStartPointY.Size = new System.Drawing.Size(17, 18);
            this.labelStartPointY.TabIndex = 26;
            this.labelStartPointY.Text = "0";
            // 
            // labelStartPointX
            // 
            this.labelStartPointX.AutoSize = true;
            this.labelStartPointX.Location = new System.Drawing.Point(1360, 26);
            this.labelStartPointX.Name = "labelStartPointX";
            this.labelStartPointX.Size = new System.Drawing.Size(17, 18);
            this.labelStartPointX.TabIndex = 25;
            this.labelStartPointX.Text = "0";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label11.Location = new System.Drawing.Point(1547, 62);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(127, 18);
            this.label11.TabIndex = 24;
            this.label11.Text = "EndPoint.Y：";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label10.Location = new System.Drawing.Point(1547, 26);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(127, 18);
            this.label10.TabIndex = 23;
            this.label10.Text = "EndPoint.X：";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label9.Location = new System.Drawing.Point(1207, 62);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(147, 18);
            this.label9.TabIndex = 22;
            this.label9.Text = "StartPoint.Y：";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label7.Location = new System.Drawing.Point(1207, 26);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(147, 18);
            this.label7.TabIndex = 21;
            this.label7.Text = "StartPoint.X：";
            // 
            // checkBox1
            // 
            this.checkBox1.AutoSize = true;
            this.checkBox1.Location = new System.Drawing.Point(24, 25);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(142, 22);
            this.checkBox1.TabIndex = 20;
            this.checkBox1.Text = "修改起始顶点";
            this.checkBox1.UseVisualStyleBackColor = true;
            this.checkBox1.CheckedChanged += new System.EventHandler(this.checkBox1_CheckedChanged);
            // 
            // txtMaxBackwardSpeed
            // 
            this.txtMaxBackwardSpeed.Location = new System.Drawing.Point(1060, 57);
            this.txtMaxBackwardSpeed.Name = "txtMaxBackwardSpeed";
            this.txtMaxBackwardSpeed.Size = new System.Drawing.Size(119, 28);
            this.txtMaxBackwardSpeed.TabIndex = 19;
            this.txtMaxBackwardSpeed.Text = "120";
            // 
            // txtMaxForwardSpeed
            // 
            this.txtMaxForwardSpeed.Location = new System.Drawing.Point(1060, 22);
            this.txtMaxForwardSpeed.Name = "txtMaxForwardSpeed";
            this.txtMaxForwardSpeed.Size = new System.Drawing.Size(119, 28);
            this.txtMaxForwardSpeed.TabIndex = 18;
            this.txtMaxForwardSpeed.Text = "120";
            // 
            // comboBox
            // 
            this.comboBox.FormattingEnabled = true;
            this.comboBox.Items.AddRange(new object[] {
            "是",
            "否"});
            this.comboBox.Location = new System.Drawing.Point(641, 23);
            this.comboBox.Name = "comboBox";
            this.comboBox.Size = new System.Drawing.Size(94, 26);
            this.comboBox.TabIndex = 17;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label8.Location = new System.Drawing.Point(24, 62);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(47, 18);
            this.label8.TabIndex = 14;
            this.label8.Text = "ID：";
            // 
            // labelID
            // 
            this.labelID.AutoSize = true;
            this.labelID.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.labelID.Location = new System.Drawing.Point(84, 62);
            this.labelID.Name = "labelID";
            this.labelID.Size = new System.Drawing.Size(18, 18);
            this.labelID.TabIndex = 15;
            this.labelID.Text = "1";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label3.Location = new System.Drawing.Point(548, 27);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(87, 18);
            this.label3.TabIndex = 16;
            this.label3.Text = "Oneway：";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label6.Location = new System.Drawing.Point(807, 62);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(247, 18);
            this.label6.TabIndex = 12;
            this.label6.Text = "MaxBackwardSpeed(km/h)：";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label5.Location = new System.Drawing.Point(817, 27);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(237, 18);
            this.label5.TabIndex = 11;
            this.label5.Text = "MaxForwardSpeed(km/h)：";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label4.Location = new System.Drawing.Point(528, 62);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(107, 18);
            this.label4.TabIndex = 8;
            this.label4.Text = "Priority：";
            // 
            // txtPriority
            // 
            this.txtPriority.Location = new System.Drawing.Point(641, 57);
            this.txtPriority.Name = "txtPriority";
            this.txtPriority.Size = new System.Drawing.Size(94, 28);
            this.txtPriority.TabIndex = 10;
            this.txtPriority.Text = "1.0";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label2.Location = new System.Drawing.Point(183, 62);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(87, 18);
            this.label2.TabIndex = 5;
            this.label2.Text = "Target：";
            // 
            // txtTarget
            // 
            this.txtTarget.Location = new System.Drawing.Point(284, 57);
            this.txtTarget.Name = "txtTarget";
            this.txtTarget.ReadOnly = true;
            this.txtTarget.Size = new System.Drawing.Size(183, 28);
            this.txtTarget.TabIndex = 6;
            this.txtTarget.Text = "10001";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label1.Location = new System.Drawing.Point(183, 27);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(87, 18);
            this.label1.TabIndex = 3;
            this.label1.Text = "Source：";
            // 
            // txtSource
            // 
            this.txtSource.Location = new System.Drawing.Point(284, 22);
            this.txtSource.Name = "txtSource";
            this.txtSource.ReadOnly = true;
            this.txtSource.Size = new System.Drawing.Size(183, 28);
            this.txtSource.TabIndex = 4;
            this.txtSource.Text = "10000";
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.ColumnCount = 2;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 70F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 30F));
            this.tableLayoutPanel2.Controls.Add(this.richTextBox1, 1, 0);
            this.tableLayoutPanel2.Controls.Add(this.axRenderControl1, 0, 0);
            this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel2.Location = new System.Drawing.Point(3, 203);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 1;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 886F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(2283, 886);
            this.tableLayoutPanel2.TabIndex = 2;
            // 
            // richTextBox1
            // 
            this.richTextBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.richTextBox1.Location = new System.Drawing.Point(1601, 3);
            this.richTextBox1.Name = "richTextBox1";
            this.richTextBox1.Size = new System.Drawing.Size(679, 880);
            this.richTextBox1.TabIndex = 1;
            this.richTextBox1.Text = "";
            // 
            // axRenderControl1
            // 
            this.axRenderControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.axRenderControl1.Enabled = true;
            this.axRenderControl1.Location = new System.Drawing.Point(3, 3);
            this.axRenderControl1.Name = "axRenderControl1";
            this.axRenderControl1.Size = new System.Drawing.Size(1592, 880);
            this.axRenderControl1.TabIndex = 0;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 18F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(2289, 1092);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "Form1";
            this.Text = "DrawRoadMapTool";
            this.tableLayoutPanel1.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.tableLayoutPanel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.axRenderControl1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtTarget;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtSource;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private Axi3dRenderEngine.AxRenderControl axRenderControl1;
        private System.Windows.Forms.RichTextBox richTextBox1;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox txtPriority;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox txtMaxBackwardSpeed;
        private System.Windows.Forms.TextBox txtMaxForwardSpeed;
        private System.Windows.Forms.ComboBox comboBox;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label labelID;
        private System.Windows.Forms.CheckBox checkBox1;
        private System.Windows.Forms.Label labelPointCount;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.Label labelEndPointY;
        private System.Windows.Forms.Label labelEndPointX;
        private System.Windows.Forms.Label labelStartPointY;
        private System.Windows.Forms.Label labelStartPointX;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label labelTotalPointCount;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.Button button6;
        private System.Windows.Forms.Button button5;
    }
}

