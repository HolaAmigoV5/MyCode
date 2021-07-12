using ModulePerson.Business;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;
using System.Collections.ObjectModel;
using System.Linq;

namespace ModulePerson.ViewModels
{
    public class PersonListViewModel : BindableBase, INavigationAware
    {
        IRegionManager _regionManager;
        IRegionNavigationJournal _journal;

        private ObservableCollection<Person> people;
        public ObservableCollection<Person> People
        {
            get { return people; }
            set { SetProperty(ref people, value); }
        }

        private string _selectedItemText;
        public string SelectedItemText
        {
            get { return _selectedItemText; }
            set { SetProperty(ref _selectedItemText, value); }
        }

        public DelegateCommand<Person> PersonSelectedCommand { get; private set; }
        public DelegateCommand<object[]> SelectedCommand { get; private set; }

        public DelegateCommand GoForwardCommand { get; set; }

        public PersonListViewModel(IRegionManager regionManager)
        {
            _regionManager = regionManager;
            PersonSelectedCommand = new DelegateCommand<Person>(PersonSelected);

            CreatedPeople();

            GoForwardCommand = new DelegateCommand(GoForward, CanGoForward);

            SelectedCommand = new DelegateCommand<object[]>(selectedItem =>
            {
                if (selectedItem != null && selectedItem.Count() > 0)
                    SelectedItemText = selectedItem.FirstOrDefault().ToString();
            });
        }

        private void PersonSelected(Person person)
        {
            var parameters = new NavigationParameters
            {
                { "person", person }
            };

            //向指定目标Region传入导航参数
            //if (person != null)
            //    _regionManager.RequestNavigate("PersonDetailsRegion", "PersonDetail", parameters);

            if (person != null)
            {
                SelectedItemText = person.ToString();
                _regionManager.RequestNavigate("PersonListRegion", "PersonDetail", parameters);
            }
               
        }

        private void GoForward()
        {
            _journal.GoForward();
        }

        private bool CanGoForward()
        {
            return _journal != null && _journal.CanGoForward;
        }

        private void CreatedPeople()
        {
            var people = new ObservableCollection<Person>();
            for (int i=1; i<10; i++)
            {
                people.Add(new Person()
                {
                    FirstName = string.Format("First {0}", i),
                    LastName = string.Format("Last {0}", i),
                    Age = i
                });
            }
            People = people;
        }

        #region INavigationAware
        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            _journal = navigationContext.NavigationService.Journal;
            GoForwardCommand.RaiseCanExecuteChanged();
        }

        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return true;
        }

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {
            
        } 
        #endregion
    }
}
