namespace Agregator.API.Services;

public record OrderItem(string ProductId, decimal Quantity);

public interface IAgregatorService
{
    Task<ICollection<OrderItem>> GetAllOrdersAsync();
    Task AddOrdersAsync(ICollection<OrderItem> items);
    Task<ICollection<OrderItem>> CreateSnapshotAndClear();
}
