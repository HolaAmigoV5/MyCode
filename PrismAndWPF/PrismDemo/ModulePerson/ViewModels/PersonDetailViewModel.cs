using ModulePerson.Business;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;

namespace ModulePerson.ViewModels
{
    public class PersonDetailViewModel : BindableBase, INavigationAware
    {
        private Person _selectedPerson;
        IRegionNavigationJournal _journal;

        public Person SelectedPerson
        {
            get { return _selectedPerson; }
            set { SetProperty(ref _selectedPerson, value); }
        }

        public DelegateCommand GoBackCommand { get; set; }

        public PersonDetailViewModel()
        {
            GoBackCommand = new DelegateCommand(GoBack);
        }

        private void GoBack()
        {
            _journal.GoBack();
        }

        #region INavigationAware
        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            _journal = navigationContext.NavigationService.Journal;

            if (navigationContext.Parameters["person"] is Person person)
                SelectedPerson = person;
        }

        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            if (navigationContext.Parameters["person"] is Person person)
                return (SelectedPerson != null && SelectedPerson.LastName == person.LastName);
            else
                return true;
        }

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {

        } 
        #endregion
    }
}
