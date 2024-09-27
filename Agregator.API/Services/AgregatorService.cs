namespace Agregator.API.Services;

public class AgregatorService(ILogger<AgregatorService> _logger) : IAgregatorService
{
    private readonly ConcurrentDictionary<string, decimal> _orders = new ConcurrentDictionary<string, decimal>();

    public Task<ICollection<OrderItem>> GetAllOrdersAsync()
    {
        ICollection<OrderItem> result = _orders
            .Select(f => new OrderItem(f.Key, f.Value))
            .ToList();

        _logger.LogDebug("Pridano");

        return Task.FromResult(result);
    }

    public Task AddOrdersAsync(ICollection<OrderItem> items)
    {
        foreach (var order in items)
            _orders.AddOrUpdate(order.ProductId, order.Quantity, (key, current) => current += order.Quantity);

        _logger.LogDebug("Pridano");

        return Task.CompletedTask;
    }

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

        _logger.LogDebug("Pridano");
        return Task.FromResult((ICollection<OrderItem>)orders);
    }
}
