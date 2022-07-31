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

namespace TrajectoryMonitor
{
    public partial class Form1 : Form
    {
        MapOperation controlOperation = null;
        BaseRequestService service;
        string Url = "http://47.101.51.219:10000/api/Carrier/RealtimePositon";
        public Form1()
        {
            InitializeComponent();

            controlOperation = new MapOperation();
            controlOperation.InitializationMapControl(axRenderControl1, "SH.3DM");
            controlOperation.InitlizedCameraPosition();

            dateTimePicker1.Value = DateTime.Today.AddDays(-1);
            dateTimePicker2.Value = DateTime.Today;

            service = new BaseRequestService();
        }

        // 加载模型
        bool isLoadCar = false;
        private void button1_Click(object sender, EventArgs e)
        {
            controlOperation.LoadCarModel();
            isLoadCar = true;
        }


        // 实时轨迹
        string route;
        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                timer1.Stop();
                route = string.Empty;
                if (!string.IsNullOrEmpty(textBox1.Text.Trim()) && isLoadCar)
                {
                    route = $"{Url}?id={textBox1.Text.Trim()}";
                    timer1.Start();
                }
            }
            catch (Exception ex)
            {
                timer1.Stop();
                lock (getDataLockObj)
                {
                    isBusy = false;
                }
                MessageBox.Show(ex.Message);
            }
        }

        // 定时执行
        private void timer1_Tick(object sender, EventArgs e)
        {
            _ = GetNotations();
        }

        private object getDataLockObj = new object();
        private bool isBusy = false;
        private async Task GetNotations()
        {
            lock (getDataLockObj)
            {
                if (isBusy)
                {
                    return;
                }
                isBusy = true;
            }
            if (!string.IsNullOrEmpty(textBox1.Text.Trim()))
            {
                var res = await service.RequestFromThirdPartyService<BaseResponse<Trajectory>>(route, null, RestSharp.Method.GET);
                if (res?.Code == 0)
                {
                    var notation = res.Data;
                    controlOperation.RenderRealTimeTrajectory(notation);
                }

                lock (getDataLockObj)
                {
                    isBusy = false;
                }
            }  
        }

        /// <summary>
        /// 历史轨迹
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button3_Click(object sender, EventArgs e)
        {

        }

        // 暂停历史轨迹
        private void button4_Click(object sender, EventArgs e)
        {
            controlOperation.PauseVehicleTrajectory();
        }

        // 继续历史轨迹
        private void button5_Click(object sender, EventArgs e)
        {
            controlOperation.ContinuePlayVT();
        }
    }
}
