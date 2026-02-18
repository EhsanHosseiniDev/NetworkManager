using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NetworkManager.Domain;
using NetworkManager.Domain.Aggregates.Users;
using NetworkMangar.Infrastructure.Services.CloudflerTunnel;
using NetworkMangar.Infrastructure.Services.Settings;
using NetworkMangar.Infrastructure.Services.Telegrams;
using NetworkMangar.Infrastructure.Services.Xrays;
using NetworkMangar.Infrastructure.Users;

namespace NetworkMangar.Infrastructure;

public static class IocInfraExtentions
{

    public static void AddInfrastructure(this IServiceCollection services, string baseDirectory)
    {
        string dbPath = Path.Combine(baseDirectory, "users.db");
        string resourcesPath = Path.Combine(baseDirectory, "Resources");
        string xrayPath = Path.Combine(baseDirectory, resourcesPath, "xray.exe");
        string cloudflaredPath = Path.Combine(resourcesPath, "cloudflared.exe");
        var settings = new AppSettings();
        if (!File.Exists(xrayPath))
        {
            Directory.CreateDirectory(Path.GetDirectoryName(xrayPath)!);
        }
        services.AddSingleton<ISettingsService>(provider=>new JsonSettingsService(baseDirectory));

        services.AddDbContextFactory<AppDbContext>(options =>
        {
            options.UseSqlite($"Data Source={dbPath}");
        });
        services.AddSingleton<IUserRepository, UserRepository>();

        services.AddSingleton<IXrayConfigService>(provider =>
        {
            var settingService = provider.GetRequiredService<ISettingsService>();
           
            settings = settingService.Setting;
            return new XrayConfigService(xrayPath, 10085, settings.XrayPort);
        });

        services.AddSingleton<IXrayApiService>(provider => new XrayApiService(xrayPath));

        services.AddSingleton<ITelegramBotListenerService, TelegramBotListenerService>();
        services.AddSingleton<IQuickTunnelService>(provider => new QuickTunnelService(cloudflaredPath, settings.XrayPort));
        services.AddSingleton<INotificationService, TelegramService>();

        services.AddSingleton<TrafficMonitorService>();
    }
}
