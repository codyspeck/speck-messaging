using Microsoft.Extensions.Hosting;

namespace Speck.Messaging;

internal class HostStoppingService(Wrapper<CancellationTokenSource> cancellationTokenSource) : IHostedService
{
    public Task StartAsync(CancellationToken cancellationToken)
    {
        cancellationToken.Register(() => cancellationTokenSource.Value.Cancel());
        return Task.CompletedTask;
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        await cancellationTokenSource.Value.CancelAsync();
    }
}
