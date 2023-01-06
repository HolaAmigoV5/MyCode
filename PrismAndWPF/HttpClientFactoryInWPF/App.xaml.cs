using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace HttpClientFactoryInWPF
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public ServiceProvider? ServiceProvider { get; private set; }

        protected override void OnStartup(StartupEventArgs e)
        {
            var serviceCollection = new ServiceCollection();
            ConfigureServices(serviceCollection);
            ServiceProvider = serviceCollection.BuildServiceProvider();
            var mainView = ServiceProvider.GetRequiredService<MainWindow>();
            mainView.Show();
            base.OnStartup(e);
        }

        private void ConfigureServices(ServiceCollection services)
        {
            services.AddHttpClient();
            services.AddTransient(typeof(MainWindow));
        }
    }
}
