namespace NetworkMangar.Infrastructure.Services.Settings;

public class AppSettings
{
    public int XrayPort { get; set; } = 10080;
    public string Language { get; set; } = "en";
    public bool IsDarkMode { get; set; } = true;
    public string TelegramBot { get; set; } = "";
}
