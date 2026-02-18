using NetworkManager.Domain.Aggregates.Users;
using NetworkManager.Domain.Exeptions;
using NetworkMangar.Infrastructure.Services.CloudflerTunnel;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace NetworkMangar.Infrastructure.Services.Telegrams;

public class TelegramBotListenerService(IUserRepository userRepository, IQuickTunnelService quickTunnelService) : ITelegramBotListenerService
{
    private TelegramBotClient? _botClient;
    private readonly IUserRepository _userRepository = userRepository;
    private readonly IQuickTunnelService _quickTunnelService = quickTunnelService;
    private CancellationTokenSource _cts;
    private List<NetworkManager.Domain.Aggregates.Users.User> _cashedUsers = new();

    public void StartReceiving(string botToken)
    {
        if (string.IsNullOrEmpty(botToken))
            throw new TelegramBotApiNotValidExeption();
        _botClient = _botClient ?? new TelegramBotClient(botToken);

        _cts = new CancellationTokenSource();
        var receiverOptions = new ReceiverOptions
        {
            AllowedUpdates = Array.Empty<UpdateType>()
        };
        _cashedUsers = _userRepository.GetAllAsync().Result.ToList();

        _botClient.StartReceiving(
            updateHandler: HandleUpdateAsync,
            errorHandler: HandlePollingErrorAsync,
            receiverOptions: receiverOptions,
            cancellationToken: _cts.Token
        );
    }

    public void StopReceiving()
    {
        _cts?.Cancel();
    }

    private async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
    {
        if (update.Message is not { } message) return;
        if (message.Text is not { } messageText) return;
        var chatId = message.Chat.Id;
        var username = message.Chat.Username ?? $"{message.Chat.FirstName} {message.Chat.LastName}";

        var user = _cashedUsers.Where(x => x.TelegramChatId == chatId.ToString()).SingleOrDefault();

        if (messageText.StartsWith("/start", StringComparison.OrdinalIgnoreCase))
        {
            if (user == null)
            {
                user = new NetworkManager.Domain.Aggregates.Users.User
                {
                    TelegramChatId = chatId.ToString(),
                    TelegramUsername = username,
                    JoinedAt = DateTime.UtcNow,
                    CreatedAt = DateTime.UtcNow,
                    IsActive = true,
                    Uuid = Guid.NewGuid().ToString(),
                    MonthlyLimit = 1073741824 // 1GB
                };
                await _userRepository.AddAsync(user);
                _cashedUsers.Add(user);
            }
            await botClient.SendMessage(
                chatId: chatId,
                text: "✅ You are registered! Wait for VPN config.",
                cancellationToken: cancellationToken);

            await botClient.SendMessage(
                chatId: chatId,
                text: $"{user.GenerateVlessLink(_quickTunnelService.Host)}",
                cancellationToken: cancellationToken);
        }
        else
        {
            if (user == null) return;

            await botClient.SendMessage(
                chatId: chatId,
                text: $"{user.GenerateVlessLink(_quickTunnelService.Host)}",
                cancellationToken: cancellationToken);
        }
    }

    private Task HandlePollingErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
    {
        System.Diagnostics.Debug.WriteLine($"Telegram API Error: {exception.Message}");
        return Task.CompletedTask;
    }
}
