namespace Agregator.API.Services;

/// <summary>
/// Služba k uložení agregovaných objenávek.
/// </summary>
public class OrderDatabaseService(ILogger<AgregatorService> _logger) : IOrderDatabaseService
{
    /// <summary>
    /// Přidá objednávky do systému.
    /// </summary>
    /// <param name="orders">Výčet objednávek.</param>
    public Task SaveOrdersAsync(IEnumerable<OrderItem> orders)
    {
        _logger.LogDebug("OrderDatabaseService: Ukládám objednávky..");
        try
        {
            Console.WriteLine();
            Console.WriteLine("BEGIN TRANS");

            int count = 0;
            foreach (var o in orders)
            {
                Console.WriteLine($"INSERT INTO [Orders] ([ProductId],[Quantity]) VALUES ('{o.ProductId}', {o.Quantity:N4});");
                count++;
            }

            Console.WriteLine("COMMIT");

            _logger.LogDebug($"OrderDatabaseService: Uloženo {count} agregovaných objednávek.");
        }
        catch (Exception ex)
        {
            _logger.LogError($"OrderDatabaseService: Selhalo uložení objednávek. {ex.Message}");

            throw new Exception("Selhalo uložení objednávek.", ex);
        }

        return Task.CompletedTask;
    }
}
