namespace Agregator.API.Services;

/// <summary>
/// Manager pro agregační službu na pozadí.
/// </summary>
public class AgregatorManagerService(
    IOptions<BackgroundTaskOptions> options,
    ILogger<AgregatorManagerService> logger,
    IServiceScopeFactory scopeFactory,
    IAgregatorService service) : BackgroundService
{
    private readonly BackgroundTaskOptions _options = options?.Value ?? throw new ArgumentNullException(nameof(options));

    /// <summary>
    /// Spustí agregační službu na pozadí.
    /// </summary>
    /// <param name="stoppingToken">Zastavení.</param>
    /// <returns>Úloha.</returns>
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
                logger.LogDebug("AgregatorManagerService background task is doing background work.");

            await CheckConfirmedGracePeriodOrdersAsync();

            await Task.Delay(delayTime, stoppingToken);
        }

        if (logger.IsEnabled(LogLevel.Debug))
            logger.LogDebug("AgregatorManagerService background task is stopping.");
    }

    /// <summary>
    /// Zkontrolujte objednávky s dodatečným časovým odstupem.
    /// </summary>
    /// <returns>Úloha.</returns>
    private async Task CheckConfirmedGracePeriodOrdersAsync()
    {
        if (logger.IsEnabled(LogLevel.Debug))
            logger.LogDebug("Checking confirmed grace period orders");

        var orderIds = await service.CreateSnapshotAndClear();
        if (orderIds.Any())
        {
            using IServiceScope scope = scopeFactory.CreateScope();
            IOrderDatabaseService service = scope.ServiceProvider.GetService<IOrderDatabaseService>() ?? throw new ArgumentNullException(nameof(IOrderDatabaseService));

            await service.SaveOrdersAsync(orderIds);

            if (logger.IsEnabled(LogLevel.Debug))
                logger.LogDebug($"AgregatorManagerService uloženo {orderIds.Count} objednávek.");
        }
        else
        {
            if (logger.IsEnabled(LogLevel.Debug))
                logger.LogDebug($"AgregatorManagerService prázdný seznam.");
        }
    }
}
