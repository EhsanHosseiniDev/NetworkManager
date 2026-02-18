namespace NetworkMangar.Infrastructure.Services.Telegrams;

public interface ITelegramBotListenerService
{
    void StartReceiving(string botToken);
    void StopReceiving();
}