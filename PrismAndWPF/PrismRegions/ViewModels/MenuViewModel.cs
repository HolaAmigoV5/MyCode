using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace BootstrapperShell.ViewModels
{
    public class MenuViewModel : BindableBase
    {
        private readonly IRegionManager _regionManager;

        private ObservableCollection<object> _views = new ObservableCollection<object>();
        public ObservableCollection<object> Views
        {
            get { return _views; }
            set { SetProperty(ref _views, value); }
        }

        public DelegateCommand<string> NavigateCommand { get; private set; }
        public MenuViewModel(IRegionManager regionManager)
        {
            _regionManager = regionManager;
            _regionManager.Regions.CollectionChanged += Regions_CollectionChanged;
            NavigateCommand = new DelegateCommand<string>(Navigate);
        }

        private void Regions_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
           if(e.Action== NotifyCollectionChangedAction.Add)
            {
                var region = (IRegion)e.NewItems[0];
                region.Views.CollectionChanged += Views_CollectionChanged;
            }
        }

        private void Views_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
                Views.Add(e.NewItems[0].GetType().Name);
            else if (e.Action == NotifyCollectionChangedAction.Remove)
                Views.Remove(e.OldItems[0].GetType().Name);
        }

        private void Navigate(string navigatePath)
        {
            if (navigatePath != null)
                _regionManager.RequestNavigate("MContentRegion", navigatePath, NavigationComplete);
        }

        private void NavigationComplete(NavigationResult result)
        {
            //System.Windows.MessageBox.Show(string.Format("Navigation to {0} complete.", result.Context.Uri));
        }
    }
}
