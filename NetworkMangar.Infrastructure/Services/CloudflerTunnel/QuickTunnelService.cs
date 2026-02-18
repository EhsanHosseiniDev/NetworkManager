using System.Diagnostics;

namespace NetworkMangar.Infrastructure.Services.CloudflerTunnel;

public class QuickTunnelService : IQuickTunnelService
{
    private readonly string _executablePath;
    private readonly int port;
    private Process? _process;
    public string Host { get; private set; } = string.Empty;
    public event Action<string>? OnTunnelUrlChanged;

    public QuickTunnelService(string executablePath, int port)
    {
        _executablePath = executablePath;
        this.port = port;
    }

    public async Task StartAsync()
    {
        Stop();

        var startInfo = new ProcessStartInfo
        {
            FileName = _executablePath,
            Arguments = $"tunnel --no-autoupdate --url http://127.0.0.1:{port}",
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        _process = new Process { StartInfo = startInfo };

        _process.ErrorDataReceived += (s, e) => CheckForUrl(e.Data);
        _process.OutputDataReceived += (s, e) => CheckForUrl(e.Data);

        _process.Start();
        _process.BeginErrorReadLine();
        _process.BeginOutputReadLine();

        await Task.CompletedTask;
    }

    private void CheckForUrl(string? logLine)
    {
        if (string.IsNullOrEmpty(logLine)) return;

        var match = System.Text.RegularExpressions.Regex.Match(logLine, @"https://[a-zA-Z0-9-]+\.trycloudflare\.com");
        if (match.Success)
        {
            Host = match.Value.Replace("https://", "").Replace("/", "").Trim();
            OnTunnelUrlChanged?.Invoke(Host);
        }
    }

    public void Stop()
    {
        if (_process != null && _process?.HasExited == false)
        {
            _process.Kill();
            _process.Dispose();
            _process = null;
        }
    }
}
