using Microsoft.Extensions.DependencyInjection;
using SteamStorage.Utilities;
using SteamStorage.ViewModels;
using SteamStorage.Windows;
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

            MainWindowVM? MainWindowVM = Singleton.GetObject<MainWindowVM>();

            var window = Singleton.GetObject<MainWindow>();
            if (window is null)
                throw new Exception("something went wrong during initializing DI container. MainWindow is missing");
            window.DataContext = MainWindowVM;

            window.Height = Config.Height;
            window.Width = Config.Width;
            window.Top = Config.Top;
            window.Left = Config.Left;

            window.Show();

            if (Config.IsMaximized) window.WindowState = WindowState.Maximized;

            base.OnStartup(e);
        }
        private static void InitContainer()
        {
            ServiceCollection services = new();

            services.AddSingleton<MainWindowVM>();
            services.AddSingleton<MainWindow>();

            services.AddSingleton<Context>();
            services.AddSingleton<Logger>(new Logger(Constants.Logpath));

            Container = services.BuildServiceProvider();
        }
    }
}
