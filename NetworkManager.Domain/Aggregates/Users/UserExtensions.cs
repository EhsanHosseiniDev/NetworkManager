namespace NetworkManager.Domain.Aggregates.Users;

public static class UserExtensions
{
    public static long TotalUsage(this User user)
    {
        return user.UploadUsage + user.DownloadUsage;
    }
    public static string GenerateVlessLink(this User user , string host)
    {
        return $"vless://{user.Uuid}@{host}:443?security=tls&encryption=none&type=ws&host={host}&path=%2F#{user.TelegramUsername}_Home";
    }
}
