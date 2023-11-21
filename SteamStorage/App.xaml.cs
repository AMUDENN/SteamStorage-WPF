using Microsoft.Extensions.DependencyInjection;
using SteamStorage.Services.Config;
using SteamStorage.Services.Dialog;
using SteamStorage.Services.Logger;
using SteamStorage.Services.Parser;
using SteamStorage.Services.ReferenceInformation;
using SteamStorage.Services.ToolTip;
using SteamStorage.Utilities;
using SteamStorage.ViewModels;
using SteamStorage.Windows;
using System;
using System.Windows;
using System.Windows.Threading;

namespace SteamStorage
{
    public partial class App : Application
    {
        public static IServiceProvider Container { get; protected set; }
        protected override void OnStartup(StartupEventArgs e)
        {
            InitContainer();

            MainWindowVM? MainWindowVM = Singleton.GetService<MainWindowVM>();

            var window = Singleton.GetService<MainWindow>();
            if (window is null)
                throw new Exception("something went wrong during initializing DI container. MainWindow is missing");
            window.DataContext = MainWindowVM;

            ConfigService? configService = Singleton.GetService<ConfigService>();
            window.Height = configService.Height;
            window.Width = configService.Width;
            window.Top = configService.Top;
            window.Left = configService.Left;

            window.Show();

            if (configService.IsMaximized) window.WindowState = WindowState.Maximized;

            ToolTipServiceHelper.SetToolTipInitialShowDelay(300);

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
            services.AddSingleton<SteamParseService>();
            services.AddSingleton<ReferenceInformationService>();

            Container = services.BuildServiceProvider();
        }
        private void OnUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            Singleton.GetService<LoggerService>().WriteMessage(e.Exception, "Необработанное программное исключение");
            e.Handled = true;
        }
    }
}
