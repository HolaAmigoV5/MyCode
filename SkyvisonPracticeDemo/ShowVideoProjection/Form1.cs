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

namespace ShowVideoProjection
{
    public partial class Form1 : Form
    {
        MapOperation controlOperation = null;
        public Form1()
        {
            InitializeComponent();

            controlOperation = new MapOperation();
            controlOperation.InitializationMapControl(axRenderControl1, "UCAS.3DM");
            controlOperation.InitlizedCameraPosition();
        }

        // 坐标拾取
        private void button1_Click(object sender, EventArgs e)
        {

        }

        // 视频投射
        private void button2_Click(object sender, EventArgs e)
        {
            double.TryParse(tBoxX.Text.Trim(), out double x);
            double.TryParse(tBoxY.Text.Trim(), out double y);
            double.TryParse(tBoxZ.Text.Trim(), out double z);
            double.TryParse(tBoxH.Text.Trim(), out double heading);
            double.TryParse(tBoxR.Text.Trim(), out double roll);
            double.TryParse(tBoxT.Text.Trim(), out double tilt);
            double.TryParse(tBoxF.Text.Trim(), out double farclip);
            double.TryParse(tBoxA.Text.Trim(), out double aspectRatio);
            double.TryParse(tBoxFi.Text.Trim(), out double fieldOfView);
            double.TryParse(tBoxV.Text.Trim(), out double vPos);

            var res = controlOperation.ShowVideoProjection(x, y, z, heading, tilt, roll, farclip, aspectRatio, fieldOfView, vPos);
            tBoxPos.Text = res;
        }

        // 获取相机位置
        private void button3_Click(object sender, EventArgs e)
        {
            var pos = controlOperation.GetCameraPosition();
            tBoxPos.Text = $"X = {pos.X}, Y = {pos.Y}, Z = {pos.Z}, Heading = {pos.Heading}, Roll = {pos.Roll}, Tilt = {pos.Tilt}";
        }
    }
}
