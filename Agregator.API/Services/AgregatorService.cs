namespace Agregator.API.Services;

/// <summary>
/// Služba k agregování objenávek.
/// </summary>
public class AgregatorService(ILogger<AgregatorService> _logger) : IAgregatorService
{
    private readonly ConcurrentDictionary<string, decimal> _orders = new ConcurrentDictionary<string, decimal>();

    /// <summary>
    /// Seznam agregovaných objednávek.
    /// </summary>
    /// <returns>Seznam objednávek.</returns>
    public Task<ICollection<OrderItem>> GetAllOrdersAsync()
    {
        ICollection<OrderItem> result = _orders
            .Select(f => new OrderItem(f.Key, f.Value))
            .ToList();

        _logger.LogDebug($"Agregátor obsahuje {result.Count} objednávek.");
        return Task.FromResult(result);
    }

    /// <summary>
    /// Přidání nového seznamu objednávek k agregaci.
    /// </summary>
    /// <param name="items">Seznam objednávek.</param>
    /// <returns>Úloha.</returns>
    public Task AddOrdersAsync(ICollection<OrderItem> items)
    {
        foreach (var order in items)
            _orders.AddOrUpdate(order.ProductId, order.Quantity, (key, current) => current += order.Quantity);

        _logger.LogDebug($"Nově agregováno {items.Count} objednávek.");
        return Task.CompletedTask;
    }

    /// <summary>
    /// Vyčistí agregované objednávky a vrátí jejich seznam.
    /// </summary>
    /// <returns>Seznam agregovaných objednávek.</returns>
    public Task<ICollection<OrderItem>> CreateSnapshotAndClear()
    {
        List<OrderItem> orders = new List<OrderItem>();

        foreach (var keyValue in _orders)
        {
            if(_orders.TryRemove(keyValue))
            {
                orders.Add(new OrderItem(keyValue.Key, keyValue.Value));
            }
        }

        _logger.LogDebug($"Vytvořen snímek {orders.Count} objednávek.");
        return Task.FromResult((ICollection<OrderItem>)orders);
    }
}
