using NetworkManager.Domain;
using System.Diagnostics;

namespace NetworkMangar.Infrastructure.Services.Xrays;

public class XrayApiService : IXrayApiService, IDisposable
{
    private readonly string _xrayPath;
    private readonly string _apiPort;
    private Process? _mainProcess;

    public XrayApiService(string xrayPath = "Resources/xray.exe", string apiPort = "10085")
    {
        var baseDir = AppDomain.CurrentDomain.BaseDirectory;
        _xrayPath = Path.Combine(baseDir, xrayPath);
        _apiPort = apiPort;
    }

    public void Start()
    {
        if (_mainProcess != null && !_mainProcess.HasExited) return;

        if (!File.Exists(_xrayPath))
        {
            throw new FileNotFoundException($"Xray executable not found at: {_xrayPath}");
        }

        var startInfo = new ProcessStartInfo
        {
            FileName = _xrayPath,
            WorkingDirectory = Path.GetDirectoryName(_xrayPath),
            UseShellExecute = false,
            CreateNoWindow = true,
            RedirectStandardOutput = false,
            RedirectStandardError = false
        };

        _mainProcess = Process.Start(startInfo);
    }

    public void Stop()
    {
        try
        {
            if (_mainProcess != null && !_mainProcess.HasExited)
            {
                _mainProcess.Kill();
                _mainProcess.WaitForExit(2000);
            }
        }
        catch (Exception)
        {
        }
        finally
        {
            _mainProcess?.Dispose();
            _mainProcess = null;
        }
    }

    public async Task<IEnumerable<TrafficStatDto>> GetTrafficStatsAsync()
    {
        string output = await RunXrayCommandAsync($"api statsquery --server=127.0.0.1:{_apiPort}");

        return ParseXrayStats(output);
    }

    private async Task<string> RunXrayCommandAsync(string arguments)
    {
        var startInfo = new ProcessStartInfo
        {
            FileName = _xrayPath,
            Arguments = arguments,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        using var process = new Process { StartInfo = startInfo };
        process.Start();

        var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));

        try
        {
            string result = await process.StandardOutput.ReadToEndAsync(cts.Token);
            await process.WaitForExitAsync(cts.Token);
            return result;
        }
        catch (OperationCanceledException)
        {
            process.Kill();
            return string.Empty;
        }
        catch (Exception)
        {
            return string.Empty;
        }
    }

    private IEnumerable<TrafficStatDto> ParseXrayStats(string rawOutput)
    {
        var stats = new List<TrafficStatDto>();
        if (string.IsNullOrWhiteSpace(rawOutput)) return stats;

        var userStats = new Dictionary<string, TrafficStatDto>();
        var lines = rawOutput.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

        foreach (var line in lines)
        {
            try
            {
                if (!line.Contains(">>>traffic>>>")) continue;

                var parts = line.Split(':');
                if (parts.Length < 2) continue;

                if (!long.TryParse(parts[1].Trim(), out long value)) continue;

                var keyParts = parts[0].Split(new[] { ">>>" }, StringSplitOptions.RemoveEmptyEntries);

                if (keyParts.Length < 4) continue;

                var email = keyParts[1];
                var type = keyParts[3];

                if (!userStats.ContainsKey(email))
                {
                    userStats[email] = new TrafficStatDto { Email = email };
                }

                if (type == "uplink") userStats[email].Upload = value;
                if (type == "downlink") userStats[email].Download = value;
            }
            catch
            {
                continue;
            }
        }

        return userStats.Values;
    }

    public Task AddUserAsync(string uuid, string email)
    {
        throw new NotImplementedException("Use ConfigService to rewrite config.json and restart core.");
    }

    public Task RemoveUserAsync(string email)
    {
        throw new NotImplementedException();
    }

    public void Dispose()
    {
        Stop();
    }
}