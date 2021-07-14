using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FeatureClassQuery.QueryForm
{
    public partial class QueryFilterDlg : Form
    {
        public QueryFilterDlg()
        {
            InitializeComponent();
        }

        private void FieldList_listBox_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (FieldList_listBox.SelectedItem != null)
                QueryFilter_txt.SelectedText = FieldList_listBox.SelectedItem.ToString();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Button button = sender as Button;
            QueryFilter_txt.SelectedText = " " + button.Text + " ";
        }
    }
}
