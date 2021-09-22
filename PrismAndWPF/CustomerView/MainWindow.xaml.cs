using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace CustomerView
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new MainViewModel();
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBoxItem selectedItem = (ComboBoxItem)comBox.SelectedItem;
            lstPeople.View = (ViewBase)FindResource(selectedItem.Content);
        }
    }

    public class Person
    {
        public string Name { get; set; }
        public int Age { get; set; }
        public float UnitCost { get; set; }
        public string IconImage { get; set; }
    }

    public class MainViewModel
    {
        public MainViewModel()
        {
            PersonList = new List<Person>()
            {
                new Person{ Name="张三", Age=22, UnitCost=12.5f, IconImage="Images/Image1.jpg"},
                new Person{ Name="李四", Age=23, UnitCost=111.5f, IconImage="Images/Image2.jpg"},
                new Person{ Name="王五", Age=34, UnitCost=1121.5f, IconImage="Images/Image3.jpg"},
                new Person{ Name="赵六", Age=56, UnitCost=112.5f, IconImage="Images/Image4.jpg"},
                new Person{ Name="刘三", Age=22, UnitCost=1888.5f, IconImage="Images/Image5.jpg"}
            };
        }
        private List<Person> personList;
        public List<Person> PersonList
        {
            get => personList;
            set => personList = value;
        }
    }
}
