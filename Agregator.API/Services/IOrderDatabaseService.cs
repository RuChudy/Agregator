namespace Agregator.API.Services;

/// <summary>
/// Reprezentuje rozhraní pro uložení objednávek do systému.
/// </summary>
public interface IOrderDatabaseService
{
    /// <summary>
    /// Přidá objednávky do systému.
    /// </summary>
    /// <param name="orders">Výčet objednávek.</param>
    Task SaveOrdersAsync(IEnumerable<OrderItem> orders);
}
