using Axi3dRenderEngine;
using CommonMapLib;
using System;
using System.Windows.Forms;

namespace LoadShapFile
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
            splitContainer2.Panel2.Controls.Add(axRenderControl);
            axRenderControl.EndInit();

            controlOperation = new MapOperation();
            controlOperation.InitializationMapControl(axRenderControl, "SHBM.3dm", true);

            controlOperation.RebindingListView(listView1);
        }


        /// <summary>
        /// 选择shp文件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button1_Click(object sender, EventArgs e)
        {
            var ofd = new OpenFileDialog
            {
                Filter = "Shape Files(*.shp)|*.shp",
                Title = "选择Shp文件"
            };
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                textBox1.Text = ofd.FileName;
            }
        }


        // 拉高shap文件
        string filename = "Floor";  //Floor/JZXG
        private void Button2_Click(object sender, EventArgs e)
        {
            
            if (selectNode != null)
                controlOperation.PullupBlock(selectNode.name, filename);
            else
                MessageBox.Show("请选择一个文件再拉高");
        }

        /// <summary>
        /// 加载地形
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button3_Click(object sender, EventArgs e)
        {
            var ofd = new OpenFileDialog
            {
                Filter = "TDF Files(*.tdf)|*.tdf",
                Title = "选择TDF文件"
            };
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                textBox2.Text = ofd.FileName;
                controlOperation.CreateTerrainLayers(textBox2.Text.Trim());
            }
        }

        /// <summary>
        /// 加载shp文件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button4_Click(object sender, EventArgs e)
        {
            string path = textBox1.Text.Trim();
            if (!string.IsNullOrEmpty(path))
            {
                var res = controlOperation.UpLoadShapFile(path);
                if (res.Item1 < 1)
                    MessageBox.Show(res.Item2);
                controlOperation.RebindingListView(listView1);
            }
                
            else
                MessageBox.Show("请选择Shp文件路径！");
        }

        ListNode selectNode;
        private void listView1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (listView1.SelectedItems.Count == 0) return;
            ListNode item = (ListNode)listView1.SelectedItems[0];
            item.Checked = true;
            selectNode = item;

            controlOperation.LookAtEnvelope(item);
        }

        private void listView1_ItemChecked(object sender, ItemCheckedEventArgs e)
        {

        }
    }
}
