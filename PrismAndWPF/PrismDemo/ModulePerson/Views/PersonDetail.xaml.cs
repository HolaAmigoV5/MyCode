using ModulePerson.Business;
using ModulePerson.ViewModels;
using Prism.Common;
using Prism.Regions;
using System.Windows.Controls;

namespace ModulePerson.Views
{
    /// <summary>
    /// PersonDetail.xaml 的交互逻辑
    /// </summary>
    public partial class PersonDetail : UserControl
    {
        public PersonDetail()
        {
            InitializeComponent();

            //对RegionContext对象进行变更跟踪
            //RegionContext.GetObservableContext(this).PropertyChanged += PersonDetail_PropertyChanged;
        }

        private void PersonDetail_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            var context = (ObservableObject<object>)sender;
            var selectedPerson = (Person)context.Value;
            (DataContext as PersonDetailViewModel).SelectedPerson = selectedPerson;
        }
    }
}
