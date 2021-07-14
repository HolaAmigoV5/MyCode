using i3dFdeCore;
using System.Windows.Forms;

namespace CommonLibrary
{
    public class MyTreeNode: TreeNode
    {
        private string _name;
        private IConnectionInfo _con;

        public MyTreeNode(string name, IConnectionInfo con)
        {
            _name = name;
            _con = con;
            Text = name;
        }
    }
}
