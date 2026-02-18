namespace NetworkMangar.Infrastructure.Services.CloudflerTunnel;

public interface IQuickTunnelService
{
    string Host { get; }
    event Action<string> OnTunnelUrlChanged;
    Task StartAsync();
    void Stop();
}
