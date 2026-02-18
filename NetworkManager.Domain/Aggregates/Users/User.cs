using System.ComponentModel.DataAnnotations.Schema;

namespace NetworkManager.Domain.Aggregates.Users;

public class User
{
    public int Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Uuid { get; set; } = Guid.NewGuid().ToString();
    public DateTime CreatedAt { get; set; } = DateTime.Now;

    public string TelegramUsername { get; set; } = string.Empty;
    public DateTime JoinedAt { get; set; } = DateTime.Now;
    public string TelegramChatId { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;


    // Traffic Stats
    public long UploadUsage { get; set; }
    public long DownloadUsage { get; set; }
    public long MonthlyLimit { get; set; }

    [NotMapped]
    public long TotalUsage => UploadUsage + DownloadUsage;
}
