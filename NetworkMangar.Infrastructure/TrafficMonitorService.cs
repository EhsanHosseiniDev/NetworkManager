using NetworkManager.Domain;
using NetworkManager.Domain.Aggregates.Users;
using NetworkMangar.Infrastructure.Users;

namespace NetworkMangar.Infrastructure;

public class TrafficMonitorService
{
    private readonly IUserRepository _userRepository;
    private readonly IXrayApiService _xrayApi;
    private readonly System.Timers.Timer _timer;

    public TrafficMonitorService(IUserRepository userRepository, IXrayApiService xrayApi)
    {
        _userRepository = userRepository;
        _xrayApi = xrayApi;

        _timer = new System.Timers.Timer(10000);
        _timer.Elapsed += async (sender, e) => await CheckTraffic();
        _timer.AutoReset = true;
    }

    public void Start() => _timer.Start();
    public void Stop() => _timer.Stop();

    private async Task CheckTraffic()
    {
        try
        {
            var stats = await _xrayApi.GetTrafficStatsAsync();

            foreach (var stat in stats)
            {
                if (stat.Upload > 0 || stat.Download > 0)
                {
                    if (_userRepository is UserRepository repo)
                    {
                        await repo.UpdateTrafficUsageAsync(stat.Uuid, stat.Upload, stat.Download);
                    }
                }
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Traffic Update Failed: {ex.Message}");
        }
    }
}
