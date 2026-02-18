namespace NetworkMangar.Infrastructure;

public interface INotificationService
{
    Task SendToUserAsync(string chatId, string message);
}
