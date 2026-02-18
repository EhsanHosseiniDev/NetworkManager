using Microsoft.Extensions.DependencyInjection;
using NetworkMangar.Infrastructure;

namespace NetworkManager.Applications;

public static class IocApplicationExtentions
{
    public static void AddApplications(this IServiceCollection services, string baseDirectory)
    {
        services.AddTransient<IVpnHandler, VpnHandler>();
        services.AddInfrastructure(baseDirectory);
    }
}
