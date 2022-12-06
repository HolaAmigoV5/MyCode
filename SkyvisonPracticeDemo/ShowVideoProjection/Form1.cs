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

namespace ShowVideoProjection
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
            controlOperation.InitializationMapControl(axRenderControl, "UCAS.3DM");
            controlOperation.InitlizedGKDCameraPosition();
        }

        bool isNormal = true;
        // 坐标拾取
        private void button1_Click(object sender, EventArgs e)
        {
            isNormal = !isNormal;
            button1.Text = isNormal ? "坐标拾取" : "漫游";
            controlOperation.MeasureCoordinate(isNormal);
            //controlOperation.SetMapModel(isNormal);
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

        // 加载GIF动画
        private void button4_Click(object sender, EventArgs e)
        {
            Gif gif0 = new Gif()
            {
                X = 116.244675,
                Y = 39.906896,
                Z = 0.20545,
                GifName = "hongqi.gif",
                HeightVecX = 0,
                HeightVecY = 0,
                HeightVecZ = 10,
                WidthVecX = 3.7,
                WidthVecY = 0,
                WidthVecZ = 0
            };
            controlOperation.CreateRenderGif(gif0);

            Gif gif1 = new Gif()
            {
                X = 116.2445039,
                Y = 39.907048,
                Z = 0.20545,
                GifName = "g1.gif",
                WidthVecX = 3.98,
                WidthVecY = 0,
                WidthVecZ = 0,
                HeightVecX = 0,
                HeightVecY = 0,
                HeightVecZ = 6.39
            };
            controlOperation.CreateRenderGif(gif1);

            Gif gif2 = new Gif()
            {
                X = 116.2448101,
                Y = 39.90693597,
                Z = 0.20545,
                GifName = "g2.gif",
                WidthVecX = 4.78,
                WidthVecY = 0,
                WidthVecZ = 0,
                HeightVecX = 0,
                HeightVecY = 0,
                HeightVecZ = 7.3
            };
            controlOperation.CreateRenderGif(gif2);

            Gif gif3 = new Gif()
            {
                X = 116.24446902,
                Y = 39.90699069,
                Z = 0.20545,
                GifName = "g3.gif",
                WidthVecX = 5.79,
                WidthVecY = 0,
                WidthVecZ = 0,
                HeightVecX = 0,
                HeightVecY = 0,
                HeightVecZ = 6.13
            };
            controlOperation.CreateRenderGif(gif3);
        }
    }
}
