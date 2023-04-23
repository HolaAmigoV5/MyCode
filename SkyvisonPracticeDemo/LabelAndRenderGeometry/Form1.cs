using Axi3dRenderEngine;
using CommonLibrary;
using System;
using System.Windows.Forms;

namespace LabelAndRenderGeometry
{
    public partial class Form1 : Form
    {
        AxRenderControlOperation controlOperation = null;
        private CheckBox check = null;
        private CheckBox checkShowOutline = null;
        AxRenderControl axRenderControl;

        public Form1()
        {
            InitializeComponent();

            axRenderControl = new AxRenderControl();
            axRenderControl.BeginInit();
            axRenderControl.Dock = DockStyle.Fill;
            tableLayoutPanel1.Controls.Add(axRenderControl);
            axRenderControl.EndInit();

            controlOperation = new AxRenderControlOperation(axRenderControl);
            controlOperation.InitializationAxRenderControl("JD.3DM");
            controlOperation.SetI3dObjectType();
            controlOperation.SetSelectMode("仅点选");
            controlOperation.RegisterRcSelectEvent();
            controlOperation.CallbackMsg += ControlOperation_CallbackMsg;

            toolStripComboBoxObjectManager.SelectedIndex = 0;
            toolStripComboBoxColor.SelectedIndex = 0;
        }

        private void ControlOperation_CallbackMsg(string msg)
        {
            if (!string.IsNullOrEmpty(msg))
                MessageBox.Show(msg);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            check = new CheckBox();
            check.Text = "进入漫游模式";
            check.Width = 80;
            check.Checked = false;
            check.CheckedChanged += Check_CheckedChanged;
            ToolStripControlHost host = new ToolStripControlHost(check);
            toolStrip1.Items.Add(host);

            checkShowOutline = new CheckBox();
            checkShowOutline.Text = "显示外轮廓线";
            checkShowOutline.Width = 80;
            checkShowOutline.Checked = false;
            checkShowOutline.CheckedChanged += CheckShowOutline_CheckedChanged;
            ToolStripControlHost hostShowOutline = new ToolStripControlHost(checkShowOutline);
            toolStrip1.Items.Add(hostShowOutline);
        }

        private void CheckShowOutline_CheckedChanged(object sender, EventArgs e)
        {
            if (checkShowOutline.Checked)
            {
                switch (toolStripComboBoxColor.SelectedIndex)
                {
                    case 0:
                        controlOperation.SetRenderParamColor(0xffff0000);
                        break;
                    case 1:
                        controlOperation.SetRenderParamColor(0xffffff00);
                        break;
                    default:
                        controlOperation.SetRenderParamColor(0xff0000ff);  // 蓝色
                        break;
                }
            }
        }

        private void Check_CheckedChanged(object sender, EventArgs e)
        {
            if (check.Checked)
                controlOperation.SetSelectMode("漫游模式");
            else
                controlOperation.SetSelectMode("仅点选");
        }

        private void toolStripComboBoxObjectManager_SelectedIndexChanged(object sender, EventArgs e)
        {
            SetCreateType(toolStripComboBoxObjectManager.Text.Trim());
        }

        private void SetCreateType(string typeStr)
        {
            if (string.IsNullOrEmpty(typeStr))
                return;
            var type = (CreateObjType)Enum.Parse(typeof(CreateObjType), typeStr);
            controlOperation.ObjType = type;
        }
    }
}
