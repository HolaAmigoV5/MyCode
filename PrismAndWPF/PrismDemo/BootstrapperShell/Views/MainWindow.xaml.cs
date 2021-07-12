using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;
using System.Windows;

namespace BootstrapperShell.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        IContainerExtension _container;
        IRegionManager _regionManager;
        IModuleManager _moduleManager;

        IRegion _region;
        ViewB viewB;

        public MainWindow(IContainerExtension container, IRegionManager regionManager, IModuleManager moduleManager)
        {
            InitializeComponent();

            //为指定区域指定页面
            regionManager.RegisterViewWithRegion("HeadRegion", typeof(Header));
            regionManager.RegisterViewWithRegion("MenuRegion", typeof(Menu));
            //regionManager.RegisterViewWithRegion("ContentRegion", typeof(Content));
            //regionManager.RegisterViewWithRegion("PanelRegion", typeof(ViewInPanel));

            regionManager.RegisterViewWithRegion("ABRegion", typeof(ViewB));

            _container = container;
            _regionManager = regionManager;
            _moduleManager = moduleManager;

            this.Loaded += MainWindow_Loaded;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            viewB = _container.Resolve<ViewB>();
            _region = _regionManager.Regions["ABRegion"];
            _region.Add(viewB);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //注入的方式，在指定的Region中注入View
            var view = _container.Resolve<ViewInPanel>();
            var region = _regionManager.Regions["PanelRegion"];
            region.Add(view);
        }

        private void btn_ActivateView(object sender, RoutedEventArgs e)
        {
            //_region = _regionManager.Regions["ABRegion"];

            //foreach (var item in _region.Views)
            //{
            //    _region.Activate(item);
            //}

            _region.Activate(viewB);
        }

        private void btn_DeactivateView(object sender, RoutedEventArgs e)
        {
            //_region = _regionManager.Regions["ABRegion"];
            //foreach (var item in _region.Views)
            //{
            //    _region.Deactivate(item);
            //}

            _region.Deactivate(viewB);
        }

        private void btn_LoadModuleC(object sender, RoutedEventArgs e)
        {
            //手动加载显示ViewC
            _moduleManager.LoadModule("ModuleCModule");
        }
    }
}
