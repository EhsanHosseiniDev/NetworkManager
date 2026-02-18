
namespace NetworkManager.Applications
{
    public interface IVpnHandler
    {
        Task StartAsync();
        Task StopAsync();
    }
}