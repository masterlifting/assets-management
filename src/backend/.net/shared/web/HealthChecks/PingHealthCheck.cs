using System.Net.NetworkInformation;

using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Shared.Web.HealthChecks;

public sealed class HealthCheck : IHealthCheck
{
    private readonly string _host;
    public HealthCheck(string host) => _host = host;

    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        Ping ping = new();
        var reply = await ping.SendPingAsync(_host).ConfigureAwait(false);

        return reply.Status != IPStatus.Success ? HealthCheckResult.Unhealthy() : HealthCheckResult.Healthy();
    }
}