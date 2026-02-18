using NetworkManager.Domain.Aggregates.Users;

namespace NetworkManager.Domain;

public interface IXrayConfigService
{
    int ApiPort { get; }
    int VlessPort { get; }
    Task GenerateAndWriteConfigAsync(IEnumerable<User> users);
    string GetConfigPath();
}
