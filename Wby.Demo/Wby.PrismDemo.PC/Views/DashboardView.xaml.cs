using System.Collections.ObjectModel;
using System.Windows.Controls;

namespace Wby.PrismDemo.PC.Views
{
    /// <summary>
    /// DashboardView.xaml 的交互逻辑
    /// </summary>
    public partial class DashboardView : UserControl
    {
        public DashboardView()
        {
            InitializeComponent();
            InitUserModuleBar();
            DataContext = this;
        }

        public ObservableCollection<UserModule> UserModules { get; set; }

        public void InitUserModuleBar()
        {
            UserModules = new ObservableCollection<UserModule>
            {
                new UserModule() { FilePath = "/Infrastructure/Images/Image1.jpg", UserName = "James Bloor", Content = "What's up", SignTime = "32 min" },
                new UserModule() { FilePath = "/Infrastructure/Images/Image2.jpg", UserName = "Fionn Withehead", Content = "Nice one", SignTime = "2 days" },
                new UserModule() { FilePath = "/Infrastructure/Images/Image3.jpg", UserName = "Damien Bonnard", Content = "Go on in comi", SignTime = "1 weeks" },
                new UserModule() { FilePath = "/Infrastructure/Images/Image4.jpg", UserName = "Aneurin Barnard", Content = "I am coming", SignTime = "2 weeks" },
                new UserModule() { FilePath = "/Infrastructure/Images/Image5.jpg", UserName = "James Bloor", Content = "What's up", SignTime = "32 min" },
                new UserModule() { FilePath = "/Infrastructure/Images/Image6.jpg", UserName = "Fionn Withehead", Content = "Nice one", SignTime = "2 days" },
                new UserModule() { FilePath = "/Infrastructure/Images/Image7.jpg", UserName = "Damien Bonnard", Content = "Go on in comi", SignTime = "1 weeks" },
                new UserModule() { FilePath = "/Infrastructure/Images/Image8.jpg", UserName = "Aneurin Barnard", Content = "I am coming", SignTime = "2 weeks" }
            };
        }
    }

    public class UserModule
    {
        public string FilePath { get; set; }
        public string UserName { get; set; }
        public string Content { get; set; }
        public string SignTime { get; set; }
    }
}
