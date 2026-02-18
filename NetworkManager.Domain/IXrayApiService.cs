namespace NetworkManager.Domain;

public interface IXrayApiService
{
    Task<IEnumerable<TrafficStatDto>> GetTrafficStatsAsync();
    Task AddUserAsync(string uuid, string email);
    Task RemoveUserAsync(string email);
    void Start();
    void Stop();
}
