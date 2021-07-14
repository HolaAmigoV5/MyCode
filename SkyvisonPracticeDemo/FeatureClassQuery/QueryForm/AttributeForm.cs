using System;
using System.Data;
using System.Windows.Forms;

namespace FeatureClassQuery.QueryForm
{
    public partial class AttributeForm : Form
    {
        DataTable AttriTable = null;
        string FCName = "";
        string FilterWhereClause = "";
        public AttributeForm(DataTable dt, string fcName, string filterWhereClause = "")
        {
            InitializeComponent();

            AttriTable = dt;
            FCName = fcName;
            FilterWhereClause = filterWhereClause;

            this.Load += AttributeForm_Load;
        }

        private void AttributeForm_Load(object sender, EventArgs e)
        {
            dataGridView1.DataSource = AttriTable;
            if (string.IsNullOrEmpty(FilterWhereClause))
                this.Text = "Attributes of " + FCName + "  [Total records: " + AttriTable.Rows.Count.ToString() + "]";
            else
                this.Text = "Attributes of " + FCName + "  [Total records: " + AttriTable.Rows.Count.ToString() + "]" + "  Filter: " + FilterWhereClause;
        }
    }
}
