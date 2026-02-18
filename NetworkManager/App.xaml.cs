using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NetworkManager.Applications;
using NetworkManager.Services;
using NetworkManager.ViewModels.Pages;
using NetworkManager.ViewModels.Windows;
using NetworkManager.Views.Pages;
using NetworkManager.Views.Windows;
using NetworkMangar.Infrastructure;
using NetworkMangar.Infrastructure.Services.Settings;
using System.IO;
using System.Windows.Threading;
using Wpf.Ui;
using Wpf.Ui.Appearance;
using Wpf.Ui.DependencyInjection;

namespace NetworkManager;

public partial class App
{
    private static readonly IHost _host = Host
        .CreateDefaultBuilder()
        .ConfigureAppConfiguration(c => { c.SetBasePath(Path.GetDirectoryName(AppContext.BaseDirectory) ?? string.Empty); })
        .ConfigureServices((context, services) =>
        {
            services.AddNavigationViewPageProvider();

            services.AddHostedService<ApplicationHostService>();

            services.AddSingleton<IThemeService, ThemeService>();

            services.AddSingleton<ITaskBarService, TaskBarService>();
            services.AddSingleton<IContentDialogService, ContentDialogService>();

            services.AddSingleton<INavigationService, NavigationService>();

            services.AddSingleton<INavigationWindow, MainWindow>();
            services.AddSingleton<MainWindowViewModel>();

            services.AddSingleton<DashboardPage>();
            services.AddSingleton<DashboardViewModel>();

            services.AddSingleton<UserManagerPage>();
            services.AddSingleton<UserManagerViewModel>();

            services.AddSingleton<SettingsPage>();
            services.AddSingleton<SettingsViewModel>();

            services.AddApplications(AppDomain.CurrentDomain.BaseDirectory);

        }).Build();

    public static IServiceProvider Services { get { return _host.Services; } }

    private async void OnStartup(object sender, StartupEventArgs e)
    {
        await Task.Run(() =>
        {
            using (var scope = Services.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                dbContext.Database.EnsureCreated();
            }
        });

        var settingsService = Services.GetService<ISettingsService>();
        if (settingsService!.Setting.IsDarkMode)
            ApplicationThemeManager.Apply(ApplicationTheme.Dark);
        else
            ApplicationThemeManager.Apply(ApplicationTheme.Light);

        await _host.StartAsync();
    }

    private async void OnExit(object sender, ExitEventArgs e)
    {
        await _host.StopAsync();
        _host.Dispose();
    }

    private void OnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
    {
    }
}
