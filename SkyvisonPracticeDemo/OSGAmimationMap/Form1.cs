using Axi3dRenderEngine;
using CommonMapLib;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OSGAmimationMap
{
    public partial class Form1 : Form
    {
        MapOperation controlOperation = null;
        AxRenderControl axRenderControl;

        public Form1()
        {
            InitializeComponent();

            axRenderControl = new AxRenderControl();
            axRenderControl.BeginInit();
            axRenderControl.Dock = DockStyle.Fill;
            splitContainer1.Panel2.Controls.Add(axRenderControl);
            axRenderControl.EndInit();

            controlOperation = new MapOperation();
            controlOperation.InitializationMapControl(axRenderControl, isPlanarTerrain: true);
            comboBox1.SelectedIndex = 0;
        }

        // 设置点选
        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                string name = "22.OSG";
                if (!string.IsNullOrEmpty(comboBox1.Text.Trim()))
                    name = comboBox1.Text.Trim();

                controlOperation.ShowAnimationOSG2(name);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            
        }

        private void button2_Click(object sender, EventArgs e)
        {
            string name = "wrj.X";
            if (!string.IsNullOrEmpty(textBox2.Text.Trim()))
                name = textBox2.Text.Trim();

            controlOperation.ShowSkeletonAmimation(name);
        }
    }
}
