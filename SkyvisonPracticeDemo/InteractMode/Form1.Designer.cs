
namespace InteractMode
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
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.toolStripDropDownButton1 = new System.Windows.Forms.ToolStripDropDownButton();
            this.toolStripNormal = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSelect = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMeasurement = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripCoordinate = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripAerialDistance = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripHorizontalDistance = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripVerticalDistance = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripGroundDistance = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripArea = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripGroundArea = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripGroupSightLine = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripWalk = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripDisable = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStrip2DMap = new System.Windows.Forms.ToolStripMenuItem();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.toolStrip1.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolStrip1
            // 
            this.toolStrip1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.toolStrip1.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripDropDownButton1});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(1290, 50);
            this.toolStrip1.TabIndex = 0;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // toolStripDropDownButton1
            // 
            this.toolStripDropDownButton1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripNormal,
            this.toolStripSelect,
            this.toolStripMeasurement,
            this.toolStripWalk,
            this.toolStripDisable,
            this.toolStrip2DMap});
            this.toolStripDropDownButton1.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripDropDownButton1.Name = "toolStripDropDownButton1";
            this.toolStripDropDownButton1.Size = new System.Drawing.Size(100, 45);
            this.toolStripDropDownButton1.Text = "交互模式";
            // 
            // toolStripNormal
            // 
            this.toolStripNormal.Name = "toolStripNormal";
            this.toolStripNormal.Size = new System.Drawing.Size(182, 34);
            this.toolStripNormal.Text = "漫游";
            this.toolStripNormal.Click += new System.EventHandler(this.toolStripNormal_Click);
            // 
            // toolStripSelect
            // 
            this.toolStripSelect.Name = "toolStripSelect";
            this.toolStripSelect.Size = new System.Drawing.Size(182, 34);
            this.toolStripSelect.Text = "拾取";
            this.toolStripSelect.Click += new System.EventHandler(this.toolStripSelect_Click);
            // 
            // toolStripMeasurement
            // 
            this.toolStripMeasurement.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripCoordinate,
            this.toolStripAerialDistance,
            this.toolStripHorizontalDistance,
            this.toolStripVerticalDistance,
            this.toolStripGroundDistance,
            this.toolStripArea,
            this.toolStripGroundArea,
            this.toolStripGroupSightLine});
            this.toolStripMeasurement.Name = "toolStripMeasurement";
            this.toolStripMeasurement.Size = new System.Drawing.Size(182, 34);
            this.toolStripMeasurement.Text = "测量";
            // 
            // toolStripCoordinate
            // 
            this.toolStripCoordinate.Name = "toolStripCoordinate";
            this.toolStripCoordinate.Size = new System.Drawing.Size(218, 34);
            this.toolStripCoordinate.Text = "拾取坐标";
            this.toolStripCoordinate.Click += new System.EventHandler(this.toolStripCoordinate_Click);
            // 
            // toolStripAerialDistance
            // 
            this.toolStripAerialDistance.Name = "toolStripAerialDistance";
            this.toolStripAerialDistance.Size = new System.Drawing.Size(218, 34);
            this.toolStripAerialDistance.Text = "直线测距";
            this.toolStripAerialDistance.Click += new System.EventHandler(this.toolStripAerialDistance_Click);
            // 
            // toolStripHorizontalDistance
            // 
            this.toolStripHorizontalDistance.Name = "toolStripHorizontalDistance";
            this.toolStripHorizontalDistance.Size = new System.Drawing.Size(218, 34);
            this.toolStripHorizontalDistance.Text = "水平测距";
            this.toolStripHorizontalDistance.Click += new System.EventHandler(this.toolStripHorizontalDistance_Click);
            // 
            // toolStripVerticalDistance
            // 
            this.toolStripVerticalDistance.Name = "toolStripVerticalDistance";
            this.toolStripVerticalDistance.Size = new System.Drawing.Size(218, 34);
            this.toolStripVerticalDistance.Text = "垂直测距";
            this.toolStripVerticalDistance.Click += new System.EventHandler(this.toolStripVerticalDistance_Click);
            // 
            // toolStripGroundDistance
            // 
            this.toolStripGroundDistance.Name = "toolStripGroundDistance";
            this.toolStripGroundDistance.Size = new System.Drawing.Size(218, 34);
            this.toolStripGroundDistance.Text = "地表距离";
            this.toolStripGroundDistance.Click += new System.EventHandler(this.toolStripGroundDistance_Click);
            // 
            // toolStripArea
            // 
            this.toolStripArea.Name = "toolStripArea";
            this.toolStripArea.Size = new System.Drawing.Size(218, 34);
            this.toolStripArea.Text = "投影面积";
            this.toolStripArea.Click += new System.EventHandler(this.toolStripArea_Click);
            // 
            // toolStripGroundArea
            // 
            this.toolStripGroundArea.Name = "toolStripGroundArea";
            this.toolStripGroundArea.Size = new System.Drawing.Size(218, 34);
            this.toolStripGroundArea.Text = "地表面积";
            this.toolStripGroundArea.Click += new System.EventHandler(this.toolStripGroundArea_Click);
            // 
            // toolStripGroupSightLine
            // 
            this.toolStripGroupSightLine.Name = "toolStripGroupSightLine";
            this.toolStripGroupSightLine.Size = new System.Drawing.Size(218, 34);
            this.toolStripGroupSightLine.Text = "地形通视分析";
            this.toolStripGroupSightLine.Click += new System.EventHandler(this.toolStripGroupSightLine_Click);
            // 
            // toolStripWalk
            // 
            this.toolStripWalk.Name = "toolStripWalk";
            this.toolStripWalk.Size = new System.Drawing.Size(182, 34);
            this.toolStripWalk.Text = "步行";
            this.toolStripWalk.Click += new System.EventHandler(this.toolStripWalk_Click);
            // 
            // toolStripDisable
            // 
            this.toolStripDisable.Name = "toolStripDisable";
            this.toolStripDisable.Size = new System.Drawing.Size(182, 34);
            this.toolStripDisable.Text = "禁止交互";
            this.toolStripDisable.Click += new System.EventHandler(this.toolStripDisable_Click);
            // 
            // toolStrip2DMap
            // 
            this.toolStrip2DMap.Name = "toolStrip2DMap";
            this.toolStrip2DMap.Size = new System.Drawing.Size(182, 34);
            this.toolStrip2DMap.Text = "二维地图";
            this.toolStrip2DMap.Click += new System.EventHandler(this.toolStrip2DMap_Click);
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.Controls.Add(this.toolStrip1, 0, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 50F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(1290, 739);
            this.tableLayoutPanel1.TabIndex = 1;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 18F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1290, 739);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "Form1";
            this.Text = "Form1";
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripDropDownButton toolStripDropDownButton1;
        private System.Windows.Forms.ToolStripMenuItem toolStripNormal;
        private System.Windows.Forms.ToolStripMenuItem toolStripSelect;
        private System.Windows.Forms.ToolStripMenuItem toolStripMeasurement;
        private System.Windows.Forms.ToolStripMenuItem toolStripCoordinate;
        private System.Windows.Forms.ToolStripMenuItem toolStripAerialDistance;
        private System.Windows.Forms.ToolStripMenuItem toolStripHorizontalDistance;
        private System.Windows.Forms.ToolStripMenuItem toolStripVerticalDistance;
        private System.Windows.Forms.ToolStripMenuItem toolStripGroundDistance;
        private System.Windows.Forms.ToolStripMenuItem toolStripArea;
        private System.Windows.Forms.ToolStripMenuItem toolStripGroundArea;
        private System.Windows.Forms.ToolStripMenuItem toolStripGroupSightLine;
        private System.Windows.Forms.ToolStripMenuItem toolStripWalk;
        private System.Windows.Forms.ToolStripMenuItem toolStripDisable;
        private System.Windows.Forms.ToolStripMenuItem toolStrip2DMap;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
    }
}

