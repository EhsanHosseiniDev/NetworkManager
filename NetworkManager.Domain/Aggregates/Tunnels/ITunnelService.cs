namespace NetworkManager.Domain.Aggregates.Tunnels;

public interface ITunnelService
{
    Task<string> InitializeTunnelAsync(string apiToken);
    Task UpdateDnsRecordAsync(string domain, string tunnelId);
}
