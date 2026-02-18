using System.Text.Json;

namespace NetworkMangar.Infrastructure.Services.Settings;

public class JsonSettingsService : ISettingsService
{
    private readonly string _filePath;
    public AppSettings Setting { get; private set; }
    public JsonSettingsService(string baseDirectory)
    {
        _filePath = Path.Combine(baseDirectory, "settings.json");
        Setting = LoadSettingsAsync();
    }

    private AppSettings LoadSettingsAsync()
    {
        if (!File.Exists(_filePath))
        {
            var setting = new AppSettings();
            SaveSettingsAsync(setting).Wait();
            return setting;
        }

        try
        {
            var json = File.ReadAllText(_filePath);
            return JsonSerializer.Deserialize<AppSettings>(json) ?? new AppSettings();
        }
        catch
        {
            return new AppSettings();
        }
    }

    public async Task SaveSettingsAsync(AppSettings setting)
    {
        var options = new JsonSerializerOptions { WriteIndented = true };
        var json = JsonSerializer.Serialize(setting, options);
        await File.WriteAllTextAsync(_filePath, json);
        Setting = setting;
    }
}
