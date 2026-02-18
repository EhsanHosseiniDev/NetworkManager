
namespace NetworkManager.Applications
{
    public interface IVpnHandler
    {
        Task StartAsync();
        Task RefreshAsync();
        Task StopAsync();
    }
}