using CommonLibrary;
using System.Windows.Forms;

namespace FeatureSelect
{
    public partial class Form1 : Form
    {
        AxRenderControlOperation controlOperation = null;
        public Form1()
        {
            InitializeComponent();

            controlOperation = new AxRenderControlOperation(axRenderControl1);
            controlOperation.InitializationAxRenderControl();
            controlOperation.RegisterRcSelectEvent();

            // 设置控件默认值
            this.toolStripSelectModeSetting.SelectedIndex = 0;
        }

        private void toolStripUnHightLight_Click(object sender, System.EventArgs e)
        { 
            controlOperation.UnhighlightAll();
        }

        private void cbCtrlEnable_CheckedChanged(object sender, System.EventArgs e)
        {
            controlOperation.CTRL = this.cbCtrlEnable.Checked;
        }

        private void toolStripSelectModeSetting_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            ToolStripComboBox cb = sender as ToolStripComboBox;
            controlOperation.SetSelectMode(cb.Text);
        }
    }
}
