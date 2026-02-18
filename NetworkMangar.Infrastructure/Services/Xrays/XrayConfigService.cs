using NetworkManager.Domain;
using NetworkManager.Domain.Aggregates.Users;
using System.Text.Json;

namespace NetworkMangar.Infrastructure.Services.Xrays;

public class XrayConfigService : IXrayConfigService
{
    private readonly string _xrayExecutablePath;
    public  int ApiPort { get; }
    public  int VlessPort { get; }
    private readonly string _configFilePath;

    public XrayConfigService(string xrayExecutablePath, int apiPort, int vlessPort)
    {
        _xrayExecutablePath = xrayExecutablePath;
        ApiPort = apiPort;
        VlessPort = vlessPort;

        string directory = Path.GetDirectoryName(_xrayExecutablePath)!;
        _configFilePath = Path.Combine(directory, "config.json");
    }

    public async Task GenerateAndWriteConfigAsync(IEnumerable<User> users)
    {
        var vlessClients = users.Where(u => u.IsActive).Select(u => new
        {
            id = u.Uuid,
            email = u.Username, 
            level = 0
        }).ToArray();

        
        var configStructure = new
        {
            log = new
            {
                loglevel = "warning"
            },
            api = new
            {
                tag = "api",
                services = new[] { "StatsService" } 
            },
            stats = new { }, 
            inbounds = new object[]
            {
                    new
                    {
                        listen = "127.0.0.1",
                        port = ApiPort, 
                        protocol = "dokodemo-door",
                        settings = new { address = "127.0.0.1" },
                        tag = "api"
                    },
                    new
                    {
                        listen = "127.0.0.1",
                        port = VlessPort, 
                        protocol = "vless",
                        settings = new
                        {
                            clients = vlessClients,
                            decryption = "none"
                        },
                        streamSettings = new
                        {
                            network = "ws", 
                            wsSettings = new
                            {
                                path = "/" 
                            }
                        },
                        tag = "proxy"
                    }
            },
            outbounds = new object[]
            {
                    new { protocol = "freedom", tag = "direct" },
                    new { protocol = "blackhole", tag = "block" }
            },
            routing = new
            {
                rules = new object[]
                {
                        new
                        {
                            inboundTag = new[] { "api" },
                            outboundTag = "api",
                            type = "field"
                        }
                }
            }
        };

        var jsonOptions = new JsonSerializerOptions { WriteIndented = true };
        string jsonContent = JsonSerializer.Serialize(configStructure, jsonOptions);

        await File.WriteAllTextAsync(_configFilePath, jsonContent);
    }

    public string GetConfigPath() => _configFilePath;
}