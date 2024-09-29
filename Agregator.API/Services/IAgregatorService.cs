namespace Agregator.API.Services;

public record OrderItem(string ProductId, decimal Quantity);

/// <summary>
/// Reprezentuje rozhraní pro agregargaci objednávek.
/// </summary>
public interface IAgregatorService
{
    /// <summary>
    /// Seznam agregovaných objednávek.
    /// </summary>
    /// <returns>Seznam objednávek.</returns>
    Task<ICollection<OrderItem>> GetAllOrdersAsync();

    /// <summary>
    /// Přidání seznamu objednávek k agregaci.
    /// </summary>
    /// <param name="items">Seznam objednávek.</param>
    /// <returns>Úloha.</returns>
    Task AddOrdersAsync(ICollection<OrderItem> items);

    /// <summary>
    /// Vyčistí agregované objednávky a vrátí jejich seznam.
    /// </summary>
    /// <returns>Seznam agregovaných objednávek.</returns>
    Task<ICollection<OrderItem>> CreateSnapshotAndClear();
}
