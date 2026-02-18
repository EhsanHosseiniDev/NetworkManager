namespace NetworkMangar.Infrastructure.Services.Settings;

public interface ISettingsService
{
    AppSettings Setting { get; } 
    Task SaveSettingsAsync(AppSettings setting);
}
