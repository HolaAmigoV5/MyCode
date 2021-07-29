using I3DMapOperation;
using Prism.Ioc;
using System.Windows.Controls;

namespace DaJuTestDemo.Views
{
    /// <summary>
    /// Interaction logic for i3dMapView
    /// </summary>
    public partial class I3dMapView : UserControl
    {
        public I3dMapView()
        {
            InitializeComponent();

            IMapOperation mapOperation = ContainerLocator.Current.Resolve<IMapOperation>();
            mapOperation.InitializationAxRenderControl(MapControlHost);
        }
    }
}
