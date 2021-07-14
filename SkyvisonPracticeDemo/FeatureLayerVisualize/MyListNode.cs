using i3dRenderEngine;
using System.Windows.Forms;

namespace FeatureLayerVisualize
{
    public class MyListNode: ListViewItem
    {
        public string name;
        public IFeatureLayer layer;
        public MyListNode(string n, IFeatureLayer fl)
        {
            name = n;
            layer = fl;
            this.Text = n;
        }
    }
}
