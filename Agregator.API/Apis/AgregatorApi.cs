namespace Agregator.API;

/// <summary>
/// REST-API DTO objednávky.
/// </summary>
/// <param name="productId">Identifikátor objednávky.</param>
/// <param name="quantity">Počet objednávky.</param>
public record Order(string productId, decimal quantity);

/// <summary>
/// Agregator REST-API
/// </summary>
public static class AgregatorApi
{
    /// <summary>
    /// Zaregistruje Agregator REST-API end-pointy.
    /// </summary>
    /// <param name="app">Aplikace</param>
    /// <returns>Builder</returns>
    public static RouteGroupBuilder MapAgregatorApiV1(this IEndpointRouteBuilder app)
    {
        var api = app.MapGroup("api/orders");

        api.MapGet("/", GetOrdersAsync);
        api.MapPost("/", CreateOrderAsync);

        // Další generace - umožní i uložit agregované objednávky
        api.MapDelete("/", ProcessOrderAsync);

        return api;
    }

    /// <summary>
    /// Vrátí aktuální seznam objednávek.
    /// </summary>
    /// <param name="services">Agregační služba.</param>
    /// <returns>Úlohu.</returns>
    public static async Task<Results<Ok<List<Order>>, NotFound>> GetOrdersAsync([FromServices] IAgregatorService services)
    {
        try
        {
            var orders = (await services.GetAllOrdersAsync()).Select(o => new Order(o.ProductId, o.Quantity)).ToList();
            return TypedResults.Ok(orders);
        }
        catch
        {
            return TypedResults.NotFound();
        }
    }

    /// <summary>
    /// Přidá objednávky do Agregatoru.
    /// </summary>
    /// <param name="orders">Seznam objednávek.</param>
    /// <param name="services">Agregační služba.</param>
    /// <returns>Úlohu.</returns>
    public static async Task<Results<Ok, ProblemHttpResult>> CreateOrderAsync(List<Order> orders, [FromServices] IAgregatorService services)
    {
        try
        {
            var items = orders.Select(o => new OrderItem(o.productId, o.quantity)).ToList();
            await services.AddOrdersAsync(items);
            return TypedResults.Ok();
        }
        catch(Exception ex)
        {
            return TypedResults.Problem(detail: ex.Message, statusCode: 500);
        }
    }

    /// <summary>
    /// Zpracuje agegované objednávky.
    /// </summary>
    /// <param name="agregator">Agregační služba.</param>
    /// <param name="database">Ukládací služba.</param>
    /// <returns>Úloha.</returns>
    public static async Task<Results<Ok, ProblemHttpResult>> ProcessOrderAsync([FromServices] IAgregatorService agregator, [FromServices] IOrderDatabaseService database)
    {
        try
        {
            var orders = await agregator.CreateSnapshotAndClear();
            await database.SaveOrdersAsync(orders);

            return TypedResults.Ok();
        }
        catch (Exception ex)
        {
            return TypedResults.Problem(detail: ex.Message, statusCode: 500);
        }
    }
}
