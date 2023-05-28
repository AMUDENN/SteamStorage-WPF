using Microsoft.Extensions.DependencyInjection;
using SteamStorage.ViewModels;
using System;
using System.Windows;

namespace SteamStorage
{
    public partial class App : Application
    {
        public static IServiceProvider Container { get; protected set; }
        protected override void OnStartup(StartupEventArgs e)
        {
            InitContainer();

            MainWindowVM MainVM = Container.GetService<MainWindowVM>() as MainWindowVM;

            var window = Container.GetService(typeof(MainWindow)) as MainWindow;
            if (window is null)
                throw new Exception("something went wrong during initializing DI container. MainWindow is missing");
            window.DataContext = MainVM;
            window.Show();
            base.OnStartup(e);
        }

        private static void InitContainer()
        {
            ServiceCollection services = new();

            services.AddSingleton<MainWindowVM>();
            services.AddSingleton<MainWindow>();

            Container = services.BuildServiceProvider();
        }
    }
}
