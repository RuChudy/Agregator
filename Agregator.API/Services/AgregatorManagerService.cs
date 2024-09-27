using Microsoft.Extensions.Options;

namespace Agregator.API.Services;

public class AgregatorManagerService(
    IOptions<BackgroundTaskOptions> options,
    ILogger<AgregatorManagerService> logger,
    IAgregatorService service) : BackgroundService
{
    private readonly BackgroundTaskOptions _options = options?.Value ?? throw new ArgumentNullException(nameof(options));

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var delayTime = TimeSpan.FromSeconds(_options.CheckUpdateTime);

        if (logger.IsEnabled(LogLevel.Debug))
        {
            logger.LogDebug("AgregatorManagerService is starting.");
            stoppingToken.Register(() => logger.LogDebug("AgregatorManagerService background task is stopping."));
        }

        while (!stoppingToken.IsCancellationRequested)
        {
            if (logger.IsEnabled(LogLevel.Debug))
            {
                logger.LogDebug("AgregatorManagerService background task is doing background work.");
            }

            await CheckConfirmedGracePeriodOrders();

            await Task.Delay(delayTime, stoppingToken);
        }

        if (logger.IsEnabled(LogLevel.Debug))
        {
            logger.LogDebug("AgregatorManagerService background task is stopping.");
        }
    }

    private async Task CheckConfirmedGracePeriodOrders()
    {
        if (logger.IsEnabled(LogLevel.Debug))
        {
            logger.LogDebug("Checking confirmed grace period orders");
        }

        var orderIds = await service.CreateSnapshotAndClear();

        foreach (var orderId in orderIds)
        {
            if (logger.IsEnabled(LogLevel.Debug))
            {
                logger.LogDebug($"AgregatorManagerService {orderId.ProductId} {orderId.Quantity}.");
            }

            // TODO: tady nekdy posleme
        }
    }
}
