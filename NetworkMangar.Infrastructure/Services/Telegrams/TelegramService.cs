using NetworkMangar.Infrastructure.Services.Settings;
using System.Net.Http.Json;

namespace NetworkMangar.Infrastructure.Services.Telegrams;

public class TelegramService : INotificationService
{
    private readonly HttpClient _httpClient;
    private readonly ISettingsService _settingsService;
    public TelegramService(ISettingsService settingsService)
    {
        _httpClient = new HttpClient();
        _settingsService = settingsService;
    }

    public async Task SendToUserAsync(string chatId, string message)
    {
        if (string.IsNullOrEmpty(chatId)) return;

        string url = $"https://api.telegram.org/bot{_settingsService.Setting.TelegramBot}/sendMessage";

        var payload = new
        {
            chat_id = chatId,
            text = message,          
        };

        try
        {
            var response = await _httpClient.PostAsJsonAsync(url, payload);
            response.EnsureSuccessStatusCode();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Telegram Error: {ex.Message}");
        }
    }
}
