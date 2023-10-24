using Microsoft.Extensions.DependencyInjection;
using SteamStorage.Services.Config;
using SteamStorage.Services.Dialog;
using SteamStorage.Services.Logger;
using SteamStorage.Services.ReferenceInformation;
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

            ConfigService? configService = Singleton.GetObject<ConfigService>();
            window.Height = configService.Height;
            window.Width = configService.Width;
            window.Top = configService.Top;
            window.Left = configService.Left;

            window.Show();

            if (configService.IsMaximized) window.WindowState = WindowState.Maximized;

            base.OnStartup(e);
        }
        private static void InitContainer()
        {
            ServiceCollection services = new();

            services.AddSingleton<MainWindowVM>();
            services.AddSingleton<MainWindow>();

            services.AddSingleton<Context>();
            
            services.AddSingleton<ConfigService>();
            services.AddSingleton<WindowDialogService>();
            services.AddSingleton<LoggerService>(new LoggerService(ProgramConstants.LogPath));
            services.AddSingleton<ReferenceInformationService>();

            Container = services.BuildServiceProvider();
        }
    }
}
