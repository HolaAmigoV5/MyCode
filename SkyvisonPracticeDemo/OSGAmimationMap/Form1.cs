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

        public Form1()
        {
            InitializeComponent();

            controlOperation = new MapOperation();
            controlOperation.InitializationMapControl(axRenderControl1);
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
