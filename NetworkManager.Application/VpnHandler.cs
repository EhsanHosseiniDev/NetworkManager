using NetworkManager.Domain;
using NetworkManager.Domain.Aggregates.Users;
using NetworkManager.Domain.Exeptions;
using NetworkMangar.Infrastructure;
using NetworkMangar.Infrastructure.Services.CloudflerTunnel;
using NetworkMangar.Infrastructure.Services.Settings;
using NetworkMangar.Infrastructure.Services.Telegrams;

namespace NetworkManager.Applications;

public class VpnHandler : IVpnHandler
{
    private readonly IUserRepository _userRepository;
    private readonly IXrayConfigService _configService;
    private readonly IXrayApiService _xrayProcess;
    private readonly IQuickTunnelService _tunnelService;
    private readonly INotificationService _notificationService;
    private readonly ISettingsService _settingsService;
    private readonly ITelegramBotListenerService _telegramBotListenerService;

    public VpnHandler(
        IUserRepository userRepository,
        IXrayConfigService configService,
        IXrayApiService xrayProcess,
        IQuickTunnelService tunnelService,
        INotificationService notificationService,
        ISettingsService settingsService,
        ITelegramBotListenerService telegramBotListenerService)
    {
        _userRepository = userRepository;
        _configService = configService;
        _xrayProcess = xrayProcess;
        _tunnelService = tunnelService;
        _notificationService = notificationService;
        _settingsService = settingsService;
        _telegramBotListenerService = telegramBotListenerService;
        _tunnelService.OnTunnelUrlChanged += async (url) => await SendConfigsToAllUsers(url);
    }

    public async Task StartAsync()
    {
        if (string.IsNullOrEmpty(_settingsService.Setting.TelegramBot))
            new TelegramBotApiNotValidExeption();

        var users = await _userRepository.GetAllAsync();
        var activeUsers = users.Where(u => u.IsActive).ToList();

        await _configService.GenerateAndWriteConfigAsync(activeUsers);

        _telegramBotListenerService.StartReceiving(_settingsService.Setting.TelegramBot);
        _xrayProcess.Stop();
        _xrayProcess.Start();
        await _tunnelService.StartAsync();
    }

    public async Task StopAsync()
    {
        _telegramBotListenerService.StopReceiving();
        _xrayProcess.Stop();
        _tunnelService.Stop();
    }

    private async Task SendConfigsToAllUsers(string newHostUrl)
    {
        var vpnUsers = await _userRepository.GetAllAsync();
        var activeVpnUsers = vpnUsers.Where(u => u.IsActive && !string.IsNullOrEmpty(u.TelegramChatId)).ToList();

        if (activeVpnUsers.Any())
        {
            foreach (var user in activeVpnUsers)
            {
                string vlessConfig = user.GenerateVlessLink(newHostUrl);
                await _notificationService.SendToUserAsync(user.TelegramChatId, vlessConfig);
            }
        }
    }
}
