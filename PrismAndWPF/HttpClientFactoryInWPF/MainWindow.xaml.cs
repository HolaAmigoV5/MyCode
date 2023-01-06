using System.Net.Http;
using System.Windows;

namespace HttpClientFactoryInWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly IHttpClientFactory _httpClientFactory;
        public MainWindow(IHttpClientFactory httpClientFactory)
        {
            InitializeComponent();

            _httpClientFactory = httpClientFactory;
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            var client = _httpClientFactory.CreateClient();
            var html = await client.GetStringAsync("https://www.baidu.com");
            MessageBox.Show(html);
        }
    }
}
